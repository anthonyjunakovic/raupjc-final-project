using FinalProject.Database;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FinalProject.Services
{
    public class FacebookInfo
    {
        public bool Ok { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string FacebookId { get; set; }
    }

    public static class Facebook
    {
        public static string GetProfileURL(string FacebookID)
        {
            return @"https://www.facebook.com/" + FacebookID;
        }

        public static async Task<FacebookInfo> GetFacebookInfo(string AccessToken)
        {
            FacebookInfo fi = new FacebookInfo();
            string uri = @"https://graph.facebook.com/v2.11/me" + $"?access_token={WebUtility.UrlEncode(AccessToken)}&fields=email,first_name,last_name,gender";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                string data;

                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            data = await reader.ReadToEndAsync();
                        }
                    }
                }

                JObject result = JObject.Parse(data);

                fi.Email = result["email"].ToString();
                fi.FirstName = result["first_name"].ToString();
                fi.LastName = result["last_name"].ToString();

                string localGender = result["gender"].ToString().Trim().ToLower();
                switch (localGender)
                {
                    case "male":
                        fi.Gender = Gender.Male;
                        break;
                    case "female":
                        fi.Gender = Gender.Female;
                        break;
                    default:
                        fi.Gender = Gender.Other;
                        break;
                }

                fi.FacebookId = result["id"].ToString();

                fi.Ok = true;
                return fi;
            }
            catch { }

            fi.Email = "";
            fi.FirstName = "";
            fi.LastName = "";

            fi.Gender = Gender.Other;
            fi.FacebookId = "";

            fi.Ok = false;
            return fi;
        }
    }
}
