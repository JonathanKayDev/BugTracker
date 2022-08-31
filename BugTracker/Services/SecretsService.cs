using BugTracker.Models;
using BugTracker.Models.Settings;
using BugTracker.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BugTracker.Services
{
    public class SecretsService : ISecretsService
    {

        private readonly AppSettings _appSettings;
        private readonly MailSettings _mailSettings;

        public SecretsService(IOptions<AppSettings> appSettings, IOptions<MailSettings> mailSettings)
        {
            _appSettings = appSettings.Value;
            _mailSettings = mailSettings.Value;
        }

        public string GetDefaultUserEmail()
        {
            var ev = Environment.GetEnvironmentVariable("DefaultUserEmail");

            return string.IsNullOrEmpty(ev) ? _appSettings.DefaultCredentials.Email : ev;
        }

        public string GetDefaultUserPassword()
        {
            var ev = Environment.GetEnvironmentVariable("DefaultUserPassword");

            return string.IsNullOrEmpty(ev) ? _appSettings.DefaultCredentials.Password : ev;
        }

        public string GetMailSettingsMail()
        {
            var ev = Environment.GetEnvironmentVariable("MailSettingsMail");

            return string.IsNullOrEmpty(ev) ? _mailSettings.Mail : ev;
        }

        public string GetMailSettingsPassword()
        {
            var ev = Environment.GetEnvironmentVariable("MailSettingsPassword");

            return string.IsNullOrEmpty(ev) ? _mailSettings.Password : ev;
        }

        public string GetSiteAdminEmail()
        {
            var ev = Environment.GetEnvironmentVariable("AdminEmail");

            return string.IsNullOrEmpty(ev) ? _appSettings.SiteAdminCredentials.Email : ev;
        }

        public string GetSiteAdminPassword()
        {
            var ev = Environment.GetEnvironmentVariable("AdminPassword");

            return string.IsNullOrEmpty(ev) ? _appSettings.SiteAdminCredentials.Password : ev;
        }
    }
}
