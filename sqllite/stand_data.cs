using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table ("stand_data")]
    public class stand_data
    {
        [PrimaryKey]
        [Column("id")]
        public int Id { get; set; }

        [Column("date_last_entry")]
        public DateTime Date { get; set; }

        [Column("fvs_variant")]
        public string FvsVariant { get; set; } = string.Empty;

        [Column("site_index")]
        public string SiteIndex { get; set; } = string.Empty;

        [Column("habitat_type")]
        public string HabitatType { get; set; } = string.Empty;

        [Column("acres")]
        public float Acres { get; set; }

        [Column("stand_latitude")]
        public float Latitude { get; set; }

        [Column("stand_longitude")]
        public float Longitude { get; set; }

        [Column("stand_aspect(degrees)")]
        public float Aspect { get; set; }

        [Column("stand_slope(degrees)")]
        public float Slope { get; set; }

        [Column("stand_elevation(ft)")]
        public float Elevation { get; set; }

        [Column("plot_list")]
        public List<int> plot_ids_in_stand { get; set; } = new List<int>();
    }
}
