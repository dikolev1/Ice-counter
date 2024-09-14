
using SQLite;

namespace IceCounter
{
    public class ItemHistory
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
