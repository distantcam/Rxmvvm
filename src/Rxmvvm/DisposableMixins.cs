using System;

namespace Rxmvvm
{
    public static class DisposableMixins
    {
        public static void DisposeWithParent(this IDisposable disposable, BindableObject parent)
        {
            parent.AddChildDisposable(disposable);
        }
    }
}