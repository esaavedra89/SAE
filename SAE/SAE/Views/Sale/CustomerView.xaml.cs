using SAE.Models;
using SAE.Models.Inventory;
using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class CustomerView : ContentPage
{
    MenuItemModel _itemSelected = null;
    APIServices _apiSrvices = new APIServices();

    public CustomerView(MenuItemModel itemSelected)
	{
		InitializeComponent();

        _itemSelected = itemSelected;
        lblTitle.Title = "Clientes";
        LoadScreen();
    }

    public async void LoadScreen()
    {
        try
        {
            List<CustomerModel> listCustomers = await _apiSrvices.GetCustomers();
            if (listCustomers.Count > 0)
                lvCustomers.ItemsSource = listCustomers.OrderBy(m => m.Name);
            else
                lvCustomers.ItemsSource = new List<CustomerModel>();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message + ". " + exc.HelpLink + ". " + exc.StackTrace + ". " + exc.Source, "Aceptar");
        }
    }

    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new CustomerDetailView(null, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void lvCustomer_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            CustomerModel itemSelected = e.SelectedItem as CustomerModel;
            if (itemSelected == null) return;

            await Navigation.PushAsync(new CustomerDetailView(itemSelected, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }
}