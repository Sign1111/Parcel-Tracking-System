using Microsoft.AspNetCore.Identity;
using Parcel_Tracking.Models;
using Microsoft.EntityFrameworkCore;




namespace Parcel_Tracking.Middleware
{
    public class ForceLogoutMiddleware
    {
        private readonly RequestDelegate _next;

        public ForceLogoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    var session = await db.UserSessionControls.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (session != null && session.ForceLogout)
                    {
                        session.ForceLogout = false;
                        db.UserSessionControls.Update(session);
                        await db.SaveChangesAsync();

                        await signInManager.SignOutAsync();

                        // ✅ Corrected redirect here
                        context.Response.Redirect("/Identity/Account/Login?forced=true");
                        return;
                    }
                }
            }

            await _next(context);
        }

    }

}


