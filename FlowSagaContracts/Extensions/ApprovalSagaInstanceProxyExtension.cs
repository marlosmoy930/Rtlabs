using ESD.Domain.StaffProcesses.Proxies;

using FlowSagaContracts.Approving;

using Newtonsoft.Json;

namespace FlowSagaContracts.Extensions
{
    public static class ApprovalSagaInstanceProxyExtension
    {
        public static ApprovalData GetApprovalData(this ApprovalSagaInstanceProxy approvalSagaInstanceProxy) 
            => JsonConvert.DeserializeObject<ApprovalData>(approvalSagaInstanceProxy.ApprovalDataJson!);
    }
}
