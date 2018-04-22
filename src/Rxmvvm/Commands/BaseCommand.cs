using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Windows.Input;

namespace Rxmvvm.Commands
{
    internal abstract class BaseCommand<T> : IRaiseCanExecuteChanged, ICommand
    {
        private EventHandler canExecuteChanged;
        private readonly Func<T, bool> canExecuteMethod;
        private SemaphoreSlim isExecuting = new SemaphoreSlim(1);

        public BaseCommand(Func<T, bool> canExecuteMethod = null)
        {
            this.canExecuteMethod = canExecuteMethod;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { canExecuteChanged += value; }
            remove { canExecuteChanged -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(T parameter)
        {
            if (isExecuting.CurrentCount == 0)
                return false;
            if (canExecuteMethod == null)
                return true;

            return canExecuteMethod(parameter);
        }

        bool ICommand.CanExecute(object parameter) => CanExecute((T)parameter);
        void ICommand.Execute(object parameter) => throw new NotImplementedException();

        protected abstract void Execute(object parameter);

        protected IDisposable StartExecuting()
        {
            isExecuting.Wait();
            RaiseCanExecuteChanged();

            return Disposable.Create(() =>
            {
                isExecuting.Release();
                RaiseCanExecuteChanged();
            });
        }
    }
}