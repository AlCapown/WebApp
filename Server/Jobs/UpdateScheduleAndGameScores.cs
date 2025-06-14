using Mediator;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Enums;
using WebApp.ExternalIntegrations.ESPN.Service.Api;
using WebApp.ExternalIntegrations.ESPN.Service.Models;
using WebApp.Server.Features.Game;
using WebApp.Server.Features.Season;
using WebApp.Server.Features.SeasonService.Command;
using WebApp.Server.Features.Team;
using WebApp.Server.Infrastructure.Exceptions;
using WebApp.Server.Services.BackgroundJobLogging;

namespace WebApp.Server.Jobs;

public class UpdateScheduleAndGameScores
{
    private readonly IMediator _mediator;
    private readonly IESPNApi _espnApi;

    private Dictionary<string, int> TeamLookupByAbbreviation;

    private readonly CreateBackgroundJobLog.Command _logCommand;

    public UpdateScheduleAndGameScores(IMediator mediator, IESPNApi espnApi)
    {
        _mediator = mediator;
        _espnApi = espnApi;

        _logCommand = new CreateBackgroundJobLog.Command
        {
            BackgroundJobName = nameof(UpdateScheduleAndGameScores),
            Started = DateTimeOffset.Now
        };
    }

    public async Task Process(bool overrideShouldRunJob, CancellationToken token)
    {
        try
        {
            bool shouldRunJob = await ShouldRunJob(token);

            if (overrideShouldRunJob || shouldRunJob)
            {
                var scoreboard = await _espnApi.GetScoreboardAsync(token);

                await UpdateSeason(scoreboard, token);
                await UpdateSeasonWeeks(scoreboard, token);
                await CreateTeamLookupByAbbreviation(token);
                await UpdateGames(scoreboard, token);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (WebAppValidationException ex)
        {
            foreach(var error in ex.GetErrors())
            {
                foreach(var value in error.Value)
                {
                    AddError($"{error.Key}: {value}");
                }
            }
        }
        catch (Exception ex)
        {
            AddError(ex.Message, ex.StackTrace);
        }

        _ = await _mediator.Send(_logCommand, CancellationToken.None);
        return;
    }

    private async Task<bool> ShouldRunJob(CancellationToken token)
    {
        var dateTimeNow = DateTime.Now;

        // If 12:00 PM - 12:09 PM always run.
        if (dateTimeNow.Hour == 12 && dateTimeNow.Minute >= 0 && dateTimeNow.Minute < 10)
        {
            return true;
        }

        var searchResult = await _mediator.Send(new GameSearch.Query
        {
            GameStartsOnMin = dateTimeNow.AddHours(-5),
            GameStartsOnMax = dateTimeNow.AddMinutes(10),
            IsGameComplete = false,
        }, token);

        return searchResult.Match
        (
            success => success.Games.Length != 0,
            validationProblem =>
            {
                AddError("Failed to query games.", validationProblem);
                return false;
            }
        );
    }

    private async Task UpdateSeason(ESPNScoreboardModel scoreboard, CancellationToken token)
    {
        var league = scoreboard.Leagues.First(x => x.Abbreviation.Equals("NFL"));

        int year = league.CalendarStartDate.Year;

        await _mediator.Send(new UpdateSeason.Command
        {
            SeasonId = year,
            Description = $"{year}-{year + 1} Football Season",
            SeasonStart = DateOnly.FromDateTime(league.CalendarStartDate),
            SeasonEnd = DateOnly.FromDateTime(league.CalendarEndDate)
        }, token);
    }

    private async Task UpdateSeasonWeeks(ESPNScoreboardModel scoreboard, CancellationToken token)
    {
        var league = scoreboard.Leagues.FirstOrDefault(x => x.Abbreviation.Equals("NFL"))
            ?? throw new Exception("Failed to find NFL league.");

        foreach (var item in league.Calendar)
        {
            var weekType = item.Label?.Replace(" ", string.Empty)?.ToWeekTypeEnum();

            if (weekType == null)
            {
                AddError($"Failed to parse WeekType with value: {item.Label}");
                continue;
            }

            if (weekType.Value == WeekType.OffSeason)
            {
                // Not an Error, intentionally skipping
                continue;
            }

            foreach (var entry in item.Entries)
            {
                if (!int.TryParse(entry.Value, out int week))
                {
                    AddError($"Failed to parse week to an Int32: {entry.Value}");
                    continue;
                }

                await _mediator.Send(new UpsertWeekForSeason.Command
                {
                    SeasonId = league.CalendarStartDate.Year,
                    Week = week,
                    WeekType = weekType.Value,
                    Description = entry.Label,
                    WeekStart = DateOnly.FromDateTime(entry.StartDate),
                    WeekEnd = DateOnly.FromDateTime(entry.EndDate),
                }, token);
            }
        }
    }

    private async Task CreateTeamLookupByAbbreviation(CancellationToken token)
    {
        var result = await _mediator.Send(new TeamSearch.Query(), token);

        var teamList = result.Match
        (
            success => success.Teams,
            failure =>
            {
                AddError("Failed to get Team List");
                return [];
            }
        );

        TeamLookupByAbbreviation = teamList.ToDictionary(key => key.Abbreviation, value => value.TeamId, StringComparer.OrdinalIgnoreCase);
    }

    private async Task UpdateGames(ESPNScoreboardModel scoreboard, CancellationToken token)
    {
        foreach (var game in scoreboard.Events)
        {
            var year = game.Season.Year;
            var week = game.Week.Number;
            var weekType = game.Season.TypeName?.ToWeekTypeEnum();
            var date = game.Date;

            var competition = game.Competitions.FirstOrDefault();

            if (competition == null)
            {
                AddError($"Failed to find any competitions for this game.");
                continue;
            }

            var homeTeam = competition.Competitors.FirstOrDefault(x => x.HomeAway == "home");
            var awayTeam = competition.Competitors.FirstOrDefault(x => x.HomeAway == "away");
            var eventStatus = competition.Status;

            if (homeTeam == null || awayTeam == null)
            {
                AddError($"Failed to find the Home or Away team");
                continue;
            }

            if (!TeamLookupByAbbreviation.TryGetValue(homeTeam.Team.Abbreviation, out int homeTeamId))
            {
                AddError($"Failed to lookup Home team identifier by the abbreviated name {homeTeam.Team.Abbreviation}");
                continue;
            }

            if (!TeamLookupByAbbreviation.TryGetValue(awayTeam.Team.Abbreviation, out int awayTeamId))
            {
                AddError($"Failed to lookup Away team identifier by the abbreviated name {awayTeam.Team.Abbreviation}");
                continue;
            }

            var seasonWeekResult = await _mediator.Send(new SeasonWeekSearch.Query
            {
                SeasonId = year,
                Week = week,
                WeekType = weekType.Value,
            }, token);

            var seasonWeek = seasonWeekResult.Match
            (
                success => success.SeasonWeeks.FirstOrDefault(),
                validationProblem =>
                {
                    AddError($"Failed to find the SeasonWeek with the following search params; SeasonId: {year} Week: {week} WeekType: {weekType.Value}", validationProblem);
                    return null;
                }
            );

            if (seasonWeek is null)
            {
                continue;
            }

            await _mediator.Send(new CreateOrUpdateGame.Command
            {
                SeasonWeekId = seasonWeek.SeasonWeekId,
                StartsOn = date,
                HomeTeamId = homeTeamId,
                HomeTeamScore = int.TryParse(homeTeam.Score, out var homeScore) ? homeScore : 0,
                AwayTeamId = awayTeamId,
                AwayTeamScore = int.TryParse(awayTeam.Score, out var awayScore) ? awayScore : 0,
                Quarter = eventStatus.Period,
                ClockTime = eventStatus.DisplayClock,
                IsComplete = eventStatus.Type.Completed,
            }, token);
        }
    }

    private void AddError(string message, string stackTrace = null)
    {
        _logCommand.Errors.Add(new CreateBackgroundJobLog.Command.Error
        {
            Message = message,
            ValidationErrors = null,
            StackTrace = stackTrace
        });
    }

    private void AddError(string message, ValidationProblemDetails validationProblemDetails)
    {
        _logCommand.Errors.Add(new CreateBackgroundJobLog.Command.Error
        {
            Message = message,
            ValidationErrors = validationProblemDetails.Errors,
            StackTrace = null
        });
    }
}
