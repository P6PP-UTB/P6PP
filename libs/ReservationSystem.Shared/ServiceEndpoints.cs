namespace ReservationSystem.Shared;

public static class ServiceEndpoints
{
    public static class UserService
    {
        // USERS
        private const string BaseUrl = "http://user-service:5189";
        
        public static string CreateUser => $"{BaseUrl}/api/user";
        public static string GetUserById(int id) => $"{BaseUrl}/api/user/{id}";
        public static string UpdateUser(int id) => $"{BaseUrl}/api/user/{id}";
        public static string GetUsers => $"{BaseUrl}/api/users";
        public static string DeleteUser(int id) => $"{BaseUrl}/api/user/{id}";
        
        // ROLES
        public static string CreateRole => $"{BaseUrl}/api/role";
        public static string GetRoleById(int id) => $"{BaseUrl}/api/role/{id}";
        public static string GetRoles => $"{BaseUrl}/api/roles";
        public static string AssignRole => $"{BaseUrl}/api/user/role";
    }

    public static class AuthService
    {
        private const string BaseUrl = "http://auth-service:8005";
        public static string Login => $"{BaseUrl}/api/auth/login";
        public static string Register => $"{BaseUrl}/api/auth/register";
        public static string ResetPassword => $"{BaseUrl}/api/auth/reset-password";

    }
    
    public static class NotificationService
    {
        private const string BaseUrl = "http://notification-service:5181";
        public static string SendEmail => $"{BaseUrl}/api/notification/sendemail";
        public static string SendVerificationEmail => $"{BaseUrl}/api/notification/user/sendverificationemail";
        public static string SendPasswordResetEmail => $"{BaseUrl}/api/notification/user/sendpasswordresetemail";
        public static string SendRegistrationEmail(int id) => $"{BaseUrl}/api/notification/user/sendregistrationemail/{id}";
    }

    public static class AdminSettingsService
    {
        private const string BaseUrl = "http://admin-settings:9090";
        // SystemSettings
        public static string GetSystemSettings => $"{BaseUrl}/api/system-settings";
        public static string UpdateSystemSettings => $"{BaseUrl}/api/system-settings";
        public static string GetTimezones => $"{BaseUrl}/api/system-settings/timezones";
        public static string UpdateTimezone => $"{BaseUrl}/api/system-settings/timezones";
        public static string GetDatabaseBackupSetting => $"{BaseUrl}/api/system-settings/BackupSetting";
        public static string UpdateDatabaseBackupSetting => $"{BaseUrl}/api/system-settings/BackupSetting";
        public static string GetNotificationEnabled => $"{BaseUrl}/api/system-settings/notification-enabled";

        // Backup
        public static string RunBackup => $"{BaseUrl}/api/backup/run";

        // AuditLogs
        public static string GetAuditLogs => $"{BaseUrl}/api/audit";
        public static string AddAuditLog => $"{BaseUrl}/api/audit";
        public static string GetUserAuditLogs(string userId) => $"{BaseUrl}/api/audit/user/{userId}";
        public static string GetAuditLogsByAction(string action) => $"{BaseUrl}/api/audit/action/{action}";
        public static string ArchiveAuditLog(int id) => $"{BaseUrl}/api/audit/{id}/archive";
    }

}