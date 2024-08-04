using SAE.Models.Sale;
using SAE.Services;

namespace SAE.Views.Sale;

public partial class DebtorsDetailView : ContentPage
{
    #region Variables

    DebtorModel _itemSelected = null;
    DebtorModel debtorToSave = new DebtorModel(){ CustomerName = string.Empty, RelatedDeliveryNote = string.Empty };
    List<DebtorPaymentModel> _payments = new List<DebtorPaymentModel>();
    APIServices _apiServices = new APIServices();
    DebtorView _parent;
    CustomerModel _customer = null;

    #endregion

    #region Constructors

    public DebtorsDetailView(DebtorModel debtor, DebtorView parent)
    {
        InitializeComponent();
        _itemSelected = debtor == null ? new DebtorModel() { CustomerName = string.Empty, RelatedDeliveryNote = string.Empty } : debtor;
        _parent = parent;

        LoadScreen();
    }

    #endregion

    #region Methods

    private async void LoadScreen()
    {
        try
        {
            this.UpdateBusyIndicator(true);

            dpkDate.Date = DateTime.Now;
            lblNumber.Text = "0";
            if (_itemSelected == null || _itemSelected.Id == 0)
            {
                dpkDate.Date = DateTime.Now;
                this.UpdateBusyIndicator(false);
                return;
            }

            dpkDate.Date = _itemSelected.Date;
            lblNumber.Text = _itemSelected.Id.ToString();
            entCustomerName.Text = _itemSelected.CustomerName;
            entCustomerIdentification.Text = _itemSelected.CustomerIdentification;
            entRelativedNotes.Text = _itemSelected.RelatedDeliveryNote;
            entObservations.Text = _itemSelected.Observation;
            entSummation.Text = _itemSelected.Summation.ToString();
            entActualDebt.Text = _itemSelected.Debt.ToString();
            entTotalDebt.Text = _itemSelected.TotalDebt.ToString();

            _payments = await GetPayments();
            lvDebtorPayments.ItemsSource = _payments;
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

            for (int i = 0; i < _payments.Count; i++)
            {
                DebtorPaymentModel item = _payments[i];
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

            debtorToSave = _itemSelected;

            debtorToSave.CustomerName = entCustomerName.Text;
            debtorToSave.CustomerIdentification = entCustomerIdentification.Text;
            debtorToSave.Date = dpkDate.Date;
            debtorToSave.CustomerIdentification = entCustomerIdentification.Text;
            debtorToSave.RelatedDeliveryNote = entRelativedNotes.Text;
            debtorToSave.Observation = entObservations.Text;
            debtorToSave.Debt = Convert.ToDecimal(entActualDebt.Text);
            debtorToSave.Summation = Convert.ToDecimal(entSummation.Text);
            debtorToSave.TotalDebt = Convert.ToDecimal(entTotalDebt.Text);
            debtorToSave.Payments = _payments;

            if (debtorToSave.Id == 0)
            {
                debtorToSave.CreatedDate = DateTime.Now;
                debtorToSave.CreatedBy = user;
            }
            else
                debtorToSave.UpdateddBy = user;

            await _apiServices.SaveDebtorAsync(debtorToSave);

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

    private async void lvDebtorPayments_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            DebtorPaymentModel itemSelected = e.SelectedItem as DebtorPaymentModel;
            if (itemSelected == null) return;

            await Navigation.PushModalAsync(new DebtorPaymentView(itemSelected, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async void btnAdd_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (_itemSelected.Id == 0)
            {
                await DisplayAlert("Error", "Debe guardar primero al deudor", "Aceptar");
                return;
            }

            await Navigation.PushModalAsync(new DebtorPaymentView(null, this));
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    private async Task<List<DebtorPaymentModel>> GetPayments()
    {
        List<DebtorPaymentModel> payments = new List<DebtorPaymentModel>();

        try
        {
            if (_itemSelected != null && _itemSelected.Id > 0)
            {
                payments = await _apiServices.GetPayments(_itemSelected.Id);
                if (payments.Count == 0) payments = new List<DebtorPaymentModel>();
            }
        }
        catch (Exception exc)
        {
            throw exc;
        }

        return payments;
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
                await DisplayAlert("Advertencia", "Debe agregar un cliente", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(entRelativedNotes.Text))
            {
                entCustomerName.PlaceholderColor = Colors.Red;
                await DisplayAlert("Advertencia", "Debe agregar la nota(s) de entrega relacionadas", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(entTotalDebt.Text))
            {
                entCustomerIdentification.PlaceholderColor = Colors.Red;
                await DisplayAlert("Advertencia", "Debe agregar el monto total de la deuda", "Aceptar");
                return false;
            }
            else
            {
                decimal total = Convert.ToDecimal(entTotalDebt.Text);
                if (total == 0 || total < 0)
                {
                    await DisplayAlert("Advertencia", "Total no puede ser cero", "Aceptar");
                    return false;
                }
            }
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
        return true;
    }

    public async Task AddPayment(DebtorPaymentModel payment)
    {
        try
        {
            payment.DebtorId = _itemSelected.Id;

            if (payment.Id == 0)
            {
                _payments.Add(payment);
            }
            else
            {
                DebtorPaymentModel paymentFromList = _payments.Where(m => m.Id == payment.Id).FirstOrDefault();
                if (paymentFromList == null)
                    _payments.Add(paymentFromList);
                else
                {
                    foreach (DebtorPaymentModel pay in _payments)
                    {
                        pay.PaymentAmount = payment.PaymentAmount;
                        pay.PaymentMethod = payment.PaymentMethod;
                        pay.Date = payment.Date;
                        pay.Observation = payment.Observation;
                    }
                }
            }

            lvDebtorPayments.ItemsSource = new List<DebtorPaymentModel>();
            lvDebtorPayments.ItemsSource = _payments;

            decimal totalDebt = Convert.ToDecimal(string.IsNullOrEmpty(entTotalDebt.Text) ? "0" : entTotalDebt.Text);
            decimal payments = _payments.Sum(m => m.PaymentAmount);
            entSummation.Text = payments.ToString();
            entActualDebt.Text = (totalDebt - payments).ToString();
        }
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }

    #endregion

    private async void entTotalDebt_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            decimal totalDebt = Convert.ToDecimal(string.IsNullOrEmpty(entTotalDebt.Text) ? "0" : entTotalDebt.Text);
            decimal summation = Convert.ToDecimal(string.IsNullOrEmpty(entSummation.Text) ? "0" : entSummation.Text);

            entActualDebt.Text = (totalDebt - summation).ToString();
        }
        catch (Exception exc)
        {
            //await DisplayAlert("Error", exc.Message, "Aceptar");
        }
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