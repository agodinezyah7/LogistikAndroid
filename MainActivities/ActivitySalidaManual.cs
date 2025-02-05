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
using System.Data;
using System.Data.SqlClient;
using BilddenLogistik.EFWorkBD;
using BilddenLogistik.MainActivities;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivitySalidaManual")]
    public class ActivitySalidaManual : Activity
    {
        private ListView listViewSalidaManual;
        public List<ClassUbicaProd> catalogoUbicar;
        public List<ClassUbicaProd> listaUbicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private ImageButton imgbtnRegresarSalidaM, imgbtnGuardarSalidaM;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layoutSalidaManual);
            listViewSalidaManual = FindViewById<ListView>(Resource.Id.listViewSalidaManual);
            imgbtnRegresarSalidaM = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarSalidaM);
            imgbtnGuardarSalidaM = FindViewById<ImageButton>(Resource.Id.imgbtnGuardarSalidaM);
            CargarForma();
            imgbtnGuardarSalidaM.Visibility = ViewStates.Invisible;
            imgbtnRegresarSalidaM.Click += delegate
            {
                StartActivity((typeof(ActivitySalida)));
                Finish();
            };
            imgbtnGuardarSalidaM.Click += delegate
            {
                //desactivado
            };
        }
        private void CargarForma()
        {
            LlenarPedido();

        }
        private void LlenarPedido()
        {
            try
            {
                mItems = new List<string>();
                listaUbicar = new List<ClassUbicaProd>();
                catalogoUbicar = new List<ClassUbicaProd>();
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "select lit.num_parte, lpl.description, lit.cantidad, lit.id_product " +
                    " from Logistik_Inventario_Temp AS lit INNER JOIN Logistik_product AS lp ON lit.num_parte = lp.upc INNER JOIN" +
                    " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product"; //+
                   // " WHERE id_order='" + CrearBD.vgOrder + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    CrearBD.vgOrder = 1;
                    while (reader.Read())
                    {
                        ClassUbicaProd UbicarDetalle = new ClassUbicaProd()
                        {
                            Codigo = (string)reader["num_parte"],
                            Descrip = (string)reader["description"],
                            Cantidad = Convert.ToInt32(reader["cantidad"])
                        };
                        listaUbicar.Add(UbicarDetalle);
                    }
                    catalogoUbicar = listaUbicar;
                    adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Codigo + " = " + x.Descrip + " = " + x.Cantidad).ToArray()));
                    listViewSalidaManual.Adapter = adapter;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }
        }
    }
}