using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Link11Checker.ViewModels.Base
{
    public class ParametrizedCommand : ICommand 
    {
        private Action<object> action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public ParametrizedCommand(Action<object> action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}
