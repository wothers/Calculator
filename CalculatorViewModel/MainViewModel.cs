using CalculatorService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Windows.Input;

namespace CalculatorViewModel;

public class MainViewModel : ObservableObject
{
    ICalculator _calculator;
    string _entry;
    string? _buffered1, _buffered2;
    string? _op;
    bool _entryOverwritable;

    string Entry
    {
        get => _entry;
        set
        {
            SetProperty(ref _entry, value);
            OnPropertyChanged(nameof(DisplayEntry));
        }
    }

    public string DisplayEntry
    {
        get
        {
            if (Entry.Length == 0) return "0";

            StringBuilder builder = new();
            int index = 0;
            for (int i = Entry.Contains('.') ? Entry.IndexOf(".") - 1 : Entry.Length - 1; i >= 0; i--)
            {
                builder.Append(Entry[index++]);
                if (i % 3 == 0 && i > 0 && builder[index - 1] != '-') builder.Append(',');
            }

            if (Entry.Contains('.')) return builder.ToString() + Entry[Entry.IndexOf(".")..];
            else return builder.ToString();
        }
    }

    public string DisplayBuffered
    {
        get
        {
            if (_buffered1 == null) return string.Empty;
            if (_buffered2 == null) return _op != null ? $"{_buffered1} {_op}" : $"{_buffered1} =";
            return $"{_buffered1} {_op} {_buffered2} =";
        }
    }

    public bool HasAnswer => _calculator.GetAnswer() != null;

    public string Zero => "0";
    public string One => "1";
    public string Two => "2";
    public string Three => "3";
    public string Four => "4";
    public string Five => "5";
    public string Six => "6";
    public string Seven => "7";
    public string Eight => "8";
    public string Nine => "9";
    public string Plus => "+";
    public string Minus => "-";
    public string Multiply => "*";
    public string Divide => "/";

    public ICommand BackspaceCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ClearEntryCommand { get; }
    public ICommand DecimalPointCommand { get; }
    public ICommand DigitCommand { get; }
    public ICommand EvaluateCommand { get; }
    public ICommand NegateCommand { get; }
    public ICommand OperationCommand { get; }
    public ICommand PercentCommand { get; }
    public ICommand ReciprocalCommand { get; }
    public ICommand SquareCommand { get; }
    public ICommand SquareRootCommand { get; }

    public MainViewModel(ICalculator calculator)
    {
        _calculator = calculator;

        BackspaceCommand = new RelayCommand(Backspace);
        ClearCommand = new RelayCommand(ClearAll);
        ClearEntryCommand = new RelayCommand(ClearEntry);
        DecimalPointCommand = new RelayCommand(AppendDecimalPoint);
        DigitCommand = new DigitCommand(this);
        EvaluateCommand = new RelayCommand(Evaluate);
        NegateCommand = new RelayCommand(Negate);
        OperationCommand = new OperationCommand(this);
        PercentCommand = new RelayCommand(Percent);
        ReciprocalCommand = new RelayCommand(Reciprocal);
        SquareCommand = new RelayCommand(Square);
        SquareRootCommand = new RelayCommand(SquareRoot);

        ClearEntry();
    }

    public void AppendDecimalPoint()
    {
        if (HasAnswer) ClearBuffered();
        if (_entryOverwritable) ClearEntry();
        if (Entry.Contains('.')) return;

        Entry += ".";
        _entryOverwritable = false;
    }

    public void AppendDigit(string digit)
    {
        if (HasAnswer) ClearBuffered();
        if (digit == "0" && Entry == "0") return;

        if (Entry == "0" || _entryOverwritable) Entry = digit;
        else Entry += digit;
        _entryOverwritable = false;
    }

    public void Backspace()
    {
        if (_entryOverwritable)
        {
            ClearBuffered();
            _entryOverwritable = false;
            return;
        }

        Entry = Entry.Length > 1 ? Entry[..^1] : "0";
        if (Entry == "-") Entry = "0";
        _entryOverwritable = false;
    }

    public void ClearAll()
    {
        ClearBuffered();
        ClearEntry();
    }

    public void ClearEntry()
    {
        Entry = "0";
    }

    public void Evaluate()
    {
        if (_op != null)
        {
            _buffered2 ??= RemoveTrailingDecimalPoint(Entry);
            double ans = (double)_calculator.Calculate(double.Parse(_buffered1), double.Parse(_buffered2), _op);
            OnPropertyChanged(nameof(DisplayBuffered));
            _buffered1 = ans.ToString();
            Entry = ans.ToString();
        }
        else
        {
            Entry = RemoveTrailingDecimalPoint(Entry);
            _buffered1 = Entry;
            _calculator.Calculate(double.Parse(_buffered1), 0, "+");
            OnPropertyChanged(nameof(DisplayBuffered));
        }
        _entryOverwritable = true;
    }

    public void Negate()
    {
        if (Entry == "0") return;

        if (Entry[0] == '-') Entry = Entry[1..];
        else Entry = "-" + Entry;
        if (_buffered1 != null)
        {
            if (_buffered1[0] == '-') _buffered1 = _buffered1[1..];
            else _buffered1 = '-' + _buffered1;
        }
    }

    public void Percent()
    {
        if (HasAnswer) ClearBuffered();
        Entry = _calculator.Calculate(double.Parse(Entry), 100, "/").ToString();
        _entryOverwritable = true;
    }

    public void Reciprocal()
    {
        if (HasAnswer) ClearBuffered();
        Entry = _calculator.Calculate(1, double.Parse(Entry), "/").ToString();
        _entryOverwritable = true;
    }

    public void Square()
    {
        if (HasAnswer) ClearBuffered();
        Entry = _calculator.Calculate(double.Parse(Entry), 2, "pow").ToString();
        _entryOverwritable = true;
    }

    public void SquareRoot()
    {
        if (HasAnswer) ClearBuffered();
        Entry = _calculator.Calculate(double.Parse(Entry), 2, "nrt").ToString();
        _entryOverwritable = true;
    }

    public void Store(string op)
    {
        if (_buffered1 != null && !_entryOverwritable) Evaluate();
        if (HasAnswer) ClearBuffered();
        Entry = RemoveTrailingDecimalPoint(Entry);
        _buffered1 ??= Entry;
        _op = op;
        OnPropertyChanged(nameof(DisplayBuffered));
        _entryOverwritable = true;
    }

    private void ClearBuffered()
    {
        _buffered1 = null;
        _buffered2 = null;
        _op = null;
        _calculator.Clear();
        OnPropertyChanged(nameof(DisplayBuffered));
    }

    private string RemoveTrailingDecimalPoint(string value)
    {
        if (value[^1] == '.') return value[..^1];
        return value;
    }
}
