using System;

namespace CalculatorService;

public class SimpleCalculator : ICalculator
{
    double? _ans;

    public double Calculate(double a, double b, string op)
    {
        return (double)(_ans = op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => a / b,
            "pow" => Math.Pow(a, b),
            "nrt" => Math.Pow(a, 1 / b),
            _ => 0,
        });
    }

    public double? GetAnswer() => _ans;

    public void Clear() => _ans = null;
}
