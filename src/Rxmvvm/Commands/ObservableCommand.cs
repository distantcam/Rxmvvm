using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rxmvvm.Commands
{
    internal class ObservableCommand : IObservableCommand, IRaiseCanExecuteChanged
    {
        private EventHandler canExecuteChanged;
        private Func<object, Task> action;
        private IDisposable canExecuteSubscription;
        private bool latest;
        private bool isExecuting;

        public ObservableCommand(IObservable<bool> canExecuteObservable, Func<object, Task> action)
        {
            this.action = action;

            canExecuteSubscription = canExecuteObservable
                .ObserveOn(Schedulers.MainScheduler)
                .Subscribe(b =>
                {
                    latest = b;
                    RaiseCanExecuteChanged();
                });
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { canExecuteChanged += value; }
            remove { canExecuteChanged -= value; }
        }

        public bool CanExecute(object parameter) => !isExecuting && latest;

        public async void Execute(object parameter) => await ExecuteAsync(parameter);

        public void RaiseCanExecuteChanged()
        {
            canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref canExecuteSubscription, null)?.Dispose();
        }

        private async Task ExecuteAsync(object parameter)
        {
            using (StartExecuting())
                await action(parameter);
        }

        private IDisposable StartExecuting()
        {
            isExecuting = true;
            RaiseCanExecuteChanged();

            return Disposable.Create(() =>
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            });
        }
    }
}