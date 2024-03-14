using KULTIV8.Data;
using KULTIV8.Data.DTO;
using KULTIV8.Models;
using KULTIV8.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace KULTIV8.Controllers
{
    public class UserController : Controller
    {
        //Accessing the DB use the DBCOntext class created
        private readonly ApplicationDBContext _db;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IWebHostEnvironment _env;


        //Use dependency injection to get an instance of the dbContext
        public UserController(ApplicationDBContext db, IUserAuthenticationService UAS, IWebHostEnvironment env)
        {
            _db = db;
            _userAuthenticationService = UAS;
            _env = env;
        }

        public async Task<IActionResult> Logout()
        {
            await _userAuthenticationService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {

            if (ModelState.IsValid)
            {
                var status = await _userAuthenticationService.LoginAsync(login);

                if(status.StatusCode == 0 )
                {
                    TempData["LoginMessage"] = status.Message;
                    return RedirectToAction("Index", "Home");
                }
                
                TempData["message"] = status.Message;
                return RedirectToAction(nameof(Login));


            }
            else
            {
                return View(login);

            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel registration)
        {
            if (ModelState.IsValid)
            {
                //if its null then set tje image
                registration.ProfilePic ??= Path.Combine(_env.WebRootPath, "img/profile_pic/default_pic.jpg");


                var status = await _userAuthenticationService.RegistrationAsync(registration);

                TempData["message"] = status.Message;

                if (status.StatusCode == 0)
                {
                    return RedirectToAction(nameof(Login));
                }

                return RedirectToAction(nameof(Register));
            }
            else
            {
                return View(registration);
            }
                
         }

        public IActionResult GetUsers()
        {
            IEnumerable<UserModel> Users = _db.Users.Where(user => user.Id != "884bc0ec-ff52-4227-bb40-169499a1be3d");
            return View(Users);
        }

        public IActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteUser(string id)
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == id);
            if(user != null)
            {
                try
                {
                    _db.Users.Remove(user);
                    _db.SaveChanges();
                }
                catch(Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return RedirectToAction("GetUsers");
        }

        //Registering new user as admin
        [HttpPost]
        public async Task<IActionResult> NewUser(RegistrationModel registration)
        {
            registration.ProfilePic ??= Path.Combine(_env.WebRootPath, "img/profile_pic/default_pic.jpg");

            if (!ModelState.IsValid) {
                TempData["Message"] = "Warning: Make sure all fields are filled";
                TempData["MessageType"] = "warning";
                return View();
            }



            registration.Password = "12345*Agh";
            registration.PasswordComfirm = "12345*Agh";


            if (!ModelState.IsValid)
                return View();

            var status = await _userAuthenticationService.RegistrationAsync(registration);

            TempData["message"] = status.Message;

            if (status.StatusCode == 0)
            {
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(NewUser));
            }
            else
            {
                TempData["MessageType"] = "danger";
                return View();
            }

            



        }

      
    }
}
