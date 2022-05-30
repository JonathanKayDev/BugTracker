namespace BugTracker.Models.Settings
{
    public class AppSettings
    {
        public DefaultCredentials? DefaultCredentials { get; set; }
        public SiteAdminCredentials? SiteAdminCredentials { get; set; }
    }

    public class DefaultCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SiteAdminCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
