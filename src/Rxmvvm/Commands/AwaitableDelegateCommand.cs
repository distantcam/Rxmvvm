using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rxmvvm.Commands
{
    internal class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
    {
        public AwaitableDelegateCommand(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod = null) : base(executeMethod, canExecuteMethod)
        {
        }
    }

    internal class AwaitableDelegateCommand<T> : BaseCommand<T>, IAsyncCommand<T>
    {
        private readonly Func<T, Task> executeMethod;

        public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod = null) : base(canExecuteMethod)
        {
            this.executeMethod = executeMethod ??
                throw new ArgumentNullException(nameof(executeMethod), $"{nameof(executeMethod)} is null.");
        }

        public async Task ExecuteAsync(T parameter)
        {
            using (StartExecuting())
            {
                await executeMethod(parameter);
            }
        }

        protected override async void Execute(object parameter) => await ExecuteAsync((T)parameter);
    }
}