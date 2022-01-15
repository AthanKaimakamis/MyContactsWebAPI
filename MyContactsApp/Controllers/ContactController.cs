using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyContactsApp.Models;
using MyContactsApp.Services;

namespace MyContactsApp.Controllers;
[Authorize]
public class ContactController : Controller
{
    private readonly IContactService _contactService;
    
    public ContactController(ContactService contactService)
    {
        _contactService = contactService;
    }
    
    // GET
    public IActionResult Index()
    {
        var model = _contactService.GetContactsByUserClaims(User.Claims);
        return View(model);
    }

    // GET - CREATE
    [HttpGet("contact/create")]
    public IActionResult Create()
    {
        // ViewBag.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)!.Value;
        // return View(new ContactViewModel(GetFieldsAmountByUserSubscription()));
        return View();
    }

    // POST - CREATE
    // [HttpPost("contact/create"), ValidateAntiForgeryToken]
    // public IActionResult Create(ContactViewModel model)
    // {
    //     
    //     Contact contact = _contactService.PopulateContact(model);
    //     
    //     return View(model);
    // }
    
    [HttpPost("contact/create"), ValidateAntiForgeryToken]
    public IActionResult Create(Contact contact, List<PhoneNumber> phoneNumbers)
    {
        contact.PhoneNumbers = phoneNumbers;
        // contact = _contactService.PopulateContact(model);
        
        return View(contact);
    }

    private int GetFieldsAmountByUserSubscription()
    {
        bool isSubscriber = Boolean.Parse(User.Claims.FirstOrDefault(c => c.Type == "IsSubscriber")!.Value);
        return isSubscriber ? 4 : 2;
    }
}
