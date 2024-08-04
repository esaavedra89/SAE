

namespace SAE.ViewModels
{
    public class LoginViewModel : EntityViewModel
    {
        #region Attributes

        string _user;
        string _password;

        string _user_Eleazar = "Eleazar";
        string _user_Daniela = "Daniela";

        #endregion

        #region Properties

        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        } 

        #endregion

        #region Constructor

        public LoginViewModel()
        {

        }

        #endregion

        #region Commands

        public Command LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await LoginAssync();
                });
            }
        } 

        #endregion

        private async Task LoginAssync()
        {
            
        }
    }
}
