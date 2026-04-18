using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBWEB_TEST.Models
{
    public  class Defects
    {
        public int treeID { get; set; }
        public int ID { get; set; }
        public string? DefectType { get; set; }
        public double topHeight{ get; set; }

        public double bottomHeight { get; set; }
    }
}
