namespace OnlineShopping.Api.Interfaces;
public interface IMessageProducer
{
    Task PublishProductUpdatedAsync<T>(T message);
}
