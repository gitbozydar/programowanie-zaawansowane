private bool HasItems(Order order, out string errorMessage)
{
    
}
private bool TotalAmountLimit(Order order, out string errorMessage)
{
    
}
private bool NotOutOfStock(Order order, out string errorMessage)
{
    
}

public class OrderValidator
{
    private List<ValidationRule> Rules = new List<ValidationRule>();
    public OrderValidator(Order order){
        
    }
}

public delegate bool ValidationRule(Order order, out string errorMessage);
