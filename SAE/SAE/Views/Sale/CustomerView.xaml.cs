using SAE.Models;
using SAE.Models.Inventory;
using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class CustomerView : ContentPage
{

    #region Variables

    APIServices _apiSrvices = new APIServices();
    List<CustomerModel> listCustomers = new List<CustomerModel>();
    MenuItemModel _itemSelected = null;

    #endregion

    #region Constructors

    public CustomerView(MenuItemModel itemSelected)
    {
        InitializeComponent();

        _itemSelected = itemSelected;
        lblTitle.Title = "Clientes";
        LoadScreen();
    }

    #endregion

    #region Methods

    public async void LoadScreen()
    {
        try
        {
            listCustomers = await _apiSrvices.GetCustomers();
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

    #endregion

    #region Events

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

    private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            SearchBar searchBar = (SearchBar)sender;
            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                List<CustomerModel> list = listCustomers.Where(x => x.FullName.Contains(searchBar.Text, StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x.Name).ToList();

                lvCustomers.ItemsSource = new List<CustomerModel>();
                lvCustomers.ItemsSource = list;
            }
            else
            {
                lvCustomers.ItemsSource = new List<CustomerModel>();
                lvCustomers.ItemsSource = listCustomers.OrderBy(m => m.Name);
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    } 

    #endregion
}