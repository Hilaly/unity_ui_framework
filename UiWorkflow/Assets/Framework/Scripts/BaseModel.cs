using System;
using System.Collections.Generic;

namespace Framework.Flow
{
    public class BaseModel<T> where T : BaseModel<T>
    {
        //TODO: add support for multithreading
        
        private readonly HashSet<ISubscription> _subscriptions = new HashSet<ISubscription>();
        private readonly List<ISubscription> _cache = new List<ISubscription>();
        private bool _dirty;

        class ModelSubscription : ISubscription, IDisposable
        {
            private readonly Action _subscription;
            private readonly Action<ISubscription> _unsubscribe;

            public ModelSubscription(Action subscription, Action<ISubscription> unsubscribe)
            {
                _subscription = subscription;
                _unsubscribe = unsubscribe;
            }

            public void Notify()
            {
                _subscription();
            }

            public void Dispose()
            {
                _unsubscribe(this);
            }
        }

        void Unsubscribe(ISubscription subscription)
        {
            _subscriptions.Remove(subscription);
            _dirty = true;
        }
        
        public IDisposable Subscribe(Action subscription)
        {
            var s = new ModelSubscription(subscription, Unsubscribe);

            _subscriptions.Add(s);
            _dirty = true;

            return s;
        }
        
        public IDisposable Subscribe(IModelObserver<T> subscription)
        {
            var s = new ModelSubscription(() => subscription.ModelChanged((T) this), Unsubscribe);

            _subscriptions.Add(s);
            _dirty = true;

            return s;
        }

        public void TriggerChange()
        {
            if (_dirty)
            {
                _cache.Clear();
                _cache.AddRange(_subscriptions);
            }

            foreach (var subscription in _cache) 
                subscription.Notify();
        }
    }
}