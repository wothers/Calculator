using CalculatorService;
using CalculatorViewModel;
using System.Windows;

namespace CalculatorView;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        ICalculator calculator = new SimpleCalculator();
        MainViewModel viewModel = new(calculator);
        MainView view = new(viewModel);
        view.Show();
    }
}
