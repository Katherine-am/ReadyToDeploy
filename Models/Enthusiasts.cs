using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace examTakeThree.Models
{
    public class Enthusiast
    {
        public int EnthusiastID {get; set;}
        public int UserID {get; set;}
        public int HobbyID {get; set;}
        public Hobby Hobby {get; set;}
        public User User {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}
