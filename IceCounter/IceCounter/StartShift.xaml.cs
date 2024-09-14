using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IceCounter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartShift : ContentPage
	{
		public StartShift ()
		{
			InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ShowItems();
        }

        private void ShowItems()
        {
            IceCreams.ItemsSource = App.AllItemsDb.GetOneTypeItems(Item.Type.Icecream);
            Dishes.ItemsSource = App.AllItemsDb.GetOneTypeItems(Item.Type.Dish);
        }

        private async void GoShift(object sender, EventArgs e)
        {
            bool anyIcecreams = false;
            bool anyDishes = false;

            foreach (Item item in App.ShiftItemsDb.GetItems<Item>())
            {
                if (item.type == Item.Type.Icecream)
                    anyIcecreams = true;
                else
                    anyDishes = true;
            }
            if (!anyIcecreams || !anyDishes)
            {
                await DisplayAlert("Внимание!", "Нужно выбрать не меньше чем один тип мороженого и посуды!", "Ща исправлю");
                return;
            }

            foreach (Item item in App.ShiftItemsDb.GetItems<Item>())
            {
                item.IsChoosed = false;
                item.HexColor = Item.HexDefaultColor[(int)item.type];
                App.ShiftItemsDb.UpdateItem(item);
            }
            foreach (Item item in App.AllItemsDb.GetItems<Item>())
            {
                item.IsChoosed = false;
                item.HexColor = Item.HexDefaultColor[(int)item.type];
                App.AllItemsDb.UpdateItem(item);
            }

            Application.Current.Properties["IsShiftStarted"] = true;
            await Application.Current.SavePropertiesAsync();

            await Navigation.PushAsync(new Shift());

        }

        private void ItemChoosed(object sender, EventArgs e)
        {
            var button = sender as Button;
            int itemId = (int)button.CommandParameter;
            Item item = App.AllItemsDb.GetItemById<Item>(itemId);

            item.IsChoosed = !item.IsChoosed;
            App.AllItemsDb.UpdateItem(item);

            if (item.IsChoosed)
            {
                item.HexColor = Item.HexChoosedColor[(int)item.type];
                App.AllItemsDb.UpdateItem(item);

                App.ShiftItemsDb.SaveItem<Item>(item);
            }
            else
            {
                item.HexColor = Item.HexDefaultColor[(int)item.type];
                App.AllItemsDb.UpdateItem(item);

                App.ShiftItemsDb.DeleteItem<Item>(App.ShiftItemsDb.GetFirstItemByName(item.Name).ID);
            }

            App.AllItemsDb.UpdateItem(item);
            ShowItems();
        }
    }
}