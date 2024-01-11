using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjSharadaMech.Models
{
    public class BOMModel
    {
        public string DocNo { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string sNarration { get; set; }
        public string VersionNo { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Quantity { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Number { get; set; }
        public string UnitConv { get; set; }
        public string ReqQty { get; set; }
        public string Rate { get; set; }
        public string Gross { get; set; }
        public string TypeofItem { get; set; }
        public string OutputItem { get; set; }
    }
    public partial class Temperatures
    {
        [JsonProperty("data")]
        public Datum[] Data { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public partial class Datum
    {
        [JsonProperty("fSessionId")]
        public string FSessionId { get; set; }
    }
    public class HashData3
    {
        //public string url { get; set; }
        public List<Hashtable> data { get; set; }
        public int result { get; set; }
        public string message { get; set; }
    }
    public class PostingData
    {
        public PostingData()
        {
            data = new List<Hashtable>();
        }
        public List<Hashtable> data { get; set; }
    }
    public class Fieldids
    {
        public int Lengthfid { get; set; }
        public int Widthfid { get; set; }
        public int Numberfid { get; set; }
        public int ReqQtyfid { get; set; }
        public int UnitConvfid { get; set; }
    }
}