using Microsoft.Extensions.Configuration;

namespace FinalProject.Services
{
    public static class EmailInfo
    {
        public static IConfiguration Configuration;

        public static string SmtpServer {
            get
            {
                return Configuration.GetSection("Data").GetSection("Server").Value;
            }
        }
        public static int SmtpPort
        {
            get
            {
                return int.Parse(Configuration.GetSection("Data").GetSection("Port").Value);
            }
        }
        public static string EmailAddress
        {
            get
            {
                return Configuration.GetSection("Data").GetSection("Address").Value;
            }
        }
        public static string EmailUsername
        {
            get
            {
                return Configuration.GetSection("Data").GetSection("Username").Value;
            }
        }
        public static string EmailPassword
        {
            get
            {
                return Configuration.GetSection("Data").GetSection("Password").Value;
            }
        }
    }
}
