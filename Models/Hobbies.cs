using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace examTakeThree.Models
{
    public class Hobby
    {
        [Key]
        public int HobbyID {get; set;}
        
        [Required]
        [MinLength(2)]
        public string Name {get; set;}

        [Required]
        [MinLength(10)]
        public string Description {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;

        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        public int UserID {get; set;}

        public User Creator {get; set;}

        public List<Enthusiast> Enthusiast {get; set;}
    }
}