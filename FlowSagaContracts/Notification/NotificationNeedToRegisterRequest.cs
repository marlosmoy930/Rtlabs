using ESD.Domain.Enums;

namespace FlowSagaContracts.Notification;

public record NotificationNeedToRegisterRequest : NotificationRequest
{
    public Guid DepartmentId { get; set; }

    public int StageId { get; set; }
}
