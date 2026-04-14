using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table ("parcel_data")]
    public class parcel_data
    {
        [Column("parent_user_id")]
        [Indexed]
        public int parentUserId { get; set; }


        [Column("id"),AutoIncrement]
        public int Id { get; set; }

        [Column("date_last_entry")]
        public DateTime Date { get; set; }

        [Column("parcel_acres")]
        public float Acres { get; set; }
    }
}
