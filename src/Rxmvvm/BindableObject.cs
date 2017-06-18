﻿using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Rxmvvm
{
    public partial class BindableObject : IObservablePropertyChanged, IObservablePropertyChanging, IObservableDataErrorInfo, IDisposable
    {
        private long changeNotificationSuppressionCount;

        private Subject<PropertyChangedData> changed;
        private Subject<PropertyChangingData> changing;
        private Subject<DataErrorChanged> errorChanged;

        private IObservable<PropertyChangedData> whenChanged;
        private IObservable<PropertyChangingData> whenChanging;
        private IObservable<DataErrorChanged> whenErrorChanged;

        private CompositeDisposable disposables;

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

            changing = new Subject<PropertyChangingData>();
            whenChanging = changing.AsObservable();
            whenChanging.ObserveOn(Schedulers.MainScheduler)
                .Subscribe(args =>
                {
                    propertyChanging?.Invoke(this, new PropertyChangingEventArgs(args.PropertyName));
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
        }

        IObservable<PropertyChangedData> IObservablePropertyChanged.Changed => whenChanged;

        IObservable<PropertyChangingData> IObservablePropertyChanging.Changing => whenChanging;

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
            Interlocked.Exchange(ref changing, null)?.OnCompleted();
            Interlocked.Exchange(ref changed, null)?.OnCompleted();
            Interlocked.Exchange(ref errorChanged, null)?.OnCompleted();
        }

        public void AddChildDisposable(IDisposable subscription)
        {
            disposables.Add(subscription);
        }

        protected void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (ChangeNotificationEnabled)
                changed.OnNext(new PropertyChangedData(propertyName, before, after));
        }

        protected void OnPropertyChanging(string propertyName, object before)
        {
            if (ChangeNotificationEnabled)
                changing.OnNext(new PropertyChangingData(propertyName, before));
        }

        public void SetDataError(string propertyName, string error)
        {
            errorChanged.OnNext(new DataErrorChanged(propertyName, error));
        }

        public void ResetDataError(string propertyName)
        {
            errorChanged.OnNext(new DataErrorChanged(propertyName, ""));
        }
    }
}