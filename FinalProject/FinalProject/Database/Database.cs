using System.Data.Entity;

namespace FinalProject.Database
{
    public class Database : DbContext
    {
        public IDbSet<Account> Accounts { get; set; }

        public Database(string ConnectionString) : base(ConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>().HasKey(i => i.Id);
            modelBuilder.Entity<Account>().Property(i => i.Username).IsRequired();
            modelBuilder.Entity<Account>().Property(i => i.Email).IsRequired();
            modelBuilder.Entity<Account>().Property(i => i.PasswordHashed).IsRequired();
            modelBuilder.Entity<Account>().Property(i => i.FirstName).IsRequired();
            modelBuilder.Entity<Account>().Property(i => i.LastName).IsRequired();
            modelBuilder.Entity<Account>().Property(i => i.UserGender).IsRequired();
            modelBuilder.Entity<Account>().Property(i => i.Verified).IsRequired();
        }
    }
}
