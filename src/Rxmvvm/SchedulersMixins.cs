using System;
using System.Reactive.Linq;

namespace Rxmvvm
{
    public static class SchedulersMixins
    {
        public static IObservable<TSource> ObserveOnMain<TSource>(this IObservable<TSource> source) =>
            source.ObserveOn(Schedulers.MainScheduler);

        public static IObservable<TSource> ObserveOnBackground<TSource>(this IObservable<TSource> source) =>
            source.ObserveOn(Schedulers.BackgroundScheduler);

        public static IObservable<TSource> SubscribeOnMain<TSource>(this IObservable<TSource> source) =>
            source.SubscribeOn(Schedulers.MainScheduler);

        public static IObservable<TSource> SubscribeOnBackground<TSource>(this IObservable<TSource> source) =>
            source.SubscribeOn(Schedulers.BackgroundScheduler);
    }
}