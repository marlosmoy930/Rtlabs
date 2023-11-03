using Infrastructure.Contracts.DateTime;

namespace Infrastructure.DateTime
{
    public class DateTimeService : IDateTimeService
    {
        System.DateTimeOffset IDateTimeService.Now => System.DateTimeOffset.Now;
    }
}
