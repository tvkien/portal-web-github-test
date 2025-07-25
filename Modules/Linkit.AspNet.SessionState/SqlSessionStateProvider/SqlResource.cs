namespace Linkit.AspNet.SessionState
{
    public static class SqlResource
    {
        public const string Login_failed_sql_session_database = "Failed to login to session state SQL server for user '{0}'.";
        public const string Cant_connect_sql_session_database = "Unable to connect to SQL Server session database.";
        public const string Session_id_too_long = "SessionId is too long.";
        public const string Invalid_session_state = "The session state information is invalid and might be corrupted.";
        public const string Connection_name_not_specified = "The attribute 'connectionStringName' is missing or empty.";
        public const string Connection_string_not_found = "The connection name '{0}' was not found in the applications configuration or the connection string is empty.";
    }
}
