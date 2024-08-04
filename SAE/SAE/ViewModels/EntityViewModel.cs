using System.ComponentModel;

namespace SAE.ViewModels
{
    public class EntityViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Variables

        bool _busy;

        #endregion

        #region Properties

        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                OnPropertyChanged("Busy");
            }
        } 

        #endregion
    }
}
