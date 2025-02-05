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

using BilddenLogistik.EFWorkBD;

using System.Data;
using System.Data.SqlClient;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivitySalida")]
    public class ActivitySalida : Activity, ListView.IOnItemClickListener
    {

        private ListView listViewSalidas;
        public List<ClassUbicaProdExit> catalogoUbicar;
        public List<ClassUbicaProdExit> listaUbicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private Spinner spinner1S;
        private TextView LabelInfo;
        private EditText editTextUbicaS, editTextTarimaE, editTextCodS, editTextCantS, editTextContenidoS;

        private ImageButton imgbtnRegresarSalida, imgbtnReportSalida, imgbtnGuardarSalida;
        string vlCtrlCant;
        string vAlmacen;
        decimal vlcant;
        int vlid_producto;
        string vlUtilizaNumSer;
        string vlUtilizaLotes;
        int idProd = 0;
        string CantOri;
        string UbicaOri;
        string IDUbica;
        string IDWharehouse;
        string vDescripcion;
        int CantProducto;
        bool verifExist;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layoutSalida);
            LabelInfo = FindViewById<TextView>(Resource.Id.LabelInfo);
            spinner1S = FindViewById<Spinner>(Resource.Id.spinner1S);
            editTextUbicaS = FindViewById<EditText>(Resource.Id.editTextUbicaS);
            editTextTarimaE = FindViewById<EditText>(Resource.Id.editTextTarimaE);
            editTextCodS = FindViewById<EditText>(Resource.Id.editTextCodS);
            editTextCantS = FindViewById<EditText>(Resource.Id.editTextCantS);
            editTextContenidoS = FindViewById<EditText>(Resource.Id.editTextContenidoS);
            listViewSalidas = FindViewById<ListView>(Resource.Id.listViewSalidas);
            imgbtnRegresarSalida = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarSalida);
            imgbtnReportSalida = FindViewById<ImageButton>(Resource.Id.imgbtnReportSalida);
            imgbtnGuardarSalida = FindViewById<ImageButton>(Resource.Id.imgbtnGuardarSalida);
            editTextCantS.KeyPress += editTextCantS_KeyPress;
            editTextCantS.EditorAction += HandleEditorActionCantS;
            editTextCodS.KeyPress += editTextCodS_KeyPress;
            editTextCodS.EditorAction += HandleEditorActionCodBarS;
            editTextUbicaS.KeyPress += editTextUbicaS_KeyPress;
            editTextUbicaS.EditorAction += HandleEditorActionUbicaS;
            editTextTarimaE.KeyPress += editTextTarimaS_KeyPress;
            editTextTarimaE.EditorAction += HandleEditorActionTarimaS;
            listViewSalidas.OnItemClickListener = this;
            Class1.FlagSalidaM = false;
            CargarForma();
            LlenacmbMotivo();
            imgbtnRegresarSalida.Click += delegate
            {
                string sql1;
                //'Borra todo lo de Inv_Temp para limpiar la tabla al salir
                sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                EjecutarQuerySQLWifi(sql1);

                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            imgbtnReportSalida.Click += delegate
            {

                StartActivity((typeof(ActivitySalidaManual)));
                Finish();
            };
            imgbtnGuardarSalida.Click += delegate
            {
                string sql1;
                decimal dCant = 0;
                bool verifExist;
                string CodigoTemp;
                int CantTemp;
                string TarTemp;
                if (CantProducto > 0)
                {
                    string cmbMotivo = spinner1S.SelectedItem.ToString();
                    if (cmbMotivo != "")
                    {
                        if (editTextCantS.Text == "")
                        {
                            if (Class1.FlagLeyoSalidaManual)
                            {
                                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                                {
                                    con.Open();
                                    Class1.strSQL = "SELECT * FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder.ToString() + "')";
                                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                                    SqlDataReader reader = command.ExecuteReader();
                                    CrearBD.vgOrder = 1;
                                    int valor = 0;
                                    while (reader.Read())
                                    {

                                        CodigoTemp = (string)reader["num_parte"];
                                        idProd = (int)reader["id_product"];
                                        CantTemp = (int)reader["cantidad"];
                                        UbicaOri = (string)reader["Ubicacion"];
                                        TarTemp = (string)reader["Tarima"];
                                        Class1.vEmbarque = (string)reader["Embarque"];
                                        Class1.vOC = (string)reader["oc"];
                                        Class1.vFactura = (string)reader["factura"];
                                        valor = (int)reader["existe"];
                                        Class1.vEstado = Convert.ToString(valor);

                                        //' Revisa si ya existia ese producto en la tabla de ubicaciones
                                        verifExist = FnExisteArticuloInv(CodigoTemp, UbicaOri, TarTemp, Class1.vEmbarque, Class1.vOC, Class1.vFactura);
                                        //' Descuenta lo que movió de la ubicacio original
                                        dCant = Convert.ToDecimal(Class1.Inv_cant) - Convert.ToDecimal(CantTemp);
                                        if (UbicaOri != "")
                                        {
                                            if (dCant != 0)
                                                sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() +
                                                 " where codigo_producto='" + CodigoTemp + "'" +
                                                 " and ubicacion='" + UbicaOri + "' and tarima='" + TarTemp.Trim() + "' and estado='" +
                                                 Class1.vEstado.Trim() + "' and embarque='" + Class1.vEmbarque.Trim() + "' and oc='" + Class1.vOC.Trim() +
                                                 "' and factura='" + Class1.vFactura.Trim() + "'";
                                            else
                                                sql1 = "DELETE FROM vLogistik_Ubicaciones WHERE codigo_producto='" + CodigoTemp + "'" +
                                                " and ubicacion='" + UbicaOri + "' and tarima='" + TarTemp.Trim() + "' and estado='" +
                                                Class1.vEstado.Trim() + "' and embarque='" + Class1.vEmbarque.Trim() + "' and oc='" + Class1.vOC.Trim() +
                                                "' and factura='" + Class1.vFactura.Trim() + "'";
                                            EjecutarQuerySQLWifi(sql1);
                                        }
                                        else
                                            Toast.MakeText(this, "ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...", ToastLength.Short).Show();
                                        RegistrarMovimiento(CodigoTemp, CantTemp, UbicaOri, TarTemp);
                                    }
                                    //' Finalmente borra todo lo de Inv_Temp para limpiar la tabla
                                    sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder.ToString() + "')";
                                    EjecutarQuerySQLWifi(sql1);
                                    LlenarPedido();
                                    Class1.FlagLeyoSalidaManual = false;

                                    editTextUbicaS.Enabled = true;
                                    editTextUbicaS.Text = "";
                                    editTextTarimaE.Text = "";

                                    editTextCodS.Text = "";
                                    editTextCantS.Text = "";
                                    editTextContenidoS.Text = "";
                                    //cmbMotivo.SelectedIndex = -1
                                    editTextCodS.RequestFocus();

                                    //lblAguarde.Visible = false;

                                    StartActivity((typeof(Activitymenu)));
                                    Finish();
                                }
                            }
                            else
                                Toast.MakeText(this, "No ha leido nada!", ToastLength.Short).Show();
                        }
                        else
                            Toast.MakeText(this, "No ha dado Enter para terminar de registrar la cantidad del producto que ya indicó...", ToastLength.Short).Show();
                    }
                    else
                        Toast.MakeText(this, "Indique el motivo de la entrada", ToastLength.Short).Show();
                }
                else
                    Toast.MakeText(this, "No ha capturado nada!", ToastLength.Short).Show();
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
        }

        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewSalidas.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            char[] delimiterChars = { '=' };
            string select = adapter.GetItem(position).ToString();
            string[] words = select.Split(delimiterChars);
            string valores = words[0] + "$" + words[1] + "$" + words[2];
            editTextCodS.Text = words[0];
            //Class1.Pedido = words[1];
            Class1.vEmbarque = words[3];
            Class1.OC = words[4];
            Class1.vFactura = words[5];
            Class1.vEstado = words[6];
            LabelInfo.Text = "E:" + Class1.vEmbarque + "O:" + Class1.OC + "F:" + Class1.vFactura;
            editTextCantS.Enabled = true;
            editTextCantS.RequestFocus();
        }
        private void editTextTarimaS_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionTarimaS(object sender, TextView.EditorActionEventArgs e)
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
            //char lastchar;
            string cadena;
            int numTarima = editTextTarimaE.Length();
            if (numTarima > 0)
            {
                //lastchar = txtTarima.Text(txtTarima.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = editTextTarimaE.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextTarimaE.Text = cadena;
                editTextCodS.RequestFocus();
                //End If
            }


        }

        private void editTextUbicaS_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarUbicar();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void HandleEditorActionUbicaS(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarUbicar();
                e.Handled = true;
            }
        }
        private void VerificarUbicar()
        {
            char lastchar;
            string cadena;
            int numUbica = editTextUbicaS.Length();
            if (numUbica > 0)
            {
                //lastchar = txtUbica.Text(txtUbica.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = editTextUbicaS.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextUbicaS.Text = cadena;

                //' Valida que la ubicacion indicada sea valida
                Verifica_Ubica(editTextUbicaS.Text);
                if (IDWharehouse == "")
                {
                    Toast.MakeText(this, "La ubicacion no existe en SAE, favor de darla de alta primero y sincronizar", ToastLength.Short).Show();
                    editTextUbicaS.Text = "";
                    editTextUbicaS.RequestFocus();
                }
                else
                {
                    editTextCantS.Enabled = false;
                    editTextUbicaS.Enabled = false;
                    editTextTarimaE.RequestFocus();
                }
                //End If
            }


        }

        private void Verifica_Ubica(string ubica)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select id_warehouse from Logistik_warehouse " +
                                    "where name = '" + ubica + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    IDWharehouse = "";
                    int valor = 0;
                    while (reader.Read())
                    {
                        valor = reader.GetInt32(0);
                        IDWharehouse =Convert.ToString(valor); 
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }
        }

        private void editTextCodS_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionCodBarS(object sender, TextView.EditorActionEventArgs e)
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
            int numCodigo = editTextCodS.Length();
            if (numCodigo > 0)
            {
                //lastchar = TxtCodigo.Text(TxtCodigo.TextLength - 1)
                // If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = editTextCodS.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextCodS.Text = cadena;

                Class1.vbUPCProducto = "";
                //' Verifica si fue un codigo UPC para extraer el codigo del producto que es con lo que trabajamos. 
                ExisteArt(editTextCodS.Text);
                if (Class1.vbUPCProducto != "")
                {
                    editTextCodS.Text = Class1.vbUPCProducto;

                    //' Verifica que el codigo del produto este en la lista
                    idProd = GetIDProduct(editTextCodS.Text);

                    if (idProd == 0)
                    {
                        Toast.MakeText(this, "El código que indicó no existe!", ToastLength.Short).Show();
                        editTextCodS.Text = "";
                        editTextCodS.RequestFocus();
                    }
                    else
                        LlenarProveedores();
                }
                else
                {
                    Toast.MakeText(this, "El Código No Existe en la empresa 06!", ToastLength.Short).Show();
                    editTextCodS.Text = "";
                    editTextCodS.RequestFocus();
                }
                //End If
            }


        }
        private void editTextCantS_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionCantS(object sender, TextView.EditorActionEventArgs e)
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
            //lack encoding
            char lastchar;
            string cadena;
            string sql1;
            decimal dCant;

            Class1.TipoUbica = 8; // indica es una salida manual
            UbicaOri = IDWharehouse;
            try
            {
                if (Int32.Parse(editTextCantS.Text) > 0)
                {
                    decimal valor = Int32.Parse(editTextCantS.Text);
                    if (LabelInfo.Text != "")
                    {
                        if (FnExisteArticuloInvTemp(editTextCodS.Text, CrearBD.vgOrder, UbicaOri))
                        {
                            dCant = Class1.Inv_cant + valor;
                            sql1 = "update Logistik_Inventario_Temp set cantidad = " + dCant.ToString() +
                                " Where num_parte='" + editTextCodS.Text + "'" +
                                " and ubicacion='" + UbicaOri + "' and id_order='" + CrearBD.vgOrder.ToString() + "'" + 
                                " and embarque='" + Class1.vEmbarque.Trim() + "' and oc='" + Class1.OC.Trim() +"' and factura='" + Class1.vFactura.Trim() +"'";
                        }
                        else
                        {
                            sql1 = "insert into Logistik_Inventario_Temp(id_order, id_product, id_product_attribute, num_parte, cantidad" +
                                ",ubicacion, tipo, almacen, existe, tarima, embarque, oc, factura) values ('" + CrearBD.vgOrder.ToString() + "','" +
                                idProd.ToString() + "','0','" + editTextCodS.Text +"','"+ editTextCantS.Text.Trim() + "','" + UbicaOri + "','" + 
                                Class1.TipoUbica.ToString() + "','" + UbicaOri + "','" + Class1.vEstado.Trim() + "','" + editTextTarimaE.Text.Trim() + "','" +
                               Class1.vEmbarque.Trim() + "','" + Class1.OC.Trim() + "','" + Class1.vFactura.Trim() + "')";
                        }
                        EjecutarQuerySQLWifi(sql1);
                        Class1.FlagLeyoSalidaManual = true;
                        CantProducto = CantProducto + 1;
                        LabelInfo.Text = "";
                        editTextCantS.Text = "";
                        editTextCantS.Enabled = false;
                        editTextCodS.Text = "";
                        LlenarProveedores();
                        editTextCodS.RequestFocus();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Favor de seleccionar del listado el renglon del embarque, OC o Factura de donde va a sacar el producto", ToastLength.Long).Show();
                }
            }
            catch( Exception ex)
            {
                Toast.MakeText(this, "Error:" + ex.Message.ToString(), ToastLength.Long).Show();
            }

        }

        private Boolean FnExisteArticuloInvTemp(string codigo, int orden, string ubica)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select cantidad from Logistik_Inventario_Temp" +
                    " where num_parte = '" + codigo + "' and id_order=" + orden.ToString() + " and ubicacion='" + ubica + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    Class1.Inv_cant = 0;
                    int valor = 0;
                    while (reader.Read())
                    {
                        valor = reader.GetInt32(0);
                        Class1.Inv_cant = Convert.ToDecimal(valor);
                        return true;
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
        private void CargarForma()
        {
            if (Class1.FlagSalidaM)
            {
                Class1.FlagSalidaM = false;
                editTextUbicaS.Enabled = true;
                editTextUbicaS.Text = "";
                CantProducto = 0;

                editTextTarimaE.Text = "";
                editTextCodS.Text = "";
                editTextCantS.Text = "";
                editTextContenidoS.Text = "";
                editTextUbicaS.RequestFocus();

                SiguienteDocumento();
                LlenarProveedores();
            }
        }
        private void LlenacmbMotivo()
        {
            List<String> labels = getAllLabels();

            // Creating adapter for spinner  
            ArrayAdapter<String> dataAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, labels);

            // Drop down layout style - list view with radio button  
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            // attaching data adapter to spinner  
            spinner1S.Adapter = dataAdapter;
            //spinner1.setAdapter(dataAdapter);
        }
        public List<string> getAllLabels()
        {
            List<string> list = new List<string>();

            // Select All Query
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "SELECT id_motivo_salida, motivo FROM vLogistik_Motivos_Salida";
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
        private void LlenarProveedores()
        {
            try
            {
                mItems = new List<string>();
                listaUbicar = new List<ClassUbicaProdExit>();
                catalogoUbicar = new List<ClassUbicaProdExit>();
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "select lu.codigo_producto, lu.tarima, lw.name, lpl.description, lu.cantidad, lu.embarque, lu.oc, lu.factura, lu.estado " +
                     " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
                     " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product INNER JOIN Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion " +
                     " WHERE (ubicacion='" + IDWharehouse + "' and codigo_producto='" + editTextCodS.Text + "' and lu.tarima='" + editTextTarimaE.Text + "')";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    CrearBD.vgOrder = 1;
                    while (reader.Read())
                    {
                        ClassUbicaProdExit UbicarDetalle = new ClassUbicaProdExit()
                        {
                            Codigo = (string)reader["codigo_producto"],
                            Descrip = (string)reader["description"],
                            Cantidad = (decimal)reader["cantidad"],
                            Embarque = (string)reader["embarque"],
                            OC = (string)reader["oc"],
                            Factura = (string)reader["factura"],
                            Estado = (string)reader["estado"]
                        };
                        listaUbicar.Add(UbicarDetalle);
                    }
                    catalogoUbicar = listaUbicar;
                    adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Codigo + " = " + x.Descrip + " = " + x.Cantidad + " = " + x.Embarque + " = " + x.OC + " = " + x.Factura + " = " + x.Estado).ToArray()));
                    listViewSalidas.Adapter = adapter;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
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
        private Boolean FnExisteArticuloInv(string codigo, string ubica, string Tarima, string Embarque, string OC, string Factura)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select cantidad from vLogistik_Ubicaciones" +
                    " where codigo_producto = '" + codigo + "' and ubicacion='" +
                    ubica + "' and tarima='" + Tarima.Trim() + "'and embarque='" + Embarque.Trim() +
                    "' and oc='" + OC.Trim() + "' and factura ='" + Factura.Trim() + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    Class1.Inv_cant = 0;
                    while (reader.Read())
                    {
                        Class1.Inv_cant = reader.GetDecimal(0);
                        return true;
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
        private void RegistrarMovimiento(string Codigo, decimal qty, string ubica, string tarima)
        {
            string Sql1;
            Class1.vgIDProducto = idProd;
            Class1.vgAlmOrdCompra = "1";
            string Vspinner = spinner1S.SelectedItem.ToString();
            int valor = 0;
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "SELECT id_motivo_salida FROM vLogistik_Motivos_Salida" +
                    " where motivo = '" + Vspinner.Trim() + "'";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                Class1.Inv_cant = 0;
                while (reader.Read())
                {
                    valor = reader.GetInt32(0);
                    Vspinner = Convert.ToString(valor);
                }
            }
            //' Genera el registro de la tabla del Kardex
            Sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
            "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
            "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product) VALUES ('" + Class1.vEmbarque.Trim() +
            "','" + Class1.vgID_employee.ToString() + "',getdate(),'" + Codigo + "','" + qty.ToString() + "',NULL,'0','3','2','" + ubica.Trim() +
            "','1','" + tarima.Trim() + "','',NULL,'" + editTextContenidoS.Text.Trim() + "',NULL,NULL,'" + Vspinner +
            "',NULL,'" + Class1.vOC.Trim() + "','" + Class1.vFactura.Trim() + "','" + Class1.vgIDProducto.ToString() + "')";

            EjecutarQuerySQLWifi(Sql1);
        }
        private void LlenarPedido()
        {
            mItems = new List<string>();
            listaUbicar = new List<ClassUbicaProdExit>();
            catalogoUbicar = new List<ClassUbicaProdExit>();
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "select lit.num_parte, lpl.description, lit.cantidad, lit.id_product " +
                " from Logistik_Inventario_Temp AS lit INNER JOIN Logistik_product AS lp ON lit.num_parte = lp.upc INNER JOIN" +
                " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product WHERE id_order=" + CrearBD.vgOrder + "";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                CrearBD.vgOrder = 1;
                while (reader.Read())
                {
                    ClassUbicaProdExit UbicarDetalle = new ClassUbicaProdExit()
                    {
                        Codigo = (string)reader["num_parte"],
                        Descrip = (string)reader["description"],
                        Cantidad = (decimal)reader["cantidad"]
                    };
                    listaUbicar.Add(UbicarDetalle);
                }
                catalogoUbicar = listaUbicar;
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Codigo + " = " + x.Descrip + " = " + x.Cantidad).ToArray()));
                listViewSalidas.Adapter = adapter;
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
        private int GetIDProduct(string upc)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select id_product from Logistik_Product " +
                          "where upc = '" + upc + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    idProd = 0;
                    while (reader.Read())
                    {
                        idProd = reader.GetInt32(0);
                        return idProd;
                    }
                }
                return idProd;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
                return idProd;
            }
        }
    }
}