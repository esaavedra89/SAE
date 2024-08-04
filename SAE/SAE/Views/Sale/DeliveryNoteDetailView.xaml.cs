using SAE.Models.Inventory;
using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class DeliveryNoteDetailView : ContentPage
{
    DeliveryNoteModel _itemSelected = null;
    DeliveryNoteModel dNoteToSave = new DeliveryNoteModel();
    List<ItemDeliveryNoteModel> _items = new List<ItemDeliveryNoteModel>();
    APIServices _apiServices = new APIServices();
    DeliveryNoteView _parent;
    CustomerModel _customer = null;

    public DeliveryNoteDetailView(DeliveryNoteModel deliveryNote, DeliveryNoteView parent)
	{
		InitializeComponent();
        _itemSelected = deliveryNote == null ? new DeliveryNoteModel() { Active = true } : deliveryNote;
        _parent = parent;

        LoadScreen();
    }

    private async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);

            dpkDate.Date = DateTime.Now;
            lblNumber.Text = "0";
            entDiscountPercentage.Text = "0";

            if (_itemSelected == null || _itemSelected.Id == 0)
            {
                dpkDate.Date = DateTime.Now;
                this.UpdateBusyIndicator(false);
                return;
            }

            dpkDate.Date = _itemSelected.Date;
            lblNumber.Text = _itemSelected.Number.ToString();
            entCustomerName.Text = _itemSelected.CustomerName;
            entCustomerIdentification.Text = _itemSelected.CustomerIdentification;
            entPaymentMethod.Text = _itemSelected.PaymentMethod;
            entDiscountPercentage.Text = _itemSelected.DiscountPercentage.ToString();
            entDiscount.Text = _itemSelected.Discount.ToString();
            entSubtotal.Text = _itemSelected.Subtotal.ToString();
            entTotal.Text = _itemSelected.Total.ToString();
            swtDeliver.IsToggled = _itemSelected.Deliver;

            _items = await GetProductos();
            lvItems.ItemsSource = _items;
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }

        this.UpdateBusyIndicator(false);
    }

    private async void btnAdd_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushModalAsync(new DeliveryNoteItemDetailView(null, this));
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
            ItemDeliveryNoteModel itemSelected = e.SelectedItem as ItemDeliveryNoteModel;
            if (itemSelected == null) return;

            string action = await DisplayActionSheet("Seleccione", "Cancel", null, "Ver", "Eliminar");

            if (string.IsNullOrEmpty(action)) return;

            if (action == "Ver")
                await Navigation.PushModalAsync(new DeliveryNoteItemDetailView(itemSelected, this));
            else if (action == "Eliminar")
            {
                bool answer = await DisplayAlert("Pregunta?", "Desea eliminar este producto de la lista?", "Si", "No");
                if (answer)
                {
                    if (itemSelected.Id > 0)
                    {
                        ItemDeliveryNoteModel item = _items.Find(x => x.Id == itemSelected.Id);
                        if (item != null)
                            item.Active = false;
                        else
                        {
                            await DisplayAlert("Error", "Artículo no encontrado", "Aceptar");
                            return;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _items.Count; i++)
                        {
                            if (_items[i].Equals(itemSelected))
                            {
                                _items.Remove(itemSelected);
                                continue;
                            }
                        }
                    }

                    // Update the subtotal.
                    this.UpdateSubtotal();

                    lvItems.ItemsSource = new List<ItemDeliveryNoteModel>();
                    lvItems.ItemsSource = new List<ItemDeliveryNoteModel>(_items.Where(x => x.Active).ToList());
                }
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
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

            for (int i = 0; i < _items.Count; i++)
            {
                ItemDeliveryNoteModel item = _items[i];
                if (item.Id == 0)
                {
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = user;
                }
                else
                {
                    item.UpdateddDate = DateTime.Now;
                    item.UpdateddBy = user;
                }
            }

            dNoteToSave = _itemSelected;

            dNoteToSave.CustomerName = entCustomerName.Text;
            dNoteToSave.CustomerIdentification = entCustomerIdentification.Text;
            dNoteToSave.Date = dpkDate.Date;
            dNoteToSave.PaymentMethod = entPaymentMethod.Text;
            dNoteToSave.DiscountPercentage = Convert.ToDecimal(entDiscountPercentage.Text);
            dNoteToSave.Discount = Convert.ToDecimal(entDiscount.Text);
            dNoteToSave.Subtotal = Convert.ToDecimal(entSubtotal.Text);
            dNoteToSave.Total = Convert.ToDecimal(entTotal.Text);
            dNoteToSave.Deliver = swtDeliver.IsToggled;
            dNoteToSave.Items = _items;

            if (dNoteToSave.Id == 0)
            {
                dNoteToSave.CreatedDate = DateTime.Now;
                dNoteToSave.CreatedBy = user;
            }
            else
                dNoteToSave.UpdateddBy = user;

            await _apiServices.SaveDeliveryNotelAsync(dNoteToSave);

            await DisplayAlert("Exito", "Actualizado", "Aceptar");

            _parent.LoadScreen();

            await Task.Delay(500);

            await Navigation.PopAsync();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }

        this.UpdateBusyIndicator(false);
    }

    private async Task<List<ItemDeliveryNoteModel>> GetProductos()
    {
        List<ItemDeliveryNoteModel> items = new List<ItemDeliveryNoteModel>();

        try
        {
            if (_itemSelected != null && _itemSelected.Id > 0)
            {
                items = await _apiServices.GetItemsDeliveryNote(_itemSelected.Id);
                if (items.Count == 0) items = new List<ItemDeliveryNoteModel>();
            }
        }
        catch (Exception exc)
        {
            throw exc;
        }

        return items;
    }

    private async Task<bool> Validate()
    {
        try
        {

            if (_customer == null)
            {
                await DisplayAlert("Advertencia", "Debe agregar cliente", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(entCustomerName.Text))
            {
                entCustomerName.PlaceholderColor = Colors.Red;
                return false;
            }

            if (string.IsNullOrEmpty(entCustomerIdentification.Text))
            {
                entCustomerIdentification.PlaceholderColor = Colors.Red;
                return false;
            }

            if (_items.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe agregar productos", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(entTotal.Text))
            {
                entCustomerIdentification.PlaceholderColor = Colors.Red;
                return false;
            }
            else
            {
                decimal total = Convert.ToDecimal(entTotal.Text);
                if (total == 0 || total < 0)
                {
                    await DisplayAlert("Advertencia", "Total no puede ser cero", "Aceptar");
                    return false;
                }
            }

            decimal discountPercentage = Convert.ToDecimal(string.IsNullOrEmpty(entDiscountPercentage.Text) ? "0" : entDiscountPercentage.Text);
            if (discountPercentage > 0)
            {
                bool answer = await DisplayAlert("Advertencia", $"Está aplicando un descuento del {discountPercentage}%, desea continuar? ", "Si", "No");
                return answer;
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
        return true;
    }

    public async Task AddProduct(ItemDeliveryNoteModel item) 
    {
        try
        {
            ItemDeliveryNoteModel itemList = _items.Where(m => m.Item.Name.Contains(item.Item.Name) && m.Active).FirstOrDefault();
            if (itemList == null)
                _items.Add(item);
            else
            {
                itemList.ItemQuantity = item.ItemQuantity;
                itemList.PriceItem = item.PriceItem;
                itemList.TotalItem = item.TotalItem;
            }

            lvItems.ItemsSource = new List<ItemDeliveryNoteModel>();
            lvItems.ItemsSource = _items.Where(m => m.Active).ToList();
            // Update the subtotal.
            this.UpdateSubtotal();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void entDiscountPercentage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            decimal discountPercentage = Convert.ToDecimal(string.IsNullOrEmpty(entDiscountPercentage.Text) ? "0" : entDiscountPercentage.Text);
            if (discountPercentage < 0 || _items.Where(m => m.Active).ToList().Count == 0)
                return;
            else if (discountPercentage > 15)
            {
                await DisplayAlert("Advertencia", "No se permiten descuentos mayor al 15%", "Aceptar");
                entDiscountPercentage.Text = "0";
                return;
            }

            decimal subtotal = _items.Where(m => m.Active).Sum(m => m.TotalItem);
            if (subtotal > 0)
            {
                decimal discount = 0;
                if (discountPercentage > 0)
                {
                    discount = subtotal * (discountPercentage / 100);

                    entDiscount.Text = discount.ToString();

                    decimal totalAmount = subtotal - discount;

                    entTotal.Text = totalAmount.ToString();
                }
                else
                {
                    entDiscount.Text = "0";
                    entTotal.Text = subtotal.ToString();
                }
            }
        }
        catch (Exception exc)
        {
            
        }
    }

    private void UpdateSubtotal()
    {
        decimal subtotal = _items.Where(m => m.Active).Sum(m => m.TotalItem);
        entSubtotal.Text = subtotal.ToString();
        entTotal.Text = subtotal.ToString();
    }

    private void UpdateBusyIndicator(bool value)
    {
        busyIndicator.IsRunning = value;
        busyIndicator.IsVisible = value;

        stkMain.IsEnabled = !value;
    }

    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        try
        {
            var client = new SearchClientView();

            client.SelectedClient(async () => await this.FillCustomer(client._customerSelected, client));

            await Navigation.PushAsync(client);
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async Task FillCustomer(CustomerModel customerSelected, SearchClientView view)
    {
        try
        {
            if (customerSelected == null)
            {
                await DisplayAlert("Error", "Cliente es nulo", "Aceptar");
                return;
            }

            _customer = customerSelected;

            entCustomerName.Text = $"{customerSelected.Name} {customerSelected.LastName}";

            await view.Navigation.PopAsync();
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }
}