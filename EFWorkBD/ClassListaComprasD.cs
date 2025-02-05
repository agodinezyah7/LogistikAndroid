using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BilddenLogistik.EFWorkBD
{
    public class ClassListaComprasD
    {
        //sod.id_supply_order_detail, so.id_supply_order, sod.upc, pl.description, sod.quantity_expected, sod.quantity_received
        // int, string, decimal, Dattime
        public int id_supply_order_detail { get; set; }
        public string id_supply_order { get; set; }
        public string upc { get; set; }
        public string description { get; set; }
        public decimal quantity_received { get; set; }
        public decimal quantity_expected { get; set; }
        
    }
}