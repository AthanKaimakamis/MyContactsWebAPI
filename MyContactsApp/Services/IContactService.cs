using System.Security.Claims;
using MyContactsApp.Models;

namespace MyContactsApp.Services;

public interface IContactService
{
    internal Contact GetContactByName(string? firstName, string? lastName);
    internal IEnumerable<Contact> GetContactsByUserClaims(IEnumerable<Claim> userClaims);

    internal Contact AddNewContact(Contact contact);
    internal Contact PopulateContact(ContactViewModel model);
}