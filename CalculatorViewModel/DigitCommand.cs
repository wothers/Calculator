namespace CalculatorViewModel;

public class DigitCommand : BaseInputCommand
{
    public DigitCommand(MainViewModel viewModel) : base(viewModel) { }

    public override void Execute(object? parameter) => ViewModel.AppendDigit(parameter as string);
}
