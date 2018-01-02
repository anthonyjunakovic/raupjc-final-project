using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class LogInModel
    {
        [Required]
        public string Identifier { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(64)]
        public string Password { get; set; }
        public bool ErrorMessage { get; set; }
        public bool InfoMessage { get; set; }

        public LogInModel()
        {
        }

        public LogInModel(string Identifier, string Password, bool ErrorMessage = false, bool InfoMessage = false)
        {
            this.Identifier = Identifier;
            this.Password = Password;
            this.ErrorMessage = ErrorMessage;
            this.InfoMessage = InfoMessage;
        }
    }
}
