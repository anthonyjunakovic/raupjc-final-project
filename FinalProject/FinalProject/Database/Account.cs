using System;
using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Database
{
    public enum Gender
    {
        Female,
        Male,
        Other
    }

    public class Account
    {
        public const string AppendSalt = "?keE7cp&d-ZMYeK";

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHashed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender UserGender { get; set; }
        public bool Verified { get; set; }
        public int VerificationCode { get; set; }

        public Account(string Username, string Email, string Password, string FirstName, string LastName, Gender UserGender)
        {
            this.Username = Username;
            this.Email = Email.ToLower();
            PasswordHashed = HashPassword(Password);
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.UserGender = UserGender;
            Verified = false;
            VerificationCode = (new Random()).Next(100000, 999999);
        }

        public byte[] HashPassword(string Password)
        {
            byte[] output;
            using (MD5 md5 = MD5.Create())
            {
                byte[] input = Encoding.UTF8.GetBytes(Password + AppendSalt);
                output = md5.ComputeHash(input);
            }
            return output;
        }

        public override bool Equals(object obj)
        {
            if (obj is Account)
            {
                if (obj is null)
                {
                    return false;
                }
                return (Id == ((Account)obj).Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Username;
        }

        public static bool operator ==(Account obj1, Account obj2)
        {
            if (obj2 is null)
            {
                if (obj1 is null)
                {
                    return true;
                }
                return obj1.Equals(obj2);
            }
            return obj2.Equals(obj1);
        }

        public static bool operator !=(Account obj1, Account obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
