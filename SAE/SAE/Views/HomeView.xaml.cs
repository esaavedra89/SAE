
using SAE.Models;
using SAE.Views.Inventory;
using SAE.Views.Sale;

namespace SAE.Views;

public partial class HomeView : ContentPage
{
	public HomeView()
	{
		InitializeComponent();
        Loadscreen();
    }

    private async void Loadscreen()
    {
		try
		{
			List<MenuItemModel> listMenu = new List<MenuItemModel>();

			listMenu.Add(new MenuItemModel 
			{
				Id = 1,
				Name = "Productos"
			});

            listMenu.Add(new MenuItemModel
            {
                Id = 2,
                Name = "Notas de entrega"
            });

            listMenu.Add(new MenuItemModel
            {
                Id = 3,
                Name = "Deudores"
            });

            listMenu.Add(new MenuItemModel
            {
                Id = 4,
                Name = "Clientes"
            });

            lvMenuItems.ItemsSource = listMenu;
        }
		catch (Exception exc)
		{
			await DisplayAlert("Error", exc.Message, "Aceptar");
		}
    }

    private async void lvMenuItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
		try
		{
            MenuItemModel itemSelected = e.SelectedItem as MenuItemModel;
            if (itemSelected == null) return;

            switch (itemSelected.Id)
            {
                case 1:
                    await Navigation.PushAsync(new ItemView(itemSelected));
                    break;
                case 2:
                    await Navigation.PushAsync(new DeliveryNoteView(itemSelected));
                    break;
                case 3:
                    await Navigation.PushAsync(new DebtorView(itemSelected));
                    break;

                case 4:
                    await Navigation.PushAsync(new CustomerView(itemSelected));
                    break;
            }
		}
        catch (Exception exc)
        {
            await DisplayAlert("Error", exc.Message, "Aceptar");
        }
    }
}