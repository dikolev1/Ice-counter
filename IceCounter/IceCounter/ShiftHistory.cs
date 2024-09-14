using SQLite;
using System.Collections.Generic;

namespace IceCounter
{
    public class ShiftHistory
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public bool IsActive { get; set; }

        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Deals { get; set; }

        // Item
        public int ItemId { get; set; }
        public List<string> Names { get; set; }
        public List<int> Counts { get; set; }

    }
}
