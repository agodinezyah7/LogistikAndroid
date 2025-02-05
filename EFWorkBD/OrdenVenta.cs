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
    public class OrdenVenta
    {
        public string Codigo { get; set; }
        public string Descrip { get; set; }
        public decimal Cant_Rec { get; set; }
    }
}