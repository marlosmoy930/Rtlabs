using FlowSagaContracts.Approving;

namespace FlowSagaContracts;

public interface IEsdFlowEngineDbContext
{
    IQueryable<ApprovalSagaInstance> GetApprovalSagaInstances();
    
    Task InitDatabaseAsync();
}