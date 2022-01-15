using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace MyContactsApp.Models;

public abstract class User
{
    public int Id { get; set; }
    [Required, MinLength(2, ErrorMessage = "Name is too short"), DisplayName("First Name")]
    public string FirstName { get; set; }
    [Required, MinLength(2, ErrorMessage = "Name is too short"), DisplayName("Last Name")]
    public string LastName { get; set; }
    public bool IsSubscribed { get; set; } = false;
    
    public virtual ICollection<Contact> Contacts { get; set; }
}