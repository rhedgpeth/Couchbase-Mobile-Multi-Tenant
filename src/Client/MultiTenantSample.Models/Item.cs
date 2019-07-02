using System;

namespace MultiTenantSample.Models
{
    public class Item
    {
        public string Type => "item";
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string[] Channels { get; set; }
    }
}
