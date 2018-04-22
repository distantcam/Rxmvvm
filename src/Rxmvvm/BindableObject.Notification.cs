using System;
using System.ComponentModel;
using System.Threading;

namespace Rxmvvm
{
    partial class BindableObject : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler propertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler handler2;
                var newEvent = propertyChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (PropertyChangedEventHandler)Delegate.Combine(handler2, value);
                    Interlocked.CompareExchange(ref propertyChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
            remove
            {
                PropertyChangedEventHandler handler2;
                var newEvent = propertyChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (PropertyChangedEventHandler)Delegate.Remove(handler2, value);
                    Interlocked.CompareExchange(ref propertyChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
        }
    }
}