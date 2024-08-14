using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;


namespace WebApiPortfolioApp.API.Handlers
{
    public class RegisteringHandler(IMapper mapper, UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager, IConfiguration configuration,
                              IEmailService emailService, ViewRender viewRenderer) : IRequestHandler<RegisteringRequest, RegisteringResponse>
    {
        public async Task<RegisteringResponse> Handle(RegisteringRequest request, CancellationToken cancellationToken)
        {
            var emailContent = await viewRenderer.RenderToStringAsync("SendEmail/ConfirmationEmail/ConfirmationEmail", new { UserName = request.Name });

            var userExist = await userManager.FindByNameAsync(request.Name);
            if (userExist != null)
            {
                throw new UsernameAlreadyTakenException("The username is already taken.");
            }

            var emailExist = await userManager.FindByEmailAsync(request.Email);
            if (emailExist != null)
            {
                throw new EmailNotUniqueException("The email is already in use.");
            }

            var user = mapper.Map<ApplicationUser>(request);
            IdentityResult result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                string roleName = "Admin";
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await userManager.AddToRoleAsync(user, roleName);

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = $"{configuration["FrontendUrl"]}/confirm-email?user={user.Id}&code={Uri.EscapeDataString(code)}";

                await emailService.SendEmailAsync(new EmailRequest
                {
                    ToEmail = user.Email,
                    Subject = "Confirm your email",
                    Body = emailContent.Replace("{{callbackUrl}}", callbackUrl)
                });
                var registeringDto = mapper.Map<RegisteringDto>(user);

                return new RegisteringResponse
                {
                    Data = registeringDto,
                };
            }
            else
            {
                throw new CantCreateUserExeption("Cant create user");
            }
        }
    }
}
