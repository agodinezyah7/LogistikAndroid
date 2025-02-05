using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using System.Data;
using System.Data.SqlClient;
using BilddenLogistik.EFWorkBD;
using BilddenLogistik.MainActivities;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "Consulta Verificador")]
    public class ActivityConsultaVerif : Activity, ListView.IOnItemClickListener
    {
        private ListView listViewConsultaVerif;
        public List<ClassConsualtaVer> catalogo;
        public List<ClassConsualtaVer> listaEmpaques;
        public List<string> mItems;

        private ArrayAdapter adapter;
        private EditText editTextTarimaCV, editTextCodCV, editTextUbicaCV;
        private ImageButton imgbtnRegresarCV, imgbtnsiguienteCV;
        string OldTarima;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_consult_verif);
            listViewConsultaVerif = FindViewById<ListView>(Resource.Id.listViewConsultaVerif);
            editTextTarimaCV = FindViewById<EditText>(Resource.Id.editTextTarimaCV);
            editTextCodCV = FindViewById<EditText>(Resource.Id.editTextCodCV);
            editTextUbicaCV = FindViewById<EditText>(Resource.Id.editTextUbicaCV);
            imgbtnRegresarCV = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarCV);
            imgbtnsiguienteCV = FindViewById<ImageButton>(Resource.Id.imgbtnsiguienteCV);
            CargarForma();
            editTextCodCV.RequestFocus();
            listViewConsultaVerif.OnItemClickListener = this;
            editTextTarimaCV.KeyPress += editTextTarimaCV_KeyPress;
            editTextCodCV.KeyPress += editTextCodCV_KeyPress;
            editTextUbicaCV.KeyPress += editTextUbicaCV_KeyPress;
            editTextTarimaCV.EditorAction += HandleEditorActionTarima;
            editTextCodCV.EditorAction += HandleEditorActionCodBar;
            editTextUbicaCV.EditorAction += HandleEditorActionUbica;
            imgbtnRegresarCV.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            imgbtnsiguienteCV.Click += delegate
            {
                string sql1;

                if (OldTarima != "")
                {
                    AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                    Win_Save.SetMessage("Desea imprimir la etiqueta de la tarima que seleccionó: " + OldTarima + " ?");
                    Win_Save.SetTitle("CONSULTA");
                    Win_Save.SetPositiveButton("No", (send, arg) =>
                    {
                        Win_Save.Dispose();
                    });
                    Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                    {
                        sql1 = "update vLogistik_Ubicaciones set FlagPrint = '1' where tarima = '" + OldTarima + "'";
                        EjecutarQuerySQLWifi(sql1);

                        StartActivity((typeof(Activitymenu)));
                        Finish();
                        Win_Save.Dispose();
                    });
                    Win_Save.Show();
                }
            };
        }
        private void CargarForma()
        {
            if (Class1.FlagVerificador)
            {
                //CreaTabla()
                Class1.FlagVerificador = false;
            }
            //picturebox1 = imprimir,  picturebox5 = regresar
            imgbtnsiguienteCV.Visibility = ViewStates.Invisible;
            //Label4.Visible = false;
            LlenaTabla("", "", "");
        }
        private void LlenaTabla(string Codigo, string Tarima, string Ubica)
        {
            mItems = new List<string>();
            listaEmpaques = new List<ClassConsualtaVer>();
            catalogo = new List<ClassConsualtaVer>();
            //using (var conn = new SQLite.SQLiteConnection(BDConexionLocalSQLite.dbPath))
            var sql = "";
            if (Codigo != "")
                sql = "select lu.codigo_producto, lu.tarima, lw.name, lp.descripcion, lu.cantidad, lu.embarque, lu.oc, lu.factura " +
                " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
                " Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion " +
                " WHERE (lu.codigo_producto = '" + Codigo + "') ORDER BY lw.id_Warehouse, lu.tarima";
            else if (Tarima != "")
                sql = "select lu.codigo_producto, lu.tarima, lw.name, lp.descripcion, lu.cantidad, lu.embarque, lu.oc, lu.factura  " +
                " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
                " Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion " +
                " WHERE (lu.tarima = '" + Tarima + "')";
            else if (Ubica != "")
                sql = "select lu.codigo_producto, lu.tarima, lw.name, lp.descripcion, lu.cantidad, lu.embarque, lu.oc, lu.factura  " +
                " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
                " Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion " +
                " WHERE (lw.name = '" + Ubica + "')";
            if (Codigo == "" && Tarima == "" && Ubica == "")
            {
                //dtOrdenCompra.Clear()
                //DGVClientes.DataSource = dtOrdenCompra
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(sql, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        ClassConsualtaVer pedidosE = new ClassConsualtaVer()
                        {
                            Codigo = (string)reader["codigo_producto"],
                            Descrip = (string)reader["descripcion"],
                            Cantidad = (decimal)reader["cantidad"],
                            Tarima = (string)reader["tarima"],
                            Ubicacion = (string)reader["name"],
                            Embarque = (string)reader["embarque"],
                            OC = (string)reader["oc"],
                            Factura = (string)reader["factura"]
                        };
                        listaEmpaques.Add(pedidosE);
                    }
                }
                catalogo = listaEmpaques;
                //ArrayAdapter<string> adapter7 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
                //lstVVentas.Adapter = adapter7;
                //ClassListaPedidos.date_delivery_expected = ClassListaPedidos.date_delivery_expected.ToString("yyyy-MM-dd HH:mm:ss");
                //Class1.sCodigo sUbica sQty sID 
                //ClassListaPedidos.id_supply_order = id_supply_order.trim;
                //adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Estatus + " = " + x.ID_Orden + " = " + x.Fecha_Ent.ToString("yyyy-MM-dd HH:mm:ss") + " = " + x.Cliente + " = " + x.Empresa).ToArray()));
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Ubicacion + " = " + x.Tarima + " = " + x.Cantidad.ToString() + " = " + x.Codigo + " = " + x.Descrip).ToArray()));
                listViewConsultaVerif.Adapter = adapter;
                //listView1.ItemSelected += listView1_ItemSelected;
                //listView1.ItemClick += listView1_ItemClick;
                this.editTextTarimaCV.RequestFocus();
                editTextCodCV.RequestFocus();
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
        private void editTextTarimaCV_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarTarima();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void HandleEditorActionTarima(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarTarima();
                e.Handled = true;
            }
        }
        private void VerificarTarima()
        {
            //Dim lastchar As Char
            string cadena;
            int numTarima = this.editTextTarimaCV.Length();
            if (numTarima > 0)
            {
                //lastchar = txtTarima.Text(txtTarima.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = editTextTarimaCV.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2)
                //txtTarima.Text = cadena

                //'Va a llenar la tabla y muestra el resultado

                LlenaTabla("", cadena, "");

                OldTarima = cadena;
                imgbtnsiguienteCV.Visibility = ViewStates.Visible;
                //PictureBox1.Visible = True
                //Label4.Visible = True

                editTextTarimaCV.Text = "";



                //End If
            }



        }
        private void editTextCodCV_KeyPress(object sender, View.KeyEventArgs e)
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
            //Dim lastchar As Char
            string cadena;
            int numCodigo = editTextCodCV.Length();
            if (numCodigo > 0)
            {
                cadena = editTextCodCV.Text;
                Class1.vbUPCProducto = "";
                ExisteArt(cadena);
                //If(txtCodigo.TextLength > 0) Then
                //lastchar = txtCodigo.Text(txtCodigo.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = txtCodigo.Text
                //cadena = cadena.Substring(0, cadena.Length - 2)
                //txtCodigo.Text = cadena

                //' Verifica si fue un codigo UPC para extraer el codigo del producto que es con lo que trabajamos. 
                //vbUPCProducto = ""
                //inst.ExisteArt(txtCodigo.Text)
                if (Class1.vbUPCProducto != "")
                {
                    editTextCodCV.Text = Class1.vbUPCProducto;

                    //'Va a llenar la tabla y muestra el resultado
                    LlenaTabla(editTextCodCV.Text, "", "");

                    imgbtnsiguienteCV.Visibility = ViewStates.Invisible;
                    //Label4.Visible = False

                    editTextCodCV.Text = "";
                }
                else
                {
                    Toast.MakeText(this, "El Código No Existe en la empresa 06!", ToastLength.Short).Show();
                    editTextCodCV.Text = "";
                    editTextCodCV.RequestFocus();
                }
                //End If
                //End If
            }


        }
        private void editTextUbicaCV_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarUbica();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void HandleEditorActionUbica(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarUbica();
                e.Handled = true;
            }
        }
        private void VerificarUbica()
        {
            //Dim lastchar As Char
            string cadena;
            int numUbica = editTextUbicaCV.Length();
            if (numUbica > 0)
            {
                //If(txtUbica.TextLength > 0) Then
                //lastchar = txtUbica.Text(txtUbica.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = editTextUbicaCV.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2)
                //txtUbica.Text = cadena
                imgbtnsiguienteCV.Visibility = ViewStates.Invisible;
                //PictureBox1.Visible = False;
                //Label4.Visible = False;

                //'Va a llenar la tabla y muestra el resultado
                LlenaTabla("", "", cadena);

                editTextUbicaCV.Text = "";

                //End If
                //End If
            }

        }
        private string ExisteArt(string cb)
        {
            string Sql1 = "Select p.id_product, p.price, pl.description, P.UPC from Logistik_product p, Logistik_product_lang pl" +
            " where p.id_product = pl.id_product" +
            " and ltrim(p.upc) = '" + cb.Trim() + "'" +
            " and pl.id_lang = 2";
            string desc = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd2.ExecuteReader();
                    Boolean Flag1 = false;
                    while (reader.Read())
                    {
                        Class1.vgIDProducto = (int)reader["id_product"];
                        desc = (string)reader["description"];
                        Class1.vbNomProd = desc;
                        Class1.vbUPCProducto = (string)reader["UPC"];
                        Flag1 = true;
                        return desc;
                    }
                    reader.Close();
                    if (Flag1 == false)
                    {
                        Sql1 = "Select p.id_product, p.price, pl.description, P.UPC from Logistik_product p, Logistik_product_lang pl, Logistik_product_Claves_Alter pca" +
                        " where p.id_product = pl.id_product" +
                        " and pca.CVE_ART = p.upc" +
                        " and pca.CVE_ALTER = '" + cb.Trim() + "'" +
                        " and pl.id_lang = 2";
                        SqlCommand sqlcmd7 = new SqlCommand(Sql1, con);
                        SqlDataReader reader7;
                        reader7 = sqlcmd7.ExecuteReader();
                        while (reader7.Read())
                        {
                            Class1.vgIDProducto = (int)reader7["id_product"];
                            desc = (string)reader7["description"];
                            Class1.vbNomProd = desc;
                            Class1.vbUPCProducto = (string)reader7["UPC"];
                            return desc;
                        }
                        reader7.Close();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error" + ex.Message.ToString(), ToastLength.Long).Show();
                return "0";
            }
        }
        //private void ExisteArt(string cb)
        //{
        //    try
        //    {
        //        string Sql1;
        //        decimal precio;
        //        using (SqlConnection con = new SqlConnection(Class1.cnSQL))
        //        {
        //            Sql1 = "Select p.id_product, p.price, p.descripcion from Logistik_product p" +
        //                    " where ltrim(p.upc) = '" + cb.Trim() + "'";
        //            con.Open();
        //            SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
        //            SqlDataReader reader;
        //            reader = sqlcmd1.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                //vlid_producto = int.Parse(id_product);
        //                editTextCodCV.Text = (string)reader["descripcion"];
        //                precio = (decimal)reader["Price"];
        //                editTextUbicaCV.Text =Convert.ToString(precio);
        //            }
        //            reader.Close();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        string msg = ex.ToString();
        //        Toast.MakeText(this, msg, Android.Widget.ToastLength.Short).Show();
        //    }


        //}
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewConsultaVerif.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //////////////////////////funcion
            //Dim x As Integer
            //Dim cell As String

            //If DGVClientes.VisibleRowCount > 0 Then
            //    x = DGVClientes.CurrentRowIndex
            //    cell = DGVClientes.Item(x, 1)
            //    OldTarima = cell
            //    txtTarima.Text = OldTarima
            //    LlenaTabla("", OldTarima, "")
            //    txtTarima.Text = ""
            //End If
            /////////////////////////////////////////////////////////////////////
            int numFilas = listViewConsultaVerif.CheckedItemCount;
            if (numFilas > 0)
            {
                position = listViewConsultaVerif.CheckedItemPosition;
                string select = adapter.GetItem(position).ToString();
                //Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
                string lastWord = select.Substring(25, 20);
                Toast.MakeText(this, lastWord, Android.Widget.ToastLength.Short).Show();
                Class1.OC = lastWord;
                int tam_var = select.Length;
                String Var_Sub = select.Substring((tam_var - 2), 2);
                string lastWord1 = Var_Sub;
                //Toast.MakeText(this, lastWord1, Android.Widget.ToastLength.Short).Show();
                Class1.vbID_Supplier = lastWord1;

                OldTarima = lastWord;
                this.editTextTarimaCV.Text = OldTarima;
                LlenaTabla("", OldTarima, "");
                editTextTarimaCV.Text = "";

            }


        }
    }
}