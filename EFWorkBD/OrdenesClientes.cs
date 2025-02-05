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
    public class OrdenesClientes
    {
        public decimal ID_Orden { get; set; }
        public decimal Cliente { get; set; }
        public string Fecha_Ent { get; set; }
        public string ProductID { get; set; }
        public string Producto { get; set; }
        public string UPC { get; set; }
        public int Qty { get; set; }
        public string NumEmpresa { get; set; }

    }
}