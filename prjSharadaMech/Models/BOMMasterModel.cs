using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjSharadaMech.Models
{
    public class BOMMasterModel
    {
        public int  Date { get; set; }
        public string Narration { get; set; }
        public int  Output_Item_Id { get; set; }
        public string Output_Item_Code { get; set; }
        public string Output_Item { get; set; }
        public int DefaultReplinishment { get; set; }
        public int Output_DefaultBaseUnit { get; set; }
        public int Input_DefaultBaseUnit { get; set; }
        public int Input_Item_Id { get; set; }
        public string Input_Item_Code { get; set; }
        public string Input_Item { get; set; }
        public string Description { get; set; }
        public decimal Req_Quantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public string VersionNo { get; set; }
    }
}