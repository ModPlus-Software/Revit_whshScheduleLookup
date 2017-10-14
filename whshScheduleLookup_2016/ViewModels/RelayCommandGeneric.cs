using System;
using System.Diagnostics;
using System.Windows.Input;

namespace whshScheduleLookup.ViewModels
{
    public class RelayCommandGeneric<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommandGeneric(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute != null)
            {
                _execute = execute;
            }
            else
            {
                throw new ArgumentNullException(nameof(execute));
            }
            //_execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (!(parameter is T)) return false;
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            //var tmp = (T) parameter;
            _execute((T)parameter);
        }
    }
}
