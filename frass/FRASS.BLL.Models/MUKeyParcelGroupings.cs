using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
    public class MUKeyParcelGroupings
    {
        public List<MUKeyParcel> MUKeyParcels { get; set; }
        public MUKeyParcelGroupings()
        {
            MUKeyParcels = new List<MUKeyParcel>();
        }
    }
}