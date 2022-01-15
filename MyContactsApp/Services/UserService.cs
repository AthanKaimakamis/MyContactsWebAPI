using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using MyContactsApp.Data;
using MyContactsApp.Models;

namespace MyContactsApp.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Get & Temps

    internal Object GetUserByExternalProvider(string provider, string nameIdentifier)
    {
        if (provider.Equals("google"))
        {
            return _context.GoogleUsers.FirstOrDefault(g => g.NameIdentifier.Equals(nameIdentifier));
        }

        return default;

    }

    internal User? GetUserById(int id)
    {
        return _context.Users.Find(id);
    }

    internal async Task<User?> GetCurrentUser(IEnumerable<Claim> userClaims)
    {
        if (userClaims.Where(c => c.Type == ClaimTypes.Email) is null)
        {
            return await _context.GoogleUsers
                .FirstOrDefaultAsync(g =>
                    g.NameIdentifier == userClaims.FirstOrDefault(c =>
                        c.Type == ClaimTypes.NameIdentifier)!.Value);
        }
        return await _context.LocalUsers
            .FirstOrDefaultAsync(l =>
                l.Email == userClaims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.Email)!.Value);
    }

    private async Task<LocalUser> CreateTempLocalUserFromEmailMach(string email)
    {
        return await _context.LocalUsers.FirstOrDefaultAsync(l => l.Email == email);
    }

    private async Task<GoogleUser> CreateTempGoogleUserFromNameIdentifierMatch(string nameIdentifier)
    {
        return await _context.GoogleUsers.FirstOrDefaultAsync(g => g.NameIdentifier == nameIdentifier);
    }

    #endregion

    #region Register
    
    internal LocalUser RegisterLocalUser(LocalUser newUserData, string textPassword)
        {
            newUserData = UserPasswordHash(newUserData, textPassword);
            var entity = _context.LocalUsers.Add(newUserData);
            _context.SaveChanges();
            return entity.Entity;
        }
        
        internal GoogleUser RegisterGoogleUser(List<Claim> claims)
        {
            var newUser = new GoogleUser();
    
            newUser.NameIdentifier = claims.GetClaim(ClaimTypes.NameIdentifier);
            newUser.FirstName = claims.GetClaim(ClaimTypes.GivenName);
            newUser.LastName = claims.GetClaim(ClaimTypes.Surname);
            newUser.IsSubscribed = false;
    
            var entity = _context.GoogleUsers.Add(newUser);
            _context.SaveChanges();
    
            return entity.Entity;
        }

    #endregion

    #region Validation

    internal bool TryValidateUser(string email, string password, out List<Claim> claims)
    {
        claims = new List<Claim>();
        var appUser = CreateTempLocalUserFromEmailMach(email).Result;
        if (appUser is null)
        {
            return false;
        }
        if (!MatchPasswordHash(password, appUser.Password, appUser.PasswordKey))
        {
            return false;
        }
        
        claims.Add(new Claim(ClaimTypes.Sid, appUser.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.GivenName, appUser.FirstName));
        claims.Add(new Claim(ClaimTypes.Surname, appUser.LastName));
        claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
        claims.Add(new Claim("IsSubscriber", appUser.IsSubscribed.ToString()));
        return true;
    }

    internal async Task<bool> LocalUserExists(string userEmail)
    {
        return await _context.LocalUsers.AnyAsync(l => l.Email == userEmail);
    }
    
    #endregion

    #region Password Managment

    private LocalUser UserPasswordHash(LocalUser user, string password)
    {
        byte[] passwordHash, passwordKey;
        using (var hmac = new HMACSHA512())
        {
            passwordKey = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        user.Password = passwordHash;
        user.PasswordKey = passwordKey;

        return user;
    }

    private bool MatchPasswordHash(string passwordAttempt, byte[] userPassword, byte[] userPasswordKey)
    {
        using (var hmac = new HMACSHA512(userPasswordKey))
        {
            var attemptHashed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordAttempt));

            for (int i = 0; i < attemptHashed.Length; i++)
            {
                if (attemptHashed[i] !=  userPassword[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    #endregion

}
public static class Extensions
{
    public static string GetClaim(this List<Claim> claims, string name)
    {
        return claims.FirstOrDefault(c => c.Type == name)?.Value;
    }
}