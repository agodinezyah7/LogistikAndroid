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
using BilddenLogistik.MainActivities;

//using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

using BilddenLogistik.EFWorkBD;
using BilddenLogistik.MainActivities;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityPedidoSalida")]
    public class ActivityPedidoSalida : Activity, ListView.IOnItemClickListener
    {
        private int vProd_attrib = 0;
        private Boolean vcambio;
        decimal vlcant;
        decimal CantOri;
        private int vlid_producto;
        private string vfUMedida;
        private string vTipoProduct;
        private string vlCodBar;
        private string vAlmacen;
        private string vlCtrlCant;

        string vlFinPrg;
        public TextView textViewUbicacionPS, textViewTarimaPS;
        public EditText editTextUbicacionPS, editTextTarimaPS, editTextCantPS, editTextCodigoPS;
        public TextView lblBox_producto, lblTotalC, lblCantSurtida, textViewCodigoPS, textViewPedidoPS;
        public ListView listViewMsgPs;
        public ImageButton imgbtnRegresarPS;
        public List<OrdenPedidoSalida> catalogo;
        public List<OrdenPedidoSalida> listaPedidosD2;
        public List<string> mItems;
        private ArrayAdapter adapter;
        Boolean FlagDatos = false;
        Boolean FlagInic = false;
        Boolean FlagAgrega;
        Boolean FlagContinua;

        string vDescripcion;
        string vgOrderTraspaso;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutPedidosSalida);
            textViewUbicacionPS = FindViewById<TextView>(Resource.Id.textViewUbicacionPS);
            textViewTarimaPS = FindViewById<TextView>(Resource.Id.textViewTarimaPS);
            textViewCodigoPS = FindViewById<TextView>(Resource.Id.textViewCodigoPS);
            textViewPedidoPS = FindViewById<TextView>(Resource.Id.textViewPedidoPS);
            textViewCodigoPS.Text = "Codigo:" + Class1.vgOrdCodBar;
            textViewPedidoPS.Text = "Pedido:" + Class1.Pedido;
            editTextCodigoPS = FindViewById<EditText>(Resource.Id.editTextCodigoPS);
            editTextCodigoPS.Text = Class1.vgOrdCodBar;

            lblBox_producto = FindViewById<TextView>(Resource.Id.lblBox_producto);
            lblTotalC = FindViewById<TextView>(Resource.Id.lblTotalC);
            lblCantSurtida = FindViewById<TextView>(Resource.Id.lblCantSurtida);
            editTextUbicacionPS = FindViewById<EditText>(Resource.Id.editTextUbicacionPS);
            editTextTarimaPS = FindViewById<EditText>(Resource.Id.editTextTarimaPS);
            editTextCantPS = FindViewById<EditText>(Resource.Id.editTextCantPS);
            editTextCantPS.KeyPress += editTextCantPS_KeyPress;
            editTextCantPS.EditorAction += HandleEditorActionCant;
            listViewMsgPs = FindViewById<ListView>(Resource.Id.listViewMsgPs);
            listViewMsgPs.OnItemClickListener = this;
             imgbtnRegresarPS = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarPS);
            imgbtnRegresarPS.Click += delegate
            {
                StartActivity((typeof(ActivityPedidosD1)));
                Finish();
            };
            CargarForma();
            //hidekeyboard();
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
            try
            {
                decimal cant = Convert.ToDecimal(editTextCantPS.Text);
                string codbar = editTextCodigoPS.Text;
                string ubica = editTextUbicacionPS.Text;

                if (cant>0 && codbar != "" && ubica !="")
                {
                    if ((Class1.vgEmployee_Perfil == 8) && (cant == Class1.vgCant_Surtida))
                            FlagContinua = true;
                    else if (Class1.vgEmployee_Perfil == 8 && cant != Class1.vgCant_Surtida)
                    {
                        Toast.MakeText(this,"La Cantidad que indico es diferente a la indicada!",ToastLength.Long).Show();
                        FlagContinua = false;
                        editTextCantPS.Text = "";
                        editTextCantPS.RequestFocus();
                    }
                    else if (cant <= CantOri)
                        FlagContinua = true;
                    else
                    {
                        FlagContinua = false;
                        Toast.MakeText(this, "La Cantidad que indico es mayor que la disponible!", ToastLength.Long).Show();
                        editTextCantPS.Text = "";
                        editTextCantPS.RequestFocus();
                    }

                }
                else if (cant > 0 && codbar != "")
                {
                    if ((Class1.vgEmployee_Perfil == 8) && (cant == Class1.vgCant_Surtida))
                        FlagContinua = true;
                    else if (Class1.vgEmployee_Perfil == 8 && cant != Class1.vgCant_Surtida)
                    {
                        Toast.MakeText(this, "La Cantidad que indico es diferente a la indicada!", ToastLength.Long).Show();
                        FlagContinua = false;
                        editTextCantPS.Text = "";
                        editTextCantPS.RequestFocus();
                    }
                    else if (cant <= CantOri)
                        FlagContinua = true;
                    else
                    {
                        FlagContinua = false;
                        Toast.MakeText(this, "La Cantidad que indico es mayor que la disponible!", ToastLength.Long).Show();
                        editTextCantPS.Text = "";
                        editTextCantPS.RequestFocus();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Especifique el codigo, la ubicacion  y la cantidad por favor" , ToastLength.Long).Show();
                }
                if (FlagContinua)
                {
                    if (editTextCantPS.Visibility == ViewStates.Invisible)
                        editTextCantPS.Text = "1";
                    string sql;
                    // Registra primer movimiento en tabla de Kardex para los reportes de tiempos
                    if (Class1.vgEmployee_Perfil == 8)
                        sql = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                        "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                        "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product, empresa_destino) VALUES ('" + Class1.vgEmbarque + "','" + Class1.vgID_employee + "',getdate()," +
                        "'',NULL,NULL,'0','3','5','" + Class1.vgAlmOrdCompra + "','1',NULL,'',NULL,NULL,NULL,NULL,NULL,'" + Class1.Pedido.ToString() + "',NULL,NULL,NULL,'" + Class1.vgEmpresaSelect + "')";
                    else
                        sql = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                        "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                        "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product, empresa_destino) VALUES ('" + Class1.vgEmbarque + "','" + Class1.vgID_employee + "',getdate()," +
                        "'',NULL,NULL,'0','3','5.5','" + Class1.vgAlmOrdCompra + "','1',NULL,'',NULL,NULL,NULL,NULL,NULL,'" + Class1.Pedido.ToString() + "',NULL,NULL,NULL,'" + Class1.vgEmpresaSelect + "')";
                    EjecutarQuerySQLWifi(sql);

                    Boolean vlexiste=false;
                    //MsgBox("Valida Orden de Cliente")
                    if (Class1.vgEnt_Sal == "S")
                        vlexiste = fnValidarOrdenCliente(Class1.Pedido, codbar);
                    if (vlexiste == true)
                    {
                        Class1.vgOrdCodBar = codbar;
                        decimal vlCant2 = cant;
                        //'--------------------------utiliza numero de serie el producto
                        string vlUtilizaNumSer = "N";
                        string vUtilizaLote = "N";
                        string vUtilizaPedimento = "N";
                        string Sql1 = "Select NumSerie, Lote, Pedimento from Logistik_product " +
                                      "where upc = '" + Class1.vgOrdCodBar.ToString().Trim() + "'";
                        using (SqlConnection ConnWifi = new SqlConnection(Class1.cnSQL))
                        {
                            ConnWifi.Open();
                            SqlCommand sqlcmd1 = new SqlCommand(Sql1, ConnWifi);
                            SqlDataReader reader;
                            reader = sqlcmd1.ExecuteReader();
                            while (reader.Read())
                            {
                                //vlid_producto = (decimal)reader["id_product"];
                                vlUtilizaNumSer = (string)reader["NumSerie"];
                                Class1.vgNumSer = (string)reader["NumSerie"];
                                vUtilizaLote = Convert.ToString(reader["Lote"]);
                                vUtilizaPedimento = (string)reader["Pedimento"];
                            }
                            reader.Close();
                            if (vUtilizaLote == "S" && vUtilizaPedimento == "S")
                            {
                                Class1.vgFaltanLotes = true;
                                Class1.vgArtLotePed = "S";
                            }
                            else
                                Class1.vgArtLotePed = "N";

                            GardarTemp(1, vlUtilizaNumSer, vUtilizaLote, vUtilizaPedimento);
                            //'-----------------------------------------------
                            if (vlCtrlCant == "S" && vlUtilizaNumSer == "S" && vUtilizaLote == "S")
                            {
                                Class1.vgFaltanLotes = true;
                                Class1.vgNumSer2 = 1;
                                if (Class1.vgCtrlNumSer == "OrdenesSalidasDet")
                                {
                                    Class1.vgCtrlNumSer = "Captura1";
                                    if (vUtilizaPedimento == "S")
                                        Class1.vgFaltanPedim = true;
                                }
                                if (Class1.vgCtrlNumSer == "OrdenesDetalles")
                                {
                                    Class1.vgCtrlNumSer = "Captura";
                                    Class1.vgFaltanPedim = false;
                                }
                                vlCtrlCant = "N";
                            }
                            else if (vlCtrlCant == "S" && vlUtilizaNumSer == "S")
                            {
                                Class1.vgFaltanLotes = false;
                                Class1.vgNumSer2 = 1;
                                if (Class1.vgCtrlNumSer == "OrdenesSalidasDet")
                                {
                                    Class1.vgCtrlNumSer = "Captura1";
                                    if (vUtilizaPedimento == "S")
                                        Class1.vgFaltanPedim = true;
                                }
                                if (Class1.vgCtrlNumSer == "OrdenesDetalles")
                                    Class1.vgCtrlNumSer = "Captura";
                                vlCtrlCant = "N";
                            }
                            //else if (vlCtrlCant == "S" && vUtilizaPedimento == "S" && Class1.vgCtrlNumSer == "OrdenesSalidasDet")

                            else if (vlCtrlCant == "S" && vUtilizaLote == "S")
                            {
                                Class1.vgCtrlLotes = "CapturaOC";
                                //'VERIFICAR SI EXISTE PARA NO PEDIR NUEVAMENTE LA VENTANA
                                sql = "select lote from Logistik_Lote_Pedimento" +
                                    " WHERE CVE_ART = '" + Class1.vgOrdCodBar.ToString().Trim() + "'" +
                                    " and CVE_DOC = '" + Class1.Pedido.ToString().Trim() + "'";
                                SqlCommand sqlcmd2 = new SqlCommand(sql, ConnWifi);
                                SqlDataReader reader2;
                                reader2 = sqlcmd2.ExecuteReader();
                                string vREG_LTPD = "0";
                                string vLote = "";
                                while (reader2.Read())
                                {
                                    vLote = (string)reader2["Lote"];
                                }
                                reader2.Close();
                                if (Class1.vgEnt_Sal == "S")
                                    Class1.vgCtrlLotes = "CapturaVta";
                                if (Class1.vgEnt_Sal == "E")
                                    Class1.vgCtrlLotes = "CapturaOC";
                                //'---------------------------------------------------------
                                }
                                else if (vlFinPrg == "S")
                                {
                                    StartActivity((typeof(Activitymenu)));
                                    Finish();
                                    //Principal.Focus();
                                    //Principal.Show();
                                }
                        }
                    }
                    else
                        Toast.MakeText(this, "No existe la orden o el código del producto", ToastLength.Long).Show(); 

                    


                    if (Class1.vgEmployee_Perfil == 8)
                    {
                        if (vlFinPrg == "S")
                        {
                            StartActivity((typeof(Activitymenu)));
                            Finish();
                        }
                        else
                        {
                        StartActivity((typeof(ActivityPedidosD2)));
                        Finish();
                        //OrdenesSalidasDet.Show()
                        //OrdenesSalidasDet.Focus()
                        //OrdenesSalidasDet.TxtCodigo.Text = ""
                        //OrdenesSalidasDet.TxtCodigo.Focus()
                        }

                    }
                    else
                    {
                        if (vlFinPrg == "S")
                        {
                            StartActivity((typeof(Activitymenu)));
                            Finish();
                        }
                        else
                        {
                            StartActivity((typeof(ActivityPedidosD1)));
                            Finish();
                            //OrdenesSalidasSur.Show()
                            //OrdenesSalidasSur.Focus()
                            //OrdenesSalidasSur.TxtCodigo.Text = ""
                            //OrdenesSalidasSur.TxtCodigo.Focus()
                        }
                    }
                }       
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error:" + ex.Message.ToString(),ToastLength.Long).Show();
            }
        }

        private void GardarTemp(int pexiste, string pNumSer, string pUtilizaLote, string pUtilizaPedimento)
        {
            try
            {
                string vUnidad = "N";
                string vUbicacion = "N";
                decimal cant = Convert.ToDecimal(editTextCantPS.Text);
                string codbar = editTextCodigoPS.Text;
                string ubica = editTextUbicacionPS.Text;
                //if (cant > 0 && codbar != "" && ubica != "")
                if (cant > 0 && codbar != "")
                {
                    vlFinPrg = "N";
                    Class1.Pedido = Class1.Pedido.Trim();
                    Class1.Pedido = espacios(Class1.Pedido, 20);
                    //'TRAER LA CANTIDAD PEDIDA
                    int vOrderDatail = 0;
                    decimal vCantPedida = 0;
                    decimal vCantRecibida = 0;
                    string Sql1;
                    if (Class1.vgEnt_Sal == "S" || Class1.vgEnt_Sal == "ED")
                    {
                        if (Class1.vgEmployee_Perfil == 8)
                            Sql1 = "Select id_order_detail, product_quantity, quantity_received from Logistik_order_detail " +
                            " where trim(id_order) = '" + Class1.Pedido.Trim() + "' and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                        else
                            Sql1 = "Select id_order_detail, product_quantity, quantity_temporal from Logistik_order_detail " +
                            " where trim(id_order) = '" + Class1.Pedido.Trim() + "' and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                        using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                        {
                            con.Open();
                            SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                            SqlDataReader reader;
                            reader = sqlcmd1.ExecuteReader();
                            while (reader.Read())
                            {
                                vOrderDatail = (int)reader["id_order_detail"];
                                vCantPedida = (decimal)reader["product_quantity"];
                                if (Class1.vgEmployee_Perfil == 8)
                                    vCantRecibida = (decimal)reader["quantity_received"];
                                else
                                    vCantRecibida = (decimal)reader["quantity_temporal"];
                            }
                            reader.Close();
                            vCantRecibida = vCantRecibida + cant;
                            //'SI LA CANTIDAD RECIBIDA ES MAYOR A LA PEDIDA QUE PONGA UN MENSAJE A EN AL PANTALLA "LA CANTIDAD EXCEDE A LA ORDEN Y NO SE GUARDARA"
                            if (vCantRecibida > vCantPedida)
                            {
                                Class1.vgNoExiste = "C";
                                Toast.MakeText(this,"LA CANTIDAD EXCEDE A LA ORDEN Y NO SE GUARDARA",ToastLength.Long).Show();
                                vlCtrlCant = "N";
                                //Exit Sub
                            }
                            else
                            {
                                vlCtrlCant = "S";
                                vUbicacion  = Class1.UbicaSelect; //' Me.txtUbica.Text
                                lblTotalC.Text = Convert.ToString(Convert.ToDecimal(lblTotalC.Text) + cant);
                                //''agregar datos adicionales
                                int vAditional = 0;
                                //'-----------------------------------------------------
                                //'ACTUALIZAR LA CANTIDAD RECIBIDA EN EL DETALLE DE LA ORDEN
                                //'verificar que todos los registros del detalle de la orden quantity_expected = quantity_received
                                Boolean VerificarTodo;
                                Class1.Pedido = Class1.Pedido.Trim();
                                Class1.Pedido = espacios(Class1.Pedido, 19);
                                if (Class1.vgEnt_Sal == "S" || Class1.vgEnt_Sal == "ED")
                                {
                                    if (Class1.vgEmployee_Perfil == 8)
                                        Sql1 = "update Logistik_order_detail set quantity_received = quantity_received + '" + cant.ToString() + "'" + 
                                        " where id_order = '" + Class1.Pedido.ToString() + "'" + 
                                        " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                    else
                                        Sql1 = "update Logistik_order_detail set quantity_temporal = quantity_temporal + '" + cant.ToString() + "'" + 
                                        " where id_order = '" + Class1.Pedido.ToString() + "'" + 
                                        " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                    EjecutarQuerySQLWifi(Sql1);
                                    //'---------------------VERIFICAR ORDEN DE VENTA
                                    VerificarTodo = fVerificarRegClientes(Class1.Pedido.ToString());
                                    //'ACTUALIZAR ESTATUS DE LA ORDEN, ES POR REGISTRO DE LA TABLA DE 
                                    if (Class1.vgDocPedido == "") 
                                        Class1.vgDocPedido = "0";
                                    //'---------------------------------------------
                                    if (Class1.vgEmployee_Perfil == 8)
                                        Sql1 = "update Logistik_order_detail set quantity_received = quantity_received - '" + cant.ToString() + "'" +
                                        " where id_order = '" + Class1.Pedido.ToString() + "'" +
                                        " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                    else
                                        Sql1 = "update Logistik_order_detail set quantity_temporal = quantity_temporal - '" + cant.ToString() + "'" +
                                        " where id_order = '" + Class1.Pedido.ToString() + "'" +
                                        " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                    EjecutarQuerySQLWifi(Sql1);
                                    if (VerificarTodo == false)
                                    {
                                        //' SOLO guarda los datos cuando esta surtiendo
                                        if (Class1.vgEmployee_Perfil != 8)
                                        {
                                           Sql1 = "insert into Logistik_Order_Temp(" + 
                                          "id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " + 
                                          "aduana, Ubicacion, Unidad, tipo, existe, tarima, product_name, embarque, num_empresa) values ('" + Class1.Pedido.ToString() + "','" + vlid_producto.ToString() + "','" + 
                                          vProd_attrib.ToString() + "','" + cant.ToString() + "','" + codbar + "','" + 
                                          "" + "','" + "" + "','" + "" + " ','" + 
                                          "" + " ',getdate(),'" + 
                                          "" + "','" + vUbicacion + "','" + vUnidad + "','" + 
                                          Class1.vgEnt_Sal + "','" + pexiste + "','" + editTextTarimaPS.Text + "','" + lblBox_producto.Text + "','" + Class1.vgEmbarque + "','" + Class1.vgEmpresaSelect + "')";
                                          EjecutarQuerySQLWifi(Sql1);
                                        }
                                        //'----------------------
                                        //' Ahora guarda los datos en inventario *******_____________
                                        //'verificar si existe
                                        decimal dcant;
                                        if (Class1.vgEmployee_Perfil != 8)
                                        {
                                            Boolean verifExist = FnExisteArticuloInv(codbar, Class1.UbicaSelect, editTextTarimaPS.Text, Class1.vgEmbarque);
                                            if (verifExist == false)
                                                Sql1 = "insert into vLogistik_Ubicaciones(ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque) values ('" + 
                                                Class1.vgAlmOrdCompra.ToString() + "','" + editTextTarimaPS.Text + "','','" + vlid_producto.ToString() + "','" + codbar + "','-" + cant.ToString() + "','2','N','" + Class1.vgEmbarque + "')";
                                            else
                                            {
                                                dcant = Class1.Inv_cant - cant;
                                                if (Class1.UbicaSelect != "")
                                                {
                                                   if (dcant != 0)
                                                       Sql1 = "update vLogistik_Ubicaciones set cantidad = " + dcant.ToString() +
                                                       " where codigo_producto='" + codbar + "'" +
                                                       " and ubicacion='" + Class1.UbicaSelect + "' AND tarima='" + editTextTarimaPS.Text + "' and embarque='" + Class1.vgEmbarque + "'";
                                                    else
                                                        Sql1 = "DELETE FROM vLogistik_Ubicaciones WHERE codigo_producto='" + codbar + "'" + 
                                                       " and ubicacion='" + Class1.UbicaSelect + "' AND tarima='" + editTextTarimaPS.Text + "' and embarque='" + Class1.vgEmbarque + "'";
                                                }
                                                else
                                                    Toast.MakeText(this,"ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...",ToastLength.Long).Show();
                                            }
                                            EjecutarQuerySQLWifi(Sql1);
                                           
                                        }
                                        //'-------------------actualiza tabla
                                        if (Class1.vgEmployee_Perfil == 8)
                                            Sql1 = "update Logistik_order_detail set quantity_received = quantity_received + '" + cant.ToString() + "'" + 
                                            " where id_order = '" + Class1.Pedido.ToString() + "'" + 
                                            " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                        else
                                            Sql1 = "update Logistik_order_detail set quantity_temporal = quantity_temporal + '" + cant.ToString() + "', id_warehouse = '" + Class1.vgAlmRemision + "'" + 
                                            " where id_order = '" + Class1.Pedido.ToString() + "'" + 
                                            " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                        EjecutarQuerySQLWifi(Sql1);
                                        if (Class1.vgEmployee_Perfil != 8)
                                        {
                                            //'--------------actualiza el estatus
                                            Sql1 = "update Logistik_orders set current_state = '4', Tipo_Rem_Fact = '" + Class1.vgTipoOrdSalidas + "'," +
                                            " doc_sig_sae = '" + Class1.vgDocPedido.ToString() + "'" +
                                            " where id_order = '" + Class1.Pedido.ToString() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                            EjecutarQuerySQLWifi(Sql1);
                                        }
                                        LimpiarDatos();
                                        Class1.vFinOrden = 0;
                                    }
                                    else
                                    {
                                        Class1.vFinOrden = 1;
                                        //'si tiene numeros de serie y sincroniza no funciona, porque no pueden pasar los NS
                                        if (pNumSer == "S" || pUtilizaPedimento == "S" || pUtilizaLote == "S" )
                                        {
                                            //'no graba en la tabla temporal
                                            //'no actualiza las cantidades
                                            //'no actualiza el estatus de la orden hasta que capture los NS
                                            //'
                                        }
                                        else
                                        {
                                            //'------------------------guarda en la tabla temporal
                                            //'Sql1 = "insert into Logistik_Order_Temp(" + _
                                            //'"id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " + _
                                            //'"aduana, Ubicacion, Unidad, tipo, existe) values ('" + vgOrder.ToString + "','" + vlid_producto.ToString + "','" + _
                                            //'vProd_attrib.ToString + "','" + Me.TextBox_cantidad.Text + "','" + Me.TextBox_no_parte.Text + "','" + _
                                            //'Me.TextBox_serie.Text + "','" + Me.TextBox1_lote.Text + "','" + Me.TextBox_caducidad.Text + "','" + _
                                            //'Me.TextBox_pedimento.Text + "','" + Format(Date.Now, "yyyy/MM/dd HH:mm:ss") + "','" + _
                                            //'Me.TextBox_aduana.Text + "','" + vUbicacion + "','" + vUnidad + "','" + _
                                            //'vgEnt_Sal + "','" + pexiste + "')"

                                            //' SOLO guarda los datos cuando esta surtiendo
                                            if (Class1.vgEmployee_Perfil != 8)
                                            {
                                              Sql1 = "insert into Logistik_Order_Temp(" + 
                                                "id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " + 
                                                "aduana, Ubicacion, Unidad, tipo, existe,tarima, product_name, embarque, num_empresa) values ('" + Class1.Pedido.ToString() + "','" + vlid_producto.ToString() + "','" + 
                                                vProd_attrib.ToString() + "','" + cant.ToString() + "','" + codbar + "','" +
                                                "" + "','" + "" + "','" + "" + " ','" +
                                                "" + " ',getdate(),'" +
                                                "" + "','" + vUbicacion + "','" + vUnidad + "','" +
                                                Class1.vgEnt_Sal + "','" + pexiste + "','" + editTextTarimaPS.Text + "','" + lblBox_producto.Text + "','" + Class1.vgEmbarque + "','" + Class1.vgEmpresaSelect + "')";
                                                EjecutarQuerySQLWifi(Sql1);
                                            }
                                            //'----------------------
                                            //' Ahora guarda los datos en inventario *******_____________
                                            //'verificar si existe
                                            decimal dcant;
                                            if (Class1.vgEmployee_Perfil != 8)
                                            {
                                                Boolean verifExist = FnExisteArticuloInv(codbar, Class1.UbicaSelect, editTextTarimaPS.Text, Class1.vgEmbarque);
                                                if (verifExist == false)
                                                    Sql1 = "insert into vLogistik_Ubicaciones(ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque) values ('" + 
                                                    Class1.vgAlmOrdCompra.ToString() + "','" + editTextTarimaPS.Text + "','','" + vlid_producto.ToString() + "','" + codbar + "','-" + cant.ToString() + "','2','N','" + Class1.vgEmbarque + "')";
                                                else
                                                {
                                                    dcant = Class1.Inv_cant - cant;
                                                    if (Class1.UbicaSelect != "")
                                                    {
                                                        if (dcant != 0)
                                                           Sql1 = "update vLogistik_Ubicaciones set cantidad = " + dcant.ToString() + 
                                                           " where codigo_producto='" + codbar + "'" + 
                                                           " and ubicacion='" + Class1.UbicaSelect + "' AND tarima='" + editTextTarimaPS.Text + "' and embarque='" + Class1.vgEmbarque + "'";
                                                        else
                                                            Sql1 = "DELETE FROM vLogistik_Ubicaciones WHERE codigo_producto='" + codbar + "'" +
                                                           " and ubicacion='" + Class1.UbicaSelect + "' AND tarima='" + editTextTarimaPS.Text + "' and embarque='" + Class1.vgEmbarque + "'";
                                                    }
                                                    else
                                                        Toast.MakeText(this,"ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...",ToastLength.Long).Show();
                                                }
                                                EjecutarQuerySQLWifi(Sql1);
                                          
                                            }

                                            //'----------------------
                                            //'-------------------actualiza cantidad tabla
                                            if (Class1.vgEmployee_Perfil == 8)
                                                Sql1 = "update Logistik_order_detail set quantity_received = quantity_received + '" + cant.ToString() + "'" +
                                               " where id_order = '" + Class1.Pedido.ToString() + "'" +
                                               " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                            else
                                                Sql1 = "update Logistik_order_detail set quantity_temporal = quantity_temporal + '" + cant.ToString() + "', id_warehouse = '" + Class1.vgAlmRemision + "'" +
                                                " where id_order = '" + Class1.Pedido.ToString() + "'" +
                                                " and product_id = '" + vlid_producto.ToString().Trim() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";

                                            EjecutarQuerySQLWifi(Sql1);
                                            //'------------------actualiza estatus
                                            if (Class1.vgEmployee_Perfil == 8)
                                                Sql1 = "update Logistik_orders set current_state = '5', Tipo_Rem_Fact = '" + Class1.vgTipoOrdSalidas + "'" +
                                                " where id_order = '" + Class1.Pedido.ToString() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                                //'" doc_sig_sae = '" + vgDocPedido.ToString + "'" + _
                                            else
                                                Sql1 = "update Logistik_orders set current_state = '10', Tipo_Rem_Fact = '" + Class1.vgTipoOrdSalidas + "'" + 
                                                " where id_order = '" + Class1.Pedido.ToString() + "' and num_empresa='" + Class1.vgEmpresaSelect + "'";
                                            //'" doc_sig_sae = '" + vgDocPedido.ToString + "'" + _
                                            EjecutarQuerySQLWifi(Sql1);
                                            //'MsgBox("Captura Total Registrada")
                                            vlFinPrg = "S";
                                            //if (vgPedimentoArt == "S")
                                                //'Call Pedimento()
                                           

                                            RegistrarMovimientosSalida();
                                            LimpiarDatos();
                                            //' Solo borra luego de verificar. 
                                            //if (Class1.vgEmployee_Perfil == 8 && GeneroTraspasos == true)
                                                //'Call StookTemporal()
                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {
                    Toast.MakeText(this, "Especifique el codigo, la ubicacion  y la cantidad por favor", ToastLength.Long).Show();
                }

            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message.ToString(), ToastLength.Long).Show();
            }
        }
        private void RegistrarMovimientosSalida()
        {
            Boolean FlagInicial;
            string vAutorizar = "S";
            string vSTATE = "8";
            string oldubica = "";
            FlagInicial = true;
            //'--------------------CREAR MOVIEMNTOS DE LA ORDEN DE COMPRA
            string Sql1;
            if (Class1.vgEmployee_Perfil == 8)
                Sql1 = "Select id_product, num_parte, product_name, SUM(cantidad) as quantity_received, tarima, ubicacion, embarque " +
                " from Logistik_order_temp " +
                " where id_order = '" + Class1.Pedido.ToString() + "' AND num_empresa='" + Class1.vgEmpresaSelect + 
                "' GROUP BY id_product, num_parte, product_name, tarima, ubicacion, embarque order by ubicacion";
            else
                Sql1 = "Select id_product, num_parte, product_name, SUM(cantidad) as quantity_temporal, tarima, ubicacion, embarque " +
                " from Logistik_order_temp " +
                " where id_order = '" + Class1.Pedido.ToString() + "' AND num_empresa='" + Class1.vgEmpresaSelect +
                "' GROUP BY id_product, num_parte, product_name, tarima, ubicacion, embarque";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                string vArt;
                string idArt;
                string vDesc;
                decimal vCant;
                decimal dCant;
                string STarima;
                while (reader.Read())
                {
                    vArt = Convert.ToString(reader["num_parte"]);
                    idArt = Convert.ToString(reader["id_product"]);
                    vDesc = Convert.ToString(reader["product_name"]);
                    if (Class1.vgEmployee_Perfil == 8)
                        vCant = Convert.ToDecimal(reader["quantity_received"]);
                    else
                        vCant = Convert.ToDecimal(reader["quantity_temporal"]);

                    STarima = Convert.ToString(reader["tarima"]);
                    Class1.UbicaSelect = Convert.ToString(reader["ubicacion"]);
                    Class1.vgEmbarque = Convert.ToString(reader["embarque"]);

                    if (vCant >= 1)
                    {
                        if (Class1.vgEmployee_Perfil == 8)
                            Sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                            "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                            "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product,empresa_destino) VALUES ('" + Class1.vgEmbarque + "','" + Class1.vgID_employee.ToString() + "',getdate()," +
                            "'" + vArt.ToString() + "','" + vCant.ToString() + "',NULL,'0','3','5.6','" + Class1.UbicaSelect + "','1',NULL,'" + STarima + 
                            "',NULL,NULL,NULL,NULL,NULL,'" + Class1.Pedido + "',NULL,NULL," + idArt.ToString() + ",'" + Class1.vgEmpresaSelect + "')";
                        else
                            Sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                            "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                            "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product,empresa_destino) VALUES ('" + Class1.vgEmbarque + "','" + Class1.vgID_employee.ToString() + "',getdate()," +
                            "'" + vArt.ToString() + "','" + vCant.ToString() + "',NULL,'0','3','5.1','" + Class1.UbicaSelect + "','" + Class1.UbicaSelect + 
                            "',NULL,'" + STarima + "',NULL,NULL,NULL,NULL,NULL,'" + Class1.Pedido + "',NULL,NULL," + idArt.ToString() + ",'" + Class1.vgEmpresaSelect + "')";
                    
                            EjecutarQuerySQLWifi(Sql1);
                    }



                    Class1.GeneroTraspasos = false;

                    //' Ahora genera el traspaso para mover todos los productos a la ubicacion 1 y ademas descontar lo tomado de la ubicacion correspondiente, para el perfil 8 solamente
                    if (Class1.vgEmployee_Perfil == 8)
                    {
                        SiguienteDocumento();
                        //'Verifica_Ubica(UbicaTemp, "2")
                        //' Insertar el encabezado de la orden de traspaso
                        Sql1 = "INSERT INTO Logistik_Traspasos(id_order," + 
                        "reference, id_shop_group, id_shop, id_carrier, id_lang, id_customer, " + 
                        "id_cart, id_currency, id_address_delivery, id_address_invoice, current_state," + 
                        "secure_key,	payment, conversion_rate, module, recyclable," + 
                        "gift, gift_message, mobile_theme, shipping_number, total_discounts," + 
                        "total_discounts_tax_incl, total_discounts_tax_excl, total_paid, total_paid_tax_incl, total_paid_tax_excl," + 
                        "total_paid_real, total_products, total_products_wt, total_shipping, total_shipping_tax_incl," + 
                        "total_shipping_tax_excl, carrier_tax_rate, total_wrapping, total_wrapping_tax_incl, total_wrapping_tax_excl," + 
                        "round_mode,	invoice_number,	delivery_number, invoice_date, delivery_date," + 
                        "valid, date_add, date_upd, Almacen_destino, Almacen_origen, EnviadaSAE, Id_Employee, Documento, Autorizar,TIPO_MOV, num_pedido, num_empresa) values ('" + vgOrderTraspaso + "','" + 
                        "Ref','1','1','1','2','1'," + 
                        "'1','1','1','1','" + vSTATE + "'," + 
                        "'202cb962ac59075b964b07152d234b70','Pago500','300','Modulo','1'," + 
                        "'1','bueno','1','NumEnv5454','0'," + 
                        "'0','0','100','16','0'," + 
                        "'116','1','100','1','1'," + 
                        "'1','1','1','1','1'," + 
                        "'1','1','1','01/01/2015','01/01/2015'," + 
                        "0,'01/01/2015',getdate(),'" + "1" + "','" + Class1.UbicaSelect + "','U','" + 
                        Class1.vgID_employee.ToString() + "','" + vgOrderTraspaso + "','" + vAutorizar.ToString() + "','58','" + Class1.Pedido + "','" + Class1.vgEmpresaSelect + "')";
                        //' Originalmente estaba IDWharehouse en lugar de UbicaOri
                        EjecutarQuerySQLWifi(Sql1);
                        FlagInicial = false;
                        oldubica = Class1.UbicaSelect;
                        FlagInicial = true;

                        //' Obtiene la descripcion del producto
                        Get_Descri(idArt);

                        //' Inserta el detalle en la tabla de Traspasos
                        Sql1 = "INSERT INTO Logistik_Traspasos_detail(id_order, id_order_invoice, id_warehouse, id_shop, product_id" + 
                        ",product_attribute_id, product_name, product_quantity, quantity_received, product_quantity_in_stock" + 
                        ",product_quantity_refunded, product_quantity_return, product_quantity_reinjected, product_price, reduction_percent " + 
                        ",reduction_amount, reduction_amount_tax_incl, reduction_amount_tax_excl, group_reduction, product_quantity_discount" + 
                        ",product_ean13, product_upc, product_reference, product_supplier_reference, product_weight" + 
                        ",id_tax_rules_group, tax_computation_method, tax_name, tax_rate, ecotax" + 
                        ",ecotax_tax_rate, discount_quantity_applied, download_hash, download_nb, download_deadline " + 
                        ",total_price_tax_incl, total_price_tax_excl, unit_price_tax_incl, unit_price_tax_excl, total_shipping_price_tax_incl " + 
                        ",total_shipping_price_tax_excl, purchase_supplier_price, original_product_price, quantity_input, quantity_output, num_empresa) VALUES ('" + 
                        vgOrderTraspaso + "','1','" + Class1.UbicaSelect + "','1','" + idArt + "','" + 
                        "0','" + vDescripcion + "'," + vCant + ",0,1,1," + 
                        "1,1,5000,0,0," + 
                        "0,0,0,0,''," + 
                        "'" + vArt + "','','',0,0," + 
                        "1,'IVA',1,1,1," + 
                        "1,'',1,'01/01/2015',16," + 
                        "0,0,0,116,0," + 
                        "0,0,0,0,'" + Class1.vgEmpresaSelect + "')";
                        EjecutarQuerySQLWifi(Sql1);
                        Class1.GeneroTraspasos = true;
                    }
                    
                

                    //'' Ahora guarda los datos en inventario *******_____________
                    //''verificar si existe
                    //'If vgEmployee_Perfil <> "8" Then
                    //'    Dim verifExist As Boolean = FnExisteArticuloInv(vArt, UbicaSelect, STarima, vgEmbarque)
                    //'    If verifExist = False Then
                    //'        Sql1 = "insert into vLogistik_Ubicaciones(ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque) values ('" + _
                    //'        vgAlmOrdCompra.ToString + "','" & STarima & "','','" & idArt & "','" & vArt & "','-" & vCant & "','2','N','" & vgEmbarque & "')"
                    //'    Else
                    //'        dCant = Inv_cant - Val(vCant)
                    //'        If dCant <> 0 Then
                    //'            Sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString + _
                    //'           " where codigo_producto='" & vArt & "'" + _
                    //'           " and ubicacion='" + UbicaSelect + "' AND tarima='" & STarima & "' and embarque='" & vgEmbarque & "'"
                    //'        Else
                    //'            Sql1 = "DELETE FROM vLogistik_Ubicaciones WHERE codigo_producto='" & vArt & "'" + _
                    //'           " and ubicacion='" + UbicaSelect + "' AND tarima='" & STarima & "' and embarque='" & vgEmbarque & "'"
                    //'        End If

                    //'    End If
                    //'    If xmlBatch = True Then
                    //'        EjecutarQuerySQL(Sql1)
                    //'    Else
                    //'        EjecutarQuerySQLWifi(Sql1)
                    //'    End If
                    //'End If   
                }
                reader.Close();
            }
        }
        private void SiguienteDocumento()
        {
            try
            {
                string Sql1 = "SELECT * FROM Logistik_Consecutivo_SAE where num_doc=1";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    vgOrderTraspaso = "0";
                    while (reader.Read())
                    {
                        vgOrderTraspaso = Convert.ToString(reader["num_traspaso"]);
                        vgOrderTraspaso = Convert.ToString(Convert.ToInt32(vgOrderTraspaso) + 1);
                    }
                    reader.Close();
                    string sqlt = "UPDATE Logistik_Consecutivo_SAE SET num_traspaso=" + vgOrderTraspaso;
                    EjecutarQuerySQLWifi(sqlt);
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message.ToString(), ToastLength.Long).Show();
            }
        }

        private void Get_Descri(string codigo)
        {
            try
            {

                string Sql1 = "Select * from Logistik_product_lang " +
                              "where id_product = '" + codigo + "'";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        vDescripcion = Convert.ToString(reader["description"]);
                    }
                    reader.Close();
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message.ToString(), ToastLength.Long).Show();
            }
        }
        private void LimpiarDatos()
        {
            editTextCodigoPS.Text = "";
            editTextCantPS.Text = "";
            editTextUbicacionPS.Text = "";
            lblBox_producto.Text = "";
            editTextCodigoPS.RequestFocus();
        }
        private Boolean FnExisteArticuloInv(string codigo, string ubica, string tarima, string embarque)
        {
            try
            {
                vProd_attrib = 0;
                lblBox_producto.Text = "";
                string Sql1 = "Select cantidad from vLogistik_Ubicaciones" +
                " where codigo_producto = '" + codigo + "' and ubicacion='" + ubica + "' and tarima='" + tarima + "' and embarque ='" + embarque + "'";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        Class1.Inv_cant = Convert.ToDecimal(reader["cantidad"]);
                        return true;
                    }
                    reader.Close();
                    Class1.Inv_cant = 0;
                    return false;
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message.ToString(), ToastLength.Long).Show();
                return false;
            }
            
        }
        private Boolean fVerificarRegClientes(string porder)
        {
            try
            {
                string Sql1;
                if (Class1.vgEmployee_Perfil == 8)
                    Sql1 = "Select product_quantity, quantity_received, quantity_temporal from Logistik_order_detail " +
                    "where id_order = '" + Class1.Pedido.ToString() + "' AND num_empresa='" + Class1.vgEmpresaSelect + "'";
                else
                    Sql1 = "Select product_quantity, quantity_temporal from Logistik_order_detail " + 
                    "where id_order = '" + Class1.Pedido.ToString() + "' AND num_empresa='" + Class1.vgEmpresaSelect + "'";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    decimal cantP = 0;
                    decimal cantR = 0;
                    while (reader.Read())
                    {
                        cantP = Convert.ToDecimal(reader["product_quantity"]);
                        if (Class1.vgEmployee_Perfil == 8)
                        {
                            cantP = Convert.ToDecimal(reader["quantity_received"]);
                            cantR = Convert.ToDecimal(reader["quantity_temporal"]);
                        }
                        else
                            cantR = Convert.ToDecimal(reader["quantity_temporal"]);

                        if (cantP != cantR)
                            return false;
                    }
                    reader.Close();
                    return true;
                }


                return false;
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message.ToString(), ToastLength.Long).Show();
                return false;
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
            catch(Exception ex)
            {
                return false;
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

        private void editTextCantPS_KeyPress(object sender, View.KeyEventArgs e)
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

        private void CargarForma()
        {
            lblCantSurtida.Text= Convert.ToString(Class1.CantOriEmbarque);
            decimal diferencia;
            if (Class1.flagControl)
            {
                Class1.flagControl = false;
                if (Class1.vFinOrden == 1)
                    Finish();

                //Label1.Visible = false;
                //lblTotalC.Visible = false;
                lblTotalC.Visibility = ViewStates.Invisible;

                
                //dtOrdenVenta.Clear();
                //dsOrdenVenta.Clear();

                FlagInic = true;
                FlagAgrega = false;
                //regresar
                //PictureBox6.Enabled = true;
                imgbtnRegresarPS.Enabled = true;
                if (Class1.vgEmployee_Perfil == 8)
                {
                    lblCantSurtida.Visibility = ViewStates.Visible;
                    //lblCantSurtida.Text = lblCantSurtida.ToString("0,0.00",Cul)
                    lblCantSurtida.Text = String.Format( "{0:0,0.00}", Class1.vgCant_Surtida);
                    //lblCantSurtida.Text = Format(Class1.vgCant_Surtida, "#,###");
                }
                else
                    lblCantSurtida.Visibility = ViewStates.Invisible;

                diferencia = Class1.CantOriEmbarque - Class1.vgCant_Surtida;
                lblCantSurtida.Text = Convert.ToString(diferencia);
                //lblPedido.Text = String.Format("{0:0,0.00}", diferencia);
                //lblPedido.Text = Format(diferencia, "#,###");

                editTextCodigoPS.Text = Class1.vgOrdCodBar;
                vlCodBar = Class1.vgOrdCodBar;
                AgregarDatos();
                vlid_producto = ExisteArt(vlCodBar);
                lblBox_producto.Text = "";
                editTextTarimaPS.Text = "";
                editTextTarimaPS.Enabled = false;
                
                if (Class1.vgEmployee_Perfil == 8)
                {
                    listViewMsgPs.Enabled = false;
                    textViewUbicacionPS.Visibility = ViewStates.Invisible;
                    editTextUbicacionPS.Enabled = false;
                    editTextUbicacionPS.Visibility = ViewStates.Invisible;
                    editTextUbicacionPS.Text = "";
                    textViewTarimaPS.Visibility = ViewStates.Invisible;
                    editTextTarimaPS.Visibility = ViewStates.Invisible;
                    editTextCantPS.Enabled = true;
                    editTextCantPS.Text = "";
                    editTextCantPS.RequestFocus();
                }
                else
                {
                    listViewMsgPs.Enabled = true;
                    editTextCantPS.Text = "";
                    editTextCantPS.Enabled = false;
                    editTextTarimaPS.Visibility = ViewStates.Visible;
                    textViewTarimaPS.Visibility = ViewStates.Visible;
                    textViewUbicacionPS.Visibility = ViewStates.Visible;
                    editTextUbicacionPS.Enabled = true;
                    editTextUbicacionPS.Visibility = ViewStates.Visible;
                    editTextUbicacionPS.Text = "";
                    editTextUbicacionPS.RequestFocus();
                }

                if (Class1.FlagOKSalidaDet && Class1.vgEmployee_Perfil == 8)
                {
                    Class1.FlagOKSalidaDet = false;
                    listViewMsgPs.Enabled = false;
                    textViewCodigoPS.Text = "Codigo:" + Class1.vgOrdCodBar;
                    editTextCodigoPS.Text = Class1.vgOrdCodBar;
                    editTextCodigoPS.RequestFocus();
                }
                else
                {
                    Class1.FlagOKSalidaDet = false;
                    listViewMsgPs.Enabled = true;
                    textViewCodigoPS.Text = "Codigo:" + Class1.vgOrdCodBar;
                }

            }
            Class1.flagControl = false;
            
            AgregarDatos();
           
        }
        private int ExisteArt(string cb)
        {
            try
            {
                vProd_attrib = 0;
                lblBox_producto.Text = "";
                string sql = "Select p.id_product, p.price, pl.description, P.UMEDIDA from Logistik_product p, Logistik_product_lang pl" +
                " where p.id_product = pl.id_product" +
                " and ltrim(p.upc) = '" + cb.Trim() + "'" +
                " and pl.id_lang = 2";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(sql, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        vlid_producto = (int)reader["id_product"];
                        lblBox_producto.Text = (string)reader["description"];
                        vfUMedida =Convert.ToString( reader["umedida"]);
                        vTipoProduct = "P";
                        return vlid_producto;
                    }
                    reader.Close();
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
                return 0;
            }
        }

        private void AgregarDatos()
        {
            mItems = new List<string>();
            listaPedidosD2 = new List<OrdenPedidoSalida>();
            catalogo = new List<OrdenPedidoSalida>();
            var sqllocal = "";

            sqllocal = "select lu.codigo_producto, lu.tarima, lw.name, lp.descripcion, lu.cantidad, lu.ubicacion, lu.embarque" +
            " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
            "  Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion " +
            " WHERE (lu.codigo_producto = '" + Class1.vgOrdCodBar.Trim() + "') and lw.management_type='v' and lw.minorista='1'";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();

                while (reader.Read())
                {
                    OrdenPedidoSalida pedidosDetalle = new OrdenPedidoSalida()
                    {
                        Cantidad = (decimal)reader["Cantidad"],
                        Ubicacion = (string)reader["name"],
                        Tarima = (string)reader["Tarima"],
                        UbicaN = (string)reader["Ubicacion"],
                        Embarque = (string)reader["Embarque"]
                    };
                    listaPedidosD2.Add(pedidosDetalle);
                }
            }
            catalogo = listaPedidosD2;
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Cantidad + " = " + x.Ubicacion + " = " + x.Tarima + " = " + x.UbicaN + " = " + x.Embarque).ToArray()));
            //adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogo.Select(x => x.Cantidad + " = " + x.Ubicacion + " = " + x.Tarima).ToArray()));
            listViewMsgPs.Adapter = adapter;


        }
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewMsgPs.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
                char[] delimiterChars = { '=' };
                string select = adapter.GetItem(position).ToString();
                string[] words = select.Split(delimiterChars);
                //x.Cantidad + " = " + x.Ubicacion + " = " + x.Tarima + " = " + x.UbicaN + " = " + x.Embarque
                 string valores = words[0] + "$" + words[1] + "$" + words[2] + "$" + words[3] + "$" + words[4];
                CantOri = Convert.ToDecimal( words[0]);
                editTextUbicacionPS.Text = words[1];

                Class1.UbicaSelect = words[3];
                Class1.vgEmbarque = words[4];

                Class1.vgAlmOrdCompra = Class1.UbicaSelect; //Class1.UbicaSelect;
                Class1.vgAlmRemision = Class1.UbicaSelect; // Class1.UbicaSelect;
                editTextTarimaPS.Text = words[2];
                //Class1.TarimaSelect = words[2];
                editTextUbicacionPS.Enabled = false;
                editTextTarimaPS.Enabled = false;
                editTextCantPS.Enabled = true;
                editTextCantPS.RequestFocus();
                showkeyboaard();




        }

        public void showkeyboaard()
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(InputMethodService);
            imm.ToggleSoftInput(Android.Views.InputMethods.ShowFlags.Forced, HideSoftInputFlags.None);

        }
        public void hidekeyboard()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
        }
        private string espacios(string Nro, int Cantidad)
        {
            Nro = Nro.Trim();
            int numero = Nro.Length;
            Cantidad = Cantidad - numero;
            string cuantos = " ";
            int i;
            for (i = 0; i < Cantidad; i++)
            {
                cuantos = cuantos + " ";
            }

            string valor = cuantos + Nro;
            return valor;
        }
    }
}