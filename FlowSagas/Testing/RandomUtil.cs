namespace FlowSagas.Testing;

public static class RandomUtil
{
    static readonly Random random = new Random();

    public static int GetRandom(int minValue, int? maxNumber = null)
    {
        var maxValue = maxNumber.GetValueOrDefault(minValue * 10);
        var randomValue = random.Next(minValue, maxValue);
        return randomValue;
    }
}