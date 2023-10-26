using FlowSagaContracts.Approving;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowSagas.Approving;

public class ApprovalSagaInstanceMap : SagaClassMap<ApprovalSagaInstance>
{
    private const int currentStateMaxLength = 25;
    private const int userEmailMaxLength = 254;
    private const int parentSagaAddressMaxLength = 2048;
    private const int guidStringMaxLength = 38;
    private const string tableName = "ApprovalSaga_Instances";

    protected override void Configure(EntityTypeBuilder<ApprovalSagaInstance> entity, ModelBuilder model)
    {
        entity.ToTable(tableName);

        entity.Property(x => x.CurrentState).HasMaxLength(currentStateMaxLength);
        entity.Property(x => x.LogCorrelationId).HasMaxLength(guidStringMaxLength);
        entity.Property(x => x.StartedAt);
        entity.Property(x => x.UserId);
        entity.Property(x => x.UserEmail).HasMaxLength(userEmailMaxLength);
        entity.Property(x => x.ParentSagaCorrelationId);
        entity.Property(x => x.ParentSagaFlowTaskId);
        entity.Property(x => x.ParentSagaAddress).HasMaxLength(parentSagaAddressMaxLength);
        entity.Property(x => x.CurrentStepIndex);
        entity.Property(x => x.DocumentId);
        entity.Property(x => x.StaffProcessStageId);

        entity.Property(x => x.ApprovalDataJson);
    }
}