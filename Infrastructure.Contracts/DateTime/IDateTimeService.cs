namespace Infrastructure.Contracts.DateTime
{
    public interface IDateTimeService
    {
        System.DateTimeOffset Now { get; }
    }
}
