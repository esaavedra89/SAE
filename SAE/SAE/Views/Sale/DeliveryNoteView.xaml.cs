using SAE.Models;
using SAE.Models.Inventory;
using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class DeliveryNoteView : ContentPage
{
    #region Variables

    APIServices _apiSrvices = new APIServices();
    List<DeliveryNoteModel> listNotes = new List<DeliveryNoteModel>();
    MenuItemModel _itemSelected = null;

    #endregion

    #region Constructors

    public DeliveryNoteView(MenuItemModel itemSelected)
    {
        InitializeComponent();
        _itemSelected = itemSelected;
        LoadScreen();
    }

    #endregion

    #region Methods

    public async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);

            listNotes = await _apiSrvices.GetDeliveryNotes();
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

    #endregion

    #region Events

    private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            SearchBar searchBar = (SearchBar)sender;
            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                List<DeliveryNoteModel> list = listNotes.Where(x => x.CustomerName.Contains(searchBar.Text, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.Number).ToList();

                lvDeliveryNotes.ItemsSource = new List<DeliveryNoteModel>();
                lvDeliveryNotes.ItemsSource = list;
            }
            else
            {
                lvDeliveryNotes.ItemsSource = new List<DeliveryNoteModel>();
                lvDeliveryNotes.ItemsSource = listNotes.OrderByDescending(m => m.Number);
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    #endregion
}