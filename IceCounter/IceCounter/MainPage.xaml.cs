using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IceCounter
{
    public partial class MainPage : ContentPage
    {
        private bool _isShiftStarted;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ShowItems();

            if (Application.Current.Properties.ContainsKey("IsShiftStarted"))
            {
                _isShiftStarted = (bool)Application.Current.Properties["IsShiftStarted"];
            }
            else
            {
                _isShiftStarted = false;
            }

            if (_isShiftStarted)
            {
                StartShiftBtn.Text = "Продолжить смену";
            }
            else
            {
                StartShiftBtn.Text = "Начать смену";
            }
        }

        private void ShowItems()
        {
            IceCreams.ItemsSource = App.AllItemsDb.GetOneTypeItems(Item.Type.Icecream);
            Dishes.ItemsSource = App.AllItemsDb.GetOneTypeItems(Item.Type.Dish);
        }

        private void DeleteItem(object sender, EventArgs e)
        {
            var button = sender as Button;
            int id = (int)button.CommandParameter;

            App.AllItemsDb.DeleteItem<Item>(id);
            ShowItems();
        }

        private void ChangeNewItemType(object sender, CheckedChangedEventArgs e)
        {
            if (isIcecream.IsChecked)
                typeName.Text = "Мороженное";
            else
                typeName.Text = "Посуда";
        }

        private void AddItem(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(newItemName.Text))
            {
                return;
            }

            string color;
            Item.Type type;
            if (isIcecream.IsChecked)
            {
                type = Item.Type.Icecream;
            }
            else
            {
                type = Item.Type.Dish;
            }
            color = Item.HexDefaultColor[(int)type];

            string name = newItemName.Text;
            name[0].ToString().ToUpper();

            Item item = new Item()
            {
                Name = name,
                type = type,
                Count = 0,
                IsChoosed = false,
                HexColor = color,
            };

            App.AllItemsDb.SaveItem(item);
            ShowItems();

            newItemName.Text = "";
        }

        private async void ChooseShiftItems(object sender, EventArgs e)
        {
            if (_isShiftStarted)
            {
                await Navigation.PushAsync(new Shift());
            }
            else
            {
                foreach (Item item in App.AllItemsDb.GetItems<Item>())
                {
                    item.IsChoosed = false;
                    item.HexColor = Item.HexDefaultColor[(int)item.type];
                    App.AllItemsDb.UpdateItem(item);
                }
                App.ShiftItemsDb.ClearTable<Item>();
                await Navigation.PushAsync(new StartShift());
            }
        }

        private async void ShowHistory(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage());
        }
    }
}
