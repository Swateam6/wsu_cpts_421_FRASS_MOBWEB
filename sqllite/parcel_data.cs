using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table ("parcel_data")]
    public class parcel_data
    {
        [PrimaryKey]
        [Column("id")]
        public int Id { get; set; }

        [Column("date_last_entry")]
        public DateTime Date { get; set; }

        [Column("parcel_acres")]
        public float Acres { get; set; }

        [Column("stand_list")]
        public List<int> stand_ids_in_parcel { get; set; } = new List<int>();
    }
}
