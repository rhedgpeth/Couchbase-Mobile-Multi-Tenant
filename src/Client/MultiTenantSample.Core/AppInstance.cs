using MultiTenantSample.Models;

namespace MultiTenantSample.Core
{
    public static class AppInstance
    {
        public static string CurrentUser { get; set; }
        public static Session CurrentSession { get; set; }

        public static void Reset()
        {
            CurrentUser = null;
            CurrentSession = null;
        }
    }
}
