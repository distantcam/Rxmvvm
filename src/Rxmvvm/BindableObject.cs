using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;

namespace Rxmvvm
{
    public partial class BindableObject : IObservablePropertyChanged, IObservableDataErrorInfo, IDisposable
    {
        private long changeNotificationSuppressionCount;

        private Subject<PropertyChangedData> changed;
        private Subject<DataErrorChanged> errorChanged;

        private IObservable<PropertyChangedData> whenChanged;
        private IObservable<DataErrorChanged> whenErrorChanged;

        private CompositeDisposable disposables;

        private Lazy<ImmutableDictionary<string, PropertyInfo>> staticProperties;

        public BindableObject()
        {
            disposables = new CompositeDisposable();

            changed = new Subject<PropertyChangedData>();
            whenChanged = changed.AsObservable();
            whenChanged.ObserveOn(Schedulers.MainScheduler)
                .Subscribe(args =>
                {
                    propertyChanged?.Invoke(this, new PropertyChangedEventArgs(args.PropertyName));
                });

            errorChanged = new Subject<DataErrorChanged>();
            whenErrorChanged = errorChanged.AsObservable();
            whenErrorChanged.ObserveOn(Schedulers.MainScheduler)
                .Subscribe(args =>
                {
                    if (string.IsNullOrEmpty(args.Error))
                    {
                        string value;
                        errors.TryRemove(args.PropertyName, out value);
                    }
                    else
                    {
                        errors.AddOrUpdate(args.PropertyName, args.Error, (_, __) => args.Error);
                    }
                    errorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(args.PropertyName));
                });

            staticProperties = new Lazy<ImmutableDictionary<string, PropertyInfo>>(() =>
            {
                var type = GetType();
                return ImmutableDictionary<string, PropertyInfo>.Empty.AddRange(
                    type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                    .OfType<PropertyInfo>()
                    .ToDictionary(m => m.Name));
            });
        }

        IObservable<PropertyChangedData> IObservablePropertyChanged.Changed => whenChanged;

        IObservable<DataErrorChanged> IObservableDataErrorInfo.ErrorsChanged => whenErrorChanged;

        public bool ChangeNotificationEnabled => Interlocked.Read(ref changeNotificationSuppressionCount) == 0L;

        public IDisposable SuppressNotifications()
        {
            Interlocked.Increment(ref changeNotificationSuppressionCount);
            return Disposable.Create(() => Interlocked.Decrement(ref changeNotificationSuppressionCount));
        }

        public virtual void Dispose()
        {
            Interlocked.Exchange(ref disposables, null)?.Dispose();
            Interlocked.Exchange(ref changed, null)?.OnCompleted();
            Interlocked.Exchange(ref errorChanged, null)?.OnCompleted();
        }

        public void AddChildDisposable(IDisposable subscription) => disposables.Add(subscription);

        protected void OnPropertyChanged(string propertyName, object value)
        {
            if (ChangeNotificationEnabled)
                changed.OnNext(new PropertyChangedData(propertyName, value));
        }

        public void SetDataError(string propertyName, string error) =>
            errorChanged.OnNext(new DataErrorChanged(propertyName, error));

        public void ResetDataError(string propertyName) =>
            errorChanged.OnNext(new DataErrorChanged(propertyName, ""));
    }
}