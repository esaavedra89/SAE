using SAE.Models;
using SAE.Models.Inventory;
using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class DeliveryNoteView : ContentPage
{
    MenuItemModel _itemSelected = null;
    APIServices _apiSrvices = new APIServices();

    public DeliveryNoteView(MenuItemModel itemSelected)
	{
		InitializeComponent();
        _itemSelected = itemSelected;
        LoadScreen();
    }

    public async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);

            List<DeliveryNoteModel> listNotes = await _apiSrvices.GetDeliveryNotes();
            if (listNotes.Count > 0)
                lvDeliveryNotes.ItemsSource = listNotes.OrderByDescending(m => m.Number);
            else
                lvDeliveryNotes.ItemsSource = new List<DeliveryNoteModel>();
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
            await Navigation.PushAsync(new DeliveryNoteDetailView(null, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void lvDeliveryNotes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            DeliveryNoteModel itemSelected = e.SelectedItem as DeliveryNoteModel;
            if (itemSelected == null) return;

            string action = await DisplayActionSheet("Opciones", "Cancel", null, "Ver", "Eliminar");
            if (action == "Ver")
                await Navigation.PushAsync(new DeliveryNoteDetailView(itemSelected, this));
            else if (action == "Eliminar")
            {
                bool answer = await DisplayAlert("Pregunta", "Desea eliminar esta nota?", "Si", "No");
                if (answer)
                {
                    this.UpdateBusyIndicator(true);

                    bool response = await _apiSrvices.DeleteDeliveryNote(itemSelected.Id);
                    if (response) 
                    {
                        await DisplayAlert("Exitos", "La nota ha sido eliminada", "Aceptar");

                        this.RefreshScreen();
                    }
                    else
                    {
                        this.UpdateBusyIndicator(false);
                    }
                }
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private void RefreshScreen()
    {
        lvDeliveryNotes.ItemsSource = new List<DeliveryNoteModel>();

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