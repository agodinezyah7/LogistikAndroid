using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BilddenLogistik.EFWorkBD;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityAlmacenRecepcion")]
    public class ActivityAlmacenRecepcion : Activity
    {
        private EditText editTextOrdComp;
        private EditText editTextEmbarque;
        private EditText editTextFactura;
        private EditText editTextContenido;
        private Button ButtonRegresarAlmRec;
        private Button ButtonGuardarAlmRec;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutAlmacenRecepcion);
            editTextOrdComp = FindViewById<EditText>(Resource.Id.editTextOrdComp);
            editTextEmbarque = FindViewById<EditText>(Resource.Id.editTextEmbarque);
            editTextFactura = FindViewById<EditText>(Resource.Id.editTextFactura);
            editTextContenido = FindViewById<EditText>(Resource.Id.editTextContenido);
            ButtonRegresarAlmRec = FindViewById<Button>(Resource.Id.ButtonRegresarAlmRec);
            ButtonGuardarAlmRec = FindViewById<Button>(Resource.Id.ButtonGuardarAlmRec);
            editTextOrdComp.Text = Class1.OC;
            editTextOrdComp.Enabled = false;
            editTextEmbarque.RequestFocus();
            ButtonRegresarAlmRec.Click += delegate
            {
                StartActivity((typeof(ActivityComprasE)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonGuardarAlmRec.Click += delegate
            {
                Class1.EmbarqueAlmRec = editTextEmbarque.Text;
                Class1.FacturaAlmRec = editTextFactura.Text;
                Class1.ContenidoAlmRec =editTextContenido.Text;
                //validar si existe
                if (Class1.EmbarqueAlmRec != "" && Class1.FacturaAlmRec != "" && Class1.OC != "")
                {
                    var sqllocal = "select * from vLogistik_Embarques" + 
                    " WHERE embarque = '" + Class1.EmbarqueAlmRec + "'" + 
                    " AND factura = '" + Class1.FacturaAlmRec + "'";
                    using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                    {
                        con.Open();
                        int vacio = 0;
                        using (SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con))
                        {
                            //SqlCommand sqlcmd1 = new SqlCommand(sqllocal, con);
                            SqlDataReader reader;
                            reader = sqlcmd1.ExecuteReader();
                            
                            while (reader.Read())
                            {
                                string vVerifDato = (string)reader["embarque"];
                                vacio = 1;
                                if (vVerifDato != "")
                                {
                                    AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                                    Win_Save.SetMessage("Ya Existe el esa factura para este embaque!, desea continuar con estos datos?");
                                        Win_Save.SetTitle("RECEPCION DE EMBARQUE");
                                        Win_Save.SetPositiveButton("No", (send, arg) =>
                                        {
                                            Win_Save.Dispose();
                                        });
                                        Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                                        {
                                            StartActivity((typeof(ActivityComprasD)));
                                            Finish();
                                            Win_Save.Dispose();
                                        });
                                        Win_Save.Show();
                                    
                                }
                                else
                                {
                                    Class1.OC = Class1.OC.Trim();
                                    var contenido = editTextContenido.Text;
                                    var sql = "insert into vLogistik_Embarques(embarque, id_proveedor, factura, oc, fecha_recepcion, id_usuario_recibio, contenido, estado, enviado_sae) VALUES ('" +
                                    Class1.EmbarqueAlmRec + "','" + Class1.vbID_Supplier + "','" + Class1.FacturaAlmRec + "','" + Class1.OC + "',getdate(),'" + Class1.vgID_employee.ToString() + "','" + contenido + "','1','N')";
                                    SqlCommand sqlcmd2 = new SqlCommand(sql, con);
                                    sqlcmd2.ExecuteNonQuery();
                                    Toast.MakeText(this, "Datos almacenados", Android.Widget.ToastLength.Short).Show();
                                    StartActivity((typeof(ActivityComprasD)));
                                    //this.Close();
                                    //Me.Close()
                                    Finish();
                                }

                            }// reader closed and disposed up here
                            reader.Close(); // <- too easy to forget
                            reader.Dispose(); // <- too easy to forget

                        }// command disposed here
                         if (vacio == 0)
                            {
                                Class1.OC = Class1.OC.Trim();
                                var contenido = editTextContenido.Text;
                                var sql = "insert into vLogistik_Embarques(embarque, id_proveedor, factura, oc, fecha_recepcion, id_usuario_recibio, contenido, estado, enviado_sae) VALUES ('" +
                                Class1.EmbarqueAlmRec + "','" + Class1.vbID_Supplier + "','" + Class1.FacturaAlmRec + "','" + Class1.OC + "',getdate(),'" + Class1.vgID_employee.ToString() + "','" + contenido + "','1','N')";
                                if (con.State == ConnectionState.Closed)
                                {
                                    con.Open();
                                }
                                SqlCommand sqlcmd2 = new SqlCommand(sql, con);
                                //sqlcmd2.Connection.Open();
                                sqlcmd2.ExecuteNonQuery();
                                Toast.MakeText(this, "Datos almacenados", Android.Widget.ToastLength.Short).Show();
                                StartActivity((typeof(ActivityComprasD)));
                                //this.Close();
                                //Me.Close()
                                Finish();
                            }
                        
                    }//connection closed and disposed here

                }
                else
                {
                    var MSG = "Favor de indicar el numero de factura, Embarque y documento de Recepción";
                    Toast.MakeText(this, MSG, Android.Widget.ToastLength.Short).Show();
                }
            };
            // Create your application here
        }
    }
}