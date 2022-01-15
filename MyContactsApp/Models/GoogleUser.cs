using System.ComponentModel.DataAnnotations;

namespace MyContactsApp.Models;

public class GoogleUser : User
{
    public string NameIdentifier { get; set; }
}