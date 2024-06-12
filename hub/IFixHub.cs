namespace OrderGenerator.API.hub;

public interface IFixHub
{
    Task ReceiveMessage(string message);
}
