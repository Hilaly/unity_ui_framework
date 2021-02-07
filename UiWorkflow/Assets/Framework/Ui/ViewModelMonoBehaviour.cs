using System;
using System.ComponentModel;
using Framework.Flow;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Ui
{
    public abstract class ViewModelMonoBehaviour : MonoBehaviour, IViewModel, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

#if USE_ZENJECT
        [Inject]
#endif
        public void InjectDependencies(IAppRouter router)
        {
            AppRouter = router;
        }
        
        protected void AddOnDispose(IDisposable d)
        {
            _disposable.Add(d);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        
        public IAppRouter AppRouter { get; private set; }

        
        //We need it for UnityWeld working
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}