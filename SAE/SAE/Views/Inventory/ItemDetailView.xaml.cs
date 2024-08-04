using Microsoft.Extensions.Options;
using SAE.Models.Inventory;
using SAE.Services;

namespace SAE.Views.Inventory;

public partial class ItemDetailView : ContentPage
{
	ItemModel _itemSelected = null;
    ItemModel itemToSave = new ItemModel();
    APIServices _apiSrvices = new APIServices();
    ItemView _parent;

    public ItemDetailView(ItemModel item, ItemView parent)
	{
		InitializeComponent();
		_itemSelected = item;
        _parent = parent;

        LoadScreen();
	}

    private async void LoadScreen()
    {
		try
		{
            this.UpdateBusyIndicator(true);

            if (_itemSelected == null)
            {
                entProfitPercentage.Text = "30";
                this.UpdateBusyIndicator(false);
                return;
            }
            else
            {
                if (_itemSelected.ProfitPercentage < 10)
                    _itemSelected.ProfitPercentage = 30;
            }

            itemToSave = _itemSelected;

            entName.Text = _itemSelected.Name;
            entBarCode.Text = _itemSelected.BarCode;
            entSellerCode.Text = _itemSelected.SellerCode;
            entQuantity.Text = _itemSelected.Quantity.ToString();
            entSellerPrice.Text = _itemSelected.SellerPrice.ToString();
            entPriceCost.Text = _itemSelected.PriceCost.ToString();
            entProfitPercentage.Text = _itemSelected.ProfitPercentage.ToString();
            entTotalPriceCalculated.Text = _itemSelected.TotalPriceCalculated.ToString();
            entFinalPrice.Text = _itemSelected.FinalPrice.ToString();
            entPercentageCosts.Text = _itemSelected.Description;
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }

        this.UpdateBusyIndicator(false);
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        try
        {
            this.UpdateBusyIndicator(true);

            bool validate = await Validate();
            if (!validate)
            {
                this.UpdateBusyIndicator(false);
                return;
            }

            string user = Preferences.Default.Get("user", "Unknown");

            itemToSave.Name = entName.Text;
            itemToSave.BarCode = entBarCode.Text;
            itemToSave.SellerCode = entSellerCode.Text;
            itemToSave.Quantity = Convert.ToDecimal(entQuantity.Text);
            itemToSave.SellerPrice = Convert.ToDecimal(entSellerPrice.Text);
            itemToSave.PriceCost = Convert.ToDecimal(entPriceCost.Text);
            itemToSave.ProfitPercentage = Convert.ToDecimal(entProfitPercentage.Text);
            itemToSave.TotalPriceCalculated = Convert.ToDecimal(entTotalPriceCalculated.Text);
            itemToSave.FinalPrice = Convert.ToDecimal(entFinalPrice.Text);
            itemToSave.Description = entPercentageCosts.Text;

            if (itemToSave.Id == 0)
            {
                itemToSave.CreatedDate = DateTime.Now;
                itemToSave.CreatedBy = user;
            }
            else
                itemToSave.UpdateddBy = user;

            await _apiSrvices.SaveItemModelAsync(itemToSave);

            await DisplayAlert("Exito", "Actualizado", "Aceptar");

            _parent.LoadScreen();

            await Task.Delay(500);

            //await Navigation.PopAsync();
            await Navigation.PopAsync();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
        this.UpdateBusyIndicator(false);
    }

    private async Task<bool> Validate()
    {
        try
        {
            if (string.IsNullOrEmpty(entName.Text))
            {
                entName.PlaceholderColor = Colors.Red;
                return false;
            }
            
            if (string.IsNullOrEmpty(entQuantity.Text))
            {
                entQuantity.PlaceholderColor = Colors.Red;
                return false;
            }
            if (string.IsNullOrEmpty(entSellerPrice.Text))
            {
                entSellerPrice.PlaceholderColor = Colors.Red;
                return false;
            }
            if (string.IsNullOrEmpty(entPriceCost.Text))
            {
                entPriceCost.PlaceholderColor = Colors.Red;
                return false;
            }
            if (string.IsNullOrEmpty(entProfitPercentage.Text))
            {
                entProfitPercentage.PlaceholderColor = Colors.Red;
                return false;
            }
            if (string.IsNullOrEmpty(entTotalPriceCalculated.Text))
            {
                entTotalPriceCalculated.PlaceholderColor = Colors.Red;
                return false;
            }
            if (string.IsNullOrEmpty(entFinalPrice.Text))
            {
                entFinalPrice.PlaceholderColor = Colors.Red;
                return false;
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
            return false;;
        }
        return true;
    }

    private void entSellerPrice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(entSellerPrice.Text) || 
                string.IsNullOrWhiteSpace(entProfitPercentage.Text) ||
                string.IsNullOrWhiteSpace(entPercentageCosts.Text)) 
                return;

            decimal sellerPrice = Convert.ToDecimal(entSellerPrice.Text);
            decimal profitPercentage = Convert.ToDecimal(entProfitPercentage.Text);
            decimal percentageCosts = Convert.ToDecimal(entPercentageCosts.Text);

            if (sellerPrice <= 0 || profitPercentage <= 0 || percentageCosts <= 0)
                return;

            /* Cost price. */
            decimal priceCost = sellerPrice * ((percentageCosts/100) + 1 );
            /* TotalPriceCalculated */
            decimal totalPriceCalculated = priceCost * ((profitPercentage/100) + 1 );

            entPriceCost.Text = priceCost.ToString();
            entTotalPriceCalculated.Text = totalPriceCalculated.ToString();
        }
        catch (Exception)
        {
        }
    }

    private void entFinalPrice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(entFinalPrice.Text) ||
                string.IsNullOrWhiteSpace(entPriceCost.Text))
                return;

            decimal finalPrice = Convert.ToDecimal(entFinalPrice.Text);
            decimal costPrice = Convert.ToDecimal(entPriceCost.Text);

            if (finalPrice <= 0 || costPrice <= 0)
                return;

            entProfitAmount.Text = (finalPrice - costPrice).ToString();
        }
        catch (Exception)
        {

        }
    }

    private void UpdateBusyIndicator(bool value)
    {
        busyIndicator.IsRunning = value;
        busyIndicator.IsVisible = value;

        stkMain.IsEnabled = !value;
    }
}