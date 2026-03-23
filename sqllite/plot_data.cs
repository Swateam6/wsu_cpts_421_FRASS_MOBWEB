using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table("plot_data")]
    class plot_data
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("stand_number")]
        public string StandNumber { get; set; } = string.Empty;

        [Column("plot_number")]
        public string PlotNumber { get; set; } = string.Empty;

        [Column("date")]
        public DateTime Date { get; set; }

        //TODO: implement lat/long

        [Column("plot_aspect(degrees)")]
        public int Aspect { get; set; }

        [Column("plot_slope(degrees)")]
        public int Slope { get; set; }

        [Column("plot_elevation(ft)")]
        public int Elevation { get; set; }

        [Column("plot_image_filepath")]
        public string ImagePath { get; set; } = string.Empty;
    }
}
