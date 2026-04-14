using SQLite;
namespace MOBWEB_TEST.sqllite
{
    [Table("user_data")]
    public class user_data
    {
        [PrimaryKey]
        [Column("id"),]
        public int Id { get; set; }

        [Column("parcel_list")]
        public string ParcelListRaw { get; set; } = string.Empty;

        [Ignore]
        public List<int> parcel_ids_in_parcel
        {
            get => ParcelListRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(int.Parse).ToList();
            set => ParcelListRaw = string.Join(",", value);
        }
    }
}
