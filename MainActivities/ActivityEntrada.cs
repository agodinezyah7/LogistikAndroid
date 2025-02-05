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
    [Activity(Label = "ActivityEntrada")]
    public class ActivityEntrada : Activity
    {
        private ListView listViewEntrada;
        public List<ClassUbicaProd> catalogoUbicar;
        public List<ClassUbicaProd> listaUbicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private Spinner spinner1;
        //private TextView lblAguarde;
        private EditText editTextUbicaE, editTextTarimaE, editTextCodE, editTextCantE, editTextComentario;
        private ImageButton imgbtnRegresarEntrada, imgbtnGuardarEntrada;
        string vlCtrlCant;
        string vAlmacen;
        decimal vlcant;
        int vlid_producto;
        string vlUtilizaNumSer;
        string vlUtilizaLotes;
        string idProd;
        string CantOri;
        string UbicaOri;
        string IDUbica;
        string IDWharehouse;
        string vDescripcion;
        int CantProducto;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_entrada);
            //lblAguarde = FindViewById<TextView>(Resource.Id.lblAguarde);
            spinner1 = FindViewById<Spinner>(Resource.Id.spinner1);
            listViewEntrada = FindViewById<ListView>(Resource.Id.listViewEntrada);
            editTextUbicaE = FindViewById<EditText>(Resource.Id.editTextUbicaE);
            editTextTarimaE = FindViewById<EditText>(Resource.Id.editTextTarimaE);
            editTextCodE = FindViewById<EditText>(Resource.Id.editTextCodE);
            editTextCantE = FindViewById<EditText>(Resource.Id.editTextCantE);
            editTextComentario = FindViewById<EditText>(Resource.Id.editTextComentario);
            imgbtnRegresarEntrada = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarEntrada);
            imgbtnGuardarEntrada = FindViewById<ImageButton>(Resource.Id.imgbtnGuardarEntrada);
            Class1.FlagSalidaM = false;
            LlenacmbMotivo();
            CargarForma();
            editTextCantE.KeyPress += editTextCantE_KeyPress;
            editTextCantE.EditorAction += HandleEditorActionCant;
            editTextCodE.KeyPress += editTextCodE_KeyPress;
            editTextCodE.EditorAction += HandleEditorActionCodBar;
            editTextUbicaE.KeyPress += editTextUbicaE_KeyPress;
            editTextUbicaE.EditorAction += HandleEditorActionUbica;
            editTextTarimaE.KeyPress += editTextTarimaE_KeyPress;
            editTextTarimaE.EditorAction += HandleEditorActionTarima;
            imgbtnRegresarEntrada.Click += delegate
            {
                string sql1;
                //'Borra todo lo de Inv_Temp para limpiar la tabla al salir
                sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                EjecutarQuerySQLWifi(sql1);
                StartActivity((typeof(Activitymenu)));
                Finish();
            };
            imgbtnGuardarEntrada.Click += delegate
            {
                string sql1;
                decimal dCant = 0;
                Boolean verifExist;
                string CodigoTemp;
                decimal CantTemp;
                string Vsprinner = spinner1.SelectedItem.ToString();
                if (Vsprinner != "")
                {
                    if (CantProducto > 0)
                    {
                        string Cantidad = editTextCantE.Text;
                        if (Cantidad == "")
                        {
                            //lblAguarde.Visibility = ViewStates.Visible;
                            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                            {
                                con.Open();
                                Class1.strSQL = "SELECT * FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                                SqlDataReader reader = command.ExecuteReader();
                                int vidProd = 0;
                                while (reader.Read())
                                {
                                    CodigoTemp = (string)reader["num_parte"];
                                    vidProd = (int)reader["id_product"];
                                    idProd = Convert.ToString(vidProd);
                                    CantTemp = Convert.ToDecimal (reader["cantidad"]);

                                    //' Revisa si ya existia ese producto en la tabla de ubicaciones, para insertarlo o agregarlo
                                    verifExist = FnExisteArticuloUbica(CodigoTemp);

                                    if (verifExist == false)
                                        sql1 = "insert into vLogistik_Ubicaciones(ubicacion, tarima, caja, id_product," +
                                        " codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + "1" +
                                        "','" + "" + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp.ToString() + "','2','N','EM','EM','EM')";
                                    else
                                    {
                                        //' Suma lo que encontro a lo nuevo
                                        dCant = Convert.ToDecimal(Class1.Inv_cant) + Convert.ToDecimal(CantTemp);
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() + " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='1' and estado='2' and embarque='EM' and oc='EM' and factura='EM'";

                                    }
                                    EjecutarQuerySQLWifi(sql1);
                                    RegistrarMovimiento(CodigoTemp, CantTemp.ToString());
                                }
                            }
                            //' Finalmente borra todo lo de Inv_Temp para limpiar la tabla
                            sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                            EjecutarQuerySQLWifi(sql1);

                            LlenarPedido();
                            editTextCodE.Text = "";
                            editTextCantE.Text = "";
                            editTextComentario.Text = "";
                            //spinner1.IndexOfChild(-1);
                            //cmbMotivo.SelectedIndex = -1
                            editTextCodE.RequestFocus();

                            //lblAguarde.Visibility = ViewStates.Visible;

                            StartActivity((typeof(Activitymenu)));
                            Finish();
                        }
                        else
                            Toast.MakeText(this, "No ha dado Enter para terminar de registrar la cantidad del producto que ya indicó...", ToastLength.Short).Show();
                    }
                    else
                        Toast.MakeText(this, "No ha leido nada!", ToastLength.Short).Show();
                }
                else
                    Toast.MakeText(this, "Indique el motivo de la entrada", ToastLength.Short).Show();
            };
        }
        private void CargarForma()
        {
            if (Class1.FlagSalidaM)
            {
                Class1.FlagSalidaM = false;
                editTextUbicaE.Enabled = true;
                editTextUbicaE.Text = "";
                SiguienteDocumento();
                LlenarPedido();
                editTextUbicaE.Text = "";
                editTextTarimaE.Text = "";

                this.editTextCodE.Text = "";
                this.editTextCantE.Text = "";
                this.editTextComentario.Text = "";
                editTextCodE.RequestFocus();
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
            spinner1.Adapter = dataAdapter;
            //spinner1.setAdapter(dataAdapter);
        }
        public List<string> getAllLabels()
        {
            List<string> list = new List<string>();

            // Select All Query
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "SELECT id_motivo_entrada, motivo FROM vLogistik_Motivos_Entrada";
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
        private void LlenarPedido()
        {
            try
            {
                mItems = new List<string>();
                listaUbicar = new List<ClassUbicaProd>();
                catalogoUbicar = new List<ClassUbicaProd>();
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    
                    Class1.strSQL = "select lit.num_parte, lpl.description, lit.cantidad, lit.id_product " +
                    " from Logistik_Inventario_Temp AS lit INNER JOIN Logistik_product AS lp ON lit.num_parte = lp.upc INNER JOIN" +
                    " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product" +
                    " WHERE id_order='" + CrearBD.vgOrder + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    CrearBD.vgOrder = 1;
                    CantProducto = 0;
                    while (reader.Read())
                    {
                        //string desc = (string)reader["description"];
                        //desc = desc.Replace("","");
                        ClassUbicaProd UbicarDet = new ClassUbicaProd()
                        {
                            Codigo = Convert.ToString(reader["num_parte"]),
                            Descrip = Convert.ToString(reader["description"]),
                            Cantidad = Convert.ToInt32(reader["cantidad"])
                        };
                        CantProducto = CantProducto + 1;
                        listaUbicar.Add(UbicarDet);
                    }
                    catalogoUbicar = listaUbicar;
                    adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Codigo + " = " + x.Cantidad + " = " + x.Descrip).ToArray()));
                    listViewEntrada.Adapter = adapter;
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
        private Boolean FnExisteArticuloUbica(string codigo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select cantidad from vLogistik_Ubicaciones" +
                        " where codigo_producto = '" + codigo + "' and ubicacion='" + "1" +
                        "' and embarque='EM' and oc='EM' and factura='EM' and Tarima=''";
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
        private void RegistrarMovimiento(string Codigo, string qty)
        {
            string Sql1;
            Class1.vgAlmOrdCompra = "1";
            int valor = 0;
            Get_ProdID(idProd);
            string Vspinner = spinner1.SelectedItem.ToString();
            //BUSCAR ID DEL SPINNER
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                Class1.strSQL = "Select id_motivo_entrada from vLogistik_Motivos_Entrada " +
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
            //DateTime fecha = DateTime.Now;
            //string vFecha = string.Format(vfecha,fecha.ToString("yyyy/MM/dd HH:mm:ss"));

            Sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
            "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
            "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product) VALUES (NULL,'" + Class1.vgID_employee.ToString() + "',getdate()," +
            "'" + Codigo + "','" + qty + "','0','0','0','2','0','" + "1" + "',NULL,'',NULL,'" + editTextComentario.Text.Trim() + "',NULL,'" +
            Vspinner.Trim() + "',NULL,NULL,NULL,NULL,'" + idProd + "')";
            EjecutarQuerySQLWifi(Sql1);
        }

        private void Get_ProdID(string codigo)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select id_product from Logistik_product " +
                          "where upc = '" + codigo + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    Class1.vgIDProducto = 0;
                    while (reader.Read())
                    {
                        Class1.vgIDProducto = reader.GetInt32(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }
        }

        private void editTextTarimaE_KeyPress(object sender, View.KeyEventArgs e)
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
            int numTarima = editTextTarimaE.Length();
            if (numTarima > 0)
            {
                //lastchar = txtTarima.Text(txtTarima.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = editTextTarimaE.Text;
                cadena = cadena.Substring(0, cadena.Length - 2);
                editTextTarimaE.Text = cadena;
                editTextCodE.RequestFocus();
                //End If
            }


        }
        private void editTextUbicaE_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionUbica(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next && e.ActionId == ImeAction.Send && e.ActionId == ImeAction.Done && e.ActionId == ImeAction.Go)
            {
                VerificarUbicar();
                e.Handled = true;
            }
        }
        private void VerificarUbicar()
        {
            //char lastchar;
            string cadena;
            int numUbica = editTextUbicaE.Length();
            if (numUbica > 0)
            {
                //lastchar = txtUbica.Text(txtUbica.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = editTextUbicaE.Text;
                cadena = cadena.Substring(0, cadena.Length - 2);
                editTextUbicaE.Text = cadena;

                //' Valida que la ubicacion indicada sea valida
                Verifica_Ubica(editTextUbicaE.Text);
                if (IDWharehouse == "")
                {
                    Toast.MakeText(this, "La ubicacion no existe en SAE, favor de darla de alta primero y sincronizar", ToastLength.Short).Show();
                    editTextUbicaE.Text = "";
                    editTextUbicaE.RequestFocus();
                }
                else
                {
                    editTextUbicaE.Enabled = false;
                    editTextTarimaE.RequestFocus();
                }
            }
        }
        private void editTextCodE_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarCodigoBarras();
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
                VerificarCodigoBarras();
                e.Handled = true;
            }
        }
        private void VerificarCodigoBarras()
        {
            //char lastchar;
            string cadena;
            //string sql1;
            int numcodigo = editTextCodE.Length();
            if (numcodigo > 0)
            {
                //lastchar = TxtCodigo.Text(TxtCodigo.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = editTextCodE.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2)
                //TxtCodigo.Text = cadena

                Class1.vbUPCProducto = "";
                //' Verifica si fue un codigo UPC para extraer el codigo del producto que es con lo que trabajamos. 
                ExisteArt(cadena);
                if (Class1.vbUPCProducto != "")
                {
                    editTextCodE.Text = Class1.vbUPCProducto;
                    //' Verifica que el codigo del produto este en la lista
                    idProd = GetIDProduct(cadena);

                    if (idProd == "0")
                    {
                        Toast.MakeText(this, "El código que indicó no existe!", ToastLength.Short).Show();
                        editTextCodE.Text = "";
                        editTextCodE.RequestFocus();
                    }
                    else
                        editTextCantE.RequestFocus();
                }
                else
                {
                    Toast.MakeText(this, "El Código No Existe en la empresa 06!", ToastLength.Short).Show();
                    editTextCodE.Text = "";
                    editTextCodE.RequestFocus();
                }
                //End If
            }


        }
        private void editTextCantE_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionCant(object sender, TextView.EditorActionEventArgs e)
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
            //char lastchar;
            int cadena;
            string sql1;
            decimal dcant = 0;

            Class1.TipoUbica = 7;   //' indica es una entrada manual
            UbicaOri = "1";         //'IDWharehouse    ' Coloca el producto en Pasillo = 1
            int numCant = editTextCantE.Length();
            if (numCant > 0)
            {
                //lastchar = txtCant.Text(txtCant.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                cadena = int.Parse(editTextCantE.Text);
                //cadena = cadena.Substring(0, cadena.Length - 2)
                //txtCant.Text = cadena
                //' Valida que sea un numero.
                if (cadena > 0)
                {
                    if (FnExisteArticuloInvTemp(editTextCodE.Text, CrearBD.vgOrder))
                    {
                        dcant = Convert.ToDecimal(Class1.Inv_cant) + Convert.ToDecimal(cadena);
                        sql1 = "update Logistik_Inventario_Temp set cantidad = " + dcant.ToString() +
                              " where num_parte='" + editTextCodE.Text + "'" +
                              " and ubicacion='" + UbicaOri + "' and id_order=" + CrearBD.vgOrder +
                              " and embarque='EM' and oc='EM' and factura='EM'";
                    }
                    else
                        //' Inserta el registro para su posterior procesamiento
                        sql1 = "insert into Logistik_Inventario_Temp(id_order, id_product, id_product_attribute," +
                            " num_parte, cantidad, ubicacion, tipo, almacen, existe, tarima, embarque, oc, factura) values ('" +
                               CrearBD.vgOrder + "','" + idProd + "','0','" + editTextCodE.Text + "','" + editTextCantE.Text +
                               "','" + UbicaOri + "','" + Class1.TipoUbica + "','" + UbicaOri + "','1','" + "" + "','EM','EM','EM')";


                    EjecutarQuerySQLWifi(sql1);
                    LlenarPedido();
                    editTextCodE.Text = "";
                    editTextCantE.Text = "";
                    editTextCodE.RequestFocus();
                }
            }

        }



        private string GetIDProduct(string upc)
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
                    idProd = "0";
                    int vidProd = 0;
                    while (reader.Read())
                    {
                        
                        vidProd = reader.GetInt32(0);
                        idProd = Convert.ToString(vidProd);
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
        private Boolean FnExisteArticuloInvTemp(string codigo, int orden)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select cantidad from Logistik_Inventario_Temp" +
                    " where num_parte = '" + codigo + "' and id_order=" + orden.ToString() + " and ubicacion='" + IDWharehouse + "'";
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
                    while (reader.Read())
                    {
                        IDWharehouse = reader.GetString(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }
        }
    }
}