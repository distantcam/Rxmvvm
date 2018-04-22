using System;
using System.Windows.Input;

namespace Rxmvvm.Commands
{
    internal class DelegateCommand : DelegateCommand<object>, ICommand
    {
        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null) : base(executeMethod, canExecuteMethod)
        {
        }
    }

    internal class DelegateCommand<T> : BaseCommand<T>, ICommand<T>
    {
        private readonly Action<T> executeMethod;

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod = null) : base(canExecuteMethod)
        {
            this.executeMethod = executeMethod ??
                throw new ArgumentNullException(nameof(executeMethod), $"{nameof(executeMethod)} is null.");
        }

        public void Execute(T parameter)
        {
            using (StartExecuting())
            {
                executeMethod(parameter);
            }
        }

        protected override void Execute(object parameter) => Execute((T)parameter);
    }
}