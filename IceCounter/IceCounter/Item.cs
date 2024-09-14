using SQLite;
using Xamarin.Forms;

namespace IceCounter
{
    public class Item
    {
        public enum Type {Icecream, Dish}
        public static string[] HexDefaultColor = new string[] { "#679fe7", "#e7af67" };
        public static string[] HexChoosedColor = new string[] { "#3c73ba", "#ba833c" };

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public Type type { get; set; }

        // Только для предметов на смене
        public int Count { get; set; }
        public bool IsChoosed { get; set; }
        public string HexColor { get; set; }

    }
}
