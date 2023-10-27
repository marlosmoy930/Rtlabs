namespace FlowSagas
{
    public interface IFlowSagaWithUserInteractionSagaData : IFlowSagaDataBase
    {
        string UserInteractionSagaResult { get; set; }
    }
}
