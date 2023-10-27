using System.Reflection;

namespace FlowSagas;

public static class FlowSagaAssembly
{
    public static Assembly Assembly => typeof(FlowSagaAssembly).Assembly;
}