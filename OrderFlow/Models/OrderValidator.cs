public delegate bool ValidationRule(Order order, out string errorMessage);


public class OrderValidator
{
    private List<ValidationRule> Rules = new List<ValidationRule>();
    private List<Func<Order, bool>> SimpleRules = new List<Func<Order, bool>>();
    public OrderValidator()
    {
        Rules.Add(HasItems);
        Rules.Add(TotalAmountLimit);
        Rules.Add(NotOutOfStock);

        SimpleRules.Add(order => order.TotalPrice > 0);
        SimpleRules.Add(order => order.Status != OrderStatus.Cancelled);
    }
  public bool Validate(Order order, out List<string> errors)
    {
    errors = new List<string>();

    foreach (var rule in Rules)
    {
        if (!rule(order, out string errorMessage))
        {
            errors.Add(errorMessage);
        }
    }

    foreach (var rule in SimpleRules)
    {
        if (!rule(order))
        {
            errors.Add("Simple rule failed.");
        }
    }

    return errors.Count == 0;
    }

    private bool HasItems(Order order, out string errorMessage)
    {
        if(order.Items == null || order.Items.Count == 0)
        {
            errorMessage = "No items in order.";
            return false;
        }
        errorMessage = "";
        return true;
    }

    private bool TotalAmountLimit(Order order, out string errorMessage)
    {
        if(order.TotalPrice > 10000)
        {
            errorMessage = "Price limit reached.";
            return false;
        }
        errorMessage = "";
        return true;
    }

    private bool NotOutOfStock(Order order, out string errorMessage)
    {
        foreach (var item in order.Items)
        {
            if(item.Quantity <= 0)
            {
                errorMessage = "Out of stock.";
                return false;
            }
        }
        errorMessage = "";
        return true;
    }
}