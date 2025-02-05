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
//using Android.Support.V7.Widget;
//using Android.Support.V7.App;
using BilddenLogistik.EFWorkBD;

namespace BilddenLogistik.MainActivities
{
    //theme="@style/Theme.AppCompat.NoActionBar"
    [Activity(Label = "Reubicar", Theme = "@style/AppTheme", MainLauncher = false)]
    public class ActivityReubicar : Activity
    {
        private Button ButtonRegresarRE, ButtonSacarProd, ButtonProductos, ButtonMeterProd, ButtonTarimaRE;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_reubicar);
            ButtonRegresarRE = FindViewById<Button>(Resource.Id.ButtonRegresarRE);
            ButtonRegresarRE.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            ButtonSacarProd = FindViewById<Button>(Resource.Id.ButtonSacarProd);
            ButtonSacarProd.Click += delegate
            {
                // Sacar Productos de Tarima
                Class1.TipoProd = 1;
                Class1.FlagReubica = true;
                StartActivity((typeof(ActivityReubicarDet)));
                Finish();
            };
            ButtonProductos = FindViewById<Button>(Resource.Id.ButtonProductos);
            ButtonProductos.Click += delegate
            {
                // Productos a nueva Ubicacion
                Class1.TipoProd = 2;
                Class1.FlagReubica = true;
                StartActivity((typeof(ActivityReubicarDet)));
                Finish();
            };
            ButtonMeterProd = FindViewById<Button>(Resource.Id.ButtonMeterProd);
            ButtonMeterProd.Click += delegate
            {
                // Productos de Tarima a Ubicacion
                Class1.TipoProd = 4;
                Class1.FlagReubica = true;
                StartActivity((typeof(ActivityReubicarDet)));
                Finish();
            };
            ButtonTarimaRE = FindViewById<Button>(Resource.Id.ButtonTarimaRE);
            ButtonTarimaRE.Click += delegate
            {
                // Tarima a nueva Ubicacion
                Class1.TipoProd = 3;
                Class1.FlagReubica = true;
                StartActivity((typeof(ActivityReubicarDet)));
                Finish();
            };
        }
    }
}