namespace CalculatorViewModel;

public class OperationCommand : BaseInputCommand
{
    public OperationCommand(MainViewModel viewModel) : base(viewModel) { }

    public override void Execute(object? parameter) => ViewModel.Store(parameter as string);
}
