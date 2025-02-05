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
    [Activity(Label = "Compra Captura")]
    public class ActivityCompraCaptura : Activity
    {
        private TextView textView44;
        private EditText editTextOrdComp;
        private EditText editTextCantCompras, editTextCodCant;
        private EditText editTextDescCompras;
        private EditText editTextCodigoCompras;
        private EditText editTextUltimoRead;
        private Button ButtonRegresarCapComp;
        private Button ButtonGuardarCapComp;
        private Button ButtonCodigoCant;
        private RadioButton RadioButton1, RadioButton2, RadioButton3;
        private Spinner ComboBox_ubicacion;
        private EditText TextBox_no_parte;
        private string cadena;
        private string vfUMedida;
        private int vlcant;
        private int vProd_attrib = 0;
        private Boolean vcambio;
        private int vlid_producto;
        private string vTipoProduct;
        private string vlCodBar;
        private string vAlmacen;
        private string vlCtrlCant;
        private string vlFinPrg;
        private string pUtilizaLote = "N";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutCompraCaptura);
            //UR = FindViewById<EditText>(Resource.Id);
            
            RadioButton1 = FindViewById<RadioButton>(Resource.Id.RadioButton1);
            RadioButton2 = FindViewById<RadioButton>(Resource.Id.RadioButton2);
            RadioButton3 = FindViewById<RadioButton>(Resource.Id.RadioButton3);
            TextBox_no_parte = FindViewById<EditText>(Resource.Id.TextBox_no_parte);
            ComboBox_ubicacion = FindViewById<Spinner>(Resource.Id.ComboBox_ubicacion);
            editTextUltimoRead = FindViewById<EditText>(Resource.Id.editTextUltimoRead);
            textView44 = FindViewById<TextView>(Resource.Id.textView44);
            editTextCantCompras = FindViewById<EditText>(Resource.Id.editTextCantCompras);
            editTextCodCant = FindViewById<EditText>(Resource.Id.editTextCodCant);
            editTextDescCompras = FindViewById<EditText>(Resource.Id.editTextDescCompras);
            editTextCodigoCompras = FindViewById<EditText>(Resource.Id.editTextCodigoCompras);
            editTextOrdComp = FindViewById<EditText>(Resource.Id.editTextOrdComp);
            ButtonGuardarCapComp = FindViewById<Button>(Resource.Id.ButtonGuardarCapComp);
            ButtonRegresarCapComp = FindViewById<Button>(Resource.Id.butRegCapComp);
            ButtonCodigoCant = FindViewById<Button>(Resource.Id.butCodigoCant);
            editTextCodCant.Enabled = false;
            editTextCodCant.Text = Convert.ToString(Class1.vgCantPedida);
            editTextOrdComp.Text = Class1.OC;
            editTextOrdComp.Enabled = false;
            editTextUltimoRead.Text= Class1.vgOrdCodBar;
            editTextUltimoRead.Enabled = false;
            editTextDescCompras.Enabled = false;
            editTextCodigoCompras.Text = Class1.vgOrdCodBar;
            editTextCantCompras.RequestFocus();
            TraerDescripcion();
            editTextCantCompras.KeyPress += editTextCantCompras_KeyPress;
            editTextCodigoCompras.KeyPress += editTextCodigoCompras_KeyPress;
            editTextCantCompras.EditorAction += HandleEditorActionCant;
            editTextCodigoCompras.EditorAction += HandleEditorActionCB;
            ButtonRegresarCapComp.Click += delegate
            {
                StartActivity((typeof(ActivityComprasD)));
                Finish();
            };
            ButtonGuardarCapComp.Click += delegate
            {
                int validError = 0;
                string cod = editTextCodigoCompras.Text;
                string cant = editTextCantCompras.Text;
                string Sql1 = "";
                string vgEnt_Sal = "E";
                string vUnidad = "";
                string pexiste = "1";
                if (RadioButton1.Checked == true)
                { vUnidad = "N"; }
                if (RadioButton2.Checked == true)
                { vUnidad = "Lbs"; }
                if (RadioButton3.Checked == true)
                { vUnidad = "Kgs"; }
                if ((cod.Length) >= 1 && (cant.Length) >= 1)
                {
                    if (editTextCantCompras.Enabled == false)
                    {
                        editTextCantCompras.Text = "1";
                    }
                    //if (vfUMedida.ToUpper().Trim() == "PZ")
                    //{
                        double vlCantx = double.Parse(cant);
                        if (vlCantx > 0)
                        {
                            Boolean vlexiste;
                            vlcant = 0;
                            vlexiste = fnValidarOrdenPedida(Class1.OC, cod);
                            Class1.OC = Class1.OC.Trim();
                            Class1.OC = espacios(Class1.OC, 19);

                            //TRAER LA CANTIDAD PEDIDA
                            
                            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                            {
                                Sql1 = "Select id_supply_order_detail, quantity_expected, quantity_received from Logistik_supply_order_detail " +
                                "where id_supply_order = '" + Class1.OC + "'" +
                                " and upc = '" + (cod.Trim()) + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                                con.Open();
                                SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                                SqlDataReader reader;
                                reader = sqlcmd1.ExecuteReader();
                                int vOrderDatail = 0;
                                decimal vCantPedida = 0;
                                decimal vCantRecibida = 0;
                                while (reader.Read())
                                {
                                    //vlid_producto = (decimal)reader["id_product"];
                                    //string upc = (string)reader["upc"];
                                    //vfUMedida = (string)reader["umedida"];
                                    //'vOrderDatail = Val(oFila.Item("id_supply_order_detail").ToString)
                                    vOrderDatail = int.Parse(Class1.vgOrdCodID);
                                    vCantPedida = (decimal)reader["quantity_expected"];
                                    vCantRecibida = (decimal)reader["quantity_received"];
                                }
                                reader.Close();
                                vCantRecibida = vCantRecibida + (decimal)vlCantx;
                                //SI LA CANTIDAD RECIBIDA ES MAYOR A LA PEDIDA QUE PONGA UN MENSAJE A EN AL PANTALLA "LA CANTIDAD EXCEDE A LA ORDEN Y NO SE GUARDARA"
                                if (vCantRecibida > vCantPedida)
                                {
                                    //vgNoExiste = "C";
                                    Toast.MakeText(this, "LA CANTIDAD EXCEDE A LA ORDEN Y NO SE GUARDARA", Android.Widget.ToastLength.Short).Show();
                                    vlCtrlCant = "N";
                                    validError = 1;
                                }
                                else
                                {
                                    vlCtrlCant = "S";
                                    //'-----------------------------------------------------
                                    string vUbicacion;
                                    if (TextBox_no_parte.Text == "") {TextBox_no_parte.Text = "N";}
                                    //'if (TextBox_serie.Text == "") {TextBox_serie.Text = "N";}
                                    //'if (TextBox1_lote.Text == "") {TextBox1_lote.Text = "N";}
                                    //'if (TextBox_caducidad.Text == "") {TextBox_caducidad.Text = "N";}
                                    //'if (TextBox_pedimento.Text == "") {TextBox_pedimento.Text = "N";}
                                    //'if (TextBox_aduana.Text == "") {TextBox_aduana.Text = "N";}
                                    //if (ComboBox_ubicacion.SelectedItem == "")
                                    if (ComboBox_ubicacion.SelectedItem == null)
                                    { vUbicacion = "N"; }
                                    else
                                    { vUbicacion = ComboBox_ubicacion.SelectedItem.ToString(); }
                                    //TextBoxTot_Capturado.Text = Convert.ToDouble(TextBoxTot_Capturado.Text) + vlCantx;
                                    //'agregar datos adicionales
                                    int vAditional = 0;
                                    if ((TextBox_no_parte.Text.Length) > 1)
                                    {
                                        Sql1 = "Select id_aditional from Logistik_aditional_lang where id_lang = '2'" +
                                        " and ltrim(name) = 'Numero de parte'";
                                        con.Open();
                                        SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                                        SqlDataReader reader2;
                                        reader2 = sqlcmd2.ExecuteReader();
                                        while (reader2.Read())
                                        {

                                            //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                                            //vCantPedida = (double)reader2["quantity_expected"];
                                            //vCantRecibida = (double)reader2["quantity_received"];
                                            vAditional = (int)reader2["id_aditional"];
                                            DatosAdicionales(vAditional, TextBox_no_parte.Text, vOrderDatail);
                                        }
                                    }

                                    //if ((ComboBox_ubicacion.SelectedItem.ToString().Length) > 1)
                                    //{
                                    //    Sql1 = "Select id_aditional from Logistik_aditional_lang where id_lang = '2'" +
                                    //           " and ltrim(name) = 'Ubicación'";

                                    //    con.Open();
                                    //    SqlCommand sqlcmd3 = new SqlCommand(Sql1, con);
                                    //    SqlDataReader reader3;
                                    //    reader3 = sqlcmd3.ExecuteReader();
                                    //    while (reader3.Read())
                                    //    {

                                    //        //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                                    //        //vCantPedida = (double)reader3["quantity_expected"];
                                    //        //vCantRecibida = (double)reader3["quantity_received"];
                                    //        vAditional = (int)reader3["id_aditional"];
                                    //        DatosAdicionales(vAditional, ComboBox_ubicacion.SelectedItem.ToString(), vOrderDatail);
                                    //    }
                                    //}
                                    if (RadioButton3.Checked == true || RadioButton2.Checked == true)
                                    {
                                        Sql1 = "Select id_aditional from Logistik_aditional_lang where id_lang = '2'" +
                                               " and ltrim(name) = 'Unidad de medida'";
                                        con.Open();
                                        SqlCommand sqlcmd4 = new SqlCommand(Sql1, con);
                                        SqlDataReader reader4;
                                        reader4 = sqlcmd4.ExecuteReader();
                                        while (reader4.Read())
                                        {
                                            //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                                            //vCantPedida = (double)reader3["quantity_expected"];
                                            //vCantRecibida = (double)reader3["quantity_received"];
                                            vAditional = (int)reader4["id_aditional"];
                                            if (RadioButton2.Checked == true)
                                            {
                                                DatosAdicionales(vAditional, "Lbs", vOrderDatail);
                                            }
                                            if (RadioButton3.Checked == true)
                                            {
                                                DatosAdicionales(vAditional, "Kgs", vOrderDatail);
                                            }
                                        }
                                    }
                                    //'-----------------------------------------------------
                                    //'ACTUALIZAR LA CANTIDAD RECIBIDA EN EL DETALLE DE LA ORDEN
                                    //'verificar que todos los registros del detalle de la orden quantity_expected = quantity_received
                                    Boolean VerificarTodo;
                                    Class1.OC = Class1.OC.Trim();
                                    Class1.OC = espacios(Class1.OC, 19);
                                    Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received + '" + (cant) + "'" +
                                    " where id_supply_order = '" + Class1.OC + "'" +
                                    " and id_product = '" + vlid_producto + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                                    //con.Open();
                                    SqlCommand sqlcmd5 = new SqlCommand(Sql1, con);
                                    sqlcmd5.ExecuteNonQuery();

                                    //'+++++++++++++++++VERIFICAR 
                                    VerificarTodo = fVerificarRegistros(Class1.OC);
                                    //'ACTUALIZAR ESTATUS DE LA ORDEN, ES POR REGISTRO DE LA TABLA DE 
                                    Class1.vgDocRecepcion = Class1.OC;
                                    if (Class1.vgDocRecepcion == "") { Class1.vgDocRecepcion = "0"; }
                                    //'--------------------actualiza cantidad de la tabla
                                    Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received - '" + vlCantx.ToString() + "'" +
                                    " where id_supply_order = '" + Class1.OC + "'" +
                                    " and id_product = '" + vlid_producto.ToString() + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                                    //con.Open();
                                    SqlCommand sqlcmd6 = new SqlCommand(Sql1, con);
                                    sqlcmd6.ExecuteNonQuery();
                                    if (VerificarTodo == false)
                                    {
                                        Sql1 = "insert into Logistik_Order_Temp(" + 
                                       "id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " +
                                       "aduana, Ubicacion, Unidad, tipo, existe) values ('" + Class1.OC + "','" + vlid_producto.ToString() + "','" +
                                       vProd_attrib.ToString() + "','" + vlCantx.ToString() + "','" + TextBox_no_parte.Text + "','" +
                                       "" + "','" + "" + "','" + "" + "','" +
                                       "" + " ',getdate(),'" +
                                       "" + " ','" + vUbicacion + "','" + vUnidad + "','" + vgEnt_Sal + "','" + pexiste + "')";
                                        //con.Open();
                                        SqlCommand sqlcmd7 = new SqlCommand(Sql1, con);
                                        sqlcmd7.ExecuteNonQuery();
                                        Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received + '" + vlCantx.ToString() + "'" +
                                        " where id_supply_order = '" + Class1.OC.ToString() + "'" +
                                        " and id_product = '" + vlid_producto.ToString() + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                                        //con.Open();
                                        SqlCommand sqlcmd8 = new SqlCommand(Sql1, con);
                                        sqlcmd8.ExecuteNonQuery();
                                        //'--------------------actualiza estatus de la tabla
                                        Sql1 = "update Logistik_supply_order set id_supply_order_state = '4'," +
                                        " doc_sig_sae = '" + Class1.vgDocRecepcion.ToString() + "'" +
                                        " where id_supply_order = '" + Class1.OC.Trim() + "'";
                                        //con.Open();
                                        SqlCommand sqlcmd9 = new SqlCommand(Sql1, con);
                                        sqlcmd9.ExecuteNonQuery();
                                        LimpiarDatos();
                                        //'MsgBox("Captura Parcial Registrada")
                                        //'OrdenesDetalles.Show()
                                        //'OrdenesDetalles.Focus()
                                        Class1.vFinOrden = 0;
                                        Class1.vgParcialLotes = "S";
                                    }
                                    else
                                    {
                                        Class1.vFinOrden = 1;
                                        Sql1 = "insert into Logistik_Order_Temp(" +
                                        "id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " +
                                        "aduana, Ubicacion, Unidad, tipo, existe) values ('" + Class1.OC + "','" + vlid_producto.ToString() + "','" +
                                        vProd_attrib.ToString() + "','" + vlCantx.ToString() + "','" + TextBox_no_parte.Text + "','" +
                                        "" + "','" + "" + "','" + "" + "','" + "" + " ',getdate(),'" +
                                        "" + " ','" + vUbicacion + "','" + vUnidad + "','" + vgEnt_Sal + "','" + pexiste + "')";
                                        //con.Open();
                                        SqlCommand sqlcmd10 = new SqlCommand(Sql1, con);
                                        sqlcmd10.ExecuteNonQuery();
                                        //'--------------------actualiza cantidad de la tabla
                                        Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received + '" + vlCantx.ToString() + "'," + 
                                        " Pedimento = '" + Class1.vgPedimento + "'" +
                                        " where id_supply_order = '" + Class1.OC + "'" +
                                        " and id_product = '" + vlid_producto.ToString() + "' AND id_supply_order_detail='" + Class1.vgOrdCodID  + "'";
                                        //con.Open();
                                        SqlCommand sqlcmd11 = new SqlCommand(Sql1, con);
                                        sqlcmd11.ExecuteNonQuery();
                                        RegistrarMovimientosEntrada(editTextCantCompras.Text);

                                        Class1.vFinOrden = 1;
                                        Class1.vgParcialLotes = "N";
                                        StookTemporal();
                                        vlFinPrg = "S";
                                        
                                        if (pUtilizaLote == "N")
                                        {
                                            if (Class1.vgPedimentoArtComp == "S")
                                             {
                                                //'Call Pedimento_Entrada()
                                                 Pedimentos_Compras();
                                             }
                                        }
                                        //'--------------------actualiza estatus de la tabla  ************** Para Uni empresa status debe ser 9, para multi empresa con costeo debe ser 5
                                        Sql1 = "update Logistik_supply_order set id_supply_order_state = '9'," +
                                        " doc_sig_sae = '" + Class1.vgDocRecepcion.ToString() + "'" +
                                        " where id_supply_order = '" + Class1.OC.Trim() + "'";
                                        //con.Open();
                                        SqlCommand sqlcmd12 = new SqlCommand(Sql1, con);
                                        sqlcmd12.ExecuteNonQuery();
                                        LimpiarDatos();
                                        //'MsgBox("Captura Total Registrada")
                                       

                                    }
                                }
                            }
                        
                                
                        }
                        else
                        {
                            Toast.MakeText(this, "Especifique una cantidad entera para este producto", Android.Widget.ToastLength.Short).Show();
                            validError = 1;
                        }
                    //}
                }
                else
                {
                    Toast.MakeText(this, "Especifique la cantidad y codigo, por favor", Android.Widget.ToastLength.Short).Show();
                    validError = 1;
                }
                if(validError==0 && Class1.vFinOrden == 0)
                {
                StartActivity((typeof(ActivityComprasD)));
                Finish();
                }
                if (validError == 0 && Class1.vFinOrden == 1)
                {
                    StartActivity((typeof(Activitymenu)));
                    Finish();
                }

            };
            ButtonCodigoCant.Click += delegate
            {
                //TextView tvPrueba = (TextView)FindViewById(Resource.Id.textView44);
                if (editTextCantCompras.Visibility == ViewStates.Visible)
                {
                    editTextCantCompras.Visibility = ViewStates.Invisible;
                    textView44.Visibility = ViewStates.Invisible;
                    editTextCodigoCompras.RequestFocus();
                    editTextCodigoCompras.SelectAll();
                }
                else
                {
                    editTextCantCompras.Visibility = ViewStates.Visible;
                    textView44.Visibility = ViewStates.Visible; 
                }
            };
        }

        private void GuardarDatos()
        {
            int validError = 0;
            string cod = editTextCodigoCompras.Text;
            string cant = editTextCantCompras.Text;
            string Sql1 = "";
            string vgEnt_Sal = "E";
            string vUnidad = "";
            string pexiste = "1";
            if (RadioButton1.Checked == true)
            { vUnidad = "N"; }
            if (RadioButton2.Checked == true)
            { vUnidad = "Lbs"; }
            if (RadioButton3.Checked == true)
            { vUnidad = "Kgs"; }
            if ((cod.Length) >= 1 && (cant.Length) >= 1)
            {
                if (editTextCantCompras.Enabled == false)
                {
                    editTextCantCompras.Text = "1";
                }
                //if (vfUMedida.ToUpper().Trim() == "PZ")
                //{
                double vlCantx = double.Parse(cant);
                if (vlCantx > 0)
                {
                    Boolean vlexiste;
                    vlcant = 0;
                    vlexiste = fnValidarOrdenPedida(Class1.OC, cod);
                    Class1.OC = Class1.OC.Trim();
                    Class1.OC = espacios(Class1.OC, 19);

                    //TRAER LA CANTIDAD PEDIDA

                    using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                    {
                        Sql1 = "Select id_supply_order_detail, quantity_expected, quantity_received from Logistik_supply_order_detail " +
                        "where id_supply_order = '" + Class1.OC + "'" +
                        " and upc = '" + (cod.Trim()) + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                        con.Open();
                        SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                        SqlDataReader reader;
                        reader = sqlcmd1.ExecuteReader();
                        int vOrderDatail = 0;
                        decimal vCantPedida = 0;
                        decimal vCantRecibida = 0;
                        while (reader.Read())
                        {
                            //vlid_producto = (decimal)reader["id_product"];
                            //string upc = (string)reader["upc"];
                            //vfUMedida = (string)reader["umedida"];
                            //'vOrderDatail = Val(oFila.Item("id_supply_order_detail").ToString)
                            vOrderDatail = int.Parse(Class1.vgOrdCodID);
                            vCantPedida = (decimal)reader["quantity_expected"];
                            vCantRecibida = (decimal)reader["quantity_received"];
                        }
                        reader.Close();
                        vCantRecibida = vCantRecibida + (decimal)vlCantx;
                        //SI LA CANTIDAD RECIBIDA ES MAYOR A LA PEDIDA QUE PONGA UN MENSAJE A EN AL PANTALLA "LA CANTIDAD EXCEDE A LA ORDEN Y NO SE GUARDARA"
                        if (vCantRecibida > vCantPedida)
                        {
                            //vgNoExiste = "C";
                            Toast.MakeText(this, "LA CANTIDAD EXCEDE A LA ORDEN Y NO SE GUARDARA", Android.Widget.ToastLength.Short).Show();
                            vlCtrlCant = "N";
                            validError = 1;
                        }
                        else
                        {
                            vlCtrlCant = "S";
                            //'-----------------------------------------------------
                            string vUbicacion;
                            if (TextBox_no_parte.Text == "") { TextBox_no_parte.Text = "N"; }
                            //'if (TextBox_serie.Text == "") {TextBox_serie.Text = "N";}
                            //'if (TextBox1_lote.Text == "") {TextBox1_lote.Text = "N";}
                            //'if (TextBox_caducidad.Text == "") {TextBox_caducidad.Text = "N";}
                            //'if (TextBox_pedimento.Text == "") {TextBox_pedimento.Text = "N";}
                            //'if (TextBox_aduana.Text == "") {TextBox_aduana.Text = "N";}
                            //if (ComboBox_ubicacion.SelectedItem == "")
                            if (ComboBox_ubicacion.SelectedItem == null)
                            { vUbicacion = "N"; }
                            else
                            { vUbicacion = ComboBox_ubicacion.SelectedItem.ToString(); }
                            //TextBoxTot_Capturado.Text = Convert.ToDouble(TextBoxTot_Capturado.Text) + vlCantx;
                            //'agregar datos adicionales
                            int vAditional = 0;
                            if ((TextBox_no_parte.Text.Length) > 1)
                            {
                                Sql1 = "Select id_aditional from Logistik_aditional_lang where id_lang = '2'" +
                                " and ltrim(name) = 'Numero de parte'";
                                con.Open();
                                SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                                SqlDataReader reader2;
                                reader2 = sqlcmd2.ExecuteReader();
                                while (reader2.Read())
                                {

                                    //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                                    //vCantPedida = (double)reader2["quantity_expected"];
                                    //vCantRecibida = (double)reader2["quantity_received"];
                                    vAditional = (int)reader2["id_aditional"];
                                    DatosAdicionales(vAditional, TextBox_no_parte.Text, vOrderDatail);
                                }
                            }

                            //if ((ComboBox_ubicacion.SelectedItem.ToString().Length) > 1)
                            //{
                            //    Sql1 = "Select id_aditional from Logistik_aditional_lang where id_lang = '2'" +
                            //           " and ltrim(name) = 'Ubicación'";

                            //    con.Open();
                            //    SqlCommand sqlcmd3 = new SqlCommand(Sql1, con);
                            //    SqlDataReader reader3;
                            //    reader3 = sqlcmd3.ExecuteReader();
                            //    while (reader3.Read())
                            //    {

                            //        //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                            //        //vCantPedida = (double)reader3["quantity_expected"];
                            //        //vCantRecibida = (double)reader3["quantity_received"];
                            //        vAditional = (int)reader3["id_aditional"];
                            //        DatosAdicionales(vAditional, ComboBox_ubicacion.SelectedItem.ToString(), vOrderDatail);
                            //    }
                            //}
                            if (RadioButton3.Checked == true || RadioButton2.Checked == true)
                            {
                                Sql1 = "Select id_aditional from Logistik_aditional_lang where id_lang = '2'" +
                                       " and ltrim(name) = 'Unidad de medida'";
                                con.Open();
                                SqlCommand sqlcmd4 = new SqlCommand(Sql1, con);
                                SqlDataReader reader4;
                                reader4 = sqlcmd4.ExecuteReader();
                                while (reader4.Read())
                                {
                                    //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                                    //vCantPedida = (double)reader3["quantity_expected"];
                                    //vCantRecibida = (double)reader3["quantity_received"];
                                    vAditional = (int)reader4["id_aditional"];
                                    if (RadioButton2.Checked == true)
                                    {
                                        DatosAdicionales(vAditional, "Lbs", vOrderDatail);
                                    }
                                    if (RadioButton3.Checked == true)
                                    {
                                        DatosAdicionales(vAditional, "Kgs", vOrderDatail);
                                    }
                                }
                            }
                            //'-----------------------------------------------------
                            //'ACTUALIZAR LA CANTIDAD RECIBIDA EN EL DETALLE DE LA ORDEN
                            //'verificar que todos los registros del detalle de la orden quantity_expected = quantity_received
                            Boolean VerificarTodo;
                            Class1.OC = Class1.OC.Trim();
                            Class1.OC = espacios(Class1.OC, 19);
                            Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received + '" + (cant) + "'" +
                            " where id_supply_order = '" + Class1.OC + "'" +
                            " and id_product = '" + vlid_producto + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                            //con.Open();
                            SqlCommand sqlcmd5 = new SqlCommand(Sql1, con);
                            sqlcmd5.ExecuteNonQuery();

                            //'+++++++++++++++++VERIFICAR 
                            VerificarTodo = fVerificarRegistros(Class1.OC);
                            //'ACTUALIZAR ESTATUS DE LA ORDEN, ES POR REGISTRO DE LA TABLA DE 
                            Class1.vgDocRecepcion = Class1.OC;
                            if (Class1.vgDocRecepcion == "") { Class1.vgDocRecepcion = "0"; }
                            //'--------------------actualiza cantidad de la tabla
                            Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received - '" + vlCantx.ToString() + "'" +
                            " where id_supply_order = '" + Class1.OC + "'" +
                            " and id_product = '" + vlid_producto.ToString() + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                            //con.Open();
                            SqlCommand sqlcmd6 = new SqlCommand(Sql1, con);
                            sqlcmd6.ExecuteNonQuery();
                            if (VerificarTodo == false)
                            {
                                Sql1 = "insert into Logistik_Order_Temp(" +
                               "id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " +
                               "aduana, Ubicacion, Unidad, tipo, existe) values ('" + Class1.OC + "','" + vlid_producto.ToString() + "','" +
                               vProd_attrib.ToString() + "','" + vlCantx.ToString() + "','" + TextBox_no_parte.Text + "','" +
                               "" + "','" + "" + "','" + "" + "','" +
                               "" + " ',getdate(),'" +
                               "" + " ','" + vUbicacion + "','" + vUnidad + "','" + vgEnt_Sal + "','" + pexiste + "')";
                                //con.Open();
                                SqlCommand sqlcmd7 = new SqlCommand(Sql1, con);
                                sqlcmd7.ExecuteNonQuery();
                                Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received + '" + vlCantx.ToString() + "'" +
                                " where id_supply_order = '" + Class1.OC.ToString() + "'" +
                                " and id_product = '" + vlid_producto.ToString() + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                                //con.Open();
                                SqlCommand sqlcmd8 = new SqlCommand(Sql1, con);
                                sqlcmd8.ExecuteNonQuery();
                                //'--------------------actualiza estatus de la tabla
                                Sql1 = "update Logistik_supply_order set id_supply_order_state = '4'," +
                                " doc_sig_sae = '" + Class1.vgDocRecepcion.ToString() + "'" +
                                " where id_supply_order = '" + Class1.OC.Trim() + "'";
                                //con.Open();
                                SqlCommand sqlcmd9 = new SqlCommand(Sql1, con);
                                sqlcmd9.ExecuteNonQuery();
                                LimpiarDatos();
                                //'MsgBox("Captura Parcial Registrada")
                                //'OrdenesDetalles.Show()
                                //'OrdenesDetalles.Focus()
                                Class1.vFinOrden = 0;
                                Class1.vgParcialLotes = "S";
                            }
                            else
                            {
                                Class1.vFinOrden = 1;
                                Sql1 = "insert into Logistik_Order_Temp(" +
                                "id_order, id_product,id_product_attribute, cantidad, num_parte, serie, Lote, Caducidad, Pedimiento, Fecha_Pedemento, " +
                                "aduana, Ubicacion, Unidad, tipo, existe) values ('" + Class1.OC + "','" + vlid_producto.ToString() + "','" +
                                vProd_attrib.ToString() + "','" + vlCantx.ToString() + "','" + TextBox_no_parte.Text + "','" +
                                "" + "','" + "" + "','" + "" + "','" + "" + " ',getdate(),'" +
                                "" + " ','" + vUbicacion + "','" + vUnidad + "','" + vgEnt_Sal + "','" + pexiste + "')";
                                //con.Open();
                                SqlCommand sqlcmd10 = new SqlCommand(Sql1, con);
                                sqlcmd10.ExecuteNonQuery();
                                //'--------------------actualiza cantidad de la tabla
                                Sql1 = "update Logistik_supply_order_detail set quantity_received = quantity_received + '" + vlCantx.ToString() + "'," +
                                " Pedimento = '" + Class1.vgPedimento + "'" +
                                " where id_supply_order = '" + Class1.OC + "'" +
                                " and id_product = '" + vlid_producto.ToString() + "' AND id_supply_order_detail='" + Class1.vgOrdCodID + "'";
                                //con.Open();
                                SqlCommand sqlcmd11 = new SqlCommand(Sql1, con);
                                sqlcmd11.ExecuteNonQuery();
                                RegistrarMovimientosEntrada(editTextCantCompras.Text);

                                Class1.vFinOrden = 1;
                                Class1.vgParcialLotes = "N";
                                StookTemporal();
                                vlFinPrg = "S";

                                if (pUtilizaLote == "N")
                                {
                                    if (Class1.vgPedimentoArtComp == "S")
                                    {
                                        //'Call Pedimento_Entrada()
                                        Pedimentos_Compras();
                                    }
                                }
                                //'--------------------actualiza estatus de la tabla  ************** Para Uni empresa status debe ser 9, para multi empresa con costeo debe ser 5
                                Sql1 = "update Logistik_supply_order set id_supply_order_state = '9'," +
                                " doc_sig_sae = '" + Class1.vgDocRecepcion.ToString() + "'" +
                                " where id_supply_order = '" + Class1.OC.Trim() + "'";
                                //con.Open();
                                SqlCommand sqlcmd12 = new SqlCommand(Sql1, con);
                                sqlcmd12.ExecuteNonQuery();
                                LimpiarDatos();
                                //'MsgBox("Captura Total Registrada")


                            }
                        }
                    }


                }
                else
                {
                    Toast.MakeText(this, "Especifique una cantidad entera para este producto", Android.Widget.ToastLength.Short).Show();
                    validError = 1;
                }
                //}
            }
            else
            {
                Toast.MakeText(this, "Especifique la cantidad y codigo, por favor", Android.Widget.ToastLength.Short).Show();
                validError = 1;
            }
            if (validError == 0 && Class1.vFinOrden == 0)
            {
                StartActivity((typeof(ActivityComprasD)));
                Finish();
            }
            if (validError == 0 && Class1.vFinOrden == 1)
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            }
        }
        private void Pedimentos_Compras()
        {
            string Sql1 = "Select upc, quantity_received, lote from Logistik_supply_order_detail " +
                     "where id_supply_order = '" + Class1.OC.ToString().Trim() + "'";

        }

        private void StookTemporal()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    string Sql1 = "delete from Logistik_Order_Temp where id_order = '" + Class1.OC.ToString() + "'";
                    con.Open();
                    SqlCommand sqlcmd10 = new SqlCommand(Sql1, con);
                    sqlcmd10.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                string msg = ex.ToString();
                Toast.MakeText(this, msg, Android.Widget.ToastLength.Short).Show();
            }
        }

        private void RegistrarMovimientosEntrada(string pCant)
        {
         
                Class1.vgLoteOC = Class1.OC.Trim();
                Class1.OC = espacios(Class1.OC,19);
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    string Sql1 = "Select id_product, upc, name, quantity_received " +
                   " from Logistik_supply_order_detail " +
                   " where id_supply_order = '" + Class1.OC.ToString() + "'";
                    string vArt;
                    int idArt;
                    string vDesc;
                    decimal vCant;
                    decimal dCant;
                    con.Open();
                    SqlCommand sqlcmd3 = new SqlCommand(Sql1, con);
                    SqlDataReader reader3;
                    reader3 = sqlcmd3.ExecuteReader();
                    while (reader3.Read())
                    {
                        vArt = (string)reader3["upc"];
                        idArt = (int)reader3["id_product"];
                        vDesc = (string)reader3["name"];
                        //'vCant = pCant
                        vCant = (decimal)reader3["quantity_received"];
                        //vOrderDatail = int.Parse(Class1.vgOrdCodID);
                        if (vCant >=1)
                        {
                            Sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                            "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                            "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product) VALUES ('" + Class1.EmbarqueAlmRec + "','" + Class1.vgID_employee + "',getdate()," +
                            "'" + vArt.ToString() + "','" + vCant.ToString() + "',NULL,'0','0','1','0','" + Class1.vgAlmOrdCompra.ToString() + 
                            "',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'" + Class1.OC.ToString() + "','" + Class1.FacturaAlmRec + "'," + idArt.ToString() + ")";
                            //con.Open();
                            SqlCommand sqlcmdKardex10 = new SqlCommand(Sql1, con);
                            sqlcmdKardex10.ExecuteNonQuery();
                        }
                        Boolean verifExist = FnExisteArticuloInv(vArt, Class1.vgAlmOrdCompra.ToString());
                        if (verifExist == false)
                        {
                            Sql1 = "insert into vLogistik_Ubicaciones(" +
                             "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura, id_usuario) values ('" + 
                             Class1.vgAlmOrdCompra.ToString() + "','','','" + idArt.ToString() + "','" + vArt + "','" + vCant.ToString() + "','2','N','" + Class1.EmbarqueAlmRec + "','" + 
                             Class1.OC.ToString().Trim() + "','" + Class1.FacturaAlmRec + "','" + Class1.vgID_employee + "')";
                     
                        }
                        else
                        {
                            dCant = vCant + (decimal)Class1.Inv_cant;
                            Sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() +
                            " where codigo_producto='" + vArt + "'" +
                            " and ubicacion='" + Class1.vgAlmOrdCompra.ToString() + "'";
                        }
                        //con.Open();
                        SqlCommand sqlcmdUbica = new SqlCommand(Sql1, con);
                        sqlcmdUbica.ExecuteNonQuery();

                    }
                    reader3.Close();

                }
            }
            catch(SqlException ex)
            {
                string msg = ex.ToString();
                Toast.MakeText(this, msg, Android.Widget.ToastLength.Short).Show();
            }
                


        }

        private bool FnExisteArticuloInv(string codigo, string ubica)
        {
            try
            {
                //vProd_attrib = 0;
                //TextBox_producto.Text = "";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    string Sql1  = "Select cantidad from vLogistik_Ubicaciones" +
                    " where codigo_producto = '" + codigo + "' and ubicacion='" + ubica + "'";
                    con.Open();
                    SqlCommand sqlcmd3 = new SqlCommand(Sql1, con);
                    SqlDataReader reader3;
                    reader3 = sqlcmd3.ExecuteReader();
                    while (reader3.Read())
                    {
                        Class1.Inv_cant = (decimal)reader3["cantidad"];
                        return true;
                    }
                    return false;
                 }
            }
            catch(SqlException ex)
            {
                string mgs = ex.ToString();
                return false;
            }
            
            
        }
        private bool fVerificarRegistros(string oC)
        {
            try
            {
                Class1.OC = Class1.OC.Trim();
                Class1.OC = espacios(Class1.OC, 19);
                decimal cantP;
                decimal cantR;
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    string Sql1 = "Select quantity_expected, quantity_received from Logistik_supply_order_detail " +
                              " where id_supply_order = '" + Class1.OC.ToString() + "'";
                              //"' and id_product = ' " + vlid_producto.ToString().Trim() + "'";
                    con.Open();
                    SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                    SqlDataReader reader2;
                    reader2 = sqlcmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        cantP = (decimal)reader2["quantity_expected"];
                        cantR = (decimal)reader2["quantity_received"];
                        if (cantP != cantR)
                        {
                            return false;
                        }
                    }

                }
                return true;
            }
            catch(SqlException ex)
            {
                return  false;
            }
        }

        private void DatosAdicionales(int pID_Aditional, string pValor, int pOrdDeatil)
        {
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                string Sql1 = "insert into Logistik_product_aditional(id_order, id_order_detail," +
                "id_product, id_product_attribute, id_aditional, value) values ('" +
                Class1.OC + "','" + pOrdDeatil.ToString() + "','" +
                vlid_producto.ToString() + "','" + vProd_attrib.ToString() + "','" +
                pID_Aditional.ToString() + "','" + pValor + "')";
                con.Open();
                SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                sqlcmd2.ExecuteNonQuery();
            }
        }

        private void LimpiarDatos()
        {
            editTextCodigoCompras.Text = "";
            editTextDescCompras.Text = "";
            editTextCantCompras.Text = "";
            TextBox_no_parte.Text = "";
            //'TextBox_serie.Text = ""
            //'TextBox1_lote.Text = ""
            //'TextBox_caducidad.Text = ""
            //'TextBox_pedimento.Text = ""
            //'TextBox_fecha_pedimento.Text = ""
            //'TextBox_aduana.Text = ""
            //ComboBox_ubicacion.SelectedValue = null;
            ComboBox_ubicacion.Selected = false;
            RadioButton1.Checked = true;
            editTextCodigoCompras.RequestFocus();
        }

        private bool fnValidarOrdenPedida(string pOrden, string pCodigo)
        {
            try
            {
                pOrden = pOrden.Trim();
                pOrden = espacios(pOrden, 20);

                string sql = "Select id_product, id_supply_order_detail from Logistik_supply_order_detail " +
                " where trim(id_supply_order) = '" + pOrden.Trim() + "' and upc = '" + pCodigo.Trim() + "'";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(sql, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        //vlid_producto = (decimal)reader["id_product"];
                        vlid_producto = (int)reader["id_product"];
                        int valor1 =(int)reader["id_supply_order_detail"];
                        Class1.vgOrdCodID = valor1.ToString();
                        return true;
                    }
                    return false;
                }     
            }
            catch( SqlException ex)
            {
                return false;
            }
            
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
        private void HandleEditorActionCB(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId== ImeAction.Send || e.ActionId==ImeAction.Done || e.ActionId==ImeAction.Go)
            {
                VerificarCodigoBarras();
                e.Handled = true;
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

        private void editTextCodigoCompras_KeyPress(object sender, View.KeyEventArgs e)
        {
           if (e.Event.Action == KeyEventActions.Down && e.KeyCode==Keycode.Enter)
            {
                VerificarCodigoBarras();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void VerificarCodigoBarras()
        {
            char lastchar;
            cadena = editTextCodigoCompras.Text;
            if (cadena.Length>0)
            {

            }
        }

        private void editTextCantCompras_KeyPress(object sender, View.KeyEventArgs e)
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

        private void VerificarCantidad()
        {
            char lastchar;
            string valtxtCant = editTextCantCompras.Text;
            if (valtxtCant.Length > 0)
            {
                cadena = valtxtCant;
                if(Int32.Parse(editTextCantCompras.Text)>0)
                {
                    if((double.Parse(editTextCantCompras.Text)>0))
                    {
                        if((editTextCodigoCompras.Text != ""))
                        {
                            if ((editTextCantCompras.Text != ""))
                            {
                                Class1.conta++;

                                //CrearBD.updateInventario(maininfo2, CodCorto, CodSKU, txtVTotalMarbete.Text, editTextCantCompras.Text);
                                editTextUltimoRead.Text = cadena;
                                //editTextCodigoCompras.Text = "";
                                //txtVTotalMarbete.Text = (string)(Int32.Parse(txtVTotalMarbete.Text)+Int32.Parse(editTextCantCompras.Text));
                                //Class1.iRackGlobal = (Class1.iRackGlobal + Int32.Parse(editTextCantCompras.Text));
                                //txtVTotalInv.Text = (string)(Int32.Parse(txtVTotalInv.Text) + Int32.Parse(editTextCantCompras.Text));
                                //Class1.iInvtGlobal = Class1.iInvtGlobal + Int32.Parse(editTextCantCompras.Text);
                                //CrearBD.updateUbica(editTextMarbete.Text, txtVTotalMarbete.Text);
                                
                                //editTextCantCompras.Text = "";
                                //editTextCantCompras.Enabled = false;
                                //editTextCodigoCompras.RequestFocus();
                                GuardarDatos();
                            }
                            else
                            {
                                Toast.MakeText(this, "Indique la cantidad", Android.Widget.ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "Indique el codigo de barras", Android.Widget.ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "escribe la cantidad", Android.Widget.ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "escribe el codigo de barras", Android.Widget.ToastLength.Short).Show();
                }
            }
        }

        private void TraerDescripcion()
        {
            Class1.vgOrdCodBar = Class1.vgOrdCodBar.Trim();
            string sql = "Select p.id_product, p.price, pl.description, p.umedida from Logistik_product p, Logistik_product_lang pl" +
            " where p.id_product = pl.id_product" +
            " and ltrim(p.upc) = '" + Class1.vgOrdCodBar + "'" +
            " and pl.id_lang = 2";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd1 = new SqlCommand(sql, con);
                SqlDataReader reader;
                reader = sqlcmd1.ExecuteReader();
                while (reader.Read())
                {
                    //vlid_producto = (decimal)reader["id_product"];
                    editTextDescCompras.Text = (string)reader["description"];
                    //string medida = (string)reader.IsDBNull["umedida"];
                    //if (medida != null)
                        if (!reader.IsDBNull(reader.GetOrdinal("umedida")))
                    {
                         vfUMedida = (string)reader["umedida"];
                    }
                    else { vfUMedida = ""; }
                    
                }
            }
        }

    }
}