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
    public class ClassListaCompras
    {
        //id_supply_order_state, so.date_delivery_expected, so.id_supply_order, so.id_supplier
        // int, string, decimal
        public int id_supply_order_state { get; set; }
        public DateTime date_delivery_expected { get; set; }
        public string id_supply_order { get; set; }
        public string id_supplier { get; set; }
        public string name_supplier { get; set; }
        public int id_warehouse { get; set; }
    }
}