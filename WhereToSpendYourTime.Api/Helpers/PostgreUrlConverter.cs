using Npgsql;

namespace WhereToSpendYourTime.Api.Helpers;

/// <summary>
/// Provides functionality for converting a PostgreSQL connection URL
/// into a standard Npgsql connection string
/// </summary>
public static class PostgreUrlConverter
{
    /// <summary>
    /// Converts a PostgreSQL database URL into an Npgsql-compatible
    /// connection string
    /// </summary>
    /// <remarks>
    /// Expected URL format:
    /// postgres://username:password@host:port/database
    /// </remarks>
    /// <param name="databaseUrl">
    /// PostgreSQL connection URL
    /// </param>
    /// <returns>
    /// A connection string compatible with Npgsql
    /// </returns>
    public static string ConvertPostgresUrlToConnectionString(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "",
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
            Database = uri.AbsolutePath.TrimStart('/'),
            SslMode = SslMode.Require
        };

        return builder.ToString();
    }
}