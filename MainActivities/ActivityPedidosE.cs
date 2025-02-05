using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.AccessibilityServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BilddenLogistik.EFWorkBD;

//using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "Pedidos Encabezado")]
    public class ActivityPedidosE : Activity, ListView.IOnItemClickListener
    {
        //private GridView gridviewCE;
        private ImageButton imgbtnRegresar;
        private ListView listViewPed;
        private EditText txtVTotalItems7;
        //private RecyclerView recyclerView1;
        public List<ClassListaPedidos> catalogo;
        public List<ClassListaPedidos> listaVentas;
        public List<string> mItems;
        private ArrayAdapter adapter;
        public static string ex;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutPedidosE);
            //CargarCompra();
            //RequestWindowFeature(Android.Views.WindowFeatures.ActionBar);
            //ActionBar.Title = "pollo";
            listViewPed = FindViewById<ListView>(Resource.Id.listViewPedidos);
            //txtVTotalItems7 = FindViewById<EditText>(Resource.Id.txtVTotalItemsPed); 
             imgbtnRegresar = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarPedido);
            AgregarDatosLista();
            ObtenerConteos();
            listViewPed.OnItemClickListener=this;
            //listView1.ItemClick += (sender, e) =>
            //{
            //    string select = listView1.GetItemAtPosition(e.Position).ToString();
            //    Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
            //};
            //txtVTotalItems7.Text = Convert.ToString(Class1.TotalOrdenes);
            imgbtnRegresar.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };

        }//fin Oncreate
        private void AgregarDatosLista()
        {
            mItems = new List<string>();
            listaVentas = new List<ClassListaPedidos>();
            catalogo = new List<ClassListaPedidos>();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            var sqllocal = "";
            if (Class1.vgEmployee_Perfil == 8){
                sqllocal = "select o.current_state ,o.ID_Order, o.date_add, c.company, o.num_empresa" +
                " from Logistik_orders o, Logistik_customer c, " +
                " Logistik_order_detail od" +
                " where c.taxid = o.taxid" +
                " and o.id_lang = '2'" +
                " and o.current_state in(10,11)" +
                " GROUP BY o.current_state, o.ID_Order, o.date_add, c.company, o.num_empresa " +
                " ORDER BY o.ID_Order DESC";
                }
            else{
                // Agregar num_empresa en el filtro
                sqllocal = "select o.current_state ,o.ID_Order, o.date_add, c.company, o.num_empresa" +
                " from Logistik_orders o, Logistik_customer c, " +
                " Logistik_warehouse_shop ws, Logistik_order_detail od" +
                " where ws.id_shop = o.id_shop" +
                " and c.taxid = o.taxid" +
                " and od.id_order = o.id_order" +
                " and o.id_lang = '2'" +
                " and o.current_state in(" + Class1.vgEnt_Sal_Datos + ",4,8,911)" +
                " GROUP BY o.current_state, o.ID_Order, o.date_add, c.company, o.num_empresa " +
                " ORDER BY o.ID_Order DESC";
            }
            //var sqllocal = "select so.id_supply_order_state, so.date_upd, so.id_supply_order, so.id_supplier, s.name, ws.id_warehouse from Logistik_supply_order so, Logistik_supplier s, Logistik_warehouse_shop ws where ws.id_warehouse = so.id_warehouse and s.id_supplier = so.id_supplier and so.id_lang = '2' and so.id_supply_order_state in(" + Class1.vgEnt_Sal_Datos + ",4,8,911)";

            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                while (reader.Read())
                {
                    ClassListaPedidos pedidosE = new ClassListaPedidos()
                    {
                        Estatus = (int)reader["current_state"],
                        ID_Orden = (string)reader["ID_Order"],
                        Fecha_Ent = (DateTime)reader["date_add"],
                        Cliente = (string)reader["company"],
                        Empresa = (string)reader["num_empresa"]
                    };
                    listaVentas.Add(pedidosE);
                }
            }
            catalogo = listaVentas;
            //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            //lstVVentas.Adapter = adapter7;
            //ClassListaPedidos.date_delivery_expected = ClassListaPedidos.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
            //Class1.sCodigo sUbica sQty sID 
            //ClassListaPedidos.id_supply_order = id_supply_order.trim;
            //adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Estatus + " = " + x.ID_Orden + " = " + x.Fecha_Ent.ToString("yyyy-MM-dd HH:mm:ss") + " = " + x.Cliente + " = " + x.Empresa).ToArray()));
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Estatus + "=" + x.ID_Orden + "=" + x.Fecha_Ent.ToString("yyyy-MM-dd") + "=" + x.Cliente + "=" + x.Empresa).ToArray()));
            listViewPed.Adapter = adapter;
            //listView1.ItemSelected += listView1_ItemSelected;
            //listView1.ItemClick += listView1_ItemClick;
        }

        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewPed.GetItemAtPosition(e.Position).ToString();
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
            //Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
            string lastWord = select.Substring(4, 20);
            //Toast.MakeText(this, lastWord, Android.Widget.ToastLength.Short).Show();
            Class1.Pedido = lastWord;
            Class1.Pedido = words[1];
            //Class1.Pedido = Class1.Pedido.Trim();
            //String var = "Hola Mundo";
            //int tam_var = var.Length;
            //String Var_Sub = var.Substring((tam_var - 2), 2);

            int tam_var = select.Length;
            String Var_Sub = select.Substring((tam_var - 2), 2);
            string lastWord1 = Var_Sub;
            //Toast.MakeText(this, lastWord1, Android.Widget.ToastLength.Short).Show();
            Class1.vgEmpresaSelect = lastWord1;
            if(Class1.vgEmployee_Perfil==8)
            {
                StartActivity((typeof(ActivityPedidosD2)));
                Finish();
            }
            else
            {
                StartActivity((typeof(ActivityPedidosD1)));
                Finish();
            }
            
            //string b = string.Empty;
            //for (int i = 0; i < lastWord.Length; i++)
            //{
            //    //if (Char.IsLetter(select[i]))
            //    if (Char.IsDigit(lastWord[i]))
            //        b += select[i];
            //}
            //Toast.MakeText(this, b, Android.Widget.ToastLength.Short).Show();
        }
        private void ObtenerConteos()
        {
            var sqllocal = "select so.id_supply_order_state, so.date_delivery_expected, so.id_supply_order, so.id_supplier, s.name, ws.id_warehouse from Logistik_supply_order so, Logistik_supplier s, Logistik_warehouse_shop ws where ws.id_warehouse = so.id_warehouse and s.id_supplier = so.id_supplier and so.id_lang = '2' and so.id_supply_order_state in(" + Class1.vgEnt_Sal_Datos + ",4,8,911)";

            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                int conteo = 0;
                while (reader.Read())
                {
                    conteo++;

                }
                Class1.TotalOrdenes = conteo;
            }
        }
    }//fin class Activity
}//fin namespace