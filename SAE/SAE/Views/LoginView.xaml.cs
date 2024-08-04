using SAE.ViewModels;

namespace SAE.Views;

public partial class LoginView : ContentPage
{
    const string _password = "Ae83";
    string _user_Eleazar = "eleazar";
    string _user_Daniela = "daniela";

    public LoginView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        string user = Preferences.Default.Get("user", "");
        if (!string.IsNullOrEmpty(user))
            entUser.Text = user;

#if DEBUG
        entPass.Text = _password;
#endif
    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        if (entUser.Text != _user_Eleazar)
        if (entUser.Text != _user_Daniela)
        {
            await DisplayAlert("Alerta","Usario incorrecto","Aceptar");
            return;
        }

        if (entPass.Text != _password)
        {
            await DisplayAlert("Alerta", "contraseña incorrecta", "Aceptar");
            return;
        }

        // Set a string value:
        Preferences.Default.Set("user", entUser.Text);

        await Navigation.PushAsync(new HomeView());
    }
}