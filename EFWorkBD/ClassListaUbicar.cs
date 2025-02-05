using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BilddenLogistik.EFWorkBD
{
    public class ClassListaUbicar
    {
        public string Ubicacion { get; set; }
        public string Codigo { get; set; }
        public string Descrip { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Merma { get; set; }
        public string Embarque { get; set; }
        public string OC { get; set; }
        public string Factura { get; set; }
        
    }
}