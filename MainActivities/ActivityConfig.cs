﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityConfig")]
    public class ActivityConfig : Activity
    {
        private ImageButton imgbtnRegresarConfig;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_configuracion);
            imgbtnRegresarConfig = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarConfig);
            imgbtnRegresarConfig.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
        }
    }
}