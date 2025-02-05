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
    public class ClassUbicaProd
    {
        public string Codigo { get; set; }
        public string Descrip { get; set; }
        public int Cantidad { get; set; }
    }
}