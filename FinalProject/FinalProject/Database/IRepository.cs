using Microsoft.AspNetCore.Http;

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
        void SetAccountStatus(HttpResponse response, string CookieId, string CookieHash);
        AccountStatus GetAccountStatus(HttpRequest request, HttpResponse response, out Account account);
        void ClearAccountStatus(HttpResponse response);
        void ForceSave();
    }
}
