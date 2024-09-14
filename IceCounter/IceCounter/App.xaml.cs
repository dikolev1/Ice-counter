using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace IceCounter
{
    public partial class App : Application
    {
        private static DBController allItemsDb;
        public static DBController AllItemsDb
        {
            get
            {
                if (allItemsDb == null)
                {
                    allItemsDb = new DBController(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "allItemsDb.sqlite3"));
                    allItemsDb.CreateTableOfType<Item>();
                }
                return allItemsDb;
            }
        }

        private static DBController shiftItemsDb;
        public static DBController ShiftItemsDb
        {
            get
            {
                if (shiftItemsDb == null)
                {
                    shiftItemsDb = new DBController(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "shitItemsDb.sqlite3"));
                    shiftItemsDb.CreateTableOfType<Item>();
                }
                return shiftItemsDb;
            }
        }

        private static DBController shiftHistoryDb;
        public static DBController ShiftHistoryDb
        {
            get
            {
                if (shiftHistoryDb == null)
                {
                    shiftHistoryDb = new DBController(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "shitHistoryDb.sqlite3"));
                    shiftHistoryDb.CreateTableOfType<ItemHistory>();
                }                
                return shiftHistoryDb;
            }
        }


        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
