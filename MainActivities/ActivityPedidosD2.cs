using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
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
    [Activity(Label = "Pedidos Detalle 2")]
    public class ActivityPedidosD2 : Activity, ListView.IOnItemClickListener
    {
        private ListView listViewDetallePed2;
        private EditText editTextCodigo;
        private ImageButton imgbtnRegresarPedD2, imgbtnsiguienteD2;
        //private RecyclerView recyclerView1;
        public List<ClassListaPedidoD2> catalogo;
        public List<ClassListaPedidoD2> listaPedidosD2;
        public List<string> mItems;
        private ArrayAdapter adapter;
        public static string ex;
        private int vProd_attrib = 0;
        private Boolean vcambio;
        private decimal vlcant;
        private double CantOri;
        private int vlid_producto;
        private string vfUMedida;
        private string vTipoProduct;
        private string vlCodBar;
        private string vAlmacen;
        private string vlCtrlCant;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutPedidosD2);
            imgbtnRegresarPedD2 = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarPedD2);
            imgbtnsiguienteD2 = FindViewById<ImageButton>(Resource.Id.imgbtnsiguienteD2);
            editTextCodigo = FindViewById<EditText>(Resource.Id.editTextCodigo);
            editTextCodigo.KeyPress += editTextCodigo_KeyPress;
            editTextCodigo.EditorAction += HandleEditorActionCodBar;
            listViewDetallePed2 = FindViewById<ListView>(Resource.Id.listViewDetallePed2);
            listViewDetallePed2.OnItemClickListener = this;
            CargarForma();
            imgbtnRegresarPedD2.Click += delegate
            {
                StartActivity((typeof(ActivityPedidosE)));
                Finish();
            };
            imgbtnsiguienteD2.Click += delegate
            {
                StartActivity((typeof(ActivityPedidosE)));
                Finish();
            };

        }
        private void editTextCodigo_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarCodigo();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void HandleEditorActionCodBar(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarCodigo();
                e.Handled = true;
            }
        }

        private void VerificarCodigo()
        {
            //char lastchar;

            string cadena;
            string sql1;
            Boolean vlexiste;
            int numCodigo = editTextCodigo.Length();
            if (numCodigo > 0)
            {
                //lastchar = TxtCodigo.Text(TxtCodigo.TextLength - 1)
                // If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = editTextCodS.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextCodS.Text = cadena;

                Class1.vbUPCProducto = "";
                //' Verifica si fue un codigo UPC para extraer el codigo del producto que es con lo que trabajamos. 
                ExisteArt(editTextCodigo.Text);
                if (Class1.vbUPCProducto != "")
                {
                    editTextCodigo.Text = Class1.vbUPCProducto;
                    vlCodBar = editTextCodigo.Text;
                    vlexiste = fnValidarOrdenCliente(Class1.vgOrder, vlCodBar);
                    if (vlexiste == true)
                    {
                        //txtCodaleer.Text = "";
                        //if (Class1.vgEmployee_Perfil == 8)
                        //{

                        //}
                        //else
                        //{
                        //    DGVClientes.Enabled = true;
                        //}
                        //falta pasar la cantidad a una variable global
                        Class1.CantOriEmbarque = vlcant;
                        Class1.vgOrdCodBar = vlCodBar;
                        //txtUbica.Focus()
                        StartActivity((typeof(ActivityPedidoSalida)));
                        Finish();
                    }
                    else
                    {
                        Class1.vgNoExiste = "S";
                        Class1.vgOrdCodBar = "";
                        Toast.MakeText(this, "El Código No Existe en Esta Orden", ToastLength.Short).Show();
                        editTextCodigo.Text = "";
                        editTextCodigo.RequestFocus();
                    }
                }
                else
                {
                    Toast.MakeText(this, "El Código No Existe en la empresa 06!", ToastLength.Short).Show();
                    editTextCodigo.Text = "";
                    editTextCodigo.RequestFocus();
                }
                //End If
            }


        }

        private bool fnValidarOrdenCliente(string pOrder, string pCodigo)
        {
            try
            {
                string sql = "Select product_quantity, product_id from Logistik_order_detail " +
                " where id_order = '" + Class1.Pedido.ToString() + "' and ltrim(product_upc) = '" + pCodigo.Trim() + "'";

                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(sql, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        vlcant = (decimal)reader["product_quantity"];
                        vlid_producto = (int)reader["product_id"];
                        return true;
                    }
                    reader.Close();
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string ExisteArt(string cb)
        {
            try
            {
                string Sql1;
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    Sql1 = "Select p.id_product, p.price, pl.description, P.UPC from Logistik_product p," +
                        " Logistik_product_lang pl where p.id_product = pl.id_product" +
                        " and ltrim(p.upc) = '" + cb.Trim() + "'" +
                        " and pl.id_lang = 2";
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    bool flag = false;
                    while (reader.Read())
                    {
                        flag = true;
                        Class1.vgIDProducto = (int)reader["id_product"];
                        Class1.vbNomProd = (string)reader["description"];
                        //infop.desc = Class1.vbNomProd;
                        Class1.vbUPCProducto = (string)reader["UPC"];
                        return Class1.vbNomProd;

                    }
                    reader.Close();
                    if (flag == false)
                    {
                        //' Si no lo encontro como arriba, lo busca por codigo de barras (alterno)
                        //'MsgBox("El producto no existe en la tabla de productos de la Empresa 06, favor de darlo de alta!!", MsgBoxStyle.Critical)
                        Sql1 = "Select p.id_product, p.price, pl.description, P.UPC from Logistik_product p," +
                                " Logistik_product_lang pl, Logistik_product_Claves_Alter pca" +
                                " where p.id_product = pl.id_product" +
                                " and pca.CVE_ART = p.upc" +
                                " and pca.CVE_ALTER = '" + cb.Trim() + "'" +
                                " and pl.id_lang = 2";
                        SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                        SqlDataReader reader3;
                        reader3 = sqlcmd2.ExecuteReader();
                        while (reader3.Read())
                        {
                            Class1.vgIDProducto = (int)reader3["id_product"];
                            Class1.vbNomProd = (string)reader3["descripcion"];
                            //infop.desc = Class1.vbNomProd;
                            Class1.vbUPCProducto = (string)reader3["UPC"];
                            return Class1.vbNomProd;
                        }
                        reader3.Close();
                    }
                }
                return Class1.vbNomProd;
            }
            catch (SqlException ex)
            {
                string msg = ex.ToString();
                Toast.MakeText(this, msg, Android.Widget.ToastLength.Short).Show();
                return Class1.vbNomProd;
            }

        }
        private void CargarForma()
        {

            agregarDatos();
        }

        private void agregarDatos()
        {
            mItems = new List<string>();
            listaPedidosD2 = new List<ClassListaPedidoD2>();
            catalogo = new List<ClassListaPedidoD2>();
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
            " and c.id_customer = o.id_customer " + 
            " and e.id_employee = es.id_employee " + 
            " and od.id_order = o.id_order " + 
            " and od.id_warehouse = ws.id_warehouse " + 
            " and o.id_lang = '2' " + 
            " and w.id_warehouse = od.id_warehouse " + 
            " and es.id_employee = '1' " + 
            " and o.current_state in(10,11)" + 
            " and od.product_quantity <> od.quantity_received" + 
            " and o.ID_Order = '" + Class1.Pedido + "' and od.num_empresa= o.num_empresa and od.num_empresa='" + Class1.vgEmpresaSelect + "'";
            }
            else
            {
                sqllocal = "SELECT od.id_order_detail, od.id_order, od.product_upc, od.product_name, od.product_quantity, od.quantity_temporal, w.name, w.orden, w.minorista" + 
            " FROM Logistik_order_detail AS od INNER JOIN " + 
            " vLogistik_Ubicaciones AS u ON u.codigo_producto = od.product_upc INNER JOIN" + 
            " Logistik_warehouse as w ON u.ubicacion = w.id_warehouse " + 
            " WHERE (od.id_order = '" + Class1.Pedido + "'and od.num_empresa='" + Class1.vgEmpresaSelect + 
            "') AND (od.product_quantity <> od.quantity_temporal) AND (w.minorista = '1') and (u.cantidad > 0)  AND (w.orden <> 25)" + 
            " ORDER BY od.product_upc, w.orden";
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
                    ClassListaPedidoD2 pedidosDetalle = new ClassListaPedidoD2()
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
                    listaPedidosD2.Add(pedidosDetalle);
                }
            }
            catalogo = listaPedidosD2;
            //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            //lstVVentas.Adapter = adapter7;
            //ClassListaCompras.date_delivery_expected = ClassListaCompras.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
            //Class1.sCodigo sUbica sQty sID 
            //ClassListaCompras.id_supply_order = id_supply_order.trim;

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Ubica + " = " + x.Cant_Rec + " = " + x.Cant_Ped + " = " + x.Codigo + " = " + x.Descrip + " = " + x.ID_Orden + " = " + x.ID).ToArray()));
            listViewDetallePed2.Adapter = adapter;
            //listViewDetalle.ItemSelected += listViewDetalle_ItemSelected;
            //listViewDetalle.ItemClick += listViewDetalle_ItemClick;
        }

        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewDetallePed2.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Class1.flagControl = true;
            Class1.FlagOKSalidaDet = true;
            if (Class1.vgEmployee_Perfil != 8)
            {
                char[] delimiterChars = { '=' };
                string select = adapter.GetItem(position).ToString();
                string[] words = select.Split(delimiterChars);
                //cant_rec, cant_ped, codigo, descripcion, 
                //x.Ubica + " = " + x.Cant_Rec + " = " + x.Cant_Ped + " = " + x.Codigo + " = " + x.Descrip + " = " + x.ID_Orden
                string valores = words[0] + "$" + words[1] + "$" + words[2] + "$" + words[3] + "$" + words[4];

                Class1.vgctrlNumSer = "OrdenesSalidasDet";
                Class1.UbicaSelect = words[0];
                Class1.vgCant_Surtida = Convert.ToDecimal(words[1]);
                Class1.CantOriEmbarque = Convert.ToDecimal(words[2]);
                Class1.vgOrdCodBar = words[3];


                //Class1.vgOrdCodID = words[6];


                StartActivity((typeof(ActivityPedidoSalida)));
                Finish();
            }
            else
            {
                char[] delimiterChars = { '=' };
                string select = adapter.GetItem(position).ToString();
                string[] words = select.Split(delimiterChars);
                //cant_rec, cant_ped, codigo, descripcion, 
                //x.Ubica + " = " + x.Cant_Rec + " = " + x.Cant_Ped + " = " + x.Codigo + " = " + x.Descrip + " = " + x.ID_Orden
                string valores = words[0] + "$" + words[1] + "$" + words[2] + "$" + words[3] + "$" + words[4];
                Class1.vgCant_Surtida = Convert.ToDecimal(words[1]);
                Class1.vgOrdCodBar = words[3];
                StartActivity((typeof(ActivityPedidoSalida)));
                Finish();
            }

        }
    }
}