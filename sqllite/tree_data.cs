using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table("tree_data")]
    public class tree_data
    {
        [Column("parent_plot_id")]
        [Indexed]
        public int parentPlotId { get; set; }


        [PrimaryKey,AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("date_last_entry")]
        public DateTime Date { get; set; }

        [Column("tree_height(ft)")]
        public int Height { get; set; }

        [Column("tree_species")]
        public string Species { get; set; } = string.Empty;

        [Column("diameter_breast_height(in)")]
        public float DiameterBreastHeight { get; set; }

        [Column("stump_height(in)")]
        public float StumpHeight { get; set; }

        [Column("base_of_live_crown(ft)")]
        public float BaseOfLiveCrown { get; set; }

        [Column("crown_ratio(%)")]
        public float CrownRatio { get; set; }


  

    }
}
