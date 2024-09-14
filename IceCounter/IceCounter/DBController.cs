using System.Collections.Generic;
using SQLite;
using Xamarin.Forms;

namespace IceCounter
{
    public class DBController
    {

        private readonly SQLiteConnection _connection;

        public DBController(string path)
        {
            _connection = new SQLiteConnection(path);
        }

        public SQLiteConnection GetConnection()
        {
            return _connection;
        }

        public void CreateTableOfType<T>()
        {
            _connection.CreateTable<T>();
        }

        public List<T> GetItems<T>() where T : new()
        {
            return _connection.Table<T>().ToList();
        }

        // Только для таблиц Item
        public List<Item> GetOneTypeItems(Item.Type type)
        {
            List<Item> allItems = _connection.Table<Item>().ToList();
            List<Item> oneTypeItems = new List<Item>();

            foreach (Item item in allItems)
            {
                if (item.type == type)
                {
                    oneTypeItems.Add(item);
                }
            }

            return oneTypeItems;
        }

        public T GetItemById<T>(int id) where T : new()
        {
            return _connection.Find<T>(id);
        }

        public Item GetFirstItemByName(string name)
        {
            foreach (Item item in GetItems<Item>())
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
        public ItemHistory GetFirstHistoryByName(string name)
        {
            foreach (ItemHistory item in GetItems<ItemHistory>())
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public int UpdateItem<T>(T item)
        {
            return _connection.Update(item);
        }
        public int UpdateAll<T>() where T : new()
        {
            return _connection.UpdateAll(GetItems<T>());
        }

        public int SaveItem<T>(T item)
        {
            return _connection.Insert(item);
        }

        public int DeleteItem<T>(int id) where T : new()
        {
            return _connection.Delete<T>(id);
        }

        public void ClearTable<T>() where T : new()
        {
            _connection.DeleteAll<T>();
        }
    }
}
