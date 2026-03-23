using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table("tree_data")]
    public class tree_data
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

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

        [Column("defect_description")]
        public string DefectDescription { get; set; } = string.Empty;

        [Column("defect_base(ft)")]
        public float DefectBase { get; set; }

        [Column("defect_top(ft)")]
        public float DefectTop { get; set; }
    }
}
