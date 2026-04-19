using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table("defect_data")]
    public class defect_data
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int parentTreeId { get; set; } // Link to the tree
        public string? Description { get; set; }

        // Raw gyro angles for the math
        public double BaseAngle { get; set; }
        public double TopAngle { get; set; }

        // The final result (no raw distance stored here)
        public double CalculatedHeight { get; set; }

        public double topHeight { get; set; }
        public double bottomHeight { get; set; }

    }
}
