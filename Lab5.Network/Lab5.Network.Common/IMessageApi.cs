namespace Lab5.Network.Common;

public interface IMessageApi
{
    Task<bool> SendMessage(string message);
    Task<bool> OrderPizza(string pizzaType);
}
