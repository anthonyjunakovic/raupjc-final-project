using FinalProject.Database;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace FinalProject.Services
{
    public static class Facebook
    {
        public static string GetProfileURL(string FacebookID)
        {
            return @"https://www.facebook.com/" + FacebookID;
        }

        public static bool GetFacebookInfo(string AccessToken, out string Email, out string FirstName, out string LastName, out Gender gender, out string FacebookID)
        {
            string uri = @"https://graph.facebook.com/v2.11/me" + $"?access_token={WebUtility.UrlEncode(AccessToken)}&fields=email,first_name,last_name,gender";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                string data;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }
                }

                JObject result = JObject.Parse(data);

                Email = result["email"].ToString();
                FirstName = result["first_name"].ToString();
                LastName = result["last_name"].ToString();

                string localGender = result["gender"].ToString().Trim().ToLower();
                switch (localGender)
                {
                    case "male":
                        gender = Gender.Male;
                        break;
                    case "female":
                        gender = Gender.Female;
                        break;
                    default:
                        gender = Gender.Other;
                        break;
                }

                FacebookID = result["id"].ToString();
                return true;
            }
            catch { }

            Email = "";
            FirstName = "";
            LastName = "";

            gender = Gender.Other;
            FacebookID = "";

            return false;
        }
    }
}
