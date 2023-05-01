using DOT6_IdentityWeb.Models.Domain;
using DOT6_IdentityWeb.Models.DTO;
using DOT6_IdentityWeb.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace DOT6_IdentityWeb.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationService(RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();

            //find the login user by username
            var user = await userManager.FindByNameAsync(model.Username);
            // no user exist
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid Username";
                return status;
            }
            //if user existed then check the password
            if (!await userManager.CheckPasswordAsync(user, model.Password)) {
                status.StatusCode = 0;
                status.Message = "Invalid Password";
                return status;
            }

            //login process
            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (signInResult.Succeeded)
            {
                //success login
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Login Successfully";
                return status;
            }
            else if(signInResult.IsLockedOut)
            {
                status.StatusCode= 0;
                status.Message = "User Locked Out";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Login Failed";
                return status;
            }
        }
        

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            //if it is existed user
            if(userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exists";
                return status;
            }

            //if new user then create this user
            ApplicationUser user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user,model.Password);
            //if failed to create user
            if(!result.Succeeded)
            {
                status.StatusCode=0;
                status.Message = "User creation failed";
                return status;
            }

            //role management
            //if do not exist this role then create this role
            if(!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            //if the role existed, then add the new user to this role
            if(await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;//success registration
            status.Message = "User registered successfully";
            return status;
        }   
    }
}
