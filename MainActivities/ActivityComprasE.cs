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
    [Activity(Label = "Compras Encabezado")]
    public class ActivityComprasE : Activity, ListView.IOnItemClickListener
    {
        //private GridView gridviewCE;
        private ImageButton imgbtnRegresar;
        private ListView listView1;
        private EditText txtVTotalItems7;
        //private RecyclerView recyclerView1;
        public List<ClassListaCompras> catalogo;
        public List<ClassListaCompras> listaVentas;
        public List<string> mItems;
        private ArrayAdapter adapter;
        public static string ex;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutComprasE);
            //CargarCompra();
            //RequestWindowFeature(Android.Views.WindowFeatures.ActionBar);
            //ActionBar.Title = "pollo";
            listView1 = FindViewById<ListView>(Resource.Id.listView1);
            txtVTotalItems7 = FindViewById<EditText>(Resource.Id.txtVTotalItems7);
            imgbtnRegresar = FindViewById<ImageButton>(Resource.Id.imgbtnRegresar);
            Class1.vgEnt_Sal_Datos = 1;
            Class1.vgEnt_Sal = "E";
            AgregarDatosLista();
            ObtenerConteos();
            listView1.OnItemClickListener = this;
            //listView1.ItemClick += (sender, e) =>
            //{
            //    string select = listView1.GetItemAtPosition(e.Position).ToString();
            //    Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
            //};
                txtVTotalItems7.Text = Convert.ToString(Class1.TotalOrdenes);
            imgbtnRegresar.Click += delegate
            {
                    StartActivity((typeof(Activitymenu)));
                    Finish();
            };
           
            //recyclerView1 = FindViewById<RecyclerView>(Resource.Id.recyclerView1);

            //listView1.ItemsSource = CrearBD.ObtenerEmpleados();
            // Create your application here
            //++++++++++++++++++++++++++imagenes
            //var gridviewCE = FindViewById<GridView>(Resource.Id.gridviewCE);
            //gridviewCE.Adapter = new LogistikBildden.EFWorkBD.ImagenAdapter(this);

            //gridviewCE.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {
            //    Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
            //};
            //++++++++++++++++++++++fin imagenes

        }
        //private void CargarCompra()
        //{
        //    DataTable dt = new DataTable();
        //    SqlConnection con = new SqlConnection();
        //    con = new SqlConnection(Class1.strSQL);
        //    string textoCmd = "select so.id_supply_order_state, so.date_delivery_expected, so.id_supply_order, so.id_supplier, s.name, ws.id_warehouse" & 
        //    " from Logistik_supply_order so, Logistik_supplier s," &
        //    " Logistik_warehouse_shop ws" &
        //    " where ws.id_warehouse = so.id_warehouse" &
        //    " and s.id_supplier = so.id_supplier" &
        //    " and so.id_lang = '2'" &
        //    " and so.id_supply_order_state in(" + Class1.vgEnt_Sal_Datos.ToString + ",4,8,911)";
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand(textoCmd, con);
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //        //LIMPIAR LIST VIEW PARA LA NUEVA CONSULTA
        //        elListview.Clear();
        //        //DEFINIR FORMATO DE LINEAS AL LIST VIEW
        //        elListview.View = View.Details;
        //        elListview.GridLines = true;
        //        elListview.FullRowSelect = true;
        //        //ASIGNAR COLUMNAS, RESPECTIVOS NOMBRES Y TAMAÑOS DE RENGLÓN
        //        elListview.Columns.Add(dt.Columns[0].ToString(), 70);
        //        elListview.Columns.Add(dt.Columns[1].ToString(), 70);
        //        elListview.Columns.Add(dt.Columns[2].ToString(), 280);
        //        foreach (DataRow renglon in dt.Rows)
        //        {
        //            string[] arr = new string[3];
        //            ListViewItem itm = new ListViewItem();
        //            //ADICIONAR ITEM AL LISTVIEW                   
        //            for (int ncolumna = 0; ncolumna < 3; ncolumna++)
        //            {
        //                arr[ncolumna] = renglon[ncolumna].ToString();
        //                itm = new ListViewItem(arr);
        //            }
        //            elListview.Items.Add(itm);
        //        }

        //        Toast.MakeText(this, "Carga correcta", ToastLength.Short).Show(); ;
        //    }
        //    catch (Exception e)
        //    {
        //        Toast.MakeText(this, e.Message, ToastLength.Short).Show();
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        private void AgregarDatosLista()
        {
            mItems = new List<string>();
            listaVentas = new List<ClassListaCompras>();
            catalogo = new List<ClassListaCompras>();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            var sqllocal = "select so.id_supply_order_state, so.date_upd, so.id_supply_order, so.id_supplier, s.name, ws.id_warehouse " +
                " from Logistik_supply_order so, Logistik_supplier s, Logistik_warehouse_shop ws " +
                " where ws.id_warehouse = so.id_warehouse and s.id_supplier = so.id_supplier and so.id_lang = '2'" +
                " and so.id_supply_order_state in(" + Class1.vgEnt_Sal_Datos + ",4,8,911)" +
                " Order by so.id_supply_order desc";

            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                while (reader.Read())
                {
                    //string ORDER = (string)reader["id_supply_order"];
                    //listaVentas.Add(ORDER);
                    ////var vestatus = item.id_supply_order;
                    //mItems.Add(reader.GetString(2));
                    //id_supply_order = (string)reader["id_supply_order".Replace(" ", "")],
                    ClassListaCompras comprasE = new ClassListaCompras()
                    {
                        id_supply_order_state = (int)reader["id_supply_order_state"],
                        date_delivery_expected = (DateTime)reader["date_upd"],
                        id_supply_order = (string)reader["id_supply_order"],
                        id_supplier = (string)reader["id_supplier"],
                        name_supplier = (string)reader["name"],
                        id_warehouse = (int)reader["id_warehouse"]
                    };
                    listaVentas.Add(comprasE);
                }
            }
            catalogo = listaVentas;
            //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            //lstVVentas.Adapter = adapter7;
            //ClassListaCompras.date_delivery_expected = ClassListaCompras.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
            //Class1.sCodigo sUbica sQty sID 
            //ClassListaCompras.id_supply_order = id_supply_order.trim;

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.id_supply_order_state + " = " + x.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss") + " = " + x.id_supply_order + " = " + x.id_supplier + " = " + x.name_supplier + " = " + x.id_warehouse).ToArray()));
            listView1.Adapter = adapter;
            //listView1.ItemSelected += listView1_ItemSelected;
            //listView1.ItemClick += listView1_ItemClick;
        }

        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listView1.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            char[] delimiterChars = { '=' };
            string select = adapter.GetItem(position).ToString();
            string[] words = select.Split(delimiterChars);
            string valores = words[0] + "$" + words[1] + "$" + words[2];

            //Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
            string lastWord = select.Substring(1, 20);
            Toast.MakeText(this, lastWord, Android.Widget.ToastLength.Short).Show();
            //Class1.OC = lastWord;
            Class1.OC = words[2];
            int tam_var = select.Length;
            String Var_Sub = select.Substring((tam_var - 2), 2);
            string lastWord1 = Var_Sub;
            //Toast.MakeText(this, lastWord1, Android.Widget.ToastLength.Short).Show();
            Class1.vbID_Supplier = lastWord1;
            //vgOrder = cell
            //vgNomProv = cell
            //vgAlmOrdCompra = cell
            //StartActivity((typeof(ActivityComprasD)));
            StartActivity((typeof(ActivityAlmacenRecepcion)));
            Finish();
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
                 int conteo=0;
                while (reader.Read())
                {
                    conteo++;
                    
                }
                Class1.TotalOrdenes = conteo;
            }
        }
    }
}