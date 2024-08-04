using SAE.Models.Sale;

namespace SAE.Views.Sale;

public partial class DebtorPaymentView : ContentPage
{
    DebtorPaymentModel _payment;
    DebtorsDetailView _parent;


    public DebtorPaymentView(DebtorPaymentModel payment, DebtorsDetailView parent)
	{
		InitializeComponent();
        _payment = payment == null ? new DebtorPaymentModel() : payment;
        _parent = parent;
        LoadScreen();
    }

    private async void LoadScreen()
    {
        try
        {
            dpkDate.Date = DateTime.Now;
            if (_payment == null || _payment.Id == 0) return;

            entPaymentAmount.Text = _payment.PaymentAmount.ToString();
            entPaymentMethod.Text = _payment.PaymentMethod;
            entObservation.Text = _payment.Observation;
            dpkDate.Date = _payment.Date;
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
            bool validate = await Validate();
            if (!validate) return;

            _payment.PaymentAmount = Convert.ToDecimal(entPaymentAmount.Text);
            _payment.PaymentMethod = entPaymentMethod.Text;
            _payment.Date = dpkDate.Date;
            _payment.Observation = entObservation.Text;

            await _parent.AddPayment(_payment);

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
            if (string.IsNullOrEmpty(entPaymentAmount.Text))
            {
                entPaymentAmount.PlaceholderColor = Colors.Red;
                entPaymentAmount.BackgroundColor = Colors.Red;
                return false;
            }
            else
            {
                decimal payment = Convert.ToDecimal(entPaymentAmount.Text);
                if (payment == 0 || payment < 0)
                {
                    await DisplayAlert("Advertencia", "Debe ingresar un monto", "Aceptar");
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
}