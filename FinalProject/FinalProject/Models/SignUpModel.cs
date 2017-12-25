using FinalProject.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class SignUpModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(64)]
        public string Password { get; set; }
        [Required]
        public string RepeatPassword { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }

        public Gender GetGender()
        {
            switch (Gender)
            {
                case "0":
                    return Database.Gender.Female;
                case "1":
                    return Database.Gender.Male;
                default:
                    return Database.Gender.Other;
            }
        }

        public bool IsModelValid()
        {
            return (IsPasswordValid()) && (IsGenderValid());
        }

        private bool IsPasswordValid()
        {
            return (Password == RepeatPassword);
        }

        private bool IsGenderValid()
        {
            return (Gender == "0") || (Gender == "1") || (Gender == "2");
        }
    }
}
