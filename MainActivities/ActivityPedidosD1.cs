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
    [Activity(Label = "Pedidos Detalle 1")]
    public class ActivityPedidosD1 : Activity, ListView.IOnItemClickListener
    {
        private ListView listViewDetallePed;
        private EditText txtVTotalPedD;
        private ImageButton imgbtnRegresarPedD1, imgbtnsiguiente;
         //private RecyclerView recyclerView1;
        public List<ClassListaPedidoD1> catalogo;
        public List<ClassListaPedidoD1> listaPedidosD;
        public List<string> mItems;
        private ArrayAdapter adapter;
        public static string ex;  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutPedidosD1);
            // Create your application here
            listViewDetallePed = FindViewById<ListView>(Resource.Id.listViewDetallePed);
            listViewDetallePed.OnItemClickListener = this;
            imgbtnsiguiente = FindViewById<ImageButton>(Resource.Id.imgbtnsiguiente);
            imgbtnsiguiente.Click += delegate
            {
                StartActivity((typeof(ActivityPedidosD2)));
                Finish();
            };
            imgbtnRegresarPedD1 = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarPedD1);   
            imgbtnRegresarPedD1.Click += delegate
            {
                StartActivity((typeof(ActivityPedidosE)));
                Finish();
            };
            AgregarDatosLista();
            ObtenerConteos();
            //txtVTotalPedD.Text = Class1.TotalOrdenesD.ToString();
        }

        private void AgregarDatosLista()
        {
            mItems = new List<string>();
            listaPedidosD = new List<ClassListaPedidoD1>();
            catalogo = new List<ClassListaPedidoD1>();
            //Class1.Pedido = Class1.Pedido.Trim();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            var sqllocal = "";
            if (Class1.vgEmployee_Perfil == 8)
            {
                sqllocal = "select od.id_order_detail, o.ID_Order, od.product_upc , od.product_name, od.product_quantity, od.quantity_temporal, w.name " +
                " from Logistik_orders o, Logistik_customer c, " +
                " Logistik_employee_shop es,  Logistik_warehouse_shop ws, " +
                " Logistik_employee e, Logistik_order_detail od, Logistik_warehouse w" +
                " where ws.id_shop = es.id_shop" +
                " and ws.id_shop = o.id_shop " +
                " and c.taxid = o.taxid " +
                " and e.id_employee = es.id_employee " +
                " and od.id_order = o.id_order " +
                " and od.id_warehouse = ws.id_warehouse " +
                " and o.id_lang = '2' " +
                " and w.id_warehouse = od.id_warehouse " +
                " and es.id_employee = '1' " +
                " and o.current_state in(10,11)" +
                " and od.quantity_temporal > 0" +
                " and od.quantity_received = 0 " +
                " and o.ID_Order = '" + Class1.Pedido + "' and od.num_empresa= o.num_empresa and od.num_empresa='" + Class1.vgEmpresaSelect + "'";
            }
            else
            {
                sqllocal = "select od.id_order_detail, o.ID_Order, od.product_upc , od.product_name, od.product_quantity, od.quantity_temporal, w.name " +
                " from Logistik_orders o, Logistik_customer c, " +
                " Logistik_employee_shop es,  Logistik_warehouse_shop ws, " +
                " Logistik_employee e, Logistik_order_detail od, Logistik_warehouse w" +
                " where ws.id_shop = o.id_shop" +
                " and e.id_employee = es.id_employee " +
                " and ws.id_shop = es.id_shop " +
                " and c.taxid = o.taxid " +
                " and od.id_order = o.id_order " +
                " and od.id_warehouse = ws.id_warehouse " +
                " and o.id_lang = '2' " +
                " and w.id_warehouse = od.id_warehouse " +
                " and es.id_employee = '1' " +
                " and o.current_state in(1,4,8,911)" +
                " and od.product_quantity <> od.quantity_temporal" +
                " and o.ID_Order = '" + Class1.Pedido + "' and od.num_empresa= o.num_empresa and od.num_empresa='" + Class1.vgEmpresaSelect + "'";
            }
            //var sqllocal = "select so.id_supply_order_state, so.date_upd, so.id_supply_order, so.id_supplier, s.name, ws.id_warehouse from Logistik_supply_order so, Logistik_supplier s, Logistik_warehouse_shop ws where ws.id_warehouse = so.id_warehouse and s.id_supplier = so.id_supplier and so.id_lang = '2' and so.id_supply_order_state in(" + Class1.vgEnt_Sal_Datos + ",4,8,911)";
            //var sqllocal = "select sod.id_supply_order_detail, so.id_supply_order, sod.upc, pl.description, sod.quantity_expected, sod.quantity_received from Logistik_supply_order so, Logistik_supply_order_detail sod, Logistik_product_lang pl where trim(so.id_supply_order) = trim(sod.id_supply_order) and pl.id_product = sod.id_product and pl.id_lang = '2' and sod.quantity_expected <> sod.quantity_received and trim(so.id_supply_order) = '" + Class1.Pedido + "' order by so.id_supply_order";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();

                while (reader.Read())
                {
                    ClassListaPedidoD1 pedidosDetalle = new ClassListaPedidoD1()
                    {
                        Cant_Rec = (decimal)reader["quantity_temporal"],
                        Cant_Ped = (decimal)reader["product_quantity"],
                        Codigo = (string)reader["product_upc"],
                        Descrip = (string)reader["product_name"],
                        ID_Orden = (string)reader["ID_Order"],
                        Ubica = (string)reader["name"],
                        ID = (int)reader["id_order_detail"],
                        Empresa = (string)Class1.vgEmpresaSelect
                    };
                    listaPedidosD.Add(pedidosDetalle);
                }
            }
            catalogo = listaPedidosD;
            //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            //lstVVentas.Adapter = adapter7;
            //ClassListaCompras.date_delivery_expected = ClassListaCompras.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
            //Class1.sCodigo sUbica sQty sID 
            //ClassListaCompras.id_supply_order = id_supply_order.trim;

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Cant_Rec + " = " + x.Cant_Ped + " = " + x.Codigo + " = " + x.Descrip + " = " + x.ID_Orden + " = " + x.Ubica + " = " + x.ID).ToArray()));
            listViewDetallePed.Adapter = adapter;
            //listViewDetalle.ItemSelected += listViewDetalle_ItemSelected;
            //listViewDetalle.ItemClick += listViewDetalle_ItemClick;
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
                Class1.TotalOrdenesD = conteo;
            }
        }
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewDetallePed.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (Class1.vgEmployee_Perfil == 8)
            {
                char[] delimiterChars = { '=' };
                string select = adapter.GetItem(position).ToString();
                string[] words = select.Split(delimiterChars);
                //cant_rec, cant_ped, codigo, descripcion, 
                string valores = words[0] + "$" + words[1] + "$" + words[2] + "$" + words[3] + "$" + words[4];

                Class1.vgctrlNumSer = "OrdenesSalidasDet";
                Class1.vgCant_Surtida = Convert.ToDecimal(words[0]);
                Class1.CantOriEmbarque = Convert.ToDecimal(words[1]);
                Class1.vgOrdCodBar = words[2];
                Class1.UbicaSelect = words[5];
                Class1.vgOrdCodID = words[6];


                StartActivity((typeof(ActivityPedidoSalida)));
                Finish();
            }
            else
            {
                StartActivity((typeof(ActivityPedidosD2)));
                Finish();
            }

        }
    }
}