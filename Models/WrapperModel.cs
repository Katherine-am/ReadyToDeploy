using System.Collections.Generic;
using examTakeThree.Models;

namespace examTakeThree.Models
{
    public class WrapperModel
    {
        public User newUser {get; set;}
        public Hobby newHobby {get; set;}
        public List<Hobby> allHobbies {get; set;}
        public List<Enthusiast> allEnthusiasts {get; set;}
        public Enthusiast Enthusiast {get; set;}
        public int EnthusiastCount {get; set;}
    }
}