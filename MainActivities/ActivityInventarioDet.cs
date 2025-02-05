using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
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
    [Activity(Label = "ActivityInventarioDet")]
    public class ActivityInventarioDet : Activity, ListView.IOnItemClickListener
    {
        string IDWharehouse;
        Boolean FlagSelecciono;
        private EditText editTextTarimaID, editTextCodID;
        private ListView listViewInvDet;
        public List<OrdenCompra> catalogoUbicar;
        public List<OrdenCompra> listaUbicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private Button btnAceptar, btnCerrarUbicacion;
        private ImageButton imgbtnRegresarID;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_inventario_det);
            editTextTarimaID = FindViewById<EditText>(Resource.Id.editTextTarimaID);
            editTextCodID = FindViewById<EditText>(Resource.Id.editTextCodID);
            listViewInvDet = FindViewById<ListView>(Resource.Id.listViewInvDet);
            listViewInvDet.OnItemClickListener = this;
            btnAceptar = FindViewById<Button>(Resource.Id.btnAceptar);
            btnCerrarUbicacion = FindViewById<Button>(Resource.Id.btnCerrarUbicacion);
            imgbtnRegresarID = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarID);
            CargarForma();
            editTextTarimaID.KeyPress += editTextTarimaID_KeyPress;
            editTextTarimaID.EditorAction += HandleEditorActionTarimaInv;
            editTextCodID.KeyPress += editTextCodIDI_KeyPress;
            editTextCodID.EditorAction += HandleEditorActionCodBarInv;
            btnAceptar.Click += delegate
            {
                AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                Win_Save.SetMessage("Esta seguro de Aceptar todas las cantidades de esta Tarima/Ubicación?");
                Win_Save.SetTitle("TARIMA O UBICACION");
                Win_Save.SetPositiveButton("No", (send, arg) =>
                {
                    Win_Save.Dispose();
                });
                Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                {
                    //' Barre el grid para copiar la cantidad Original a la cantidad Leida
                    int numGrid = listViewInvDet.CheckedItemCount;
                    int i = 0;
                    for (i = 0; i < (numGrid - 1); i++)
                    {
                        //CodTemp = DGVClientes.Item(i, 1).ToString.Trim;
                        //TarimaTemp = DGVClientes.Item(i, 2).ToString.Trim;
                        //CantOri = DGVClientes.Item(i, 3).ToString.Trim;

                        //inst.updateCodigoOKIF(CodTemp, TarimaTemp, CantOri);

                        Class1.FlagIniciaInvFisico = true;
                        StartActivity((typeof(ActivityInventario)));
                        Finish();
                    }

                    Win_Save.Dispose();
                });
                Win_Save.Show();
            };
            btnCerrarUbicacion.Click += delegate
            {
                //' Boton para Cerrar la Ubicacion
                AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                Win_Save.SetMessage("Esta seguro de cerrar esta Ubicación? Las cantidades que no haya validado, se quedaran en cero y una vez que sea cerrada ya no podra trabajar mas con ella.");
                Win_Save.SetTitle("CERRAR UBICACION");
                Win_Save.SetPositiveButton("No", (send, arg) =>
                {
                    Win_Save.Dispose();
                });
                Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                {
                    Class1.BanderaClave = "1";
                    StartActivity((typeof(LoginActivity)));
                    Finish();
                    Win_Save.Dispose();
                });
                Win_Save.Show();
            };
            imgbtnRegresarID.Click += delegate
            {
                Class1.FlagIniciaInvFisico = true;
                StartActivity((typeof(ActivityInventario)));
                Finish();
            };


        }
        private void CargarForma()
        {
            if (Class1.FlagInvFisicoInic == false)
                LlenarProveedores("");
            else if (Class1.FlagInvFisicoInic == true && Class1.NumeroToma != 1)
                LlenarProveedores("");


            if (Class1.FlagInvFisicoInic == true)
            {
                editTextTarimaID.Enabled = false;
                editTextTarimaID.Text = "";
                editTextCodID.Text = "";
                editTextCodID.RequestFocus();
            }
            else
            {
                editTextTarimaID.Enabled = true;
                editTextCodID.Text = "";
                editTextTarimaID.Text = "";
                editTextTarimaID.RequestFocus();
            }
            FlagSelecciono = false;
            Class1.FlagListaInvF = false;

            if (Class1.FlagListaInvF)
            {
                Class1.FlagListaInvF = false;

                //dtOrdenCompra.Clear()
                //dsOrdenCompra.Clear()

                if (Class1.FlagInvFisicoInic == false)
                    LlenarProveedores("");
                else if (Class1.FlagInvFisicoInic == true && Class1.NumeroToma != 1)
                    LlenarProveedores("");

                if (Class1.FlagInvFisicoInic == true)
                {
                    editTextTarimaID.Enabled = false;

                    editTextTarimaID.Text = "";
                    editTextCodID.Text = "";
                    editTextCodID.RequestFocus();
                }
                else
                {
                    editTextTarimaID.Enabled = true;

                    editTextCodID.Text = "";
                    editTextTarimaID.Text = "";
                    editTextTarimaID.RequestFocus();
                }
                FlagSelecciono = false;
            }

        }
        private void LlenarProveedores(string Tarima)
        {
            string sql = "";
            int numtarima = editTextTarimaID.Length();
            if (numtarima > 0)
            {
                if (Class1.NumeroToma == 1)
                    sql = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, lif.descripcion, lif.embarque, lif.oc, lif.factura" +
                    " from vLogistik_InventariosFisicos lif" +
                    " where lif.ubicacion = '" + Class1.UbicaSelect + "'" +
                    " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                    " and lif.tarima = '" + Tarima + "'" +
                    " and lif.FlagT" + Class1.NumeroToma + " = '0'";
                else if (Class1.NumeroToma == 2)
                    sql = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, lif.descripcion, lif.embarque, lif.oc, lif.factura" +
                   " from vLogistik_InventariosFisicos lif" +
                   " where lif.cantR <> 0" +
                   " and lif.ubicacion = '" + Class1.UbicaSelect + "'" +
                   " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                   " and lif.tarima = '" + Tarima + "'" +
                   " and lif.FlagT" + Class1.NumeroToma + " = '0'" +
                   " and lif.FlagT1 = '1'";
                else if (Class1.NumeroToma == 3)
                    sql = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, lif.descripcion, lif.embarque, lif.oc, lif.factura" +
                   " from vLogistik_InventariosFisicos lif" +
                   " where lif.cantR <> lif.CantR2" +
                   " and lif.ubicacion = '" + Class1.UbicaSelect + "'" +
                   " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                   " and lif.tarima = '" + Tarima + "'" +
                   " and lif.FlagT" + Class1.NumeroToma + " = '0'" +
                   " and lif.FlagT1 = '1'";
            }
            else
            {
                if (Class1.NumeroToma == 1)
                    sql = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, lif.descripcion, lif.embarque, lif.oc, lif.factura" +
                      " from vLogistik_InventariosFisicos lif" +
                      " where lif.ubicacion = '" + Class1.UbicaSelect + "'" +
                      " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                      " and lif.FlagT" + Class1.NumeroToma.ToString() + " = '0'";
                else if (Class1.NumeroToma == 2)
                    sql = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, lif.descripcion, lif.embarque, lif.oc, lif.factura" +
                     " from vLogistik_InventariosFisicos lif where lif.cantR <> 0" +
                     " and lif.ubicacion = '" + Class1.UbicaSelect + "'" +
                     " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                     " and lif.FlagT" + Class1.NumeroToma.ToString() + " = '0'" +
                     " and lif.FlagT1 = '1'";
                else if (Class1.NumeroToma == 3)
                    sql = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, lif.descripcion, lif.embarque, lif.oc, lif.factura" +
                   " from vLogistik_InventariosFisicos lif where lif.cantR <> lif.CantR2" +
                   " and lif.ubicacion = '" + Class1.UbicaSelect + "'" +
                   " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                   " and lif.FlagT" + Class1.NumeroToma.ToString() + " = '0'" +
                   " and lif.FlagT1 = '1'";
            }
            mItems = new List<string>();
            listaUbicar = new List<OrdenCompra>();
            catalogoUbicar = new List<OrdenCompra>();
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand command = new SqlCommand(sql, con);
                SqlDataReader reader = command.ExecuteReader();
                CrearBD.vgOrder = 1;
                while (reader.Read())
                {

                    OrdenCompra UbicarDetalle = new OrdenCompra()
                    {
                        ClaveInv = (string)reader["ClaveInv"],
                        Codigo = (string)reader["codigo_producto"],
                        Tarima = (string)reader["tarima"],
                        //if (FlagInvFisicoInic==true)
                        //    CantO = "0",
                        //else
                        //    CantO = (string)reader["CantO"],
                        CantO = (decimal)reader["CantO"],
                        Descripcion = (string)reader["descripcion"],
                        Embarque = (string)reader["embarque"],
                        OC = (string)reader["oc"],
                        Factura = (string)reader["factura"]

                    };
                    listaUbicar.Add(UbicarDetalle);

                }
                catalogoUbicar = listaUbicar;
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.ClaveInv + " = " + x.Codigo + " = " + x.Tarima).ToArray()));
                listViewInvDet.Adapter = adapter;
            }
        }
        private void editTextTarimaID_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionTarimaInv(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next && e.ActionId == ImeAction.Send && e.ActionId == ImeAction.Done && e.ActionId == ImeAction.Go)
            {
                VerificarTarima();
                e.Handled = true;
            }
        }
        private void VerificarTarima()
        {
            //char lastchar;
            string cadena;
            int numtarina = editTextTarimaID.Length();
            if (numtarina > 0)
            {
                //lastchar = editTextTarimaID.Text(editTextTarimaID.Length() - 1);
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = editTextTarimaID.Text;
                cadena = cadena.Substring(0, cadena.Length - 2);
                editTextTarimaID.Text = cadena;
                Class1.CaducaIF = cadena;
                if (Class1.FlagInvFisicoInic == false)
                    LlenarProveedores(editTextTarimaID.Text);
                else if (Class1.FlagInvFisicoInic == true && Class1.NumeroToma != 1)
                    LlenarProveedores(editTextTarimaID.Text);
                editTextCodID.RequestFocus();
                //End If
            }


        }
        private void editTextCodIDI_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionCodBarInv(object sender, TextView.EditorActionEventArgs e)
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
            int numcodigo = editTextCodID.Length();
            if (numcodigo > 0)
            {
                //lastchar = txtCodigo.Text(txtCodigo.TextLength - 1);

                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = editTextCodID.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextCodID.Text = cadena;
                //Class1.CodTarimaIF = cadena;
                Class1.CodTarimaIF = editTextCodID.Text;
                Class1.vbUPCProducto = "";
                if (Busca_Codigo(true) == true)
                {
                    Class1.CodTarimaIF = Class1.vbUPCProducto;
                    //' Verifica si son mas de 1 partida con el mismo codigo. Si es asi, pide capturar solo seleccionando en el grid
                    if (FlagSelecciono == false)
                    {
                        if (fnValida_CodigoRepetido(editTextTarimaID.Text, Class1.vbUPCProducto))
                        {
                            Toast.MakeText(this, "En la Ubicación se tiene mas de un producto repetido pero con diferente embarque, favor de seleccionarlo manualmente del listado", ToastLength.Short).Show();
                            editTextCodID.Text = "";
                            editTextCodID.RequestFocus();
                        }
                        else
                        {
                            FlagSelecciono = false;

                            Class1.FlagDetalleInvF = true;
                            StartActivity((typeof(ActivityInventarioDetCajas)));
                            Finish();
                            //if (Class1.FlagInvFisicoInic && Class1.NumeroToma == 1)
                            //    DetalleInvF.txtCant.RequestFocus();
                        }


                    }
                    else
                    {
                        FlagSelecciono = false;
                        Class1.FlagDetalleInvF = true;
                        StartActivity((typeof(ActivityInventarioDetCajas)));
                        Finish();
                        //DetalleInvF.Show();
                        //DetalleInvF.Focus();
                        //if (Class1.FlagInvFisicoInic) 
                        //DetalleInvF.txtCant.RequestFocus();

                    }
                }
                else
                {
                    if (Class1.vbUPCProducto != "")
                    {
                        Class1.CodTarimaIF = Class1.vbUPCProducto;
                        if (Class1.FlagInvFisicoInic == false && Class1.NumeroToma != 1)
                        {
                            //Module1.sound.Play();
                            Toast.MakeText(this, "Este Código-tarima no corresponde a la Ubicación que esta trabajando...", ToastLength.Short).Show();
                            editTextCodID.Text = "";
                            editTextCodID.RequestFocus();
                        }
                        else
                        {
                            if (Class1.NumeroToma == 1) //' Solo permite agregar en toma 1
                            {
                                //' Va hacer el cambio de la ubicación y luego va mostrar el detalle de la tarima para verificar los Fardos

                                InsertaCodigoUIF(Class1.CodTarimaIF, editTextTarimaID.Text, Class1.UbicaSelect, Class1.vbNomProd, "", "", "", "");

                                //' Va y muestra el detalle de la Ubicacion. 
                                Class1.FlagDetalleInvF = true;

                                StartActivity((typeof(ActivityInventarioDetCajas)));
                                Finish();
                                //if (Class1.FlagInvFisicoInic)
                                //    DetalleInvF.txtCant.RequestFocus();
                            }
                            else
                            {

                                //Module1.sound.Play();
                                Toast.MakeText(this, "Este Código-tarima no corresponde a la Ubicación que esta trabajando...", ToastLength.Short).Show();
                                editTextCodID.Text = "";
                                editTextCodID.RequestFocus();
                            }


                        }
                    }
                }

            }


        }
        private bool Busca_Codigo(bool Bandera)
        {
            string str = "";
            int value = editTextCodID.Length();
            string scanned = editTextCodID.Text;
            string aux;
            string aux2;
            int i;
            bool FlagEncontrado;
            int contacodigos = 0;
            string CaducaAux;
            if (Bandera)
            {
                CaducaAux = "";
                FlagEncontrado = false;

                //' Verifica que el codigo de producto exista
                //infoStruct.desc = inst.ExisteArt(scanned);
                ExisteArt(scanned);
                if (Class1.vbNomProd == "")
                {
                    Toast.MakeText(this, "El código de producto no es valido", ToastLength.Short).Show();
                    editTextCodID.Text = "";
                    editTextCodID.RequestFocus();
                }
                else
                {
                    scanned = Class1.vbUPCProducto;
                    //' Barre el grid para verificar el codigo sea valido para esta ubicacion y tarima cuando no es un inventario inicial

                    if (Class1.FlagInvFisicoInic && Class1.NumeroToma == 1)
                    {
                        //infoStruct = inst.verifyTarimaIF(Class1.vbUPCProducto, "");   //' Busca codigo en base de datos
                        //mainInfo2.CodFrog = infoStruct.CodFrog;
                        verifyTarimaIF(Class1.vbUPCProducto, "");

                        //if (mainInfo2.CodFrog == "")
                        //    FlagEncontrado = false;
                        //else
                        //    FlagEncontrado = true;           
                    }
                    else
                    {
                        //infoStruct = inst.verifyTarimaIF(Class1.vbUPCProducto, "");   //' Busca codigo en base de datos
                        //mainInfo2.CodFrog = infoStruct.CodFrog;
                        verifyTarimaIF(Class1.vbUPCProducto, "");

                        //if (mainInfo2.CodFrog == "")
                        //    FlagEncontrado = false;
                        //else
                        //    FlagEncontrado = true;
                    }
                }
                return FlagEncontrado;
            }
            else
            {
                //' Si no valida contra base de datos, siempre regresa TRUE
                return true;
            }
        }
        private bool fnValida_CodigoRepetido(string Tarima, string pCodigo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    if (Tarima.Length > 0)
                        Class1.strSQL = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, pl.description, lif.embarque, lif.oc, lif.factura" +
                        " from vLogistik_InventariosFisicos lif, Logistik_product_lang pl" +
                        " where pl.id_product = lif.id_product" +
                        " and pl.id_lang = '2' " +
                        " and lif.ubicacion = '" + Class1.UbicaSelect + "' " +
                        " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                        " and lif.tarima = '" + Tarima + "'" +
                        " and lif.FlagT" + Class1.NumeroToma.ToString() + " = '0'" +
                        " and lif.codigo_producto = '" + pCodigo + "'";
                    else
                        Class1.strSQL = "select lif.ClaveInv, lif.codigo_producto, lif.ubicacion, lif.tarima, lif.CantO, pl.description, lif.embarque, lif.oc, lif.factura" +
                      " from vLogistik_InventariosFisicos lif, Logistik_product_lang pl" +
                      " where pl.id_product = lif.id_product" +
                      " and pl.id_lang = '2' " +
                      " and lif.ubicacion = '" + Class1.UbicaSelect + "' " +
                      " and lif.ClaveInv = '" + Class1.NumeroInventario + "'" +
                      " and lif.FlagT" + Class1.NumeroToma.ToString() + " = '0'" +
                      " and lif.codigo_producto = '" + pCodigo + "'";

                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    int cont = 0;
                    while (reader.Read())
                    {
                        Class1.vIFEmbarque = (string)reader["embarque"];
                        Class1.vIFOC = (string)reader["oc"];
                        Class1.vIFFactura = (string)reader["factura"];
                        cont = cont + 1;
                    }
                    if (cont > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
                return false;
            }
        }
        private void InsertaCodigoUIF(string Codigo, string Tarima, string Ubica, string Descri, string BCaduda, string Familia, string Min, string Max)
        {
            int qty;
            string aux;
            string Estatus;
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    if (Class1.UbicaSelect == "1")
                        Estatus = "2";
                    else
                        Estatus = "3";
                    string vdate = DateTime.Now.ToString("yyyyMMdd hh:mm:ss tt");
                    if (Class1.FlagInvFisicoInic == true)
                        Class1.strSQL = "INSERT INTO vLogistik_InventariosFisicos (ClaveInv, id_product, codigo_producto, Descripcion, Tarima, Ubicacion, estado, CantO, CAntR, CantR2, CantR3, CantT, Fecha, id_usuario, Flag, FlagT1, FlagT2, FlagT3, embarque, oc, factura) VALUES (" +
                                    "'" + Class1.NumeroInventario + "','" + Class1.vgIDProducto + "','" + Codigo + "','" + Descri + "','" + Tarima + "','" + Ubica + "','" + Estatus + "','0','0','0','0','0','" + vdate + "','" + Class1.vgID_employee + "','0','0','0','0','II','II','II')";
                    else
                        Class1.strSQL = "INSERT INTO vLogistik_InventariosFisicos (ClaveInv, id_product, codigo_producto, Descripcion, Tarima, Ubicacion, estado, CantO, CAntR, CantR2, CantR3, CantT, Fecha, id_usuario, Flag, FlagT1, FlagT2, FlagT3, embarque, oc, factura) VALUES (" +
                                        "'" + Class1.NumeroInventario + "','" + Class1.vgIDProducto + "','" + Codigo + "','" + Descri + "','" + Tarima + "','" + Ubica + "','" + Estatus + "','0','0','0','0','0','" + vdate + "','" + Class1.vgID_employee + "','0','0','0','0','IF','IF','IF')";
                    EjecutarQuerySQLWifi(Class1.strSQL);
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
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
        private void ExisteArt(string cb)
        {
            try
            {
                string Sql1;
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    Sql1 = "Select p.id_product, p.price, pl.description, P.UPC from Logistik_product p, Logistik_product_lang pl" +
                    " where p.id_product = pl.id_product" +
                    " and ltrim(p.upc) = '" + cb.Trim() + "'" +
                    " and pl.id_lang = 2";
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    int cont = 0;
                    while (reader.Read())
                    {
                        Class1.vgIDProducto = (int)reader["id_product"];
                        Class1.vbNomProd = (string)reader["description"];
                        //infop.desc  = vbNomProd
                        Class1.vbUPCProducto = (string)reader["UPC"];
                        cont = cont + 1;
                    }
                    if (cont < 1)
                    {
                        Sql1 = "Select p.id_product, p.price, pl.description, P.UPC " +
                       "from Logistik_product p, Logistik_product_lang pl, Logistik_product_Claves_Alter pca" +
                       " where p.id_product = pl.id_product" +
                       " and pca.CVE_ART = p.upc" +
                       " and pca.CVE_ALTER = '" + cb.Trim() + "'" +
                       " and pl.id_lang = 2";
                        SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                        SqlDataReader readerInside;
                        readerInside = sqlcmd2.ExecuteReader();
                        while (readerInside.Read())
                        {
                            Class1.vgIDProducto = (int)readerInside["id_product"];
                            Class1.vbNomProd = (string)readerInside["descripcion"];
                            //infop.desc  = vbNomProd
                            Class1.vbUPCProducto = (string)readerInside["UPC"];
                        }
                        readerInside.Close();
                    }
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                string msg = ex.ToString();
                Toast.MakeText(this, msg, Android.Widget.ToastLength.Short).Show();
            }


        }
        //' Verifica el codigo y extrae descripcion. 
        private void verifyTarimaIF(string scan, string tarima)
        {
            //infop.CodFrog = scan;

        }
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewInvDet.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Dim i As Integer
            //Dim myStream As Stream = Nothing
            //i = DGVClientes.CurrentCell.RowNumber
            //txtTarima.Text = DGVClientes.Item(i, 2).ToString.Trim
            //CaducaIF = txtTarima.Text
            //vIFEmbarque = DGVClientes.Item(i, 5).ToString.Trim
            //vIFOC = DGVClientes.Item(i, 6).ToString.Trim
            //vIFFactura = DGVClientes.Item(i, 7).ToString.Trim
            //FlagSelecciono = True
            //txtCodigo.Text = DGVClientes.Item(i, 1).ToString.Trim + Chr(10) + Chr(13)
            ///////////////////////////////pendiente los datos a extraer///////////////////////////////////////////////////////
            string select = adapter.GetItem(position).ToString();
            //Toast.MakeText(this, select, Android.Widget.ToastLength.Short).Show();
            string lastWord = select.Substring(4, 20);
            //Toast.MakeText(this, lastWord, Android.Widget.ToastLength.Short).Show();
            Class1.Pedido = lastWord;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        }
    }
}