using System;
using System.Linq;
using System.Reactive.Linq;

namespace Rxmvvm
{
    public static class ObservableChangedMixins
    {
        public static IObservable<PropertyChangedData> WhenPropertyChanged(
            this IObservablePropertyChanged changed, string propertyName) =>
            changed.Changed.Where(p => p.PropertyName == propertyName);

        public static IObservable<PropertyChangedData<TProperty>> WhenPropertyChanged<TProperty>(
            this IObservablePropertyChanged changed, string propertyName) =>
            changed.Changed.Where(p => p.PropertyName == propertyName).Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<PropertyChangedData> WhenPropertiesChanged(
            this IObservablePropertyChanged changed, params string[] propertyNames) =>
            changed.Changed.Where(p => propertyNames.Contains(p.PropertyName));

        public static IObservable<PropertyChangedData<TProperty>> WhenPropertiesChanged<TProperty>(
            this IObservablePropertyChanged changed, params string[] propertyNames) =>
            changed.Changed.Where(p => propertyNames.Contains(p.PropertyName)).Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<PropertyChangedData<TProperty>> CastPropertyType<TProperty>(
            this IObservable<PropertyChangedData> observable) =>
            observable.Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<TResult> BeforeAndAfter<TSource, TResult>(
            this IObservable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            return source.Scan(
                Tuple.Create(default(TSource), default(TSource)),
                (previous, current) => Tuple.Create(previous.Item2, current))
                .Select(t => resultSelector(t.Item1, t.Item2));
        }
    }
}