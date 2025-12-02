#nullable enable

namespace WebApp.Common.Constants;

public static class Policy
{
    /// <summary>
    /// Policy for regular authenticated users with the "User" role.
    /// Grants access to features and resources available to standard users.
    /// </summary>
    public const string USER = "User";

    /// <summary>
    /// Policy for administrators with the "Admin" role.
    /// Grants access to administrative features and resources.
    /// </summary>
    public const string ADMIN = "Admin";

    /// <summary>
    /// Policy for accessing Hangfire dashboard and related features.
    /// Restricted to users with the "Admin" role.
    /// </summary>
    public const string HANGFIRE = "Hangfire";
}
