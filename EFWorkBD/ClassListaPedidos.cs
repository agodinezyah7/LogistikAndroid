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
    public class ClassListaPedidos
    {
        public int Estatus { get; set; }
        public string ID_Orden { get; set; }
        public DateTime Fecha_Ent { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        //public string Almacen { get; set; }
        //public string Importe { get; set; }
    }
}