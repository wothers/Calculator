namespace CalculatorService;

public interface ICalculator
{
    double Calculate(double a, double b, string op);

    double? GetAnswer();

    void Clear();
}
