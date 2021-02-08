using System.ComponentModel;

namespace Framework.Ui
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName = null);
    }
}