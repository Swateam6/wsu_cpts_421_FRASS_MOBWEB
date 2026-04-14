using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table ("parcel_data")]
    public class parcel_data
    {
        [Column("id"),AutoIncrement]
        public int Id { get; set; }

        [Column("date_last_entry")]
        public DateTime Date { get; set; }

        [Column("parcel_acres")]
        public float Acres { get; set; }

        [Column("stand_list")]
        public string StandListRaw { get; set; } = string.Empty;

        [Ignore]
        public List<int> stand_ids_in_parcel
        {
            get => StandListRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(int.Parse).ToList();
            set => StandListRaw = string.Join(",", value);
        }
    }
}
