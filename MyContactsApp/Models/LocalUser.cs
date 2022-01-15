using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MyContactsApp.Models;

public class LocalUser : User
{
    [Required, DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email.")]
    [Remote(action:"VerifyEmail", controller:"User")]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [Required]
    public byte[] Password { get; set; }
    public byte[] PasswordKey { get; set; }
}