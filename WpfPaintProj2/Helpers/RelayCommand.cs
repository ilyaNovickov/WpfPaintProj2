using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPaintProj2.Helpers
{
    public class RelayCommand : ICommand
    {
        private Action<object> action;
        private Predicate<object> predicate;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.action = execute;
            this.predicate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.predicate == null || this.predicate(parameter);
        }

        public void Execute(object parameter)
        {
            this.action(parameter);
        }
    }
}
