public class Customer
{
    public int Id {get; private set;}
    public string Username {get; private set;}
    public string Email {get; private set;}
    public Customer (int id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }
}