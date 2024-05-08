using System.Windows.Input;

namespace Core
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T?> _canExecute;
        private readonly Action<T?> _execute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            T? param = (parameter is not null) ? ((T)parameter) : default;
            return _canExecute(param);
        }

        public void Execute(object? parameter)
        {
            T? param = (parameter is not null) ? ((T)parameter) : default;
            _execute(param);
        }

        public RelayCommand(Action<T?> execute)
        {
            _execute = execute;
            _canExecute = p => true;
        }

        public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object?> execute) : base(execute) { }

        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute) : base(execute, canExecute) { }
    }
}
