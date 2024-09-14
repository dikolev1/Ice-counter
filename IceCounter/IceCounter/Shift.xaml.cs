using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IceCounter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Shift : ContentPage
    {
        private int _dealCount;
        private string _startTime;
        private string _endTime;

        private bool _isFirstStart;

        public Shift()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            NavigationPage.SetHasBackButton(this, false);

            _isFirstStart = true;
            if (Application.Current.Properties.ContainsKey("IsFirstStart"))
            {
                _isFirstStart = (bool)Application.Current.Properties["IsFirstStart"];
            }

            if (_isFirstStart)
            {
                _startTime = DateTime.Now.ToShortTimeString();
                Application.Current.Properties["StartTime"] = _startTime;
                _dealCount = 0;
                Application.Current.Properties["Deals"] = _dealCount.ToString();
                App.ShiftHistoryDb.ClearTable<ItemHistory>();

                _isFirstStart = false;
                Application.Current.Properties["IsFirstStart"] = _isFirstStart;
            }
            else
            {
                _dealCount = Convert.ToInt32((string)Application.Current.Properties["Deals"]);
            }
            ShowItems();
        }

        private async void ShowItems()
        {
            IceCreams.ItemsSource = App.ShiftItemsDb.GetOneTypeItems(Item.Type.Icecream);
            Dishes.ItemsSource = App.ShiftItemsDb.GetOneTypeItems(Item.Type.Dish);
            await Application.Current.SavePropertiesAsync();
        }

        private void ShiftItemClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            int itemId = (int)button.CommandParameter;
            Item item = App.ShiftItemsDb.GetItemById<Item>(itemId);

            string name = item.Name;
            ItemHistory existingHistory = App.ShiftHistoryDb.GetFirstHistoryByName(name);

            item.IsChoosed = !item.IsChoosed;

            if (item.IsChoosed)
            {
                item.HexColor = Item.HexChoosedColor[(int)item.type];
                item.Count = 1;

                if (existingHistory == null)
                {
                    ItemHistory newHistory = new ItemHistory()
                    {
                        Name = name,
                        Count = 1,
                    };
                    App.ShiftHistoryDb.SaveItem(newHistory);
                }
                else
                {
                    existingHistory.Count++;
                    App.ShiftHistoryDb.UpdateItem(existingHistory);
                }
            }
            else
            {
                item.HexColor = Item.HexDefaultColor[(int)item.type];
                item.Count = 0;

                if (existingHistory != null)
                {
                    existingHistory.Count--;

                    if (existingHistory.Count <= 0)
                    {
                        App.ShiftHistoryDb.DeleteItem<ItemHistory>(existingHistory.ID);
                    }
                    else
                    {
                        App.ShiftHistoryDb.UpdateItem(existingHistory);
                    }
                }
            }

            App.ShiftItemsDb.UpdateItem(item);
            ShowItems();
        }

        private async void StopShift(object sender, EventArgs e)
        {
            bool finishShift = await DisplayAlert("Завершить смену?", "После завершения смены вы не сможете ее снова отредактировать!", "Да", "Нет");
            if (!finishShift)
            {
                return;
            }

            _isFirstStart = true;
            Application.Current.Properties["IsFirstStart"] = _isFirstStart;
            Application.Current.Properties["IsShiftStarted"] = false;

            _endTime = DateTime.Now.ToShortTimeString();

            Application.Current.Properties["EndTime"] = _endTime;

            await Application.Current.SavePropertiesAsync();

            await DisplayAlert("Смена окончена", "Посмотреть продажи за сегодня вы сможете в Истории продаж", "Оке");
            await Navigation.PopToRootAsync();


        }


        /*
         * 
          <Grid Margin="0,1">
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto" />
                                            </Grid.RowDefinitions>
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*" />
                                                <ColumnDefinition Width="Auto" />
                                            </Grid.ColumnDefinitions>

                                            <StackLayout Grid.Column="0">
                                                <Button Text="{Binding Name}" TextColor="#222222" FontSize="21" HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" FontAttributes="Italic" BackgroundColor="{Binding HexColor}" Padding="0,0" Margin="2" Clicked="ShiftItemClick" CommandParameter="{Binding ID}"/>
                                            </StackLayout>

                                            <StackLayout Grid.Column="1" Spacing="0" Padding="0" Margin="0" VerticalOptions="CenterAndExpand" IsVisible="{Binding IsChoosed}" IsEnabled="{Binding IsChoosed}">
                                                <Button Text="+" TextColor="White" BackgroundColor="#3872a8" FontSize="20" WidthRequest="23" HeightRequest="23" Padding="0,-4,0,0" Margin="0" Clicked="ItemCountEdit" CommandParameter="{Binding ID}"/>
                                                <Frame WidthRequest="23" HeightRequest="23" Padding="0" Margin="0">
                                                    <Label Text="{Binding Count}" TextColor="Black" HorizontalOptions="CenterAndExpand" FontSize="18" Padding="1,-2,0,0" Margin="0"/>
                                                </Frame>
                                                <Button Text="-" TextColor="White" BackgroundColor="#3872a8" FontSize="30" WidthRequest="23" HeightRequest="23" Padding="0,-11,0,0" Margin="0" Clicked="ItemCountEdit" CommandParameter="{Binding ID}"/>
                                            </StackLayout>
                                        </Grid>
         * 
         */


        private async void AddDeal(object sender, EventArgs e)
        {
            var shiftItems = App.ShiftItemsDb;
            bool anyItems = false;

            foreach (Item item in shiftItems.GetItems<Item>())
            {
                if (item.IsChoosed)
                {
                    anyItems = true;
                    break;
                }
            }
            if (!anyItems)
            {
                await DisplayAlert("Ошибка!", "Чтобы завершить сделку нужно выбрать хотябы одну позицию!", "Извините, я больше так нен буду");
                return;
            }

            _dealCount++;

            foreach (Item item in App.ShiftItemsDb.GetItems<Item>())
            {
                ItemHistory existingHistory = App.ShiftHistoryDb.GetFirstHistoryByName(item.Name);
                if (item.IsChoosed)
                {
                    existingHistory.Count += item.Count - 1;
                }
                App.ShiftHistoryDb.UpdateItem(existingHistory);

                item.IsChoosed = false;
                item.Count = 0;
                item.HexColor = Item.HexDefaultColor[(int)item.type];
                App.ShiftItemsDb.UpdateItem(item);
            }
            ShowItems();
            lbl.Text = _dealCount.ToString();
            Application.Current.Properties["Deals"] = _dealCount.ToString();
            await Application.Current.SavePropertiesAsync();
        }

        private async void ShowHistory(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage());
        }

        private void ItemCountEdit(object sender, EventArgs e)
        {
            var button = sender as Button;
            string buttonType = button.Text;
            int itemId = (int)button.CommandParameter;
            Item item = App.ShiftItemsDb.GetItemById<Item>(itemId);

            string name = item.Name;
            ItemHistory existingHistory = App.ShiftHistoryDb.GetFirstHistoryByName(name);

            if (buttonType == "+")
            {
                item.Count++;
            }
            else
            {
                item.Count--;
                if (item.Count <= 0)
                {
                    item.IsChoosed = false;
                    item.HexColor = Item.HexDefaultColor[(int)item.type];
                    App.ShiftHistoryDb.DeleteItem<ItemHistory>(existingHistory.ID);
                    item.Count = 0;
                }
            }

            App.ShiftHistoryDb.UpdateItem(existingHistory);
            App.ShiftItemsDb.UpdateItem(item);
            ShowItems();
        }
    }
}