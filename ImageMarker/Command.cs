using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ImageMarker
{
    class Command : ICommand
    {
        Action<object> _executeMethod;
        Func<object, bool> _canexecuteMethod;

        public Command(
            Action<object> inExecuteMethod,
            Func<object, bool> inCanexecuteMethod)
        {
            _executeMethod = inExecuteMethod;
            _canexecuteMethod = inCanexecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        public event EventHandler CanExecuteChanged;

    }
}
