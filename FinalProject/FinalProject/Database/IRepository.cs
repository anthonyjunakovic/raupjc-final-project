namespace FinalProject.Database
{
    public interface IRepository
    {
        bool AddAccount(Account account);
        bool CheckUsername(string username);
        bool CheckEmail(string email);
    }
}
