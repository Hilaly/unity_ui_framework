using System;
using System.Collections.Generic;

namespace Framework
{
    class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _innerList = new List<IDisposable>();

        public void Add(IDisposable disposable)
        {
            _innerList.Add(disposable);
        }

        public void Remove(IDisposable disposable)
        {
            _innerList.Remove(disposable);
        }
		
        public void Dispose()
        {
            foreach (var disposable in _innerList)
                disposable.Dispose();
            _innerList.Clear();
        }
    }
}