using System; // base types like Action and Func
using System.Windows.Input; // ICommand and CommandManager

namespace cashregister.ViewModel // viewmodel namespace
{ // start namespace
    public class RelayCommand : ICommand // lightweight ICommand implementation used for data binding commands
    { // start class
        private readonly Action<object?> _execute; // action to execute when command runs
        private readonly Func<object?, bool>? _canExecute; // optional predicate that determines command executability

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) // constructor initializes execute and optional canExecute
        { // start ctor
            _execute = execute ?? throw new ArgumentNullException(nameof(execute)); // store execute or throw if null
            _canExecute = canExecute; // store optional canExecute
        } // end ctor

        public event EventHandler? CanExecuteChanged // event WPF listens to determine when to re-query executability
        { // start event
            add => CommandManager.RequerySuggested += value; // forward subscription to CommandManager
            remove => CommandManager.RequerySuggested -= value; // forward unsubscription to CommandManager
        } // end event

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true; // evaluate canExecute or return true by default

        public void Execute(object? parameter) => _execute(parameter); // invoke the execute delegate
    } // end class
} // end namespace
