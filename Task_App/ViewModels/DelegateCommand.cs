using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Task_App.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canEx;
        private ICommand command;

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        public DelegateCommand(Action<object> execute, Predicate<object> canEx = null)
        {
            this.execute = execute;
            this.canEx = canEx;
        }

        public DelegateCommand(ICommand command)
        {
            this.command = command;
        }

        public bool CanExecute(object parameter)
        {
            return canEx == null || canEx.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }
    }
}
