using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjSharadaMech.Models
{
    public class LoadVouBOM
    {
        public class Body
        {
            public int Item__Id { get; set; }
            public int Unit__Id { get; set; }
            public double Quantity { get; set; }

            [JsonProperty("Length (Mtr)")]
            public LengthMtr LengthMtr { get; set; }

            [JsonProperty("Width (Mtr)")]
            public WidthMtr WidthMtr { get; set; }
            public Number Number { get; set; }

            [JsonProperty("Unit Conv")]
            public UnitConv UnitConv { get; set; }

            [JsonProperty("Req Qty")]
            public ReqQty ReqQty { get; set; }
            public double Rate { get; set; }
            public double Gross { get; set; }
            public int TypeofItem { get; set; }
            public int OutputItem__Id { get; set; }
            public int TransactionId { get; set; }
        }

        public class Datum
        {
            public List<Body> Body { get; set; }
            public Header Header { get; set; }
        }

        public class Header
        {
            public string DocNo { get; set; }
            public int Date { get; set; }
            public string Time { get; set; }
            public string sNarration { get; set; }
            public string VersionNo { get; set; }
            public int HeaderId { get; set; }
        }

        public class LengthMtr
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class Number
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class ReqQty
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
            public string url { get; set; }
            public int result { get; set; }
            public object message { get; set; }
        }

        public class UnitConv
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class WidthMtr
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }
       
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    


}