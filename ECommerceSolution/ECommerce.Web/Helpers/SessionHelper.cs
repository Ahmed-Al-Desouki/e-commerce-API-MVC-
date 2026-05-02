namespace ECommerce.API.Helpers
{
    public static class SessionHelper
    {
        private const string TokenKey = "JwtToken";
        private const string UsernameKey = "Username";
        private const string IsAdminKey = "IsAdmin";
        private const string UserIdKey = "UserId";

        public static void SetUserSession(ISession session,
            string token, string username, bool isAdmin, int userId)
        {
            session.SetString(TokenKey, token);
            session.SetString(UsernameKey, username);
            session.SetString(IsAdminKey, isAdmin.ToString());
            session.SetInt32(UserIdKey, userId);
        }

        public static string? GetToken(ISession session) =>
            session.GetString(TokenKey);

        public static string? GetUsername(ISession session) =>
            session.GetString(UsernameKey);

        public static bool IsAdmin(ISession session) =>
            session.GetString(IsAdminKey) == "True";

        public static bool IsLoggedIn(ISession session) =>
            !string.IsNullOrEmpty(session.GetString(TokenKey));

        public static void Clear(ISession session) =>
            session.Clear();
    }
}
