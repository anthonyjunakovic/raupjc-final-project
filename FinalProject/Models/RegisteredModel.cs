namespace FinalProject.Models
{
    public class RegisteredModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }

        public bool IsValid()
        {
            return (IsFirstNameValid()) && (IsEmailValid());
        }

        private bool IsFirstNameValid()
        {
            return (FirstName != null) && (FirstName.Length > 0);
        }

        private bool IsEmailValid()
        {
            return (Email != null) && (Email.Length > 0);
        }
    }
}
