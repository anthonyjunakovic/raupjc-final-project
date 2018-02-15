using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<bool> AddAccount(Account account)
        {
            if ((await CheckUsername(account.Username)) && (await CheckEmail(account.Email)))
            {
                database.Accounts.Add(account);
                await database.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CheckUsername(string username)
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
            return (await database.Accounts.Where(i => (i.Username.ToLower() == username.ToLower())).CountAsync()) <= 0;
        }

        public async Task<bool> CheckEmail(string email)
        {
            return (await database.Accounts.Where(i => (i.Email.ToLower() == email.ToLower())).CountAsync()) <= 0;
        }

        public async Task<bool> VerifyAccount(string Email, string Code)
        {
            if ((Email != null) && (Code != null))
            {
                int vefificationCode;
                if (int.TryParse(Code, out vefificationCode))
                {
                    Account target = await database.Accounts.Where(i => (i.Email.ToLower() == Email.ToLower()) && (i.VerificationCode == vefificationCode) && (!i.Verified)).FirstAsync();
                    if (target != null)
                    {
                        target.Verified = true;
                        await database.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<AccountResult> LoginAccount(string Identifier, string Password)
        {
            AccountResult result = new AccountResult();
            result.CookieId = "";
            result.CookieHash = "";
            if ((Identifier != null) && (Password != null))
            {
                byte[] hashedPassword = Account.HashPassword(Password);
                Account target;
                if (Identifier.Contains("@"))
                {
                    target = await database.Accounts.Where(i => (!i.UseFacebook) && (i.Email.ToLower() == Identifier.ToLower()) && (i.PasswordHashed == hashedPassword)).FirstOrDefaultAsync();
                }
                else
                {
                    target = await database.Accounts.Where(i => (!i.UseFacebook) && (i.Username.ToLower() == Identifier.ToLower()) && (i.PasswordHashed == hashedPassword)).FirstOrDefaultAsync();
                }
                if (target != null)
                {
                    result.Ok = true;
                    result.CookieId = target.Id.ToString();
                    result.CookieHash = Convert.ToBase64String(hashedPassword);
                    return result;
                }
            }
            result.Ok = false;
            return result;
        }

        public async Task<AccountResult> LoginAccountFacebook(string FacebookID)
        {
            AccountResult result = new AccountResult();
            result.CookieId = "";
            result.CookieHash = "";
            if (FacebookID != null)
            {
                Account target;
                target = await database.Accounts.Where(i => (i.UseFacebook) && (i.FacebookID == FacebookID)).FirstOrDefaultAsync();
                if (target != null)
                {
                    result.Ok = true;
                    result.CookieId = target.Id.ToString();
                    result.CookieHash = Convert.ToBase64String(target.PasswordHashed);
                    return result;
                }
            }
            result.Ok = false;
            return result;
        }

        public void SetAccountStatus(HttpResponse response, string CookieId, string CookieHash)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddHours(6.0);
            response.Cookies.Append("userId", CookieId, options);
            response.Cookies.Append("userHash", CookieHash, options);
        }

        public async Task<Account> GetAccount(string Username, string Password)
        {
            byte[] HashedPassword = Account.HashPassword(Password);
            return await database.Accounts.Where(i => (i.Username == Username) && (i.PasswordHashed == HashedPassword)).FirstOrDefaultAsync();
        }

        public async Task<AccountStatusPair> GetAccountStatus(HttpRequest request, HttpResponse response)
        {
            AccountStatusPair asp = new AccountStatusPair();
            asp.account = null;
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
                        asp.status = AccountStatus.Invalid;
                        return asp;
                    }
                    Account target = await database.Accounts.Where(i => (i.Id == userId) && (i.PasswordHashed == userHash)).FirstOrDefaultAsync();
                    if (target != null)
                    {
                        asp.account = target;
                        if (target.Verified)
                        {
                            asp.status = AccountStatus.OK;
                            return asp;
                        }
                        asp.status = AccountStatus.Inactive;
                        return asp;
                    }
                    ClearAccountStatus(response);
                }
            }
            asp.status = AccountStatus.Invalid;
            return asp;
        }

        public void ClearAccountStatus(HttpResponse response)
        {
            response.Cookies.Delete("userId");
            response.Cookies.Delete("userHash");
        }

        public async void ForceSave()
        {
            await database.SaveChangesAsync();
        }

        public async Task<bool> CheckFacebookAccount(string FacebookID)
        {
            return (await database.Accounts.Where(i => (i.UseFacebook) && (i.FacebookID == FacebookID)).CountAsync()) <= 0;
        }

        public async Task<Post> UploadPost(Account account, string Title, string ImageURL)
        {
            Post post = new Post(Title, account, ImageURL);
            database.Posts.Add(post);
            account.Posts.Add(post);
            await database.SaveChangesAsync();
            return post;
        }

        public async Task<Post> GetPost(int id)
        {
            return await database.Posts.Where(p => (!p.Deleted) && (p.Id == id)).Include(p => p.Owner).FirstOrDefaultAsync();
        }

        public async Task<Models.UserModel> GetUserModel(string name)
        {
            Account account = await database.Accounts.Where(u => u.Username == name).Include(u => u.Posts).FirstOrDefaultAsync();
            if (account != null)
            {
                Models.UserModel model = new Models.UserModel();
                model.UserID = account.Id;
                model.UserName = account.Username;
                foreach (Post p in account.Posts)
                {
                    if (!p.Deleted)
                    {
                        Models.UserModel.PostModel pm = new Models.UserModel.PostModel();
                        pm.Id = p.Id;
                        pm.Title = p.Title;
                        pm.URL = p.PostURL;
                        model.Posts.AddFirst(pm);
                    }
                }
                return model;
            }
            return null;
        }

        public async Task<bool> DeletePost(int id, Account account)
        {
            Post post = await GetPost(id);
            if ((post != null) && (post.Owner == account))
            {
                post.Deleted = true;
                await database.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<LinkedList<Models.UserModel.PostModel>> GetRecentPosts()
        {
            LinkedList<Models.UserModel.PostModel> list = new LinkedList<Models.UserModel.PostModel>();
            List<Post> posts = await database.Posts.OrderByDescending(i => i.Id).Where(i => !i.Deleted).Take(50).ToListAsync();
            foreach (Post p in posts)
            {
                Models.UserModel.PostModel model = new Models.UserModel.PostModel();
                model.Id = p.Id;
                model.Title = p.Title;
                model.URL = p.PostURL;
                list.AddLast(model);
            }
            return list;
        }
    }
}
