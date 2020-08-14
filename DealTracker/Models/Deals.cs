using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealTracker.Models
{
    public class Deals
    {
        public int DealNumber { get; set; }
        public string CustomerName { get; set; }
        public string DealershipName { get; set; }
        public string Vehicle { get; set; }
        public double Price { get; set; }
        public string Date { get; set; }
    }
}