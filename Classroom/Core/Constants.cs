namespace Classroom.Core
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Roles
        /// </summary>
        /// <author>huynhdev24</author>
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Manager = "Manager";
            public const string User = "User";
        }

        /// <summary>
        /// Policies
        /// </summary>
        /// <author>huynhdev24</author>
        public static class Policies
        {
            public const string RequireAdmin = "RequireAdmin";
            public const string RequireManager = "RequireManager";
        }
    }
}
