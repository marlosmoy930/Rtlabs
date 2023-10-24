namespace FlowSagaContracts.Testing;

public class TestFlowSagaResponse
{
    public string CostingVersionId { get; set; }
    public bool IsValid { get; set; }
}

public class TestFlowSagaRequestBase
{
    public string CostingVersionId { get; set; }
}

public class TestFlowSagaRequest1 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequest2 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequest3 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequest4 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequest5 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequest6 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequest7 : TestFlowSagaRequestBase
{
}

public class TestFlowSagaRequestMissingConsumer1 : TestFlowSagaRequestBase
{
}

public class TestRequest1
{
    public string Value { get; set; }
}