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

    public class ClassConsualtaVer
    {
        public string Codigo { get; set; }

        public string Descrip { get; set; }
        
        public decimal Cantidad { get; set; }
        
        public string Tarima { get; set; }
        
        public string Ubicacion { get; set; }
        
        public string Embarque { get; set; }
        
        public string OC { get; set; }
        
        public string Factura { get; set; }
    }
}