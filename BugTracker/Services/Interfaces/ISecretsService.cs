namespace BugTracker.Services.Interfaces
{
    public interface ISecretsService
    {
        public string GetMailSettingsMail();
        public string GetMailSettingsPassword();
        public string GetSiteAdminEmail();
        public string GetSiteAdminPassword();
        public string GetDefaultUserEmail();
        public string GetDefaultUserPassword();
    }
}
