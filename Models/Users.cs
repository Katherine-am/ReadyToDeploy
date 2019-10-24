using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace examTakeThree.Models
{
    public class User
    {
        [Key]
        public int UserID {get; set;}

        [Required]
        [MinLength(2)]
        public string FirstName {get; set;}

        [Required]
        [MinLength(2)]
        public string LastName {get; set;}

        [Required]
        [MinLength(2)]
        public string Username {get; set;}

        [Required]
        [RegularExpression(@"^(?=^.{8,}$)(?=.*\d)(?=.*\W+)(?=.*[a-z])(?=.*[A-Z])(?i-msnx:(?!.*pass|.*password|.*word|.*god|.*\s))(?!^.*\n).*$")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters long and contain one uppercase, lowercase, and one number.")]
        public string Password {get; set;}

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string ConfirmPassword {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }

    public class UserLogin
    {
        [Required]
        [NotMapped]
        public string Username {get; set;}

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        public string Password {get; set;}
    }
}