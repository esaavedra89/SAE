using SAE.Models;
using SAE.Models.Inventory;
using SAE.Services;

namespace SAE.Views.Inventory;

public partial class ItemView : ContentPage
{
    MenuItemModel _menuSelected = null;
    APIServices _apiSrvices = new APIServices();

    public ItemView(MenuItemModel menuSelected)
	{
		InitializeComponent();
        _menuSelected = menuSelected;

        LoadScreen();
    }

    public async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);
            
            List<ItemModel> items = await _apiSrvices.GetItems();
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
}