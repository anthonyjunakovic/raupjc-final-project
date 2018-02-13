using FinalProject.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class DashboardModel
    {
        public string Username;
        public string FirstName, LastName, Gender;
        public bool IsFacebook;
        public string FacebookURL;

        public DashboardModel(Account UserAccount) : this(UserAccount.Username, UserAccount.FirstName, UserAccount.LastName, UserAccount.UserGender.ToString(), UserAccount.UseFacebook)
        {
            if (IsFacebook) {
                FacebookURL = Services.Facebook.GetProfileURL(UserAccount.FacebookID);
            }
        }

        public DashboardModel(string Username, string FirstName, string LastName, string Gender, bool IsFacebook = false, string FacebookURL = null)
        {
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Gender = Gender;
            this.IsFacebook = IsFacebook;
            this.FacebookURL = FacebookURL;
        }
    }
}
