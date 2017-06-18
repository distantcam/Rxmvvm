using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rxmvvm
{
    public interface IAsyncCommand : IAsyncCommand<object>
    {
    }

    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged, ICommand
    {
        Task ExecuteAsync(T obj);

        bool CanExecute(T obj);
    }

    public interface ICommand<in T> : IRaiseCanExecuteChanged, ICommand
    {
        void Execute(T obj);

        bool CanExecute(T obj);
    }

    public interface IObservableCommand : IDisposable, ICommand
    {
    }

    public interface IObservableDataErrorInfo
    {
        IObservable<DataErrorChanged> ErrorsChanged { get; }
    }

    public interface IObservablePropertyChanged
    {
        IObservable<PropertyChangedData> Changed { get; }
    }

    public interface IObservablePropertyChanging
    {
        IObservable<PropertyChangingData> Changing { get; }
    }

    public interface IRaiseCanExecuteChanged
    {
        void RaiseCanExecuteChanged();
    }
}