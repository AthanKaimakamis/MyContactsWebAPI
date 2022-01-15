using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyContactsApp.Services;

public class ServicesOptions
{
    internal CookieAuthenticationEvents CookieValidateEvent()
    {
        return new CookieAuthenticationEvents()
        {
            OnSigningIn = async context =>
            {
                var scheme = context.Properties.Items.Where(k => k.Key is ".AuthScheme").FirstOrDefault();
                var claim = new Claim(scheme.Key, scheme.Value);
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                var userService =
                    context.HttpContext.RequestServices.GetRequiredService(typeof(UserService)) as UserService;
                var nameIdentifier = claimsIdentity.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier)
                    ?.Value;
                if (userService != null && nameIdentifier != null)
                {
                    var appUser = userService.GetUserByExternalProvider(scheme.Value, nameIdentifier);
                    if (appUser is null)
                    {
                        appUser = userService.RegisterGoogleUser(claimsIdentity.Claims.ToList());
                    }
                }
                claimsIdentity.AddClaim(claim);
            }
        };
    }
}