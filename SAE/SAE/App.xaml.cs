using SAE.Views;

namespace SAE
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginView());
        }
    }
}
