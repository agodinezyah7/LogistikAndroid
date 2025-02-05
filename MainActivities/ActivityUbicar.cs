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

using BilddenLogistik.EFWorkBD;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityUbicar")]
    public class ActivityUbicar : Activity
    {
        private Button ButtonRegresarU, ButtonProdenTarima, ButtonProdenUbicacion, ButtonTarimaU;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_ubicar);
            ButtonRegresarU = FindViewById<Button>(Resource.Id.ButtonRegresarU);
            ButtonRegresarU.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            ButtonProdenTarima = FindViewById<Button>(Resource.Id.ButtonProdenTarima);
            ButtonProdenTarima.Click += delegate
            {
                //Producto a Tarima
                Class1.TipoUbica = 1;
                Class1.FlagUbica = true;
                StartActivity((typeof(ActivityUbicarProd)));
                Finish();
            };
            ButtonProdenUbicacion = FindViewById<Button>(Resource.Id.ButtonProdenUbicacion);
            ButtonProdenUbicacion.Click += delegate
            {
                //Producto a ubicacion
                Class1.TipoUbica = 2;
                Class1.FlagUbica = true;
                StartActivity((typeof(ActivityUbicarProd)));
                Finish();
            };
            ButtonTarimaU = FindViewById<Button>(Resource.Id.ButtonTarimaU);
            ButtonTarimaU.Click += delegate
            {
                //Tarima a ubicacion
                Class1.TipoUbica = 3;
                Class1.FlagUbica = true;
                StartActivity((typeof(ActivityUbicarProd)));
                Finish();
            };
           
        }
    }
}