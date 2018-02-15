using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalProject.Database
{
    public enum AccountStatus
    {
        OK,
        Inactive,
        Invalid
    }

    public struct AccountResult
    {
        public bool Ok { get; set; }
        public string CookieId { get; set; }
        public string CookieHash { get; set; }
    }

    public struct AccountStatusPair
    {
        public AccountStatus status { get; set; }
        public Account account { get; set; }
    }

    public interface IRepository
    {
        Task<bool> AddAccount(Account account);
        Task<bool> CheckUsername(string username);
        Task<bool> CheckEmail(string email);
        Task<bool> VerifyAccount(string Email, string Code);
        Task<AccountResult> LoginAccount(string Identifier, string Password);
        Task<AccountResult> LoginAccountFacebook(string FacebookID);
        void SetAccountStatus(HttpResponse response, string CookieId, string CookieHash);
        Task<Account> GetAccount(string Username, string Password);
        Task<AccountStatusPair> GetAccountStatus(HttpRequest request, HttpResponse response);
        void ClearAccountStatus(HttpResponse response);
        void ForceSave();
        Task<bool> CheckFacebookAccount(string FacebookID);
        Task<Post> UploadPost(Account account, string Title, string ImageURL);
        Task<Post> GetPost(int id);
        Task<Models.UserModel> GetUserModel(string name);
        Task<bool> DeletePost(int id, Account account);
        Task<LinkedList<Models.UserModel.PostModel>> GetRecentPosts();
    }
}
