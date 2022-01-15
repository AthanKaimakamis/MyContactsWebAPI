using Microsoft.EntityFrameworkCore;
using MyContactsApp.Models;

namespace MyContactsApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<User?> Users { get; set; }
    public DbSet<LocalUser> LocalUsers { get; set; }
    public DbSet<GoogleUser> GoogleUsers { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Page> Pages { get; set; }
    
}