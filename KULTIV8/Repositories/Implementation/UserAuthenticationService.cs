 using KULTIV8.Data.DTO;
using KULTIV8.Models;
using KULTIV8.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace KULTIV8.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<UserModel> signInManager;
        private readonly UserManager<UserModel> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationService(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
    
        public async Task<Status> LoginAsync(LoginModel loginModel)
        {
            Status status = new Status();

            //Check if user exists
            var user = await userManager.FindByNameAsync(loginModel.Username);
            if (user == null)
            {
                status.StatusCode = 1;
                status.Message = "Incorrect login details";
                return status;
            }

            //Check if password matches        
            if (!await userManager.CheckPasswordAsync(user,loginModel.Password))
            {
                status.StatusCode = 1;
                status.Message = "Incorrect login details";
                return status;
            }

            //Signing in the user now
            var signInResult = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles =await userManager.GetRolesAsync(user);
                var authclaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Role,user.UserName)
                };

                foreach (var role in userRoles)
                {
                    authclaim.Add(new Claim(ClaimTypes.Role, role));
                }

                status.StatusCode = 0;
                status.Message = "User Successfully Logged In";
                return status;
            }else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 1;
                status.Message = "User Locked Out";
                return status;
            }
            else
            {
                status.StatusCode = 1;
                status.Message = "Error Logging in";
                return status;
            }

        }


        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel registrationModel)
        {
            var status = new Status();
            var exists = await userManager.FindByNameAsync(registrationModel.Username);

            if (exists != null)
            {
                status.StatusCode = 1;
                status.Message = "User already exists";
                return status;
            }


            UserModel user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registrationModel.Username,
                Email = registrationModel.Email,
                EmailConfirmed = true,
                Profile_Pic = registrationModel.ProfilePic,
                Role = registrationModel.Role,
                Name = registrationModel.Name,
                Surname = registrationModel.Surname,
                
               
            };

            //Registering new user, the password will be hashed when passed through
            var result = await userManager.CreateAsync(user, registrationModel.Password);
            if (!result.Succeeded) {
                
                status.StatusCode = 1;
                status.Message = "User Creation Failed";
                return status;
            }

            //Role Manangement: Add Roles for the user, register roles into the sytem
            //Register New role
            if (!await roleManager.RoleExistsAsync(registrationModel.Role))
                await roleManager.CreateAsync(new IdentityRole(registrationModel.Role));

            //Assign the user to the role
            if(await roleManager.RoleExistsAsync(registrationModel.Role))
                await userManager.AddToRoleAsync(user, registrationModel.Role);


            status.StatusCode = 0;
            status.Message = "User successsfully created";

            return status;
        }
    }
}
