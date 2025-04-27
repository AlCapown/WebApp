using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WebApp.ExternalIntegrations.NFL.Service.Models
{
    [XmlRoot(ElementName = "ss")]
	public class WeekResult
	{
		[XmlElement(ElementName = "gms")]
		public WeekInfo Week { get; set; }
	}

	[XmlRoot(ElementName = "gms")]
	public class WeekInfo
	{
		
		[XmlElement(ElementName = "g")]
		public List<Game> Games { get; set; }

		/// <summary>
		/// Unknown attribute
		/// </summary>
		[XmlAttribute(AttributeName = "gd")]
		public string Gd { get; set; }


		[XmlAttribute(AttributeName = "w")]
		public int Week { get; set; }

		[XmlAttribute(AttributeName = "y")]
		public int Year { get; set; }

		/// <summary>
		/// Gametype short name. P = PRE, R = REG, P = POST
		/// </summary>
		[XmlAttribute(AttributeName = "t")]
		public string GameTypeAbbreviation { get; set; }
	}

	[XmlRoot(ElementName = "g")]
	public class Game
	{
		/// <summary>
		/// Looks to be the unique identifier but the first 4 digets are the year, followed by 2 digit month, then 2 digit day.
		/// </summary>
		[XmlAttribute(AttributeName = "eid")]
		public string Eid { get; set; }

		/// <summary>
		/// Unknown attribute
		/// </summary>
		[XmlAttribute(AttributeName = "gsis")]
		public string Gsis { get; set; }

		/// <summary>
		/// Day Abbreviation. Example: Thur, Fri, Sat
		/// </summary>
		[XmlAttribute(AttributeName = "d")]
		public string DayAbbreviation { get; set; }

		/// <summary>
		/// 12 hour time. Does not include AM or PM information
		/// </summary>
		[XmlAttribute(AttributeName = "t")]
		public string Time { get; set; }

		/// <summary>
		/// Parsed Date and Time of the game.
		/// </summary>
		/// <remarks>
		/// This isn't actually returned by the NFL api and is determined by the Helpers.ParseGameStartDateTime fuction. 
		/// </remarks>
		[XmlIgnore]
		public DateTimeOffset StartsOn { get; set; }

		/// <summary>
		/// Only have past data so far. Right now its always F
		/// </summary>
		[XmlAttribute(AttributeName = "q")]
		public string Quarter { get; set; }

		/// <summary>
		/// Unknown attribute
		/// </summary>
		[XmlAttribute(AttributeName = "k")]
		public string K { get; set; }

		/// <summary>
		/// Short 2-3 letter team name for the home team.
		/// </summary>
		[XmlAttribute(AttributeName = "h")]
		public string HomeTeamShortName { get; set; }


		[XmlAttribute(AttributeName = "hnn")]
		public string HomeTeamName { get; set; }

		[XmlAttribute(AttributeName = "hs")]
		public int HomeTeamScore { get; set; }

		/// <summary>
		/// Short 2-3 letter team name for the visitor team
		/// </summary>
		[XmlAttribute(AttributeName = "v")]
		public string VisitorTeamShortName { get; set; }

		[XmlAttribute(AttributeName = "vnn")]
		public string VisitorTeamName { get; set; }

		[XmlAttribute(AttributeName = "vs")]
		public int VisitorTeamScore { get; set; }

		/// <summary>
		/// Unknown attribute
		/// </summary>
		[XmlAttribute(AttributeName = "p")]
		public string P { get; set; }

		/// <summary>
		/// Unknown attribute
		/// </summary>
		[XmlAttribute(AttributeName = "rz")]
		public string Rz { get; set; }

		/// <summary>
		/// Unknown attribute
		/// </summary>
		[XmlAttribute(AttributeName = "ga")]
		public string Ga { get; set; }

		/// <summary>
		/// PRE, REG, or POST
		/// </summary>
		[XmlAttribute(AttributeName = "gt")]
		public string GameType { get; set; }
	}
}
