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
    public class ClassListaPedidoD1
    {
        public decimal Cant_Rec { get; set; }
        public decimal Cant_Ped { get; set; }
        public string Codigo { get; set; }
        public string Descrip { get; set; }
        public string ID_Orden { get; set; }
        public string Ubica { get; set; }
        public int ID { get; set; }
        public string Empresa { get; set; }

        //Codigo,Descrip,ID_Orden,Ubica,ID,Empresa
    }
}