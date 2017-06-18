using System.Reactive.Concurrency;

namespace Rxmvvm
{
    public static class Schedulers
    {
        static Schedulers()
        {
            MainScheduler = DispatcherScheduler.Current;
            BackgroundScheduler = TaskPoolScheduler.Default;
        }

        public static IScheduler MainScheduler { get; set; }
        public static IScheduler BackgroundScheduler { get; set; }
    }
}