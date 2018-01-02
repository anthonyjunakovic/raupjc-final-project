using System;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class ActivateModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        public string Code { get; set; }
        public bool ErrorMessage { get; set; }
        public bool InfoMessage { get; set; }

        public ActivateModel()
        {
        }

        public ActivateModel(string Code, bool ErrorMessage = false, bool InfoMessage = false)
        {
            this.Code = Code;
            this.ErrorMessage = ErrorMessage;
            this.InfoMessage = InfoMessage;
        }

        public static int GenerateCode()
        {
            return (new Random()).Next(100000, 999999);
        }
    }
}
