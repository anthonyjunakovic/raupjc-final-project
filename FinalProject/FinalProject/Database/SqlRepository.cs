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
    }
}
