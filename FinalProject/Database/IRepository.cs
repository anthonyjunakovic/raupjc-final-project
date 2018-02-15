using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FinalProject.Database
{
    public enum AccountStatus
    {
        OK,
        Inactive,
        Invalid
    }

    public interface IRepository
    {
        bool AddAccount(Account account);
        bool CheckUsername(string username);
        bool CheckEmail(string email);
        bool VerifyAccount(string Email, string Code);
        bool LoginAccount(string Identifier, string Password, out string CookieId, out string CookieHash);
        bool LoginAccountFacebook(string FacebookID, out string CookieId, out string CookieHash);
        void SetAccountStatus(HttpResponse response, string CookieId, string CookieHash);
        Account GetAccount(string Username, string Password);
        AccountStatus GetAccountStatus(HttpRequest request, HttpResponse response, out Account account);
        void ClearAccountStatus(HttpResponse response);
        void ForceSave();
        bool CheckFacebookAccount(string FacebookID);
        Post UploadPost(Account account, string Title, string ImageURL);
        Post GetPost(int id);
        Models.UserModel GetUserModel(string name);
        bool DeletePost(int id, Account account);
        LinkedList<Models.UserModel.PostModel> GetRecentPosts();
    }
}
