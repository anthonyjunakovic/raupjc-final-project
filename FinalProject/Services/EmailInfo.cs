/* // EMAIL SERVICE DISABLED

using Microsoft.Extensions.Configuration;

namespace FinalProject.Services
{
    public static class EmailInfo
    {
        public static IConfiguration Configuration;

        public static string SmtpServer {
            get
            {
                return Configuration.GetSection("EmailData").GetSection("Server").Value;
            }
        }
        public static int SmtpPort
        {
            get
            {
                return int.Parse(Configuration.GetSection("EmailData").GetSection("Port").Value);
            }
        }
        public static string EmailAddress
        {
            get
            {
                return Configuration.GetSection("EmailData").GetSection("Address").Value;
            }
        }
        public static string EmailUsername
        {
            get
            {
                return Configuration.GetSection("EmailData").GetSection("Username").Value;
            }
        }
        public static string EmailPassword
        {
            get
            {
                return Configuration.GetSection("EmailData").GetSection("Password").Value;
            }
        }
    }
}
*/