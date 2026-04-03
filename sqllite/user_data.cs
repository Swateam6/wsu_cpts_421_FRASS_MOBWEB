using SQLite;
namespace MOBWEB_TEST.sqllite
{
    [Table("user_data")]
    public class user_data
    {
        [Column("parcel_list")]
        public List<int> parcel_ids_in_user { get; set; } = new List<int>();
    }
}
