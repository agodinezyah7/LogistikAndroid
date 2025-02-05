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
    [Activity(Label = "Empaques")]
    public class ActivityEmpaques : Activity, ListView.IOnItemClickListener
    {
        private ImageButton imgbtnRegresarEmpaques, imgbtnSaveEmpaques;
        private ListView listViewEmpaques;
        public List<ClassListaEmpaques> catalogo;
        public List<ClassListaEmpaques> listaEmpaques;
        public List<string> mItems;

        private ArrayAdapter adapter;
        public static string ex;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_empaques);
            listViewEmpaques = FindViewById<ListView>(Resource.Id.listViewEmpaques);
            imgbtnSaveEmpaques = FindViewById<ImageButton>(Resource.Id.imgbtnSaveEmpaques);
            imgbtnRegresarEmpaques = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarEmpaques);
            AgregarDatosLista();
            listViewEmpaques.OnItemClickListener = this;
            imgbtnRegresarEmpaques.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            imgbtnSaveEmpaques.Click += delegate
            {
                int position;
                int numFilas = listViewEmpaques.CheckedItemCount;
                if (numFilas > 0)
                {
                    position = listViewEmpaques.CheckedItemPosition;
                    string select = adapter.GetItem(position).ToString();
                    //Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
                    //string lastWord = select.Substring(4, 20);
                    ////Toast.MakeText(this, lastWord, Android.Widget.ToastLength.Short).Show();
                    //Class1.Pedido = lastWord;

                    char[] delimiterChars = { '=' };
                    string[] words = select.Split(delimiterChars);
                    string valores = words[0] + "$" + words[1] + "$" + words[2];
                    Class1.Pedido = words[0];
                    //cell = DGVClientes.Item(x, 3);
                    //vgNomProv = cell;
                    ////'cell = DGVClientes.Item(x, 4)
                    ////'vgAlmRemision = cell
                    //cell = DGVClientes.Item(x, 5);
                    //vgOrdenImporte = cell;
                    //vgEmpresaSelect = DGVClientes.Item(x, 6);

                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    //' Lleva los datos a la tabla de Embarques
                    LlevaVenta_Empaque(Class1.OC);
                    //'FlagOrdenesEmbarqueCheck = False
                    Class1.vgPedimentoArt = "N";
                    Class1.vFinOrden = 0;
                    Class1.FlagOrdenesEmbarqueDet = true;
                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    StartActivity((typeof(ActivityEmpaques_Det)));
                    Finish();
                    //OrdenesEmbarqueDet.txtCaja.Text = "";
                    //OrdenesEmbarqueDet.txtCaja.Focus();
                }
                else
                    Toast.MakeText(this, "No hay una orden seleccionada", ToastLength.Short).Show();
            };

        }
        private void AgregarDatosLista()
        {
            mItems = new List<string>();
            listaEmpaques = new List<ClassListaEmpaques>();
            catalogo = new List<ClassListaEmpaques>();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            var sqllocal = "";

            sqllocal = "select o.current_state ,o.ID_Order, o.date_add, c.company, o.total_paid_real, o.num_empresa" +
        " from Logistik_orders o, Logistik_customer c, Logistik_order_detail od" +
        " where c.taxid = o.taxid" +
        " and od.id_order = o.id_order" +
        " and o.current_state ='9'" +
        " GROUP BY o.current_state, o.ID_Order, o.date_add, c.company , o.total_paid_real, o.num_empresa ";
            string VNOM;
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                while (reader.Read())
                {
                    //DateTime vFecha = DateTime.Now;
                    ClassListaEmpaques EmpaquesE = new ClassListaEmpaques()
                    {
                        Estatus = "E",
                        ID_Orden = (string)reader["ID_Order"],
                        Fecha_Ent = Convert.ToDateTime(reader["date_add"]),
                        Cliente = (string)reader["company"],
                        Importe = Convert.ToDecimal(reader["total_paid_real"]),
                        Empresa = (string)reader["num_empresa"]
                    };
                    listaEmpaques.Add(EmpaquesE);
                }
            }
            catalogo = listaEmpaques;
            //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            //lstVVentas.Adapter = adapter7;
            //ClassListaPedidos.date_delivery_expected = ClassListaPedidos.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
            //Class1.sCodigo sUbica sQty sID 
            //ClassListaPedidos.id_supply_order = id_supply_order.trim;
            //adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Estatus + " = " + x.ID_Orden + " = " + x.Fecha_Ent.ToString("yyyy-MM-dd HH:mm:ss") + " = " + x.Cliente + " = " + x.Empresa).ToArray()));
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Estatus + " = " + x.ID_Orden + " = " + x.Fecha_Ent.ToString("yyyy-MM-dd") + " = " + x.Cliente + " = " + x.Empresa).ToArray()));
            listViewEmpaques.Adapter = adapter;
            //listView1.ItemSelected += listView1_ItemSelected;
            //listView1.ItemClick += listView1_ItemClick;
        }
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewEmpaques.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Class1.vgEnt_Sal = "S";
            Class1.vgEnt_Sal_Datos = 1; //ventas
            char[] delimiterChars = { '=' };
            string select = adapter.GetItem(position).ToString();
            string[] words = select.Split(delimiterChars);
            string valores = words[0] + "$" + words[1] + "$" + words[2];
            Class1.Pedido = words[1];
            Class1.vgOrder = words[1];
            Class1.vgNomProv = words[3];
            ////Class1.vgOrdenImporte = words[5];
            Class1.vgEmpresaSelect = words[4];
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////' Lleva los datos a la tabla de Embarques
            LlevaVenta_Empaque(Class1.vgOrder);
            Class1.vgPedimentoArt = "N";
            Class1.vFinOrden = 0;
            Class1.FlagOrdenesEmbarqueDet = true;
            StartActivity((typeof(ActivityEmpaques_Det)));
            //StartActivity((typeof(Activitymenu)));
            Finish();
        }
        private void LlevaVenta_Empaque(string scan)
        {

            string Empresa = "";
            DateTime Fecha;
            string prodID = "";
            string ProdName = "";
            string ProdUPS = "";
            decimal ProdQty = 0;
            string NumEmpresa = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    string sql = "select o.ID_Order, o.date_add, c.company, od.product_id, od.product_name, od.quantity_received, od.product_upc, od.num_empresa" +
                    " from Logistik_orders o, Logistik_customer c, " +
                    " Logistik_order_detail od" +
                    " where c.taxid = o.taxid" +
                    " and od.id_order = o.id_order" +
                    " and o.current_state = '6'" +
                    " and trim(o.id_order) = '" + scan.Trim() + "'" +
                    " and o.num_empresa = '" + Class1.vgEmpresaSelect.Trim() + "'" +
                    " and od.num_empresa = '" + Class1.vgEmpresaSelect.Trim() + "'" +
                    " GROUP BY o.current_state, o.ID_Order, o.date_add, c.company, od.product_id, od.product_name, od.quantity_received, od.product_upc, od.num_empresa";

                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(sql, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    int vprodID = 0;
                    while (reader.Read())
                    {
                        Empresa = (string)reader["company"];
                        Fecha = Convert.ToDateTime(reader["date_add"]);
                        vprodID = Convert.ToInt32(reader["product_id"]);
                        prodID = Convert.ToString(vprodID);
                        ProdName = (string)reader["product_name"];
                        ProdUPS = (string)reader["product_upc"];
                        ProdQty = Convert.ToDecimal(reader["quantity_received"]);
                        Class1.vNumEmpresa = (string)reader["num_empresa"];


                        //'Ahora inserta en la tabla de Empaques
                        sql = "INSERT INTO vLogistik_Empaques (id_order, company, date_add, product_id, product_name," +
                             " quantity_received, qty_caja, product_upc, caja, FlagPrint, num_empresa) VALUES (" +
                             "'" + scan + "','" + Empresa + "','" + Fecha.ToString() + "','" + prodID + "','" + ProdName + "','" +
                             ProdQty.ToString() + "','0','" + ProdUPS + "','','0','" + Class1.vNumEmpresa + "')";
                        EjecutarQuerySQLWifi(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Verifique la conexion a la red y el servidor!, no hay conexion!!", ToastLength.Short).Show();
            }
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