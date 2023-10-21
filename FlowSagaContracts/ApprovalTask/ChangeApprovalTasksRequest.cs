using ESD.Domain.Enums;

namespace FlowSagaContracts.ApprovalTask
{
    public class ChangeApprovalTasksRequest
    {
        public StaffProcessStageCode StageCode { get; set; }
     
        public int DocumentId { get; set; }
        
        public CreateApprovalTasksRequest CreateApprovalTasksRequest { get; set; }
        
        public ApprovalTaskResolvedData TaskResolvedData { get; set; }
        
        public Guid? SubjectUserId { get; set; }
    }
}
