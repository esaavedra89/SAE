using SAE.Models.Inventory;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class DeliveryNoteItemDetailView : ContentPage
{
    ItemDeliveryNoteModel _deliveryItem;
    DeliveryNoteDetailView _parent;
    APIServices _apiServices = new APIServices();

    public DeliveryNoteItemDetailView(ItemDeliveryNoteModel deliveryItem, DeliveryNoteDetailView parent)
	{
		InitializeComponent();
        _deliveryItem = deliveryItem == null ? new ItemDeliveryNoteModel() { Active = true } : deliveryItem;
        _parent = parent;
        LoadScreen();
    }

    private async void LoadScreen()
    {
        try
        {
            List<ItemModel> listItems = await GetProductos();
            pkrItem.ItemsSource = listItems;

            if (_deliveryItem == null || _deliveryItem.Id == 0) return;

            if (pkrItem.ItemsSource.Count > 0)
                pkrItem.SelectedItem = listItems.Where(m => m.Id == _deliveryItem.ItemId).FirstOrDefault();

            entItemQuantity.Text = _deliveryItem.ItemQuantity.ToString();
            entPriceItem.Text = _deliveryItem.PriceItem.ToString();
            entTotalItem.Text = _deliveryItem.TotalItem.ToString();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async Task<List<ItemModel>> GetProductos()
    {
        List<ItemModel> items = new List<ItemModel>();
        try
        {
            decimal cero = 0.0000M;
            items = await _apiServices.GetItems();
            if (items.Count == 0)
                items = new List<ItemModel>();
            else
            {
                items = items.Where(m => m.Quantity > 0).OrderBy(m => m.Name).ToList();
            }
        }
        catch (Exception exc)
        {
            throw exc;
        }

        return items;
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool validate = await Validate();
            if (!validate) return;

            ItemModel item = pkrItem.SelectedItem as ItemModel;

            ItemDeliveryNoteModel objItem = new ItemDeliveryNoteModel();
            objItem.Item = item;
            objItem.ItemId = item.Id;
            objItem.ItemQuantity = Convert.ToInt32(entItemQuantity.Text);
            objItem.PriceItem = Convert.ToDecimal(entPriceItem.Text);
            objItem.TotalItem = Convert.ToDecimal(entTotalItem.Text);
            objItem.Id = _deliveryItem.Id;
            objItem.DeliveryNoteId = _deliveryItem.Id;
            objItem.Active = _deliveryItem.Active;

            await _parent.AddProduct(objItem);

            await Navigation.PopModalAsync();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async Task<bool> Validate()
    {
        try
        {
            if (pkrItem == null || pkrItem.SelectedItem == null)
                await DisplayAlert("Advertencia", "Debe seleccionar un producto", "Aceptar");

            if (string.IsNullOrEmpty(entItemQuantity.Text))
            {
                entItemQuantity.PlaceholderColor = Colors.Red;
                return false;
            }
            if (string.IsNullOrEmpty(entPriceItem.Text))
            {
                entPriceItem.PlaceholderColor = Colors.Red;
                return false;
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
        return true;
    }

    private async void entItemQuantity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            if (pkrItem != null && pkrItem.SelectedItem != null)
            {
                ItemModel item = pkrItem.SelectedItem as ItemModel;
                if (item != null)
                {
                    decimal total = item.FinalPrice * Convert.ToDecimal(string.IsNullOrEmpty(entItemQuantity.Text) ? "0" : entItemQuantity.Text);
                    entTotalItem.Text = total.ToString();
                }
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void pkrItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            if (pkrItem != null && pkrItem.SelectedItem != null)
            {
                ItemModel item = pkrItem.SelectedItem as ItemModel;
                if (item != null)
                    entPriceItem.Text = item.FinalPrice.ToString();
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }
}