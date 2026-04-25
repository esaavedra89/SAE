using SAE.Models.Inventory;
using SAE.Services;
using SAE.Views.Inventory;

namespace SAE.Views.Sale;

public partial class DeliveryNoteItemDetailView : ContentPage
{
    ItemModel? _itemSelected;
    ItemDeliveryNoteModel _deliveryItem;
    DeliveryNoteDetailView _parent;
    ItemView? _itemView;

    public DeliveryNoteItemDetailView(ItemDeliveryNoteModel? deliveryItem, DeliveryNoteDetailView parent, ItemModel? itemSelected, ItemView? itemView = null)
    {
        InitializeComponent();
        _deliveryItem = deliveryItem == null ? new ItemDeliveryNoteModel() { Active = true } : deliveryItem;
        _parent = parent;
        _itemSelected = itemSelected;
        _itemView = itemView;
        LoadScreen();
    }

    private async void LoadScreen()
    {
        try
        {
            if (_itemView != null)
                await _itemView.Navigation.PopModalAsync();


            if (_deliveryItem.Id > 0 && _itemSelected == null)
            {

            }
            else
            {
                if (_itemSelected == null)
                {
                    await DisplayAlert("Error", "Item es nulo", "Aceptar");
                    return;
                }

                _deliveryItem.Item = _itemSelected;
                _deliveryItem.PriceItem = _itemSelected.FinalPrice;
                _deliveryItem.ItemId = _itemSelected.Id;
                _deliveryItem.DeliveryNoteId = _parent._itemSelected.Id;
            }

            lblItemSelected.Text = _deliveryItem.Item.Name;
            entItemQuantity.Text = _deliveryItem.ItemQuantity.ToString();
            entPriceItem.Text = _deliveryItem.PriceItem.ToString();
            entTotalItem.Text = _deliveryItem.TotalItem.ToString();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    //private async Task<List<ItemModel>> GetProductos()
    //{
    //    List<ItemModel> items = new List<ItemModel>();
    //    try
    //    {
    //        items = await _apiServices.GetItems();
    //        if (items.Count == 0)
    //            items = new List<ItemModel>();
    //        else
    //            items = items.Where(m => m.Quantity > 0).OrderBy(m => m.Name).ToList();
    //    }
    //    catch (Exception exc)
    //    {
    //        await DisplayAlert("Error", exc.Message, "Aceptar");
    //    }

    //    return items;
    //}

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool validate = await Validate();
            if (!validate) return;

            ItemDeliveryNoteModel objItem = new ItemDeliveryNoteModel();
            objItem.Item = _deliveryItem.Item;
            objItem.ItemId = _deliveryItem.Item.Id;
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
            if (_deliveryItem.Item.Id == 0)
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
            if (_deliveryItem != null && _deliveryItem.Item != null && _deliveryItem.ItemId > 0)
            {
                decimal total = _deliveryItem.Item.FinalPrice * Convert.ToDecimal(string.IsNullOrEmpty(entItemQuantity.Text) ? "0" : entItemQuantity.Text);
                entTotalItem.Text = total.ToString();
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
            if (_deliveryItem.Item != null && _deliveryItem.ItemId > 0)
                entPriceItem.Text = _deliveryItem.Item.FinalPrice.ToString();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }
}