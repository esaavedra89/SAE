using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class CustomerDetailView : ContentPage
{
    private Action _selected = null;
    public CustomerModel _customer = null;
    object _parent = null;
    APIServices _aPIServices = new APIServices();

    public CustomerDetailView(CustomerModel customer, object parent)
	{
		InitializeComponent();

        _customer = customer;
        _parent = parent;

        LoadScreen();
    }

    private async void LoadScreen()
    {
        try
        {
            if (_customer == null)
                return;

            entName.Text = _customer.Name;
            entLastname.Text = _customer.LastName;
            entNickname.Text = _customer.Nickname;
            entIdentity.Text = _customer.Identification;
            entPhoneNumber.Text = _customer.PhoneNumber;

        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        try
        {
            string user = Preferences.Default.Get("user", "Unknown");

            bool validate = await this.ValidateFields();
            if (!validate)
                return;

            if (_customer == null)
                _customer = new CustomerModel();

            _customer.Name = entName.Text;
            _customer.LastName = entLastname.Text;
            _customer.Nickname = entNickname.Text;
            _customer.Identification = entIdentity.Text;
            _customer.PhoneNumber = entPhoneNumber.Text;

            if (_customer.Id == 0)
            {
                _customer.CreatedDate = DateTime.Now;
                _customer.CreatedBy = user;
            }
            else
                _customer.UpdateddBy = user;

            CustomerModel customer = await _aPIServices.SaveCustomerModelAsync(_customer);

            await DisplayAlert("Exito", "Actualizado", "Aceptar");

            if (_parent == null)
            {
                await DisplayAlert("Ha ocurrido un error", "Cliente es nulo", "Aceptar");
            }
            else if (_parent is CustomerView)
            {
                CustomerView parent = _parent as CustomerView;
                parent.LoadScreen();
            }
            else
            {
                SearchClientView parent = _parent as SearchClientView;
                _customer = customer;

                _selected?.Invoke();

                await parent.Navigation.PopAsync();
            }

            await Task.Delay(500);

            await Navigation.PopAsync();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async Task<bool> ValidateFields()
    {
        try
        {
            if (string.IsNullOrEmpty(entName.Text))
            {
                await DisplayAlert("Advertencia", "Debe ingresar el nombre", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(entLastname.Text))
            {
                await DisplayAlert("Advertencia", "Debe ingresar el apellido", "Aceptar");
                return false;
            }

            return true;
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public void SelectedClient(Action action)
    {
        _selected = action;
    }
}