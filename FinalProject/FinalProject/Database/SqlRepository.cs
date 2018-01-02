using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace FinalProject.Database
{
    public class SqlRepository : IRepository
    {
        private const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_.-";

        private Database database;

        public SqlRepository(Database database)
        {
            this.database = database;
        }

        public bool AddAccount(Account account)
        {
            if ((CheckUsername(account.Username)) && (CheckEmail(account.Email)))
            {
                database.Accounts.Add(account);
                database.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CheckUsername(string username)
        {
            foreach (char c in username)
            {
                if (!ValidChars.Contains(c))
                {
                    return false;
                }
            }
            if ((username.Length < 3) || (username.Length > 64))
            {
                return false;
            }
            return database.Accounts.Where(i => (i.Username.ToLower() == username.ToLower())).Count() <= 0;
        }

        public bool CheckEmail(string email)
        {
            return database.Accounts.Where(i => (i.Email.ToLower() == email.ToLower())).Count() <= 0;
        }

        public bool VerifyAccount(string Email, string Code)
        {
            if ((Email != null) && (Code != null))
            {
                int vefificationCode;
                if (int.TryParse(Code, out vefificationCode))
                {
                    Account target = database.Accounts.Where(i => (i.Email.ToLower() == Email.ToLower()) && (i.VerificationCode == vefificationCode) && (!i.Verified)).FirstOrDefault();
                    if (target != null)
                    {
                        target.Verified = true;
                        database.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool LoginAccount(string Identifier, string Password, out string CookieId, out string CookieHash)
        {
            CookieId = "";
            CookieHash = "";
            if ((Identifier != null) && (Password != null))
            {
                byte[] hashedPassword = Account.HashPassword(Password);
                Account target;
                if (Identifier.Contains("@"))
                {
                    target = database.Accounts.Where(i => (!i.UseFacebook) && (i.Email.ToLower() == Identifier.ToLower()) && (i.PasswordHashed == hashedPassword)).FirstOrDefault();
                }
                else
                {
                    target = database.Accounts.Where(i => (!i.UseFacebook) && (i.Username.ToLower() == Identifier.ToLower()) && (i.PasswordHashed == hashedPassword)).FirstOrDefault();
                }
                if (target != null)
                {
                    CookieId = target.Id.ToString();
                    CookieHash = Convert.ToBase64String(hashedPassword);
                    return true;
                }
            }
            return false;
        }

        public bool LoginAccountFacebook(string FacebookID, out string CookieId, out string CookieHash)
        {
            CookieId = "";
            CookieHash = "";
            if (FacebookID != null)
            {
                Account target;
                target = database.Accounts.Where(i => (i.UseFacebook) && (i.FacebookID == FacebookID)).FirstOrDefault();
                if (target != null)
                {
                    CookieId = target.Id.ToString();
                    CookieHash = Convert.ToBase64String(target.PasswordHashed);
                    return true;
                }
            }
            return false;
        }

        public void SetAccountStatus(HttpResponse response, string CookieId, string CookieHash)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddHours(6.0);
            response.Cookies.Append("userId", CookieId, options);
            response.Cookies.Append("userHash", CookieHash, options);
        }

        public AccountStatus GetAccountStatus(HttpRequest request, HttpResponse response, out Account account)
        {
            account = null;
            if ((request.Cookies.ContainsKey("userId")) && (request.Cookies.ContainsKey("userHash")))
            {
                int userId;
                if (int.TryParse(request.Cookies["userId"], out userId))
                {
                    byte[] userHash;
                    try
                    {
                        userHash = Convert.FromBase64String(request.Cookies["userHash"]);
                    }
                    catch
                    {
                        return AccountStatus.Invalid;
                    }
                    Account target = database.Accounts.Where(i => (i.Id == userId) && (i.PasswordHashed == userHash)).FirstOrDefault();
                    if (target != null)
                    {
                        account = target;
                        if (target.Verified)
                        {
                            return AccountStatus.OK;
                        }
                        return AccountStatus.Inactive;
                    }
                    ClearAccountStatus(response);
                }
            }
            return AccountStatus.Invalid;
        }

        public void ClearAccountStatus(HttpResponse response)
        {
            response.Cookies.Delete("userId");
            response.Cookies.Delete("userHash");
        }

        public void ForceSave()
        {
            database.SaveChanges();
        }

        public bool CheckFacebookAccount(string FacebookID)
        {
            return database.Accounts.Where(i => (i.UseFacebook) && (i.FacebookID == FacebookID)).Count() <= 0;
        }
    }
}
