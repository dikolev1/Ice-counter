using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IceCounter
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPage : ContentPage
	{
		public HistoryPage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            if (!Application.Current.Properties.ContainsKey("StartTime"))
            {
                HistoryNotFound();
            }
            else
            {
                ShowInfo();
            }
        }

        private void HistoryNotFound()
        {
            Label label = new Label
            {
                Text = "История не найдена :(",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
            };

            Content = label;
        }

		private void ShowInfo()
		{
            bool isShiftStarted = false;
            if (Application.Current.Properties.ContainsKey("IsShiftStarted"))
            {
                isShiftStarted = (bool)Application.Current.Properties["IsShiftStarted"];
            }

            List<ItemHistory> salesHistory = App.ShiftHistoryDb.GetItems<ItemHistory>();
            int numberOfItems = salesHistory.Count;
            int rows = (numberOfItems + 1) / 2;

            for (int i = 0; i < numberOfItems; i++)
            {
                ItemHistory item = salesHistory[i];
                Label itemName = new Label
                {
                    Text = item.Name,
                    TextColor = Color.White,
                    FontSize = 16,

                    Padding = 0,
                    Margin = 0,

                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                };
                Label itemCount = new Label
                {
                    Text = item.Count.ToString(),
                    TextColor = Color.White,
                    FontSize = 16,

                    Padding = 0,
                    Margin = 0,

                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                };

                StackLayout itemInfo = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
                    Padding = 4,
                    Margin = 0,
                };
                itemInfo.Children.Add(itemName);
                itemInfo.Children.Add(itemCount);

                Frame frame = new Frame
                {
                    BackgroundColor = Color.FromHex("#424242"),
                    Padding = 3,

                    Content = itemInfo

                };

                Grid.SetRow(frame, i % rows);
                Grid.SetColumn(frame, i / rows);

                Sales.Children.Add(frame);
            }

            deals.Text = (string)Application.Current.Properties["Deals"];
            startShiftLbl.Text = (string)Application.Current.Properties["StartTime"];
            endShiftLbl.Text = isShiftStarted ? "?" : (string)Application.Current.Properties["EndTime"];
        }
    }
}