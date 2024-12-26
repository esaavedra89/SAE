using SAE.Models;
using SAE.Models.Inventory;
using SAE.Services;

namespace SAE.Views.Inventory;

public partial class ItemView : ContentPage
{

    #region Variables

    APIServices _apiSrvices = new APIServices();
    List<ItemModel> items = new List<ItemModel>();
    MenuItemModel _menuSelected = null;

    #endregion

    #region Constructors

    public ItemView(MenuItemModel menuSelected)
    {
        InitializeComponent();
        _menuSelected = menuSelected;

        LoadScreen();
    }

    #endregion

    #region Methods

    public async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);

            items = await _apiSrvices.GetItems();
            if (items.Count > 0)
                lvItems.ItemsSource = items.OrderBy(m => m.Name);
            else
                lvItems.ItemsSource = new List<ItemModel>();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message + ". " + exc.HelpLink + ". " + exc.StackTrace + ". " + exc.Source, "Aceptar");
        }

        this.UpdateBusyIndicator(false);
    }

    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new ItemDetailView(null, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private void RefreshScreen()
    {
        lvItems.ItemsSource = new List<ItemModel>();

        LoadScreen();
    }

    private void UpdateBusyIndicator(bool value)
    {
        busyIndicator.IsRunning = value;
        busyIndicator.IsVisible = value;

        stkMain.IsEnabled = !value;
    }

    private void btnRefresh_Clicked(object sender, EventArgs e)
    {
        this.RefreshScreen();
    }

    #endregion

    #region Events

    private async void lvItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            ItemModel itemSelected = e.SelectedItem as ItemModel;
            if (itemSelected == null) return;

            string action = await DisplayActionSheet("Opciones", "Cancel", null, "Ver", "Eliminar");
            if (action == "Ver")
                await Navigation.PushAsync(new ItemDetailView(itemSelected, this));
            else if (action == "Eliminar")
            {
                bool answer = await DisplayAlert("Pregunta", "Desea eliminar este item?", "Si", "No");
                if (answer)
                {
                    this.UpdateBusyIndicator(true);

                    bool response = await _apiSrvices.DeleteItem(itemSelected.Id);
                    if (response)
                    {
                        await DisplayAlert("Exitos", "El item ha sido eliminado", "Aceptar");

                        this.RefreshScreen();
                    }
                    else
                        this.UpdateBusyIndicator(false);
                }
            }

        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }

        this.UpdateBusyIndicator(false);
    }

    private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            SearchBar searchBar = (SearchBar)sender;
            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                List<ItemModel> list = items.Where(x => x.Name.Contains(searchBar.Text, StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x.Name).ToList();

                lvItems.ItemsSource = new List<ItemModel>();
                lvItems.ItemsSource = list;
            }
            else
            {
                lvItems.ItemsSource = new List<ItemModel>();
                lvItems.ItemsSource = items.OrderBy(m => m.Name);
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    } 

    #endregion
}