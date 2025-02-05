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
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

using BilddenLogistik.EFWorkBD;
using BilddenLogistik.MainActivities;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityInventarioDetCajas")]
    public class ActivityInventarioDetCajas : Activity
    {
        private EditText editTextInvCodigo, editTextInvCant,editTextInvCajas, editTextInvPzCaja, editTextInvSueltas, editTextInvDefect;
        private TextView textViewInvTarimaRes, textViewDescripcion;
        private ImageButton imgbtnRegresarInv, imgbtnCancelarInv, imgbtnGuardarInv;
        private double sumatoria;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_inventario_cajas);
            editTextInvCodigo = FindViewById<EditText>(Resource.Id.editTextInvCodigo);
            editTextInvCant = FindViewById<EditText>(Resource.Id.editTextInvCant);
            editTextInvCajas = FindViewById<EditText>(Resource.Id.editTextInvCajas);
            editTextInvPzCaja = FindViewById<EditText>(Resource.Id.editTextInvPzCaja);
            editTextInvSueltas = FindViewById<EditText>(Resource.Id.editTextInvSueltas);
            editTextInvDefect = FindViewById<EditText>(Resource.Id.editTextInvDefect);
            textViewDescripcion = FindViewById<TextView>(Resource.Id.textViewDescripcion);
            textViewInvTarimaRes = FindViewById<TextView>(Resource.Id.textViewInvTarimaRes);
            imgbtnRegresarInv = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarInv);
            imgbtnCancelarInv = FindViewById<ImageButton>(Resource.Id.imgbtnCancelarInv);
            imgbtnGuardarInv = FindViewById<ImageButton>(Resource.Id.imgbtnGuardarInv);
            CargarLayout();
            editTextInvCajas.KeyPress += editTextInvCajas_KeyPress;
            editTextInvCajas.EditorAction += HandleEditorActionCajas;
            editTextInvPzCaja.KeyPress += editTextInvPzCaja_KeyPress;
            editTextInvPzCaja.EditorAction += HandleEditorActionPZCaja;
            editTextInvSueltas.KeyPress += editTextInvSueltas_KeyPress;
            editTextInvSueltas.EditorAction += HandleEditorActionSueltas;
            imgbtnRegresarInv.Click += delegate
            {
                StartActivity((typeof(ActivityInventario)));
                Finish();
            };
            imgbtnCancelarInv.Click += delegate
            {
                StartActivity((typeof(ActivityInventario)));
                Finish();
            };
            imgbtnGuardarInv.Click += delegate
            {
                guadardatos();
            };
        }
        private void editTextInvCajas_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                FuncionCajas();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void FuncionCajas()
        {
            editTextInvPzCaja.RequestFocus();
        }

        private void editTextInvPzCaja_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                FuncionPZCaja();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void FuncionPZCaja()
        {
            editTextInvSueltas.RequestFocus();
        }

        private void editTextInvSueltas_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                FuncionSultas();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void FuncionSultas()
        {
            editTextInvDefect.RequestFocus();
        }

        private void HandleEditorActionCajas(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                FuncionCajas();
                e.Handled = true;
            }
        }
        private void HandleEditorActionSueltas(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                FuncionSultas();
                e.Handled = true;
            }
        }
        private void HandleEditorActionPZCaja(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                FuncionPZCaja();
                e.Handled = true;
            }
        }
        private void CargarLayout()
        {

            if (Class1.FlagDetalleInvF == true)
            {
                Class1.FlagDetalleInvF = false;

                imgbtnCancelarInv.Visibility = ViewStates.Visible;
                imgbtnGuardarInv.Visibility = ViewStates.Visible;
                editTextInvCajas.Text = "";
                editTextInvPzCaja.Text = "";
                editTextInvSueltas.Text = "";
                editTextInvDefect.Text = "";

                editTextInvCant.Text = "";
                textViewInvTarimaRes.Text = "";
                textViewDescripcion.Text = "";
                editTextInvCant.Enabled = false;

                editTextInvCodigo.Text = Class1.CodTarimaIF;
                BuscarDescripcion(Class1.CodTarimaIF);
                editTextInvCodigo.Enabled = false;
                editTextInvCajas.RequestFocus();

                if (Class1.FlagInvFisicoInic)
                {
                    //Button1_Click(Senders, e);

                }

            }
        }

        private void BuscarDescripcion(string cod)
        {
            try
            {
                string Sql1;
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    Sql1 = "Select p.id_product, p.price, pl.description, P.UPC from Logistik_product p, Logistik_product_lang pl" +
                    " where p.id_product = pl.id_product" +
                    " and ltrim(p.upc) = '" + cod.Trim() + "'" +
                    " and pl.id_lang = 2";
                    con.Open();
                    SqlCommand sqlcmd1 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd1.ExecuteReader();
                    int cont = 0;
                    while (reader.Read())
                    {
                        //Class1.vgIDProducto = (int)reader["id_product"];
                        textViewDescripcion.Text = (string)reader["description"];
                        //infop.desc  = vbNomProd
                        //Class1.vbUPCProducto = (string)reader["UPC"];
                        cont = cont + 1;
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

        private void guadardatos()
        {
            //ARCHIVO GARDARINVENTARIO
            AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
            Win_Save.SetMessage("Esta seguro de modificar la cantidad de esta Código-Ubicacion-Tarima??");
            Win_Save.SetTitle("INVENTARIO");
            bool FlagContinua;
            Win_Save.SetPositiveButton("No", (send, arg) =>
            {
                Win_Save.Dispose();
            });
            Win_Save.SetNegativeButton("Si", (send2, arg2) =>
            {
                FlagContinua = false;
                int vsueltanum = editTextInvSueltas.Text.Length;
                if ( vsueltanum >= 0)
                {
                    if (Convert.ToInt32(vsueltanum) >= 0)
                    {

                        if (editTextInvDefect.Text.Length >= 0)
                        {
                            if (IsNumeric(editTextInvDefect.Text))
                            {

                            }
                            else
                            {
                                Toast.MakeText(this, "Introduzca un numero en Defectuosas!", Android.Widget.ToastLength.Short).Show();
                                editTextInvDefect.Text = "";
                                editTextInvDefect.RequestFocus();
                            }
                        }
                        else
                        {
                            //error
                        }

                        // Verifica que tengamos algo en cajas y piezas
                        if (editTextInvCajas.Text.Length > 0 && editTextInvPzCaja.Text.Length > 0)
                        {
                            if (IsNumeric(editTextInvCajas.Text) && IsNumeric(editTextInvPzCaja.Text))
                            {
                                sumatoria = double.Parse(editTextInvCajas.Text) * double.Parse(editTextInvPzCaja.Text) + double.Parse(editTextInvSueltas.Text);
                                FlagContinua = true;
                            }
                            else
                            {
                                Toast.MakeText(this, "Verifique que tenga cantiades validas en cajas y piezas!", Android.Widget.ToastLength.Short).Show();
                                editTextInvCajas.Text = "";
                                editTextInvPzCaja.Text = "";
                                editTextInvCajas.RequestFocus();
                            }
                        }
                        else
                        {
                            sumatoria = double.Parse(editTextInvSueltas.Text);
                            FlagContinua = true;
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Introduzca un numero en Sueltas!", Android.Widget.ToastLength.Short).Show();

                        editTextInvSueltas.Text = "";
                        editTextInvSueltas.RequestFocus();
                    }
                }
                else
                {
                    if (editTextInvDefect.Text.Length > 0)
                    {
                        if ( IsNumeric(editTextInvDefect.Text))
                        {

                        }
                        else
                        {
                            Toast.MakeText(this, "Introduzca un numero en Defectuosas!", Android.Widget.ToastLength.Short).Show();

                            editTextInvDefect.Text = "";
                            editTextInvDefect.RequestFocus();
                        }
                    }
                    else
                    {

                    }
                    // Verifica que tengamos algo en cajas y piezas
                    if (IsNumeric(editTextInvCajas.Text) && IsNumeric(editTextInvPzCaja.Text))
                    {
                        sumatoria = double.Parse(editTextInvCajas.Text) * double.Parse(editTextInvPzCaja.Text);
                        FlagContinua = true;
                    }
                    else
                    {
                        Toast.MakeText(this, "Verifique que tenga cantiades validas en cajas y piezas!", Android.Widget.ToastLength.Short).Show();

                        editTextInvCajas.Text = "";
                        editTextInvPzCaja.Text = "";
                        editTextInvCajas.RequestFocus();
                    }
                }

                if (FlagContinua)
                {
                    updateTarimaQIF(editTextInvCodigo.Text, textViewInvTarimaRes.Text, sumatoria, editTextInvDefect.Text);

                    editTextInvCant.Enabled = false;
                    editTextInvCajas.Enabled = false;
                    editTextInvPzCaja.Enabled = false;
                    editTextInvSueltas.Enabled = false;

                    // Oculta los botones para Editar la Cantidad
                    //PictureBox2.Visible = false;
                    //Label2.Visible = false;
                    //PictureBox1.Visible = false;
                    //Label9.Visible = false;
                    //Button1.Visible = true;

                    //Muestra los botones normales para aceptar y borrar
                    //PictureBox3.Visible = true;
                    //Label7.Visible = true;

                    Class1.FlagListaInvF = true;
                    StartActivity((typeof(ActivityInventario)));
                    Finish();
                }


                Win_Save.Dispose();
            });
            Win_Save.Show();
        }
        private bool IsNumeric(string a)
        {
            float output;
            return float.TryParse(a, out output);
        }
        public void updateTarimaQIF(string codigo, string tarima, double CantN, string CantDef)
        {
            try
            {
                int qty;
                string aux;
                //Dim tipoFecha As String = "dd/MM/yyyy"
                //SqlConnection conn = new SqlConnection(ConnectionString);
                //Dim ConnectionString As String = "Data Source=" & IPServidor '& ",1433;Initial Catalog=CuetaraDB;Persist Security Info=True;User ID=sa;Password=PASS;" ';Integrated Security=True"
                using (SqlConnection conn = new SqlConnection(Class1.cnSQL))
                {

                    if (CantDef == "")
                    {
                        CantDef = "0";
                    }

                    //On Error Goto SQLError VBConversions Warning: could not be converted to try/catch - logic too complex
                    // Actualiza el codigo con la cantidad
                    SqlCommand sqlUptateRow = conn.CreateCommand();
                    
                    //conn.Open();
                    //SqlCommand sqlUptateRow = new SqlCommand(Sql1, conn);

                    if (Class1.NumeroToma == 1)
                    {
                        if (Class1.FlagInvFisicoInic == false)
                        {
                            sqlUptateRow.CommandText = "UPDATE vLogistik_InventariosFisicos SET CantR = " + CantN + ", CantRD=" + CantDef + ", FlagT" + Class1.NumeroToma + "='1', id_usuario ='" + Class1.vgID_employee + "' WHERE ClaveInv = '" + Class1.NumeroInventario + "' AND codigo_producto ='" + codigo + "' AND tarima ='" + tarima + "' AND Ubicacion='" + Class1.UbicaSelect + "' and embarque ='" + Class1.vIFEmbarque + "' and oc='" + Class1.vIFOC + "' and factura='" + Class1.vIFFactura + "'";
                        }
                        else
                        {
                            sqlUptateRow.CommandText = "UPDATE vLogistik_InventariosFisicos SET CantR = " + CantN + ", CantRD=" + CantDef + ", FlagT" + Class1.NumeroToma + "='1', id_usuario ='" + Class1.vgID_employee + "' WHERE ClaveInv = '" + Class1.NumeroInventario + "' AND codigo_producto ='" + codigo + "' AND tarima ='" + tarima + "' AND Ubicacion='" + Class1.UbicaSelect + "'";
                        }

                    }
                    else if (Class1.NumeroToma == 2)
                    {
                        if (Class1.FlagInvFisicoInic == false)
                        {
                            sqlUptateRow.CommandText = "UPDATE vLogistik_InventariosFisicos SET CantR2 = " + CantN + ", CamtR2D=" + CantDef + ", FlagT" + Class1.NumeroToma + "='1', id_usuario ='" + Class1.vgID_employee + "' WHERE ClaveInv = '" + Class1.NumeroInventario + "' AND codigo_producto ='" + codigo + "' AND tarima ='" + tarima + "' AND Ubicacion='" + Class1.UbicaSelect + "' and embarque ='" + Class1.vIFEmbarque + "' and oc='" + Class1.vIFOC + "' and factura='" + Class1.vIFFactura + "'";
                        }
                        else
                        {
                            sqlUptateRow.CommandText = "UPDATE vLogistik_InventariosFisicos SET CantR2 = " + CantN + ", CamtR2D=" + CantDef + ", FlagT" + Class1.NumeroToma + "='1', id_usuario ='" + Class1.vgID_employee + "' WHERE ClaveInv = '" + Class1.NumeroInventario + "' AND codigo_producto ='" + codigo + "' AND tarima ='" + tarima + "' AND Ubicacion='" + Class1.UbicaSelect + "'";
                        }

                    }
                    else if (Class1.NumeroToma == 3)
                    {
                        if (Class1.FlagInvFisicoInic == false)
                        {
                            sqlUptateRow.CommandText = "UPDATE vLogistik_InventariosFisicos SET CantR3 = " + CantN + ", CantR3D=" + CantDef + ", FlagT" + Class1.NumeroToma + "='1', id_usuario ='" + Class1.vgID_employee + "' WHERE ClaveInv = '" + Class1.NumeroInventario + "' AND codigo_producto ='" + codigo + "' AND tarima ='" + tarima + "' AND Ubicacion='" + Class1.UbicaSelect + "'";
                        }
                        else
                        {
                            sqlUptateRow.CommandText = "UPDATE vLogistik_InventariosFisicos SET CantR3 = " + CantN + ", CantR3D=" + CantDef + ", FlagT" + Class1.NumeroToma + "='1', id_usuario ='" + Class1.vgID_employee + "' WHERE ClaveInv = '" + Class1.NumeroInventario + "' AND codigo_producto ='" + codigo + "' AND tarima ='" + tarima + "' AND Ubicacion='" + Class1.UbicaSelect + "'";
                        }
                    }
                    sqlUptateRow.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                Toast.MakeText(this, msg, Android.Widget.ToastLength.Short).Show();
            }


        }

    }
}