using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using examTakeThree.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace examTakeThree.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u=>u.Username==newUser.Username))
                {
                    ModelState.AddModelError("Username", "Username already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserID", newUser.UserID);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLogin userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u=>u.Username==userSubmission.Username);
                if(userInDb==null)
                {
                    ModelState.AddModelError("Username", "Invalid Username/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<UserLogin>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                {
                    HttpContext.Session.SetInt32("UserID", userInDb.UserID);
                    return RedirectToAction("Dashboard");
                }
            }
            else 
            {
                return View("Index");
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            }
            else
            {
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newUser = dbContext.Users.FirstOrDefault(u=>u.UserID==HttpContext.Session.GetInt32("UserID"));
                ViewModel.allHobbies = dbContext.Hobbies
                    .Include(u=>u.Creator)
                    .Include(u=>u.Enthusiast) 
                    .OrderByDescending(t=>t.Enthusiast.Count).ToList();
                int count = 0;
                foreach(Hobby hobby in ViewModel.allHobbies){
                    count += hobby.Enthusiast.Count;
                }
                ViewModel.EnthusiastCount = count;
                return View("Dashboard", ViewModel);
            }
        }

        [HttpGet("NewHobby")]
        public IActionResult NewHobby()
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            }
            else
            {   
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newUser = dbContext.Users.FirstOrDefault(u=>u.UserID==HttpContext.Session.GetInt32("UserID"));
                return View("NewHobby");
            }
        }

        [HttpPost("CreateHobby")]
        public IActionResult CreateHobby(WrapperModel Hobby)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            }
            Hobby.newHobby.UserID = (int)HttpContext.Session.GetInt32("UserID");
            if(ModelState.IsValid)
            {
                if(dbContext.Hobbies.Any(h=>h.Name==Hobby.newHobby.Name))
                {
                    ModelState.AddModelError("newHobby.Name", "Find a new hobby!");
                    return View("NewHobby");
                }
                else 
                {
                    Hobby.newHobby.UserID = (int)HttpContext.Session.GetInt32("UserID");
                    dbContext.Add(Hobby.newHobby);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return View("NewHobby", Hobby);
            }
        }

        [HttpGet("{HobbyID}")]
        public IActionResult ViewHobby(int HobbyID)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            } 
            else
            {
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newUser = dbContext.Users.FirstOrDefault(u=>u.UserID==HttpContext.Session.GetInt32("UserID"));
                ViewModel.newHobby = dbContext.Hobbies
                    .Include(u=>u.Creator)
                    .Include(u=>u.Enthusiast)
                    .ThenInclude(u=>u.User)
                    .FirstOrDefault(w=>w.HobbyID==HobbyID);
                return View("ViewHobby", ViewModel);
            }
        }

        [HttpPost("Join/{HobbyID}")]
        public IActionResult nowJoin(int HobbyID)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newUser = dbContext.Users.FirstOrDefault(u=>u.UserID==HttpContext.Session.GetInt32("UserID"));
                Hobby joinHobby = dbContext.Hobbies
                    .Include(l=>l.Enthusiast)
                    .ThenInclude(l=>l.User)
                    .SingleOrDefault(d=>d.HobbyID==HobbyID);
                Enthusiast WantsToJoin = dbContext.Enthusiasts.FirstOrDefault(d=>d.UserID==ViewModel.newUser.UserID && d.HobbyID==HobbyID);
                if(WantsToJoin == null)
                {
                    Enthusiast enthusiast = new Enthusiast
                    {
                        UserID = ViewModel.newUser.UserID,
                        HobbyID = joinHobby.HobbyID
                    };
                    dbContext.Add(enthusiast);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return RedirectToAction("Dashboard");
                }
            }
        }

        [HttpPost("Leave/{HobbyID}")]
        public IActionResult Leave(int HobbyID)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newUser = dbContext.Users.FirstOrDefault(u=>u.UserID==HttpContext.Session.GetInt32("UserID"));
                Hobby joinHobby = dbContext.Hobbies
                    .Include(l=>l.Enthusiast)
                    .ThenInclude(l=>l.User)
                    .SingleOrDefault(d=>d.HobbyID==HobbyID);
                Enthusiast WantsToJoin = dbContext.Enthusiasts.FirstOrDefault(d=>d.UserID==ViewModel.newUser.UserID && d.HobbyID==HobbyID);
                if(WantsToJoin != null)
                {
                    dbContext.Remove(WantsToJoin);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return RedirectToAction("Dashboard");
                }
            }
        }

        [HttpGet("EditHobby/{HobbyID}")]
        public IActionResult EditHobby(int HobbyID)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            }
            else
            {
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newHobby = dbContext.Hobbies.FirstOrDefault(w=>w.HobbyID==HobbyID);
                return View("EditHobby", ViewModel);
            }
        }

        [HttpPost("EditHobby/{HobbyID}")]
        public IActionResult UpdateHobby(int HobbyID, WrapperModel eHobby)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            }
            if(ModelState.IsValid)
            {
                WrapperModel ViewModel = new WrapperModel();
                Hobby UpdateHobby = dbContext.Hobbies.FirstOrDefault(h=>h.HobbyID==HobbyID);
                UpdateHobby.Name = eHobby.newHobby.Name;
                UpdateHobby.Description = eHobby.newHobby.Description;
                UpdateHobby.UpdatedAt = DateTime.Now;
                UpdateHobby.CreatedAt = UpdateHobby.CreatedAt;
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("EditHobby");
            }
        }

        [HttpGet("DeleteHobby/{HobbyID}")]
        public IActionResult DeleteHobby(int HobbyID)
        {
            if(HttpContext.Session.GetInt32("UserID")==null)
            {
                return View("Index");
            }
            else
            {
                WrapperModel ViewModel = new WrapperModel();
                ViewModel.newUser = dbContext.Users.FirstOrDefault(u=>u.UserID==HttpContext.Session.GetInt32("UserID"));
                Hobby DeleteHobby = dbContext.Hobbies.SingleOrDefault(d=>d.HobbyID==HobbyID);
                dbContext.Hobbies.Remove(DeleteHobby);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard", ViewModel); 
            }
        }

        [HttpGet("SignOut")]
        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
