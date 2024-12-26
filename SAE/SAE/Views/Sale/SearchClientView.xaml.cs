using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class SearchClientView : ContentPage
{
    #region Variables

    private Action _selected = null;
    APIServices _apiSrvices = new APIServices();
    public CustomerModel _customerSelected = null;
    private List<CustomerModel> _listClients = null;
    List<CustomerModel> listCustomers = new List<CustomerModel>();

    #endregion

    #region Constructors

    public SearchClientView()
    {
        InitializeComponent();
        LoadScreen();
    }

    #endregion

    #region Methods

    private async void LoadScreen()
    {
        try
        {
            listCustomers = await _apiSrvices.GetCustomers();
            if (listCustomers.Count > 0)
                _listClients = listCustomers.OrderBy(m => m.Name).ToList();
            else
                _listClients = new List<CustomerModel>();

            lvCustomers.ItemsSource = _listClients;
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    public void SelectedClient(Action action)
    {
        _selected = action;
    }

    private async void btnAdd_Clicked(object sender, EventArgs e)
    {
        try
        {
            var client = new CustomerDetailView(null, this);

            client.SelectedClient(() => this.AsyncInvoke(client._customer));

            await Navigation.PushAsync(client);
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private void AsyncInvoke(CustomerModel itemSelected)
    {
        _customerSelected = itemSelected;

        _selected?.Invoke();
    }

    #endregion

    #region Events

    private async void lvCustomers_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            CustomerModel itemSelected = e.SelectedItem as CustomerModel;
            if (itemSelected == null) return;

            this.AsyncInvoke(itemSelected);
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