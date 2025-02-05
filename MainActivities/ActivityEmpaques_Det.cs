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
//using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityEmpaques_Det")]
    public class ActivityEmpaques_Det : Activity
    {
        
        private ListView listViewPacking;
        public List<OrdenVenta2> catalogoEmp;
        public List<OrdenVenta2> listaEmpaques;
        public List<string> mItems;
        private ArrayAdapter adapter1;
        private TextView textViewPedido;
        private Spinner spinnerEmpaqueD;
        private EditText editTextCodED, editTextCantED, editTextPesoED, editTextCajaED;
        private ImageButton imgbtnRegresarSalida, imgbtnReporteED, imgbtnGuardarEmpaque;
        string vAlmacen;
        string vlCtrlCant;
        int vlcant;
        int vlid_producto;
        string vlUtilizaNumSer;
        string vlUtilizaLotes;
        string vlUtilizaPedimento;
        decimal vCantPedida = 0;
        decimal vCantRecibida = 0;
        bool FlagTermino = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_empaques_det);
            
            editTextCajaED = FindViewById<EditText>(Resource.Id.editTextCajaED);
            spinnerEmpaqueD = FindViewById<Spinner>(Resource.Id.spinnerEmpaqueD);
            //editTextNoCaja = FindViewById<EditText>(Resource.Id.editTextNoCaja);
            editTextCodED = FindViewById<EditText>(Resource.Id.editTextCodED);
            editTextCantED = FindViewById<EditText>(Resource.Id.editTextCantED);
            editTextPesoED = FindViewById<EditText>(Resource.Id.editTextPesoED);
            textViewPedido = FindViewById<TextView>(Resource.Id.textViewPedido);
            imgbtnRegresarSalida = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarSalida);
            imgbtnReporteED = FindViewById<ImageButton>(Resource.Id.imgbtnReporteED);
            imgbtnGuardarEmpaque = FindViewById<ImageButton>(Resource.Id.imgbtnGuardarEmpaque);
            imgbtnRegresarSalida.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            imgbtnReporteED.Click += delegate
            {
                Class1.FlagOrdenesEmbarqueCheck = true;
                StartActivity((typeof(ActivityEmpaques_Check)));
                Finish();
            };
            imgbtnGuardarEmpaque.Click += delegate
            {
                string Sql1 = "";
                int numlist = listViewPacking.CheckedItemCount;
                if (numlist > 0)
                    Toast.MakeText(this, "Aun tiene productos que empacar!", ToastLength.Short).Show();
                else
                {
                    //' Coloca estatus en 9 en encabezado de ordenes para que ya quede registrado ese pedido como empacado
                    Sql1 = "UPDATE Logistik_Orders SET current_state=" + "9" + " WHERE id_Order = " + Class1.OC + " AND num_empresa = '" + Class1.vgEmpresaSelect + "'";
                    EjecutarQuerySQLWifi(Sql1);

                    //' Coloca la bandera de imprimir las etiquetas en 1 para esa caja
                    //Sql1 = "UPDATE vLogistik_Empaques SET FlagPrint=" + "2" + ", peso=" + editTextPesoED.Text + " WHERE id_Order = " + Class1.OC + " AND caja = '" + editTextNoCaja.Text + "' AND num_empresa = '" + Class1.vgEmpresaSelect + "'";
                    //EjecutarQuerySQLWifi(Sql1);

                    StartActivity((typeof(ActivityEmpaques)));
                    Finish();
                }


            };
            //LeerXML();
            Class1.vgEnt_Sal_Datos = 1; //'"Ventas"
            Class1.vgEnt_Sal = "S";
            CargarLista();
            LlenarClientes();
            textViewPedido.Text = "Pedido: " + Class1.Pedido.Trim();

        }
        private void CargarLista()
        {
            if (Class1.FlagOrdenesEmbarqueDet)
            {
                Class1.FlagOrdenesEmbarqueDet = false;
                textViewPedido.Text = Class1.OC;

                //dtOrdenVenta.Clear()
                LlenarSpinerEmpaques();
                LlenarClientes();
                editTextCajaED.Text = "1";
                editTextCajaED.Enabled = false;
                editTextCodED.Text = "";

                if (Class1.FlagOrdenesEmbarqueCheck)
                {
                    //if (editTextNoCaja.Text != "")
                    //{
                    //    editTextCodED.Enabled = true;
                    //    editTextCantED.Enabled = true;
                    //    editTextPesoED.Enabled = true;
                    //    imgbtnRegresarSalida.Enabled = false;
                    //}
                    //else
                    //{
                    //    editTextCodED.Enabled = false;
                    //    editTextCantED.Enabled = false;
                    //    editTextPesoED.Enabled = false;
                    //    imgbtnRegresarSalida.Enabled = true;
                    //}
                    Class1.FlagOrdenesEmbarqueCheck = false;
                }
                else
                {
                    //'txtCaja.Enabled = True
                    editTextCodED.Enabled = false;
                    editTextCantED.Enabled = false;
                    editTextPesoED.Enabled = false;
                    imgbtnRegresarSalida.Enabled = true;
                }

                if (Class1.vFinOrden == 1)
                {
                    StartActivity((typeof(Activitymenu)));
                    Finish();
                }
            }
        }

        private void LlenarSpinerEmpaques()
        {
            List<String> labels = getAllLabels();

            // Creating adapter for spinner  
            ArrayAdapter<String> dataAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, labels);

            // Drop down layout style - list view with radio button  
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            // attaching data adapter to spinner  
            spinnerEmpaqueD.Adapter = dataAdapter;
            //spinner1.setAdapter(dataAdapter);
        }
        public List<string> getAllLabels()
        {
            List<string> list = new List<string>();

            // Select All Query
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "SELECT id, tipo_empaque FROM vLogistik_Tipo_Empaque";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                CrearBD.vgOrder = 1;
                //int numMotivo = 0;
                //string Motivo = "";
                while (reader.Read())
                {
                    //numMotivo = reader.GetInt32(0);
                    //Motivo = reader.GetString(1);
                    //list.Add(numMotivo + "=" + Motivo);
                    list.Add(reader.GetString(1));

                }
                return list;
            }
        }

        private void LlenarClientes()
        {
            mItems = new List<string>();
            listaEmpaques = new List<OrdenVenta2>();
            catalogoEmp = new List<OrdenVenta2>();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            var sqllocal = "";
            Class1.Pedido = Class1.Pedido.Substring(1, 20);
            sqllocal = "select company, product_upc, product_name, quantity_received from vLogistik_Empaques" +
            " where ID_Order = '" + Class1.Pedido + "' and quantity_received > 0 and num_empresa = '" + Class1.vgEmpresaSelect.Trim() + "'";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                while (reader.Read())
                {
                    string cod1 = Convert.ToString(reader["product_upc"]).Trim();
                    string desc1 = Convert.ToString(reader["product_name"]).Trim();
                    decimal cant1 = Convert.ToDecimal(reader["quantity_received"]);
                    OrdenVenta2 EmpaqueD = new OrdenVenta2()
                    {
                        Codigo = cod1,
                        Descrip = desc1,
                        Cant_Rec = cant1
                    };
                    listaEmpaques.Add(EmpaqueD);
                }
            }
            catalogoEmp = listaEmpaques;
            //adapter1 = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoEmp.Select(x => x.Codigo + " = " + x.Descrip + " = " + x.Cant_Rec).ToArray()));
            adapter1 = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoEmp.Select(x1 => x1.Codigo + "1111  pollo  7777").ToArray()));

            //listViewPacking.Adapter = adapter1;
        }
        private void EjecutarQuerySQLWifi(string sql1)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(sql1, con);
                    sqlcmd1.ExecuteNonQuery();
                    sqlcmd1.Dispose();
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }
        }
    }
}