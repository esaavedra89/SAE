using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class SearchClientView : ContentPage
{
	private Action _selected = null;
    APIServices _apiSrvices = new APIServices();
    //private object _father = null;
    public CustomerModel _customerSelected = null;
    private List<CustomerModel> _listClients = null;

    //public SearchClientView(object father)
    public SearchClientView()
	{
		InitializeComponent();
		//_father = father;
		//
		LoadScreen();
    }

    private async void LoadScreen()
    {
		try
		{
            List<CustomerModel> listCustomers = await _apiSrvices.GetCustomers();
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

    private void btnSearch_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(entName.Text))
            {
                lvCustomers.ItemsSource = _listClients.Where(m => m.Name.ToLower().Contains(entName.Text, StringComparison.InvariantCulture)).ToList();
            }
        }
        catch (Exception exc)
        {
            
        }
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
}