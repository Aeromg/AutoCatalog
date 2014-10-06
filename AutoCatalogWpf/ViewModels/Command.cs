using System;
using System.Windows.Input;

namespace AutoCatalogWpf.ViewModels
{
    public class Command : ICommand
    {
        public Func<bool> CanExecuteFunc { get; set; }
        public Action ExecuteAction { get; set; }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var handler = CanExecuteFunc;
            if (handler != null)
                return handler();

            return true;
        }

        public void Execute(object parameter)
        {
            var handler = ExecuteAction;
            if (handler != null)
                handler();
        }

        public void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}