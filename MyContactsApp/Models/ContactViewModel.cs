namespace MyContactsApp.Models;

public class ContactViewModel
{
    public ContactViewModel()
    {
        
    }
    public ContactViewModel(int capacity)
    {
        PhoneNumbers = new List<PhoneNumber>(capacity);
        PhoneNumbers.Add(new PhoneNumber());

        Emails = new List<Email>(capacity);
        Emails.Add(new Email());

        Addresses = new List<Address>(capacity);
        Addresses.Add(new Address());

        Pages = new List<Page>(capacity);
        Pages.Add(new Page());
    }

    public Contact Contact { get; set; }
    public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    public List<Email> Emails { get; set; }
    public List<Address> Addresses { get; set; }
    public List<Page> Pages { get; set; }
    
}