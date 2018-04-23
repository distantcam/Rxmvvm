using System.Collections.Concurrent;
using System.Dynamic;

namespace Rxmvvm
{
    partial class BindableObject : DynamicObject
    {
        private ConcurrentDictionary<string, object> dynamicProperties = new ConcurrentDictionary<string, object>();
        private object setMutex = new object();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (staticProperties.Value.TryGetValue(binder.Name, out var property) && property.CanRead)
            {
                result = property.GetValue(this);
                return true;
            }

            if (dynamicProperties.TryGetValue(binder.Name, out result))
                return true;

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (staticProperties.Value.TryGetValue(binder.Name, out var property))
            {
                if (!property.CanWrite)
                    return false;
                property.SetValue(this, value);
                return true;
            }

            lock (setMutex)
            {
                if (!dynamicProperties.TryGetValue(binder.Name, out var result))
                    result = null;

                if (!Equals(result, value))
                {
                    dynamicProperties[binder.Name] = value;
                    OnPropertyChanged(binder.Name, value);
                }
            }
            return true;
        }
    }
}