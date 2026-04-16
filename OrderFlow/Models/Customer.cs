public class Customer
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Customer(int id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }
    public Customer()
    {
    }
}