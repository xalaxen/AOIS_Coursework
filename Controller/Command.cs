using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductsDelliverySystem
{
    public class Command : ICommand
    {
        private readonly Func<Task> executeAsync;
        private readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public Command(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        private async Task ExecuteAsync(object parameter)
        {
            await executeAsync();
        }
    }
}
