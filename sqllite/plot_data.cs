using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table("plot_data")]
    class plot_data
    {
        [PrimaryKey]
        [Column("id")]
        public int Id { get; set; }

        [Column("date_last_entry")]
        public DateTime Date { get; set; }

        [Column("plot_latitude")]
        public float Latitude { get; set; }

        [Column("plot_longitude")]
        public float Longitude { get; set; }

        [Column("plot_aspect(degrees)")]
        public int Aspect { get; set; }

        [Column("plot_slope(degrees)")]
        public int Slope { get; set; }

        [Column("plot_elevation(ft)")]
        public int Elevation { get; set; }

        [Column("plot_image_filepath")]
        public string ImagePath { get; set; } = string.Empty;

        [Column("most_mesic_tree_species")]
        public string MostMesicTreeSpecies { get; set; } = string.Empty;

        [Column("most_mesic_bush_species")]
        public string MostMesicBushSpecies { get; set; } = string.Empty;

        [Column("tree_list")]
        public List<int> tree_ids_in_plot { get; set; } = new List<int>();
    }
}
