using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Content;

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
//using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
//using System.Data.SqlServerCe;

//using Microsoft.VisualBasic;
using System.Data.SqlClient;
using BilddenLogistik.EFWorkBD;
using Android;
namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, WindowSoftInputMode = SoftInput.StateAlwaysVisible)]
    public class LoginActivity : AppCompatActivity
    {
        private Button buttonCerrar;
        private Button buttonOK;
        private EditText editTextUsuario, editTextClave;
        private CrearBD conexion1 = new CrearBD();
        private int numinv=0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //permisos para leer y escribir 
            if (Build.VERSION.SdkInt>= Android.OS.BuildVersionCodes.M)
            {
                var permiso = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                RequestPermissions(permiso, 77);
            }
            SetContentView(Resource.Layout.activity_main);
            if (!Directory.Exists(conexion1.DirPath))
            {
                CargarRutas();
                CrearArchivos();
                numinv = 0;
            }
            else
            {
                numinv = 1;
            }
            editTextUsuario = FindViewById<EditText>(Resource.Id.editTextUsuario);
            editTextClave = FindViewById<EditText>(Resource.Id.editTextClave);
            buttonOK = FindViewById<Button>(Resource.Id.ButtonOK);
            buttonCerrar = FindViewById<Button>(Resource.Id.ButtonCerrar);
            editTextUsuario.Text = "7050";
            editTextClave.Text = "7050";
            editTextClave.KeyPress += editTextClave_KeyPress;
            editTextClave.EditorAction += HandleEditorActionClave;
            buttonOK.Click += delegate {
                OK();
            };
            buttonCerrar.Click += delegate
            {
                buttonCerrar.Text = "CERRAR";
                if (Class1.FlagCodForzado == false)
                {
                    //Class1.ShowTaskbar();
                    Finish();
                }
                else
                {
                    Class1.NombreUsuario = Class1.oldNombreUsuario;
                    Class1.ClaveUsuario = Class1.oldClaveUsuario;

                    Class1.bInventarioc = true;
                    Class1.FlagCodForzado = false;

                    //Inventario inventariov = new Inventario();
                    //inventariov.ShowDialog();
                    StartActivity((typeof(LoginActivity)));
                    Finish();
                }
            };
        }

        private void OK()
        {
            buttonOK.Text = "OK";
            if ((editTextUsuario.Length() > 0))
            {
                string usuario = editTextUsuario.Text;
                string clave = editTextClave.Text;
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Toast.MakeText(this, "Se abrió la conexión con el  servidor SQL Server y se seleccionó la base de datos", ToastLength.Short).Show();
                    Class1.strSQL = "SELECT id_usuario,nombre,id_perfil FROM vLogistik_Usuarios WHERE password = '" + clave + "' and usuario = '" + usuario + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //Console.WriteLine(String.Format("{0}", reader[0]));
                        Class1.vgID_employee = reader.GetInt32(0);
                        Class1.vgEmployee_first = reader.GetString(1);
                        Class1.vgEmployee_Perfil = reader.GetInt32(2);
                        //Class1.ClaveUsuario = editTextUsuario.Text;
                        StartActivity((typeof(Activitymenu)));
                        //this.Close();
                        //Me.Close()
                        Finish();

                    }
                    con.Close();
                    Toast.MakeText(this, "Se cerró la conexión.", ToastLength.Short).Show();

                }

                //if (conexion1.ValidarUsuario(usuario, clave) == "S")
                //{
                //            Class1.ClaveUsuario = editTextUsuario.Text;
                //            StartActivity((typeof(Activitymenu)));
                //            //this.Close();
                //            //Me.Close()
                //            Finish();
                //}
                //else
                //{
                //    //HaceBeep();
                //    Toast.MakeText(this, "Usuario no autorizado, intente nuevamente!", ToastLength.Short).Show();
                //    editTextUsuario.Text = "";
                //    editTextClave.Text = "";
                //    editTextUsuario.RequestFocus();
                //}
            }
        }
        private void editTextClave_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                OK();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void HandleEditorActionClave(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                OK();
                e.Handled = true;
            }
        }
        private void CrearArchivos()
		{
			conexion1.CrearArchivos();
		}

		private void CargarRutas()
		{
			conexion1.CargarRutasAcceso();
		}
        private static void CreateCommand(string queryString, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (InvalidOperationException)
                {
                    //log and/or rethrow or ignore
                }
                catch (SqlException)
                {
                    //log and/or rethrow or ignore
                }
                catch (ArgumentException)
                {
                    //log and/or rethrow or ignore
                }
            }
        }
        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //    {
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //    }


    }
}