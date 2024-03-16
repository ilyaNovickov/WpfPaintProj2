using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPaintProj2.Helpers
{
    public class GenericCommand<T> : ICommand
    {
        private readonly Action<T> action;
        private readonly Predicate<T> predicate;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public GenericCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.action = execute;
            this.predicate = canExecute;
        }

        public bool CanExecute(T parameter)
        {
            return this.predicate == null || this.predicate(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return this.predicate == null || this.predicate((T)parameter);
        }

        public void Execute(object parameter)
        {
            this.action((T)parameter);
        }
    }
}
