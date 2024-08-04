using SAE.Models;
using SAE.Models.Inventory;
using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class DebtorView : ContentPage
{
    MenuItemModel _itemSelected = null;
    APIServices _apiSrvices = new APIServices();

    public DebtorView(MenuItemModel itemSelected)
	{
		InitializeComponent();

        _itemSelected = itemSelected;
        lblTitle.Title = "Deudores";
        LoadScreen();
    }

    public async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);

            List<DebtorModel> listDebtors = await _apiSrvices.GetDebtors();
            if (listDebtors.Count > 0)
                lvDebtors.ItemsSource = listDebtors.OrderByDescending(m => m.Id);
            else
                lvDebtors.ItemsSource = new List<DebtorModel>();

            if (listDebtors.Count > 0)
            {
                decimal totalDeuda = listDebtors.Sum(m => m.Debt);
                if (totalDeuda > 0)
                    lblTitle.Title = $"D.T.: {totalDeuda.ToString("N2")}$";
            }
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
            await Navigation.PushAsync(new DebtorsDetailView(null, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void lvDebtors_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            DebtorModel itemSelected = e.SelectedItem as DebtorModel;
            if (itemSelected == null) return;

            string action = await DisplayActionSheet("Opciones", "Cancel", null, "Ver", "Eliminar");
            if (action == "Ver")
                await Navigation.PushAsync(new DebtorsDetailView(itemSelected, this));
            else if (action == "Eliminar")
            {
                bool answer = await DisplayAlert("Pregunta", "Desea eliminar a este deudor?", "Si", "No");
                if (answer)
                {
                    this.UpdateBusyIndicator(true);

                    bool response = await _apiSrvices.DeleteDebtorAsync(itemSelected.Id);
                    if (response)
                    {
                        await DisplayAlert("Exitos", "El deudor ha sido eliminado", "Aceptar");

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
    }

    private void UpdateBusyIndicator(bool value)
    {
        busyIndicator.IsRunning = value;
        busyIndicator.IsVisible = value;

        stkMain.IsEnabled = !value;
    }

    private void RefreshScreen()
    {
        lvDebtors.ItemsSource = new List<DebtorModel>();

        LoadScreen();
    }

    private void btnRefresh_Clicked(object sender, EventArgs e)
    {
        this.RefreshScreen();
    }
}