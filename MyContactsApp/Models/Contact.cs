using System.ComponentModel.DataAnnotations;
using MyContactsApp.Utilities;

namespace MyContactsApp.Models;

public class Contact
{
    public int Id { get; set; }
    
    [MaxLength(20, ErrorMessage = "Max length is {0}")]
    public string? FirstName { get; set; }
    [MaxLength(20, ErrorMessage = "Max length is {0}")]
    public string? LastName { get; set; }
    public bool IsCompany { get; set; } = false;

    public virtual ICollection<PhoneNumber>? PhoneNumbers { get; set; }
    public virtual ICollection<Email>? EmailAddresses { get; set; }
    public virtual ICollection<Page>? Pages { get; set; }
    public virtual ICollection<Address>? Addresses { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
}

public class PhoneNumber
{
    [Key]
    public int Id { get; set; }
    
    public EPhoneTypes PhoneType { get; set; }
    [MaxLength(16),DataType(DataType.PhoneNumber)]
    public int Number { get; set; }

    public int ContactId { get; set; } 
    public virtual Contact Contact { get; set; }
    
}

public class Email
{
    public int Id { get; set; }

    public EOnlineTypes EmailType { get; set; }
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    public int ContactId { get; set; }
    public virtual Contact Contact { get; set; }
    
}

public class Page
{
    public int Id { get; set; }
    
    public EOnlineTypes PageType { get; set; }
    [DataType(DataType.Url)]
    public string PageUrl { get; set; }

    public int ContactId { get; set; }
    public virtual Contact Contact { get; set; }
}

public class Address
{
    public int Id { get; set; }

    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public int StreetNumber { get; set; }
    [DataType(DataType.PostalCode)]
    public int PostalCode { get; set; }

    public int ContactId { get; set; }
    public virtual Contact Contact { get; set; }
}