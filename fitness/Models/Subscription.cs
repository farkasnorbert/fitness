using System;

namespace fitness.Models
{
    public class Subscription
    {
        public int deleted { get; set; }
        public int code { get; set; }
        public string name { get; set; }
        public DateTime exp { get; set; }
        public int numberofentrence { get; set; }
        public int maxnumberofentrence { get; set; }
    }
}