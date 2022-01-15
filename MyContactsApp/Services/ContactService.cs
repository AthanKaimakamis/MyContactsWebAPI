using System.Security.Claims;
using MyContactsApp.Data;
using MyContactsApp.Models;

namespace MyContactsApp.Services;

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;
    private readonly UserService _userService;

    public ContactService(ApplicationDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    IEnumerable<Contact> IContactService.GetContactsByUserClaims(IEnumerable<Claim> userClaims)
    {
        var user = _userService.GetCurrentUser(userClaims);
        return _context.Contacts.Where(c => c.UserId == user.Id).ToList();
    }

    Contact IContactService.AddNewContact(Contact contact)
    {
       var  entity = _context.Contacts.Add(contact);
       _context.SaveChanges();
       return entity.Entity;
    }

    Contact IContactService.PopulateContact(ContactViewModel model)
    {
        Contact contact = new Contact();
        if (model.PhoneNumbers.Any())
        {
            contact.PhoneNumbers = new List<PhoneNumber>();
            foreach (var number in model.PhoneNumbers)
            {
                contact.PhoneNumbers.Add(number);
            }
        }
        
        if (model.Emails.Any())
        {
            contact.EmailAddresses = new List<Email>();
            foreach (var email in model.Emails)
            {
                contact.EmailAddresses.Add(email);
            }
        }

        if (model.Addresses.Any())
        {
            contact.Addresses = new List<Address>();
            foreach (var address in model.Addresses)
            {
                contact.Addresses.Add(address);
            }
        }

        if (model.Pages.Any())
        {
            contact.Pages = new List<Page>();
            foreach (var page in model.Pages)
            {
                contact.Pages.Add(page);
            }
        }
        return contact;
    }

    Contact IContactService.GetContactByName(string? firstName, string? lastName)
    {
        throw new NotImplementedException();
    }
}