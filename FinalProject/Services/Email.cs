using System.Net;
using System.Net.Mail;

/*
 * EmailInfo file is not included because it contains sensible information.
 * It is essentially just a static class with SMTP login information.
 */
namespace FinalProject.Services
{
    public static class Email
    {
        private static SmtpClient SMTP;
        private static object mutexLock = new object();

        static Email()
        {
            SMTP = new SmtpClient(EmailInfo.SmtpServer, EmailInfo.SmtpPort);
            SMTP.UseDefaultCredentials = false;
            SMTP.EnableSsl = true;
            SMTP.Credentials = new NetworkCredential(EmailInfo.EmailUsername, EmailInfo.EmailPassword);
        }

        public static void SendVerification(string to, string code, string firstLine = "Thank you for registering.")
        {
            string url = Program.UrlAddress + "Activate?Email=" + WebUtility.UrlEncode(to) + "&" + "Code=" + WebUtility.UrlEncode(code);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(EmailInfo.EmailAddress);
            mail.To.Add(to);
            mail.IsBodyHtml = true;
            mail.Body = $"{firstLine}<br/><br/>Your verification code is: <strong>{code}</strong><br/><br/>Activation link: <a href=\"{url}\">Activate</a><br/><br/>This is an automated response mail, please do not respond to it.";
            mail.Subject = "Pinboard Confirmation Email";
            // Essentially not that great concept, but considering the fact that this app will be used by 1-2 users at a time, shouldn't generate any problems.
            SMTP.Send(mail);
        }
    }
}
