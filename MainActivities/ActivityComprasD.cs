using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.InputMethodServices;
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
    [Activity(Label = "ActivityComprasD")]
    public class ActivityComprasD : Activity, ListView.IOnItemClickListener
    {
        private TextView textViewOC;
        private ListView listViewDetalle;
        private EditText editTextCodigo;
        private ImageButton imgbtnteclado;
        private ImageButton imgbtnsave;
        private ImageButton imgbtnRegresar;
        private ImageButton imgbtnsiguiente;
        //private RecyclerView recyclerView1;
        public List<ClassListaComprasD>catalogo;
        public List<ClassListaComprasD>listaComprasD;
        public List<string> mItems;
        private ArrayAdapter adapter;
        public static string ex;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutCompraD);
            // Create your application here
            textViewOC = FindViewById<TextView>(Resource.Id.textViewOC);
            listViewDetalle = FindViewById<ListView>(Resource.Id.listViewDetalle);
            imgbtnteclado = FindViewById<ImageButton>(Resource.Id.imgbtnteclado);
            imgbtnsave = FindViewById<ImageButton>(Resource.Id.imgbtnsave);
            imgbtnRegresar = FindViewById<ImageButton>(Resource.Id.imgbtnRegresar);
            imgbtnsiguiente = FindViewById<ImageButton>(Resource.Id.imgbtnsiguiente);
            editTextCodigo = FindViewById<EditText>(Resource.Id.editTextCodigo);
            textViewOC.Text = "Orden de Compra:"+ Class1.OC;
            
            AgregarDatosLista();
            ObtenerConteos();
            listViewDetalle.OnItemClickListener = this;
            //txtVTotalCompD.Text = Class1.TotalOrdenesD.ToString();
            imgbtnRegresar.Click += delegate
            {
                StartActivity((typeof(ActivityAlmacenRecepcion)));
                Finish();
            };
            imgbtnsave.Click += delegate
            {
                AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                if (Class1.vgEmployee_Perfil ==8){
                    Win_Save.SetMessage("Esta seguro cerrar la Entrega?, faltan artículos por surtir");
                    Win_Save.SetTitle("Cerrar Entrega");
                    Win_Save.SetPositiveButton("No",(send, arg) =>
                        {
                        Win_Save.Dispose();
                        });
                    Win_Save.SetNegativeButton("Si",(send2, arg2) =>
                        {
                        //funcion guardar
                        Win_Save.Dispose();
                        });
                    Win_Save.Show();
                }
                else{
                    Win_Save.SetMessage("Esta seguro cerrar el Pedido de Venta?, faltan artículos por surtir");
                    Win_Save.SetTitle("Cerrar Pedido de Venta");
                    Win_Save.SetPositiveButton("No", (send2, arg2) =>
                        {
                        Win_Save.Dispose();
                    });
                    Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                        {
                        //funcion guardar
                        Win_Save.Dispose();
                    });
                    Win_Save.Show();
                }
                
                
            };
            imgbtnsiguiente.Click += delegate
            {
                StartActivity((typeof(ActivityCompraCaptura)));
                Finish();
            };
            imgbtnteclado.Click += delegate
            {
                if (Class1.vgTeclado == 1)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(editTextCodigo.WindowToken,0);
                    Class1.vgTeclado = 0;
                }
                else
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(editTextCodigo, 0);
                    Class1.vgTeclado = 1;
                }
                
            };
        }//fin oncreate
        private void AgregarDatosLista()
        {
            mItems = new List<string>();
            listaComprasD = new List<ClassListaComprasD>();
            catalogo = new List<ClassListaComprasD>();
            Class1.OC = Class1.OC.Trim();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            //var sqllocal = "select so.id_supply_order_state, so.date_upd, so.id_supply_order, so.id_supplier, s.name, ws.id_warehouse from Logistik_supply_order so, Logistik_supplier s, Logistik_warehouse_shop ws where ws.id_warehouse = so.id_warehouse and s.id_supplier = so.id_supplier and so.id_lang = '2' and so.id_supply_order_state in(" + Class1.vgEnt_Sal_Datos + ",4,8,911)";
            var sqllocal = "select sod.id_supply_order_detail, so.id_supply_order, sod.upc, pl.description, sod.quantity_expected, sod.quantity_received from Logistik_supply_order so, Logistik_supply_order_detail sod, Logistik_product_lang pl where trim(so.id_supply_order) = trim(sod.id_supply_order) and pl.id_product = sod.id_product and pl.id_lang = '2' and sod.quantity_expected <> sod.quantity_received and trim(so.id_supply_order) = '" + Class1.OC + "' order by so.id_supply_order";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                while (reader.Read())
                {
                    //decimal ORDER1 = (decimal)reader["quantity_received"];
                    //decimal ORDER2 = (decimal)reader["quantity_expected"];
                    //listaComprasD.Add(ORDER);
                    ////var vestatus = item.id_supply_order;
                    //mItems.Add(reader.GetString(2));
                    //id_supply_order = (string)reader["id_supply_order".Replace(" ", "")],
                    ClassListaComprasD comprasDetalle = new ClassListaComprasD()
                    {
                        id_supply_order_detail = (int)reader["id_supply_order_detail"],
                        id_supply_order = (string)reader["id_supply_order"],
                        upc = (string)reader["upc"],
                        description = (string)reader["description"],
                        quantity_received = (decimal)reader["quantity_received"],
                        quantity_expected = (decimal)reader["quantity_expected"]
                };
                    listaComprasD.Add(comprasDetalle);
                }
            }
            catalogo = listaComprasD;
            //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            //lstVVentas.Adapter = adapter7;
            //ClassListaCompras.date_delivery_expected = ClassListaCompras.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
            //Class1.sCodigo sUbica sQty sID 
            //ClassListaCompras.id_supply_order = id_supply_order.trim;

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.quantity_received + " = " + x.quantity_expected + " = " + x.upc + " = " + x.description).ToArray()));
            listViewDetalle.Adapter = adapter;
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
            string select = listViewDetalle.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            string select = adapter.GetItem(position).ToString();
            //Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
            string lastWord = select.Substring(25, 20);
            char delimitador = '=';
            string[] valores = select.Split(delimitador);
            Class1.vgOrdCodBar = valores[2];
            Toast.MakeText(this, Class1.vgOrdCodBar, Android.Widget.ToastLength.Short).Show();
            int tam_var = select.Length;
            Class1.vgCantPedida = Convert.ToDecimal(valores[1]);
            //vgOrdCodID = cell
            StartActivity((typeof(ActivityCompraCaptura)));
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
    }
}