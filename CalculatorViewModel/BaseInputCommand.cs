using System;
using System.Windows.Input;

namespace CalculatorViewModel;

public abstract class BaseInputCommand : ICommand
{
    protected MainViewModel ViewModel { get; }

    public event EventHandler? CanExecuteChanged;

    public BaseInputCommand(MainViewModel viewModel) => ViewModel = viewModel;

    public bool CanExecute(object? parameter) => true;

    public abstract void Execute(object? parameter);
}
