using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using BilddenLogistik.EFWorkBD;
using BilddenLogistik.MainActivities;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "ActivityInventario")]
    public class ActivityInventario : Activity, ListView.IOnItemClickListener
    {
        private EditText editTextUbicacionI;
        private RadioGroup radioGroup1;
        private RadioButton radioButton1, radioButton2, radioButton3;
        private CheckBox checkBoxInicial;
        private ListView listViewInv;
        public List<UbicacionesF> catalogoUbicar;
        public List<UbicacionesF> listaUbicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private ImageButton imgbtnRegresarInv;
        Boolean FlagDatos = false;
        string IDWharehouse;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_inventario);
            editTextUbicacionI = FindViewById<EditText>(Resource.Id.editTextUbicacionI);
            radioButton1 = FindViewById<RadioButton>(Resource.Id.radioButton1);
            radioButton2 = FindViewById<RadioButton>(Resource.Id.radioButton2);
            radioButton3 = FindViewById<RadioButton>(Resource.Id.radioButton3);
            checkBoxInicial = FindViewById<CheckBox>(Resource.Id.checkBoxInicial);
            listViewInv = FindViewById<ListView>(Resource.Id.listViewInv);
            listViewInv.OnItemClickListener = this;
            imgbtnRegresarInv = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarInv);
            editTextUbicacionI.KeyPress += editTextUbicacionI_KeyPress;
            editTextUbicacionI.EditorAction += HandleEditorActionUbicaI;
            CargarForma();
            checkBoxInicial.CheckedChange += delegate
            {
                if (checkBoxInicial.Checked)
                    Class1.FlagInvFisicoInic = true;
                else
                    Class1.FlagInvFisicoInic = false;

            };
            radioButton1.CheckedChange += delegate
            {
                if (radioButton1.Checked == true)
                {
                    Class1.NumeroToma = 1;
                    Actualiza_Tabla();
                }
            };
            radioButton2.CheckedChange += delegate
            {
                if (radioButton2.Checked == true)
                {
                    Class1.NumeroToma = 2;
                    Actualiza_Tabla();
                }
            };
            radioButton3.CheckedChange += delegate
            {
                if (radioButton3.Checked == true)
                {
                    Class1.NumeroToma = 3;
                    Actualiza_Tabla();
                }
            };
            imgbtnRegresarInv.Click += delegate
            {
                StartActivity((typeof(Activitymenu)));
                Finish();
            };

        }
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewInv.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ///////////////////////////////pendiente los datos a extraer///////////////////////////////////////////////////////
           
            char[] delimiterChars = { '=' };
            string select = adapter.GetItem(position).ToString();
            string[] words = select.Split(delimiterChars);
            string valores = words[0] + "$" + words[1];
            Class1.Pedido = words[1];
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
            Win_Save.SetMessage("Esta seguro de comenzar con esta Ubicación ? una vez que comienze debera realizar el conteo de todo lo que se encuentre en dicha ubicación.");
            Win_Save.SetTitle("UBICACION");
            Win_Save.SetPositiveButton("No", (send, arg) =>
            {
                Win_Save.Dispose();
            });
            Win_Save.SetNegativeButton("Si", (send2, arg2) =>
            {
                Class1.FlagIniciaUbica = true;
                //' Va a la siguiente pantalla para mostrar el contenido de la ubicación
                Class1.FlagListaInvF = true;


                StartActivity((typeof(ActivityInventarioDet)));
                Finish();
                Win_Save.Dispose();
            });
            Win_Save.Show();

        }
        private void editTextUbicacionI_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionUbicaI(object sender, TextView.EditorActionEventArgs e)
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
            //char lastchar;
            string cadena;
            int numUbica = editTextUbicacionI.Length();
            if (numUbica > 0)
            {
                //lastchar = txtUbica.Text(txtUbica.TextLength - 1)
                //If lastchar = Chr(10) Or lastchar = Chr(13) Then
                //cadena = editTextUbicacionI.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextUbicacionI.Text = cadena;
                AlertDialog.Builder Win_Save = new AlertDialog.Builder(this);
                Win_Save.SetMessage("Esta seguro de comenzar con esta Ubicación ? una vez que comienze debera realizar el conteo de todo lo que se encuentre en dicha ubicación.");
                Win_Save.SetTitle("UBICACION");
                Win_Save.SetPositiveButton("No", (send, arg) =>
                {
                    Win_Save.Dispose();
                });
                Win_Save.SetNegativeButton("Si", (send2, arg2) =>
                {
                    //' Verifica que la tarima sea valida
                    if (Verifica_Ubica(editTextUbicacionI.Text) != "")
                    {
                        Class1.UbicaSelect = IDWharehouse;
                        Class1.FlagIniciaUbica = true;

                        //' Va a la siguiente pantalla para mostrar el contenido de la ubicación
                        Class1.FlagListaInvF = true;
                        StartActivity((typeof(ActivityInventarioDet)));
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "La Ubicación no es Válida, favor de verificar e intentar nuevamente...", ToastLength.Short).Show();
                        editTextUbicacionI.Text = "";
                        editTextUbicacionI.RequestFocus();
                    }


                        //StartActivity((typeof(ActivityComprasD)));
                        //Finish();
                    Win_Save.Dispose();
                });
                Win_Save.Show();
                //End If
            }


        }
        private string Verifica_Ubica(string ubica)
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
                    int vnum = 0;
                    while (reader.Read())
                    {
                        vnum = Convert.ToInt32(reader.GetString(0));
                        IDWharehouse = Convert.ToString(vnum);
                        return IDWharehouse;
                    }
                }
                return IDWharehouse;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
                return IDWharehouse;
            };
        }
        private void CargarForma()
        {
            Class1.FlagIniciaInvFisico = false;
            Class1.FlagInvFisicoInic = true;
            checkBoxInicial.Checked = true;       //' Se puso en TRUE Temporalmente, debe esta en False por default......

            Class1.NumeroToma = 1;
            Actualiza_Tabla();
            editTextUbicacionI.Text = "";
            editTextUbicacionI.RequestFocus();
            if (Class1.FlagIniciaInvFisico)
            {
                Class1.FlagIniciaInvFisico = false;

                if (Class1.FlagInvFisicoInic)
                    checkBoxInicial.Checked = true;
                else
                    checkBoxInicial.Checked = false;
                Actualiza_Tabla();
                editTextUbicacionI.Text = "";
                editTextUbicacionI.RequestFocus();
            }

        }
        private void Actualiza_Tabla()
        {
            int TotalMarbetes;
            int TotalCerrado;
            decimal Porcentaje;
            string sPorcentaje = "0 %";
            string NombreMarbete;
            string Status="0";
            int i = 0;
            bool FlagEsta;
            string aux;
            string ValorFlag;
            try
            {
                mItems = new List<string>();
                listaUbicar = new List<UbicacionesF>();
                catalogoUbicar = new List<UbicacionesF>();
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    if (Class1.NumeroToma == 1)
                        Class1.strSQL = "SELECT DISTINCT lif.Ubicacion, lif.FlagT1, lw.name FROM vLogistik_InventariosFisicos lif INNER JOIN Logistik_warehouse lw ON lif.ubicacion=lw.id_warehouse WHERE ClaveInv = '" + Class1.NumeroInventario + "' ORDER BY lw.name";

                    else if (Class1.NumeroToma == 2)
                        Class1.strSQL = "SELECT DISTINCT lif.Ubicacion, lif.FlagT2, lw.name FROM vLogistik_InventariosFisicos lif INNER JOIN Logistik_warehouse lw ON lif.ubicacion=lw.id_warehouse WHERE ClaveInv = '" + Class1.NumeroInventario + "' ORDER BY lw.name";

                    else if (Class1.NumeroToma == 3)
                        Class1.strSQL = "SELECT DISTINCT lif.Ubicacion, lif.FlagT3, lw.name FROM vLogistik_InventariosFisicos lif INNER JOIN Logistik_warehouse lw ON lif.ubicacion=lw.id_warehouse WHERE ClaveInv = '" + Class1.NumeroInventario + "' ORDER BY lw.name";

                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    CrearBD.vgOrder = 1;

                    while (reader.Read())
                    {
                        NombreMarbete = (string)reader["name"];
                        if (Class1.NumeroToma == 1)
                            Status = (string)reader["FlagT1"];
                        if (Class1.NumeroToma == 2)
                            Status = (string)reader["FlagT2"];
                        if (Class1.NumeroToma == 3)
                            Status = (string)reader["FlagT3"];
                        aux = (string)reader["Ubicacion"];
                        Status = Status.Trim();
                        if (Status == "0")
                        {
                            UbicacionesF UbicarDetalle = new UbicacionesF()
                            {
                                Ubicacion = NombreMarbete,
                                IDUbica = aux

                            };
                            listaUbicar.Add(UbicarDetalle);
                        }

                    }
                    catalogoUbicar = listaUbicar;
                    adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoUbicar.Select(x => x.Ubicacion + " = " + x.IDUbica).ToArray()));
                    listViewInv.Adapter = adapter;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }

        }

    }
}