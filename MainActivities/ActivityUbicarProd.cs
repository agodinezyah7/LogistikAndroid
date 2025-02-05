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

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityUbicarProd")]
    public class ActivityUbicarProd : Activity, ListView.IOnItemClickListener
    {
        private ListView listViewUbicar;
        public List<ClassListaUbicar> catalogoUbicar;
        public List<ClassListaUbicar> listaUbicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private CrearBD cnn = new CrearBD();
        private TextView textView102S, textView103S, textView104S, textViewNuevaTarina;
        private ImageButton imgbtnRegresarUP, imgbtnInventUP, imgbtnTerminarUP;
        private EditText editTextTarimaUP, editTextCodUP, editTextCantUP;
        string vlCtrlCant;
        string vAlmacen;
        double vlcant;
        int vlid_producto;
        string vlUtilizaNumSer;
        string vlUtilizaLotes;
        int idProd;
        decimal CantOri;
        string UbicaOri;
        string IDUbica;
        string IDWharehouse;
        string IDWharehoseDest;
        string vDescripcion;
        bool FlagTarima = false;
        int CantProducto;
        bool FlagCapturo = false;
        string StatusTarima;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_ubicarproducto);
            listViewUbicar = FindViewById<ListView>(Resource.Id.listViewUbicar);
            listViewUbicar.OnItemClickListener = this;
            textView104S = FindViewById<TextView>(Resource.Id.textView104S);
            textView103S = FindViewById<TextView>(Resource.Id.textView103S);
            textView102S = FindViewById<TextView>(Resource.Id.textView102S);
            textViewNuevaTarina = FindViewById<TextView>(Resource.Id.textViewNuevaTarina);
            editTextTarimaUP = FindViewById<EditText>(Resource.Id.editTextTarimaUP);
            editTextTarimaUP.KeyPress += editTextTarimaUP_KeyPress;
            editTextTarimaUP.EditorAction += HandleEditorActionTarimaUP;
            editTextCodUP = FindViewById<EditText>(Resource.Id.editTextCodUP);
            editTextCodUP.KeyPress += editTextCodUP_KeyPress;
            editTextCodUP.EditorAction += HandleEditorActionCodUP;
            editTextCantUP = FindViewById<EditText>(Resource.Id.editTextCantUP);
            editTextCantUP.KeyPress += editTextCantUP_KeyPress;
            editTextCantUP.EditorAction += HandleEditorActionCantUP;
            imgbtnInventUP = FindViewById<ImageButton>(Resource.Id.imgbtnInventUP);
            imgbtnTerminarUP = FindViewById<ImageButton>(Resource.Id.imgbtnTerminarUP);
            imgbtnRegresarUP = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarUP);
            BeginLoad();
            imgbtnRegresarUP.Click += delegate
            {
                StartActivity((typeof(ActivityUbicar)));
                Finish();
            };
            imgbtnInventUP.Click += delegate
            {
                if(Class1.TipoUbica==1)
                {
                    AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                    Win_Save.SetMessage("Desea generar un nuevo numero de Tarima?");
                    Win_Save.SetTitle("NUMERO DE TARIMA");
                    Win_Save.SetPositiveButton("No", (send, arg) =>
                    {
                        Win_Save.Dispose();
                    });
                    Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                    {
                        GetTarima("1");
                        editTextTarimaUP.Text= Class1.vgNTarima;
                        //llamar al pressky de la tarima
                        VerificarTarima();
                        //txtTarima.Text = vgNTarima + Chr(10) + Chr(13);

                        imgbtnInventUP.Enabled = false;
                        FlagTarima = true;
                        editTextCantUP.RequestFocus();
                        //txtCant.Focus();
                        Win_Save.Dispose();
                    });
                    Win_Save.Show();
                }
            };
            imgbtnTerminarUP.Click += delegate
            {
                GuardarDatos();
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
        }
        private void BeginLoad()
        {
            LlenarProveedores();
            //LeerXML();
            CantProducto = 0;
            SiguienteDocumento();

            if (Class1.TipoUbica == 1)
            {
                textView102S.Text = "Tarima";
                //Label2.Text = "Tarima";
                textView103S.Text = "Código";
                //Label1.Text = "Código";

                //' Desabilita codigo y cantidad al entrar
                editTextCodUP.Enabled = false;
                //TxtCodigo.Enabled = false;
                editTextCantUP.Enabled = false;
                //txtCant.Enabled = false;
                listViewUbicar.Enabled = false;

                //' Muestra la cantidad
                //txtCant.Visible = true;
                editTextCantUP.Visibility = ViewStates.Visible;
                textView104S.Visibility = ViewStates.Visible;
                //Label3.Visible = true

                //' Muestra boton de Terminar
                imgbtnTerminarUP.Visibility = ViewStates.Visible;
                textViewNuevaTarina.Visibility = ViewStates.Visible;
                //PictureBox4.Visible = true;
                imgbtnTerminarUP.Enabled = false;
                //PictureBox4.Enabled = false;
                //Label4.Visible = true;

                //' Muetra boton de Nueva Tarima
                //PictureBox2.Visible = True
                imgbtnInventUP.Visibility = ViewStates.Visible;
                //Label6.Visible = True
            }

            if (Class1.TipoUbica == 2)
            {
                textView102S.Text = "Ubica.";
                textView103S.Text = "Código";
                //Label2.Text = "Ubica."
                //Label1.Text = "Código"
                textView104S.Visibility = ViewStates.Visible;
                //Label3.Visible = True

                editTextCantUP.Visibility = ViewStates.Visible;
                //txtCant.Visible = True
                editTextCantUP.Enabled = false;
                //txtCant.Enabled = False
                listViewUbicar.Enabled = true;

                imgbtnTerminarUP.Visibility = ViewStates.Visible;
                textViewNuevaTarina.Visibility = ViewStates.Invisible;
                //PictureBox4.Visible = true;
                imgbtnTerminarUP.Enabled = false;
                //PictureBox4.Enabled = false;
                //Label4.Visible = True

                editTextCodUP.Enabled = false;
                //TxtCodigo.Enabled = False

                imgbtnInventUP.Visibility = ViewStates.Invisible;
                //PictureBox2.Visible = False
                //Label6.Visible = False
            }

            if (Class1.TipoUbica == 3)
            {
                textView102S.Text = "Ubica.";
                textView103S.Text = "Tarima";
                //Label2.Text = "Ubica."
                //Label1.Text = "Tarima"

                textView104S.Visibility = ViewStates.Invisible;
                //Label3.Visible = False
                editTextCantUP.Visibility = ViewStates.Invisible;
                //txtCant.Visible = False
                listViewUbicar.Enabled = true;

                imgbtnTerminarUP.Visibility = ViewStates.Invisible;
                textViewNuevaTarina.Visibility = ViewStates.Invisible;
                //PictureBox4.Visible = false;
                //Label4.Visible = false;

                editTextCodUP.Enabled = true;
                //TxtCodigo.Enabled = true;

                imgbtnInventUP.Visibility = ViewStates.Invisible;
                //PictureBox2.Visible = false;
                //Label6.Visible = false;
            }
                
            if (Class1.FlagUbica)
            {
                FlagTarima = false;
                Class1.FlagUbica = false;
            }
            

            if (FlagTarima == false)
            {
                FlagCapturo = false;
                editTextTarimaUP.Enabled = true;
                //txtTarima.Enabled = true;
                editTextTarimaUP.Text = "";
                //txtTarima.Text = "";
            }
            editTextCodUP.Text = "";
            //TxtCodigo.Text = "";
            editTextCantUP.Text = "";
            //txtCant.Text = "";
            editTextTarimaUP.RequestFocus();
            //txtTarima.Focus();

            if (Class1.vFinOrden == 1)
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            }
        }
        private void LlenarProveedores()
        {
            mItems = new List<string>();
            listaUbicar = new List<ClassListaUbicar>();
            catalogoUbicar = new List<ClassListaUbicar>();

            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                if (Class1.TipoUbica == 3)
                    Class1.strSQL = "SELECT DISTINCT tarima, SUM(cantidad) AS QTY " +
                    " FROM vLogistik_Ubicaciones" +
                    " WHERE (estado = '22')" +
                    " GROUP BY tarima";
                if (Class1.TipoUbica == 1 || Class1.TipoUbica == 2)
                    Class1.strSQL = "select lu.codigo_producto, lpl.description, lu.cantidad, lu.defectuosos, lu.embarque, lu.oc, lu.factura " +
                    " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
                    " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product" +
                    " WHERE (lu.estado = '2' and lu.tarima='')";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                CrearBD.vgOrder = 1;
                while (reader.Read())
                {
                    if (Class1.TipoUbica == 3)
                    {
                        ClassListaUbicar UbicarDetalle = new ClassListaUbicar()
                        {
                            Codigo = (string)reader["tarima"],
                            Descrip = "", // (string)reader["description"],
                            Cantidad = (decimal)reader["QTY"]
                        };
                        listaUbicar.Add(UbicarDetalle);
                    }
                    else
                    {
                        ClassListaUbicar UbicarDetalle = new ClassListaUbicar()
                        {
                            Codigo = (string)reader["codigo_producto"],
                            Descrip = (string)reader["description"],
                            Cantidad = (decimal)reader["cantidad"],
                            Merma = (decimal)reader["defectuosos"],
                            Embarque = (string)reader["embarque"],
                            OC = (string)reader["oc"],
                            Factura = (string)reader["factura"]
                        };
                        listaUbicar.Add(UbicarDetalle);

                    }
                }//fin while
            }
            catalogoUbicar = listaUbicar;
            if (Class1.TipoUbica == 3)
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Codigo + " = " + x.Cantidad + " = " + x.Descrip).ToArray()));
            else
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Codigo + " = " + x.Cantidad + " = " + x.Descrip.ToString() + " = " + x.Merma.ToString() + " = " + x.Embarque.Trim() + " = " + x.OC.Trim() + " = " + x.Factura.Trim()).ToArray()));
           
            listViewUbicar.Adapter = adapter;
        }

        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewUbicar.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (FlagTarima)
            {
                char[] delimiterChars = { '=' };
                string select = adapter.GetItem(position).ToString();
                string[] words = select.Split(delimiterChars);
                string valores = words[0] + "$" + words[1] + "$" + words[2];
                editTextCodUP.Text = words[0];
                if (Class1.TipoUbica==1 || Class1.TipoUbica==2)
                {
                    Class1.vEmbarque = words[4];
                    Class1.vOC = words[5];
                    Class1.vFactura = words[6];
                    Class1.vbUPCProducto = "";
                    //' Verifica si fue un codigo UPC para extraer el codigo del producto que es con lo que trabajamos. 
                    ExisteArt(editTextCodUP.Text);
                    if (Class1.vbUPCProducto != "")
                    {
                        editTextCodUP.Text = Class1.vbUPCProducto;
                        //' Verifica que el codigo del produto este en la lista
                        //'idProd = GetIDProduct(editTextCodUP.Text, "1", "");
                        string vID = GetIDProduct(editTextCodUP.Text, "1", "");
                        idProd = Convert.ToInt32(vID);
                        if (idProd==0)
                        {
                            Toast.MakeText(this, "El código que indicó no existe como disponible para ubicar", ToastLength.Long).Show();
                            editTextCodUP.Text = "";
                            editTextCodUP.RequestFocus();
                        }
                        else
                        {
                            editTextCantUP.Enabled = true;
                            editTextCantUP.Text = "";
                            editTextCantUP.RequestFocus();
                        }

                    }
                    else
                    {
                        Toast.MakeText(this, "El Código No Existe en la empresa 06!", ToastLength.Long).Show();
                        editTextCodUP.Text = "";
                        editTextCodUP.RequestFocus();
                    }
                    editTextCantUP.RequestFocus();
                }
            }
            else
            {

                if (Class1.TipoUbica == 1)
                {
                    Toast.MakeText(this, "Favor de indicar primero la Tarima a donde va colocar el producto!...", ToastLength.Long).Show();
                }
                else if (Class1.TipoUbica == 2)
                {
                    Toast.MakeText(this, "Favor de indicar primero la Ubicacion a donde va colocar el producto!...", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "Favor de indicar primero la Ubicacion a donde va colocar la Tarima!...", ToastLength.Long).Show();
                }
                editTextTarimaUP.RequestFocus();
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
        private string GetIDProduct(string upc, string ubica, string Tarima)
        {
            string aux = "";
            try
            {
                 using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select * from vLogistik_Ubicaciones " +
                          "where codigo_producto = '" + upc + "' and ubicacion='" + ubica.Trim() +
                          "' and embarque='" + Class1.vEmbarque.Trim() + "' and oc='" + Class1.vOC.Trim() + "' and factura='" +
                          Class1.vFactura.Trim() + "' and estado='2'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        IDUbica = Convert.ToString(reader["id_ubicacion"]);
                        aux = Convert.ToString(reader["id_ubicacion"]);
                        CantOri = Convert.ToDecimal(reader["Cantidad"]);
                        UbicaOri = Convert.ToString(reader["ubicacion"]);


                    }
                    return aux;
                }
                return aux;
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error:" + ex.Message.ToString(), ToastLength.Long).Show();
                return "";
            }
        }

        private void SiguienteDocumento()
        {
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "SELECT num_traspaso FROM Logistik_Consecutivo_SAE where num_doc=1";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                CrearBD.vgOrder = 1;
                while (reader.Read())
                {
                    CrearBD.vgOrder = reader.GetInt32(0);
                    CrearBD.vgOrder = CrearBD.vgOrder + 1;
                }
                Class1.strSQL = "UPDATE Logistik_Consecutivo_SAE SET num_traspaso=" + CrearBD.vgOrder.ToString();
                SqlCommand sqlcmd5 = new SqlCommand(Class1.strSQL, con);
                sqlcmd5.ExecuteNonQuery();
                //EjecutarQuerySQL(sqlt)
                //EjecutarQuerySQLWifi(sqlt)
            }
        }
        private void GetTarima(string Flag)
        {
            int naux=0;
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "Select num_tarima from Logistik_Consecutivo_SAE where num_doc = '1'";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                Class1.vgNTarima = "";
                while (reader.Read())
                {
                    naux = reader.GetInt32(0);
                    Class1.vgNTarima = "T" + naux.ToString("D4"); //' Aqui va la tarima anterior
                    //naux = thisReader.Item("num_tarima")
                    // vgNTarima = "T" & naux.ToString("D4")   ' Aqui va la tarima anterior
                }
                if (Flag=="1")
                {
                    Class1.strSQL = "UPDATE Logistik_Consecutivo_SAE SET num_tarima=" + naux + " where num_doc = '1'";
                    SqlCommand sqlcmd5 = new SqlCommand(Class1.strSQL, con);
                    sqlcmd5.ExecuteNonQuery();
                    //EjecutarQuerySQLWifi(sqlt)
                }
                
            }
        }
        private void editTextTarimaUP_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionTarimaUP(object sender, TextView.EditorActionEventArgs e)
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
            char lastchar;
            string cadena = editTextTarimaUP.Text;
            int vtarima = editTextTarimaUP.Length();

            if (vtarima > 0)
            {
                if (Class1.TipoUbica == 3 || Class1.TipoUbica == 2)
                {
                    // Valida que la ubicacion indicada sea valida
                    IDWharehouse = Verifica_Ubica(cadena, "1");
                    if (IDWharehouse == "")
                    {
                        Toast.MakeText(this, "La ubicacion no existe en SAE, favor de darla de alta primero", ToastLength.Short).Show();
                        editTextTarimaUP.Text = "";
                        editTextTarimaUP.RequestFocus();
                    }
                    else
                    {
                        editTextTarimaUP.Enabled = false;
                        editTextCodUP.Enabled = true;
                        FlagTarima = true;
                        editTextCodUP.RequestFocus();
                    }
                }
                else
                {
                    if (editTextTarimaUP.Text != "")
                    {
                        if (editTextTarimaUP.Text.Substring(0, 1) == "T" && editTextTarimaUP.Text.Length == 5)
                        {
                            //' Ahora valida no sea mayor del contador de tarimas
                            GetTarima("0");
                            int vntarima1 = Convert.ToInt32(editTextTarimaUP.Text.Substring(1, 4));
                            int vntarima2 = Convert.ToInt32(Class1.vgNTarima.Substring(1, 4));
                            if (vntarima1 > vntarima2)
                            {
                                Toast.MakeText(this, "El Número de Tarima es mayor al contador de tarimas actual!", ToastLength.Short).Show();
                                editTextTarimaUP.Text = "";
                                editTextTarimaUP.RequestFocus();
                            }
                            else
                            {
                                //' Valida que no sea una tarima que se este ya usando
                                GetEstadoTarima(editTextTarimaUP.Text);
                                //' Si es 3 quiere decir que ya esta ubicada y no se puede usar
                                if (StatusTarima == "3")
                                {
                                    Toast.MakeText(this, "El Número de Tarima esta ya ubicada y no se puede usar en esta opción, vaya a Reubicar-Mete Producto a Tarima.", ToastLength.Short).Show();
                                    editTextTarimaUP.Text = "";
                                    editTextTarimaUP.RequestFocus();
                                }
                                else
                                {

                                    listViewUbicar.Enabled = true;

                                    imgbtnTerminarUP.Enabled = false;
                                    editTextTarimaUP.Enabled = false;
                                    editTextCodUP.Enabled = true;

                                    editTextCantUP.Enabled = false;

                                    FlagTarima = true;
                                    editTextCodUP.RequestFocus();
                                }

                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "El Número de Tarima no es valido!", ToastLength.Short).Show();
                            editTextTarimaUP.Text = "";
                            editTextTarimaUP.RequestFocus();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Favor de indicar un dato valido en la Tarima!", ToastLength.Short).Show();
                        editTextTarimaUP.Text = "";
                        editTextTarimaUP.RequestFocus();
                    }
                }
            }
        }
        private void editTextCodUP_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionCodUP(object sender, TextView.EditorActionEventArgs e)
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
            char lastchar;
            string cadena;
            string sql1 = "";
            var codigo = editTextCodUP.Text;
            var tarima = editTextTarimaUP.Text;
            var cant = editTextCantUP.Text;
            int vcodigo = editTextCodUP.Length();
            if (vcodigo >= 1)
            {
                if (Class1.TipoUbica == 1 || Class1.TipoUbica == 2)
                {
                    Toast.MakeText(this, "Favor de seleccionar el producto del listado!", ToastLength.Short).Show();
                    editTextCodUP.Text = "";
                    editTextCodUP.RequestFocus();
                }
                else
                {
                    if (tarima != "")
                    {
                        //' Ahora va a llenar los datos de encabezado y detalle de traspasos de los productos en cuestion
                        //' Obtiene el numero de orden con el que se guardo el dertalle de esa tarima en la tabla de inventarios temporal
                        //'GetIDTarima(TxtCodigo.Text)
                        LlenaTraspasodeTarima();
                        Verifica_Ubica(tarima, "1");

                        if (IDWharehouse != "")
                        {
                            //' Va y cambia la ubicacion de la tarima y limpia todo.
                            sql1 = "update vLogistik_Ubicaciones set ubicacion = '" + IDWharehouse + "', estado='3', id_usuario ='" + Class1.vgID_employee + "'" +
                                   " where tarima='" + codigo + "'";
                            EjecutarQuerySQLWifi(sql1);
                        }
                        else
                            Toast.MakeText(this, "ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga nada mas con la terminal...", ToastLength.Short).Show();

                        if (Class1.TipoUbica == 1)
                        {
                            textView102S.Text = "Tarima";
                            //Label2.Text = "Tarima";
                            textView103S.Text = "Código";
                            //Label1.Text = "Código";

                                //Label3.Visible = True
                            editTextCantUP.Visibility = ViewStates.Visible;
                            //txtCant.Visible = True

                            imgbtnTerminarUP.Visibility = ViewStates.Visible;
                            //PictureBox4.Visible = True
                            //Label4.Visible = True
                        }
                        if (Class1.TipoUbica == 2)
                        {
                            textView102S.Text = "Ubica.";
                            //Label2.Text = "Ubica."
                            textView103S.Text = "Código";
                            //Label1.Text = "Código"

                                //Label3.Visible = True
                            editTextCantUP.Visibility = ViewStates.Visible;
                            //txtCant.Visible = True
                            imgbtnTerminarUP.Visibility = ViewStates.Visible;
                            //PictureBox4.Visible = True
                            //Label4.Visible = True
                         }

                        if (Class1.TipoUbica == 3)
                        {
                            textView102S.Text = "Ubica.";
                            //Label2.Text = "Ubica."
                            textView103S.Text = "Tarima";
                            //Label1.Text = "Tarima"

                                //Label3.Visible = False
                            editTextCantUP.Visibility = ViewStates.Invisible;
                            //txtCant.Visible = False

                            editTextCodUP.Text = "";
                            //TxtCodigo.Text = ""
                            editTextTarimaUP.Text = "";
                            //txtTarima.Text = ""
                            editTextTarimaUP.Enabled = true;
                            editTextTarimaUP.RequestFocus();
                            //txtTarima.Focus()
                            imgbtnTerminarUP.Visibility = ViewStates.Invisible;
                            //PictureBox4.Visible = False
                            //Label4.Visible = False

                        }
                        FlagTarima = false;
                        LlenarProveedores();
                    }
                    else
                    {
                        Toast.MakeText(this,"Favor de indicar la ubicacion destino!", ToastLength.Short).Show();
                        editTextTarimaUP.RequestFocus();
                    }
                }
            }
        }
        private void editTextCantUP_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarCantidad();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void HandleEditorActionCantUP(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarCantidad();
                e.Handled = true;
            }
        }
        private void VerificarCantidad()
        {
            try
            {
                char lastchar;
                string cadena;
                string sql1 = "";
                decimal dCant;
                var codigo = editTextCodUP.Text;
                var tarima = editTextTarimaUP.Text;
                decimal cant = editTextCantUP.Length();
                if (cant > 0)
                {
                    // Valida que sea un numero.
                    if (Convert.ToInt32(cant) >= 1)
                    {
                        // Verifica que no este tomando mas de lo debido
                        if (cant > CantOri)
                        {
                            Toast.MakeText(this, "Esta indicando una cantidad mayor a la que se tiene disponible en esa ubicacion!", ToastLength.Short).Show();
                            cant = 0;
                            editTextCantUP.RequestFocus();
                        }
                        else
                        {
                            bool verifExist = false;
                            verifExist = FnExisteArticuloUbica(codigo);
                            if (verifExist == false)
                                Toast.MakeText(this, "El código no existe para ubicar!!", ToastLength.Short).Show();
                            else
                            {
                                //' Descuenta lo que movió de la ubicacio origina
                                dCant = Class1.Inv_cant - cant;
                                dCant = Convert.ToDecimal(String.Format("{0:0,0}", dCant));
                                if (dCant != 0)
                                       sql1 = "update vLogistik_Ubicaciones set cantidad = '" + dCant.ToString() + "'" + 
                                        " where codigo_producto='" + codigo + "'" + 
                                        " and ubicacion='1' and embarque='" + Class1.vEmbarque.Trim() + "' and oc='" + Class1.vOC.Trim() + 
                                        "' and factura='" + Class1.vFactura.Trim() + "' and estado='2'"; 
                                        //and tarima='" + tarima + "'";
                                else
                                    sql1 = "DELETE FROM vLogistik_Ubicaciones WHERE id_ubicacion = " + IDUbica;
                                EjecutarQuerySQLWifi(sql1);
                           
                                sql1 = "insert into Logistik_Inventario_Temp(id_order, id_product, id_product_attribute, num_parte," +
                                        " cantidad, ubicacion, tipo, almacen, existe, tarima, embarque, oc, factura) values ('" + 
                                        CrearBD.vgOrder + "','" + idProd + "','0','" + codigo + "','" + cant.ToString() + "','" + UbicaOri +
                                       "','" + Class1.TipoUbica + "','" + UbicaOri + "','1','" + tarima + "','" + Class1.vEmbarque.Trim() + "','" +
                                       Class1.vOC.Trim() + "','" + Class1.vFactura.Trim() + "')";
                                EjecutarQuerySQLWifi(sql1);
                                CantProducto = CantProducto + 1;
                            }
                            if (Class1.TipoUbica == 1)
                            {
                                imgbtnTerminarUP.Enabled = true;
                                //PictureBox4.Enabled = True
                                editTextCodUP.Text = "";
                                //TxtCodigo.Text = ""
                                editTextCantUP.Text = "";
                                //txtCant.Text = ""
                                editTextCodUP.RequestFocus();
                                //TxtCodigo.Focus()
                            }
                            if (Class1.TipoUbica == 2)
                            {
                                imgbtnTerminarUP.Enabled = true;
                                //PictureBox4.Enabled = True
                                editTextCodUP.Text = "";
                                //TxtCodigo.Text = ""
                                editTextCantUP.Text = "";
                                //txtCant.Text = ""
                                editTextCodUP.RequestFocus();
                                //TxtCodigo.Focus()
                            }

                            if (Class1.TipoUbica == 3)
                            {
                                editTextTarimaUP.Text = "";
                                //txtTarima.Text = ""
                                editTextCodUP.Text = "";
                                //TxtCodigo.Text = ""
                                editTextCantUP.Text = "";
                                //txtCant.Text = ""
                                editTextTarimaUP.RequestFocus();
                                //txtTarima.Focus()
                            }
                            imgbtnRegresarUP.Enabled = false;
                            //PictureBox5.Enabled = false;
                            LlenarProveedores();
                         }
                        hidekeyboard();
                    }
                    else
                    {
                        Toast.MakeText(this, "Especifica una cantidad", ToastLength.Short).Show();
                        editTextCantUP.Text = "";
                        editTextCantUP.RequestFocus();
                    }
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error:" + ex.Message.ToString(), ToastLength.Long).Show();
            }


        }

        public void hidekeyboard()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
        }
        public bool FnExisteArticuloUbica(string codigo)
        {
            var sqllocal = "Select cantidad from vLogistik_Ubicaciones" +
            " where codigo_producto = '" + codigo + "' and ubicacion='1' and embarque='" +
            Class1.vEmbarque.Trim() + "' and oc='" + Class1.vOC.Trim() + "' and factura='" + Class1.vFactura.Trim() + "' and (estado='2')";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd2 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd2.ExecuteReader();
                Class1.Inv_cant = 0;
                while (reader.Read())
                {
                    Class1.Inv_cant = (decimal)reader["cantidad"];
                    //Inv_cant = oFila.Item("cantidad")
                    return true;
                }
            }
            return false;
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
        private void LlenaTraspasodeTarima()
        {
            string sql1;
            string CodigoTemp;
            string UbicaTemp;
            decimal CantTemp;
            string vAutorizar = "S";
            string vSTATE = "8";
            bool FlagInicial = true;
            try
            {
                int nidprod = 0;
                
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "SELECT * FROM vLogistik_Ubicaciones WHERE (tarima = '" + editTextCodUP.Text +
                                    "') AND (ubicacion='1') AND (estado='22')";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        CodigoTemp = (string)reader["codigo_producto"];
                        nidprod = (int)reader["id_product"];
                        idProd = Convert.ToInt32(nidprod);
                        UbicaTemp = (string)reader["tarima"];
                        CantTemp = (decimal)reader["cantidad"];
                        UbicaOri = (string)reader["Ubicacion"];
                        //' Ahora guarda los datos en inventario *******_____________
                        //'verificar si existe
                        bool verifExist;
                        
                        if (Class1.TipoUbica == 1 && Class1.TipoUbica == 3)
                            verifExist = FnExisteArticuloInv(CodigoTemp, UbicaOri, UbicaTemp);
                        else
                        {
                            //' Obtiene el ID de la ubicacion destino
                            Verifica_Ubica(UbicaTemp, "1");
                            verifExist = FnExisteArticuloInv(CodigoTemp, IDWharehouse, "");
                        }
                        if (FlagInicial)
                        {
                            //'Verifica_Ubica(UbicaTemp, "2")
                            //' Insertar el encabezado de la orden de traspaso
                            sql1 = "INSERT INTO Logistik_Traspasos(id_order," +
                                    "reference, id_shop_group, id_shop, id_carrier, id_lang, id_customer, " +
                                    "id_cart, id_currency, id_address_delivery, id_address_invoice, current_state," +
                                    "secure_key,	payment, conversion_rate, module, recyclable," +
                                    "gift, gift_message, mobile_theme, shipping_number, total_discounts," +
                                    "total_discounts_tax_incl, total_discounts_tax_excl, total_paid, total_paid_tax_incl, total_paid_tax_excl," +
                                    "total_paid_real, total_products, total_products_wt, total_shipping, total_shipping_tax_incl," +
                                    "total_shipping_tax_excl, carrier_tax_rate, total_wrapping, total_wrapping_tax_incl, total_wrapping_tax_excl," +
                                    "round_mode,	invoice_number,	delivery_number, invoice_date, delivery_date," +
                                    "valid, date_add, date_upd, Almacen_destino, Almacen_origen, EnviadaSAE, Id_Employee, Documento, Autorizar,TIPO_MOV, num_empresa, verificacion) values ('" + CrearBD.vgOrder + "','" +
                                    "Ref',1,1,1,2,1," +
                                    "1,1,1,1,'" + vSTATE + "'," +
                                    "'202cb962ac59075b964b07152d234b70','Pago500',300,'Modulo',1," +
                                    "1,'bueno',1,'NumEnv5454',0," +
                                    "0,0,100,16,0," +
                                    "116,1,100,1,1," +
                                    "1,1,1,1,1," +
                                    "1,1,1,'01/01/2015','01/01/2015'," +
                                    "0,'01/01/2015',getdate(),'" + IDWharehoseDest + "','" + UbicaOri + "','U','" +
                                    Class1.vgID_employee.ToString() + "','" + CrearBD.vgOrder + "','" + vAutorizar.ToString() + "','58','06',1)";
                            EjecutarQuerySQLWifi(sql1);
                            FlagInicial = false;
                        }
                        //' Obtiene la descripcion del producto
                        Get_Descri(idProd);
                        CantTemp = Convert.ToDecimal( String.Format("{0:0,0}", CantTemp));
                        //' Inserta el detalle en la tabla de Traspasos
                        sql1 = "INSERT INTO Logistik_Traspasos_detail(id_order, id_order_invoice, id_warehouse, id_shop, product_id" +
                                ",product_attribute_id, product_name, product_quantity, quantity_received, product_quantity_in_stock" +
                                ",product_quantity_refunded, product_quantity_return, product_quantity_reinjected, product_price, reduction_percent " +
                                ",reduction_amount, reduction_amount_tax_incl, reduction_amount_tax_excl, group_reduction, product_quantity_discount" +
                                ",product_ean13, product_upc, product_reference, product_supplier_reference, product_weight" +
                                ",id_tax_rules_group, tax_computation_method, tax_name, tax_rate, ecotax" +
                                ",ecotax_tax_rate, discount_quantity_applied, download_hash, download_nb, download_deadline " +
                                ",total_price_tax_incl, total_price_tax_excl, unit_price_tax_incl, unit_price_tax_excl, total_shipping_price_tax_incl " +
                                ",total_shipping_price_tax_excl, purchase_supplier_price, original_product_price, quantity_input, quantity_output, num_empresa) VALUES ('" +
                                CrearBD.vgOrder.ToString() + "','1','" + UbicaOri + "','1','" + idProd + "','" +
                                "0','" + vDescripcion + "','" + CantTemp.ToString() + "','0','1','1'," +
                                "'1','1','5000','0','0'," +
                                "'0','0','0','0',''," +
                                "'" + CodigoTemp + "','','','0','0'," +
                                "'1','IVA','1','1','1'," +
                                "'1','','1','01/01/2019','16'," +
                                "'0','0','0','116','0'," +
                                "'0','0','0','0','06')";
                        EjecutarQuerySQLWifi(sql1);
                        //' Genera el registro de la tabla del Kardex
                        sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                            "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                            "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc) VALUES (NULL,'" + Class1.vgID_employee + "',getdate()," +
                            "'" + CodigoTemp + "','" + CantTemp.ToString() + "',NULL,'0','3','3','" + IDWharehouse.ToString() + "','" + IDWharehoseDest.ToString() + "',NULL,'" + UbicaTemp + "',NULL,NULL,NULL,NULL,NULL,NULL,NULL)";
                        EjecutarQuerySQLWifi(sql1);

                    }//fin while

                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public string Verifica_Ubica(string ubica, string TipoU)
        {
            try
            {
                int idAlmacen=0;
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select id_warehouse from Logistik_warehouse where name = '" + ubica + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    if (TipoU == "1")
                        IDWharehouse = "";
                    else
                        IDWharehoseDest = "";
                    while (reader.Read())
                    {
                        if (TipoU == "1")
                        {
                            idAlmacen = (int)reader["id_warehouse"];
                            IDWharehouse = Convert.ToString(idAlmacen);
                        }
                        
                        else
                        {
                            idAlmacen = (int)reader["id_warehouse"];
                            IDWharehoseDest = Convert.ToString(idAlmacen);
                        }
                    }
                    return IDWharehouse;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return IDWharehouse;
            }
        }
        public bool FnExisteArticuloInv(string codigo, string ubica, string Tarima)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select cantidad from vLogistik_Ubicaciones" +
                    " where codigo_producto = '" + codigo + "' and ubicacion='" + ubica + "' and tarima='" + Tarima +
                    "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    Class1.Inv_cant = 0;
                    while (reader.Read())
                    {
                        Class1.Inv_cant = (decimal)reader["cantidad"];
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }
        public string Get_Descri(int codigo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select * from Logistik_product_lang " +
                                  "where id_product = '" + codigo.ToString() + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();

                    vDescripcion = "";

                    while (reader.Read())
                    {
                        vDescripcion = (string)reader["description"];
                        return vDescripcion;
                    }
                    return vDescripcion;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return vDescripcion;
            }
        }
        private void GetEstadoTarima(string Tarima)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select * from vLogistik_Ubicaciones " + 
                                    "where tarima = '" + Tarima + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();

                    StatusTarima = "";

                    while (reader.Read())
                    {
                        StatusTarima = (string)reader["estado"];
                        //return vDescripcion;
                    }
                    //return vDescripcion;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                //return vDescripcion;
            }
        }
        private void GuardarDatos()
        {
            string sql1 = "";
            decimal dCant = 0;
            string CodigoTemp = "";
            string UbicaTemp = "";
            int CantTemp = 0;
            string vAutorizar = "S";
            string vSTATE = "8";
            bool FlagInicial = true;
            var cant = editTextCantUP.Text;
            var codigo = editTextCodUP.Text;
            if (CantProducto > 0)
            {
                if (cant == "")
                {
                    //lblAguarde.Visible = true;
                    //textviewMgs.Visibility = ViewStates.Visible;
                    var sqllocal = "SELECT num_parte,id_product,tarima,cantidad,Ubicacion,embarque,oc,factura" +
                            " FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                    using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                    {
                        con.Open();
                        SqlCommand sqlcmd2 = new SqlCommand(sqllocal, con);
                        SqlDataReader reader;
                        reader = sqlcmd2.ExecuteReader();
                        while (reader.Read())
                        {
                            //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                            CodigoTemp = (string)reader["num_parte"];
                            idProd = (int)reader["id_product"];
                            UbicaTemp = (string)reader["tarima"];
                            CantTemp = (int)reader["cantidad"];
                            UbicaOri = (string)reader["Ubicacion"];
                            Class1.vEmbarque = (string)reader["embarque"];
                            Class1.vOC = (string)reader["oc"];
                            Class1.vFactura = (string)reader["factura"];

                            bool verifExist = false;
                            if (Class1.TipoUbica == 1 || Class1.TipoUbica == 3)
                                verifExist = FnExisteArticuloInv(CodigoTemp, UbicaOri, UbicaTemp);
                            else
                            {
                                Verifica_Ubica(UbicaTemp, "1");
                                verifExist = FnExisteArticuloInv(CodigoTemp, IDWharehouse, "");
                            }
                            if (verifExist == false)
                            {
                                if (Class1.TipoUbica == 1)
                                    sql1 = "insert into vLogistik_Ubicaciones(" +
                                    "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + "1" +
                                    "','" + "" + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp.ToString() + "','2','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                                if (Class1.TipoUbica == 3)
                                    sql1 = "insert into vLogistik_Ubicaciones(" +
                                   "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + UbicaOri +
                                   "','" + UbicaTemp + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp.ToString() + "','3','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                                if (Class1.TipoUbica == 2)
                                    sql1 = "insert into vLogistik_Ubicaciones(" +
                                   "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + "1" +
                                   "','" + "" + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp.ToString() + "','2','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                            }
                            else
                            {
                                if (Class1.TipoUbica == 1 || Class1.TipoUbica == 3)
                                {
                                    if (UbicaOri != "")
                                    {
                                        dCant = Convert.ToDecimal(CantTemp) + Class1.Inv_cant;
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() + " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='" + UbicaOri + "' and tarima = '" + UbicaTemp + "' and embarque ='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                    }

                                    else
                                        Toast.MakeText(this,"ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...",ToastLength.Short).Show();
                                }
                                else
                                {
                                    if (IDWharehouse != "")
                                    {
                                        dCant = Convert.ToDecimal(CantTemp) + Class1.Inv_cant;
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() + " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='" + IDWharehouse + "' and tarima = '' and embarque ='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                    }
                                    else
                                        Toast.MakeText(this,"ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...", ToastLength.Short).Show();
                                }

                                EjecutarQuerySQLWifi(sql1);
                                //' Entra a reportar el movimiento de producto si es Ubicar Producto, o ubicar una tarima. 
                                if (Class1.TipoUbica != 1)
                                {
                                    if (Class1.TipoUbica == 2)
                                    {
                                        if (FlagInicial) 
                                        {
                                            Verifica_Ubica(UbicaTemp, "2");
                                            //' Insertar el encabezado de la orden de traspaso
                                            sql1 = "INSERT INTO Logistik_Traspasos(id_order," + 
                                                           "reference, id_shop_group, id_shop, id_carrier, id_lang, id_customer, " + 
                                                           "id_cart, id_currency, id_address_delivery, id_address_invoice, current_state," + 
                                                           "secure_key,	payment, conversion_rate, module, recyclable," + 
                                                           "gift, gift_message, mobile_theme, shipping_number, total_discounts," + 
                                                           "total_discounts_tax_incl, total_discounts_tax_excl, total_paid, total_paid_tax_incl, total_paid_tax_excl," + 
                                                           "total_paid_real, total_products, total_products_wt, total_shipping, total_shipping_tax_incl," + 
                                                           "total_shipping_tax_excl, carrier_tax_rate, total_wrapping, total_wrapping_tax_incl, total_wrapping_tax_excl," + 
                                                           "round_mode,	invoice_number,	delivery_number, invoice_date, delivery_date," + 
                                                           "valid, date_add, date_upd, Almacen_destino, Almacen_origen, EnviadaSAE, Id_Employee, Documento, Autorizar,TIPO_MOV, num_empresa, verificacion) values ('" + CrearBD.vgOrder + "','" + 
                                                           "Ref',1,1,1,2,1," + 
                                                           "1,1,1,1,'" + vSTATE + "'," + 
                                                           "'202cb962ac59075b964b07152d234b70','Pago500',300,'Modulo',1," + 
                                                           "1,'bueno',1,'NumEnv5454',0," + 
                                                           "0,0,100,16,0," + 
                                                           "116,1,100,1,1," + 
                                                           "1,1,1,1,1," + 
                                                           "1,1,1,'01/01/2015','01/01/2015'," + 
                                                           "0,'01/01/2015',getdate(),'" + IDWharehoseDest + "','" + UbicaOri + "','U','" + 
                                                           Class1.vgID_employee.ToString() + "','" + CrearBD.vgOrder + "','" + vAutorizar.ToString() + "','58','06',1)";
                                            EjecutarQuerySQLWifi(sql1);
                                            FlagInicial = false;
                                        }
                                        //' Obtiene la descripcion del producto
                                        Get_Descri(idProd);

                                        //' Inserta el detalle en la tabla de Traspasos
                                        sql1 = "INSERT INTO Logistik_Traspasos_detail(id_order, id_order_invoice, id_warehouse, id_shop, product_id" + 
                                                         ",product_attribute_id, product_name, product_quantity, quantity_received, product_quantity_in_stock" + 
                                                         ",product_quantity_refunded, product_quantity_return, product_quantity_reinjected, product_price, reduction_percent " + 
                                                         ",reduction_amount, reduction_amount_tax_incl, reduction_amount_tax_excl, group_reduction, product_quantity_discount" + 
                                                         ",product_ean13, product_upc, product_reference, product_supplier_reference, product_weight" + 
                                                         ",id_tax_rules_group, tax_computation_method, tax_name, tax_rate, ecotax" + 
                                                         ",ecotax_tax_rate, discount_quantity_applied, download_hash, download_nb, download_deadline " + 
                                                         ",total_price_tax_incl, total_price_tax_excl, unit_price_tax_incl, unit_price_tax_excl, total_shipping_price_tax_incl " + 
                                                         ",total_shipping_price_tax_excl, purchase_supplier_price, original_product_price, quantity_input, quantity_output, num_empresa) VALUES ('" + 
                                                         CrearBD.vgOrder + "','1','" + UbicaOri + "','1','" + idProd + "','" + 
                                                         "0','" + vDescripcion + "'," + CantTemp + ",0,1,1," + 
                                                         "1,1,5000,0,0," + 
                                                         "0,0,0,0,''," + 
                                                         "'" + CodigoTemp + "','','',0,0," + 
                                                         "1,'IVA',1,1,1," + 
                                                         "1,'',1,'01/01/2015',16," + 
                                                         "0,0,0,116,0," + 
                                                         "0,0,0,0,'06')";
                                            EjecutarQuerySQLWifi(sql1);
                                    }
                                    else
                                    {
                                        //' Entonces va a mover lo que esta en la tarima X a la ubicacion N
                                    }
                                    //' Genera el registro de la tabla del Kardex
                                    sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                                    "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                                    "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product) VALUES (NULL,'" + Class1.vgID_employee.ToString() + "',getdate()," +
                                    "'" + CodigoTemp + "','" + CantTemp + "',NULL,'0','2','3','1','" + IDWharehoseDest + "',NULL,'" + UbicaTemp +
                                    "',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL," + idProd.ToString() + ")";
                                    EjecutarQuerySQLWifi(sql1);
                                }
                                  
                            }
                        }//end while
                        //' Marca la bandera para que se imprima la etiqueta de la tarima si acaba de llenarla
                        if (Class1.TipoUbica == 1)
                        {
                            sql1 = "update vLogistik_Ubicaciones set FlagPrint = '1'" +
                            " where tarima = '" + UbicaTemp + "'"; //' and ubicacion='" + UbicaOri + "'";
                            EjecutarQuerySQLWifi(sql1);
                        }
                        //' Finalmente borra todo lo de Inv_Temp para limpiar la tabla
                        sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                        EjecutarQuerySQLWifi(sql1);


                        //If ConnWifi.State = Data.ConnectionState.Open Then
                        //    ConnWifi.Close()
                        //End If

                        FlagTarima = false;

                        imgbtnInventUP.Enabled = true;
                        //PictureBox2.Enabled = True
                        imgbtnRegresarUP.Enabled = true;
                        //PictureBox5.Enabled = True

                        //lblAguarde.Visible = False

                        StartActivity((typeof(ActivityUbicar)));
                        Finish();


                    }//end connection
                }
            }
        }
    }
}