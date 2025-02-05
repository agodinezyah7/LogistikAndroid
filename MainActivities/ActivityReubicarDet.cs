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
    [Activity(Label = "Reubicar Detalle")]
    public class ActivityReubicarDet : Activity, ListView.IOnItemClickListener
    {
        private ListView listViewRD;
        public List<ClassListaReubicar> catalogoReubicar;
        public List<ClassListaReubicar> listaReubicar;
        public List<string> mItems;
        private ArrayAdapter adapter;
        private CrearBD cnn = new CrearBD();
        private ImageButton imgbtnRegresarRD, imgbtnsaveRD;
        private TextView textviewMgs, textViewTituloRD;
        private TextView textViewTarimaRD, textView101RD, textView103S, textViewUbicaRD;
        private EditText editTextTarimaRD, editTextCodRD, editTextCatRD, editTextUbicaDest;
        private string vlCtrlCant;
        private string vAlmacen;
        private double vlcant;
        private int vlid_producto;
        private string vlUtilizaNumSer;
        private string vlUtilizaLotes;
        private string idProd;
        private string CantOri;
        private string UbicaOri;
        private int IDUbica;
        private int IDWharehouse;
        private int IDWharehoseDest;
        private string vDescripcion;
        private bool FlagTarima = false;
        private bool FlagFoco;
        private int CantProducto;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_reubicar_det);
            textViewTituloRD = FindViewById<TextView>(Resource.Id.textViewTituloRD);
            textviewMgs = FindViewById<TextView>(Resource.Id.textviewMgs);
            listViewRD = FindViewById<ListView>(Resource.Id.listViewRD);
            imgbtnRegresarRD = FindViewById<ImageButton>(Resource.Id.imgbtnRegresarRD);
            imgbtnsaveRD = FindViewById<ImageButton>(Resource.Id.imgbtnsaveRD);
            textViewTarimaRD = FindViewById<TextView>(Resource.Id.textViewTarimaRD);
            editTextTarimaRD = FindViewById<EditText>(Resource.Id.editTextTarimaRD);
            textView101RD = FindViewById<TextView>(Resource.Id.textView101RD);
            editTextCodRD = FindViewById<EditText>(Resource.Id.editTextCodRD);
            textView103S = FindViewById<TextView>(Resource.Id.textView103S);
            editTextCatRD = FindViewById<EditText>(Resource.Id.editTextCatRD);
            textViewUbicaRD = FindViewById<TextView>(Resource.Id.textViewUbicaRD);
            editTextUbicaDest = FindViewById<EditText>(Resource.Id.editTextUbicaDest);
            editTextCatRD.KeyPress += editTextCatRD_KeyPress;
            editTextCatRD.EditorAction += HandleEditorActionCantRD;
            editTextTarimaRD.KeyPress += editTextTarimaRD_KeyPress;
            editTextTarimaRD.EditorAction += HandleEditorActionTarimaRD;
            editTextCodRD.KeyPress += editTextCodRD_KeyPress;
            editTextCodRD.EditorAction += HandleEditorActionCodigoRD;
            editTextUbicaDest.KeyPress += editTextUbicaDestRD_KeyPress;
            editTextUbicaDest.EditorAction += HandleEditorActionUbicaDestRD;
            listViewRD.OnItemClickListener = this;
            loadlayout();
            //AgregarDatosLista();
            imgbtnRegresarRD.Click += delegate
            {
                FlagTarima = false;
                StartActivity((typeof(ActivityReubicar)));
                Finish();
            };
            imgbtnsaveRD.Click += delegate
            {
                GuardarDatos();
            };
        }
        public void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string select = listViewRD.GetItemAtPosition(e.Position).ToString();
            Toast.MakeText(this, select, Android.Widget.ToastLength.Long).Show();
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {

                char[] delimiterChars = { '=' };
                string select = adapter.GetItem(position).ToString();
                string[] words = select.Split(delimiterChars);
                string valores = words[0] + "$" + words[1] + "$" + words[2];
                editTextCodRD.Text = words[0];
                CantOri = words[2];
                if (Class1.TipoProd==1 || Class1.TipoProd == 2 || Class1.TipoProd == 4)
                {
                    Class1.vEmbarque = words[3];
                    Class1.vOC = words[4];
                    Class1.vFactura = words[5];
                }

                if (Class1.vbUPCProducto != "")
                {
                    idProd = GetIDProduct(editTextCodRD.Text, 1, "");
                }
                else
                {
                    idProd = GetIDProduct(editTextCodRD.Text, IDWharehouse, editTextTarimaRD.Text);
                }
                editTextCatRD.Enabled = true;
                editTextCatRD.RequestFocus();
               
        }
        private void HandleEditorActionCodigoRD(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarCodigo();
                e.Handled = true;
            }
        }

        private void loadlayout()
        {
            if (Class1.FlagReubica)
            {
                Class1.FlagReubica = false;
                cnn.NextDocument();
                CantProducto = 0;
                if ((Class1.TipoProd==1) || (Class1.TipoProd==4))
                {
                    if (Class1.TipoProd == 1)
                        textViewTituloRD.Text = "Sacar Producto de Tarima";
                    if (Class1.TipoProd == 4)
                        textViewTituloRD.Text = "Meter Producto a Tarima";
                    //Label2.Text = "Tarima"
                    //Label1.Text = "Código"
                    textViewTarimaRD.Text = "Tarima:";
                    textView101RD.Text = "Código:";

                    //TxtCodigo.Enabled = False
                    editTextCodRD.Enabled = false;
                    //Label3.Visible = True
                    textView103S.Visibility = ViewStates.Visible;
                    //txtCant.Visible = True
                    editTextCatRD.Visibility = ViewStates.Visible;

                    //txtUbica.Visible = False
                    editTextUbicaDest.Visibility = ViewStates.Invisible;
                    //Label6.Visible = False
                    textViewUbicaRD.Visibility = ViewStates.Invisible;

                    //PictureBox4.Visible = True
                    imgbtnsaveRD.Visibility = ViewStates.Visible;
                    //Label4.Visible = True
                }
                if ((Class1.TipoProd == 2))
                {
                    textViewTituloRD.Text = "Reubicar Productos";
                    //Label2.Text = "Ubica."
                    //Label1.Text = "Código"
                    textViewTarimaRD.Text = "Ubica:";
                    textView101RD.Text = "Código:";

                    //TxtCodigo.Enabled = True
                    editTextCodRD.Enabled = true;
                    //Label3.Visible = True
                    textView103S.Visibility = ViewStates.Visible;
                    //txtCant.Visible = True
                    editTextCatRD.Visibility = ViewStates.Visible;

                    //txtUbica.Visible = False
                    editTextUbicaDest.Visibility = ViewStates.Invisible;
                    //Label6.Visible = False
                    textViewUbicaRD.Visibility = ViewStates.Invisible;

                    //PictureBox4.Visible = True
                    imgbtnsaveRD.Visibility = ViewStates.Visible;
                    //Label4.Visible = True
                }
                if ((Class1.TipoProd == 3))
                {
                    textViewTituloRD.Text = "Reubicar Tarima";
                    //Label2.Text = "Ubica."
                    //Label1.Text = "Tarima"
                    textViewTarimaRD.Text = "Ubica:";
                    textView101RD.Text = "Tarima:";

                    //TxtCodigo.Enabled = True
                    editTextCodRD.Enabled = true;
                    //Label3.Visible = False
                    textView103S.Visibility = ViewStates.Invisible;
                    //txtCant.Visible = False
                    editTextCatRD.Visibility = ViewStates.Invisible;

                    //txtUbica.Text = ""
                    editTextUbicaDest.Text = "";
                    //txtUbica.Visible = True
                    editTextUbicaDest.Visibility = ViewStates.Visible;
                    //txtUbica.Enabled = False
                    editTextUbicaDest.Enabled = false;
                    //Label6.Visible = True
                    textViewUbicaRD.Visibility = ViewStates.Visible;

                    //PictureBox4.Visible = True
                    imgbtnsaveRD.Visibility = ViewStates.Visible;
                    //Label4.Visible = True
                }
                if (Class1.FlagReubica)
                {
                    FlagTarima = false;
                    FlagFoco = true;
                }
                if (FlagFoco)
                {
                    AgregarDatosLista();
                }
                if (FlagTarima==false)
                {
                    editTextTarimaRD.Enabled = true;
                    editTextTarimaRD.Text = "";
                }
                editTextCodRD.Text = "";
                editTextCatRD.Text = "";
                editTextTarimaRD.Enabled = true;
                editTextTarimaRD.RequestFocus();
                if (Class1.vFinOrden==1)
                {
                    StartActivity((typeof(Activitymenu)));
                    Finish();
                }
            }
        }
        private void AgregarDatosLista()
        {
            var vTarima = editTextTarimaRD.Text;
            var vCodigo = editTextCodRD.Text;

            //falta limpiar ClassListaReubicar
            //listaReubicar.Clear();
            //catalogoReubicar.Clear();

            mItems = new List<string>();
            listaReubicar = new List<ClassListaReubicar>();
            catalogoReubicar = new List<ClassListaReubicar>();
            
            var sqllocal = "";
            if (Class1.TipoProd == 1 && vTarima != "")
                sqllocal = "select lu.codigo_producto, lpl.description, lu.cantidad, lu.embarque, lu.oc, lu.factura " +
                " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
                " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product" +
                " WHERE (tarima = '" + vTarima + "' AND estado='3')";
            
            if (Class1.TipoProd == 2)
                sqllocal = "select lu.codigo_producto, lu.tarima, lw.name, lpl.description, lu.cantidad, lu.embarque, lu.oc, lu.factura " +
             " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" +
             " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product INNER JOIN Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion " +
             " WHERE (lw.name = '" + vTarima + "' and codigo_producto='" + vCodigo + "' and lu.tarima='')";
            if (Class1.TipoProd == 3)
                sqllocal = "SELECT DISTINCT lu.tarima, lw.name, SUM(lu.cantidad) AS QTY " +
            " FROM vLogistik_Ubicaciones AS lu INNER JOIN Logistik_Warehouse as lw ON lw.id_Warehouse = lu.ubicacion" +
            " WHERE (lw.name = '" + vTarima + "' AND lu.tarima='" + vCodigo + "')" +
            " GROUP BY lu.tarima, lw.name";
            if (Class1.TipoProd == 4)
                sqllocal = "select lu.codigo_producto, lpl.description, lu.cantidad, lu.defectuosos, lu.embarque, lu.oc, lu.factura " + 
                " from vLogistik_Ubicaciones AS lu INNER JOIN Logistik_product AS lp ON lu.codigo_producto = lp.upc INNER JOIN" + 
                " Logistik_product_lang AS lpl ON lp.id_product = lpl.id_product" + 
                " WHERE (lu.estado = '2' and lu.tarima='')";
            if (vTarima != "")
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    if (sqllocal != "")
                    { 
                        con.Open();
                        SqlCommand sqlcmd2 = new SqlCommand(sqllocal, con);
                        SqlDataReader reader;
                        reader = sqlcmd2.ExecuteReader();
                        while (reader.Read())
                        {
                            if (Class1.TipoProd == 3)
                            {
                                ClassListaReubicar comprasDetalle = new ClassListaReubicar()
                                {

                                    Codigo = (string)reader["tarima"],
                                    Descrip = (string)reader["name"],
                                    Cantidad = (decimal)reader["QTY"]
                                };
                                listaReubicar.Add(comprasDetalle);
                            }
                            else
                            {
                                ClassListaReubicar comprasDetalle = new ClassListaReubicar()
                                {
                                    //Ubicacion = (string)reader["tarima"],
                                    Codigo = (string)reader["codigo_producto"],
                                    Descrip = (string)reader["description"],
                                    Cantidad = (decimal)reader["cantidad"],
                                    Embarque = (string)reader["embarque"],
                                    OC = (string)reader["oc"],
                                    Factura = (string)reader["factura"]
                                    //Id_Ubicacion = (int)reader["Id_Ubicacion"]
                                };
                                listaReubicar.Add(comprasDetalle);
                        }

                    }//fin while
                }//sqllocal
            }

            }
            catalogoReubicar = listaReubicar;
            if (Class1.TipoProd == 3)
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoReubicar.Select(x => x.Codigo + " = " + x.Descrip + " = " + x.Cantidad).ToArray()));
            else
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, (catalogoReubicar.Select(x => x.Codigo + " = " + x.Descrip + " = " + x.Cantidad + " = " + x.Embarque + " = " + x.OC + " = " + x.Factura).ToArray()));

            listViewRD.Adapter = adapter;
        }
        
        private void editTextCodRD_KeyPress(object sender, View.KeyEventArgs e)
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

        private void VerificarCodigo()
        {
            string CodBar = editTextCodRD.Text;
            int vNumCodBar = editTextCodRD.Length();

            if (vNumCodBar > 0)
            {
                if (Class1.TipoProd == 1 || Class1.TipoProd == 2)
                {
                    Class1.vbUPCProducto = "";
                    ExisteArt(CodBar);
                    if (Class1.vbUPCProducto != "")
                    {
                        editTextCodRD.Text = Class1.vbUPCProducto;
                        idProd = GetIDProduct(CodBar, IDWharehouse, editTextTarimaRD.Text);
                        if (idProd == "")
                        {
                            Toast.MakeText(this, "El código que indicó no existe como disponible para ubicar", ToastLength.Long).Show();
                            editTextCodRD.Text = "";
                            editTextCodRD.RequestFocus();
                        }
                        else
                        {
                            AgregarDatosLista();
                            Toast.MakeText(this, "Favor de indicar del listado el renglon de donde va tomar la mercancia...", ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "El Código No Existe en la empresa 06!", ToastLength.Long).Show();
                        editTextCodRD.Text = "";
                        editTextCodRD.RequestFocus();
                    }
                }
                else 
                {
                    if (Verify_Tarima(editTextCodRD.Text) == editTextTarimaRD.Text)
                    {
                        //dtOrdenCompra.Clear()
                        //dsOrdenCompra.Clear()
                        AgregarDatosLista();
                        //LlenarProveedores()
                        editTextUbicaDest.Enabled = true;
                        //txtUbica.Enabled = True
                        editTextUbicaDest.Text = "";
                        //txtUbica.Text = ""
                        editTextUbicaDest.RequestFocus();
                        //txtUbica.Focus()
                    }
                    else
                    {
                        Toast.MakeText(this, "El Numero de Tarima no es correcto o indico la ubicación incorrecta.!", ToastLength.Long).Show();
                        //MsgBox("El Numero de Tarima no es correcto o indico la ubicación incorrecta.!", MsgBoxStyle.Critical)
                        editTextCodRD.Text = "";
                        //TxtCodigo.Text = ""
                        editTextCodRD.RequestFocus();
                        //TxtCodigo.Focus()
                    }
                }
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
                    Boolean Flag1=false;
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
                    if (Flag1==false)
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
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error" + ex.Message.ToString(), ToastLength.Long).Show();
                return "0";
            }
        }

        private string Verify_Tarima(string codigo)
        {
            string aux="";
            string Sql1 = "Select * from vLogistik_Ubicaciones " +
                          "where Tarima = '" + codigo + "'";
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        aux = (string)reader["Ubicacion"];
                    }
                }
                return Verifica_UbicaT(aux);
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Error" + ex.Message.ToString(), ToastLength.Long).Show();
                return aux;
            }
        }

        private string Verifica_UbicaT(string ubica)
        {
            string aux = "";
            string Sql1 = "Select * from Logistik_warehouse " + 
                          "where id_Warehouse = '" + ubica + "'";
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        aux = (string)reader["name"];
                    }
                }
                return aux;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error" + ex.Message.ToString(), ToastLength.Long).Show();
                return aux;
            }
        }

        private string GetIDProduct(string upc, int ubica, string Tarima)
        {
            int aux = 0; 
            string Sql1 = "";
            try
            {
                if (Class1.TipoProd == 1)
                    Sql1 = "Select * from vLogistik_Ubicaciones " +
                         "where codigo_producto = '" + upc + "' and ubicacion='" + ubica.ToString() + "' and tarima='" + Tarima + "'";
                else if (Class1.TipoProd == 2 || Class1.TipoProd == 3)
                    Sql1 = "Select * from vLogistik_Ubicaciones " +
                         "where codigo_producto = '" + upc + "' and ubicacion='" + ubica.ToString() + "' and tarima='" + "" + "'";
                else if (Class1.TipoProd == 4)
                    Sql1 = "Select * from vLogistik_Ubicaciones " +
                         "where codigo_producto = '" + upc + "' and ubicacion='" + ubica.ToString() + "' and tarima='" + "" + "' and Estado='2'";
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    SqlCommand sqlcmd2 = new SqlCommand(Sql1, con);
                    SqlDataReader reader;
                    reader = sqlcmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        aux = (int)reader["id_Product"];
                        if (Class1.TipoProd == 4)
                        {
                            IDUbica = (int)reader["id_ubicacion"];
                            UbicaOri = (string)reader["ubicacion"];
                        }

                    }
                    reader.Close();
                }
                return Convert.ToString( aux);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error" + ex.Message.ToString(), ToastLength.Long).Show();
                return Convert.ToString( aux);
            }
        }

        private void editTextTarimaRD_KeyPress(object sender, View.KeyEventArgs e)
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
        private void VerificarTarima()
        {
            //char lastchar;
            string cadena = editTextTarimaRD.Text;
            int vtarima = editTextTarimaRD.Length();

            if (vtarima > 0)
            {
                //lastchar = editTextTarimaRD.Text(editTextTarimaRD.Length - 1);
                //if (lastchar = chr(10) || lastchar = chr(13))
                //cadena = editTextTarimaRD.Text;
                //cadena = cadena.Substring(0, cadena.Length - 2);
                //editTextTarimaRD.Text = cadena;

                if (Class1.TipoProd == 3 || Class1.TipoProd == 2)
                {
                    // Valida que la ubicacion indicada sea valida
                    IDWharehouse = Verifica_Ubica(cadena, "1");
                    if (IDWharehouse == 0)
                    {
                        Toast.MakeText(this, "La ubicacion no existe en SAE, favor de darla de alta primero", ToastLength.Short).Show();
                        editTextTarimaRD.Text = "";
                        editTextTarimaRD.RequestFocus();
                    }
                    else
                    {
                        editTextTarimaRD.Enabled = false;
                        editTextCodRD.Enabled = true;
                        FlagTarima = true;
                        editTextCodRD.RequestFocus();
                    }
                }
                else
                {
                    if (editTextTarimaRD.Text != "")
                    {
                        if (editTextTarimaRD.Text.Substring(0, 1) == "T" && editTextTarimaRD.Text.Length == 5)
                        {
                            editTextTarimaRD.Enabled = false;
                            FlagTarima = true;
                            // Obtiene el ID de la ubicacion donde esta la tarima IDWharehouse
                            Verifica_Tarima(editTextTarimaRD.Text);
                            if (IDWharehouse == 0)
                            {
                                Toast.MakeText(this, "El Número de Tarima no existe!", ToastLength.Short).Show();
                                editTextTarimaRD.Text = "";
                                editTextTarimaRD.RequestFocus();
                            }
                            else
                            {
                                AgregarDatosLista();
                                editTextCodRD.RequestFocus();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "El Número de Tarima no es valido!", ToastLength.Short).Show();
                            editTextTarimaRD.Text = "";
                            editTextTarimaRD.RequestFocus();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Favor de indicar un dato valido en la Tarima!", ToastLength.Short).Show();
                        editTextTarimaRD.Text = "";
                        editTextTarimaRD.RequestFocus();
                    }
                }
            }
        }

        
        private void HandleEditorActionTarimaRD(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarTarima();
                e.Handled = true;
            }
        }
        private void HandleEditorActionUbicaDestRD(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Next || e.ActionId == ImeAction.Send || e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Go)
            {
                VerificarUbicaDest();
                e.Handled = true;
            }
        }

        private void VerificarUbicaDest()
        {
            IDWharehoseDest = 0;
            Verifica_Ubica(editTextUbicaDest.Text,"2");
            if (IDWharehoseDest == 0)
            {
                Toast.MakeText(this, "La ubicacion no existe en SAE, favor de darla de alta primero", ToastLength.Long).Show();
                editTextUbicaDest.Text = "";
                editTextUbicaDest.RequestFocus();
            }
            else
                CantProducto = CantProducto + 1;
        }

        private void editTextUbicaDestRD_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                VerificarUbicaDest();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void editTextCatRD_KeyPress(object sender, View.KeyEventArgs e)
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
        private void HandleEditorActionCantRD(object sender, TextView.EditorActionEventArgs e)
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
            //codigo Cantidad press key
            char lastchar;
            string cadena;
            string sql1="";
            decimal dCant;
            var codigo = editTextCodRD.Text;
            var tarima = editTextTarimaRD.Text;
            decimal cant = Convert.ToDecimal( editTextCatRD.Text);
            decimal vCantOri = Convert.ToDecimal(CantOri);
            // Valida que sea un numero.
            if ((cant) >= 1)
            {
                // Verifica que no este tomando mas de lo debido
                if ((cant) > vCantOri)
                {
                    Toast.MakeText(this, "Esta indicando una cantidad mayor a la que se tiene disponible en esa ubicacion!", ToastLength.Short).Show();
                    cant = 0;
                    editTextCatRD.RequestFocus();
                }
                else
                {
                    //verificar si existe
                    bool verifExist=false;
                    if (Class1.TipoProd == 1)
                        verifExist = FnExisteArticuloUbica(codigo, IDWharehouse, tarima);
                    if (Class1.TipoProd == 2 || Class1.TipoProd == 3)
                        verifExist = FnExisteArticuloUbica(codigo, IDWharehouse, "");
                    if (Class1.TipoProd == 4)
                        verifExist = FnExisteArticuloUbica(codigo, 1, "");
                    if (verifExist == false)
                        Toast.MakeText(this, "El código no existe para ubicar!!", ToastLength.Short).Show();
                    else
                    {
                        // Descuenta lo que movió de la ubicacio original
                        dCant = Convert.ToDecimal(Class1.Inv_cant) - Convert.ToDecimal(cant);
                        if (IDWharehouse != 0)
                        {
                            if (dCant != 0)
                            {
                                if (Class1.TipoProd == 1)
                                    sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() +
                                    " where codigo_producto='" + codigo + "'" +
                                    " and ubicacion='" + IDWharehouse + "' and tarima='" + tarima + "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                if (Class1.TipoProd == 2 || Class1.TipoProd == 3)
                                    sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() +
                                    " where codigo_producto='" + codigo + "'" +
                                    " and ubicacion='" + IDWharehouse + "' and tarima='" + "" + "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                if (Class1.TipoProd == 4)
                                    sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() +
                                    " where codigo_producto='" + codigo + "'" +
                                    " and ubicacion='" + "1" + "' and tarima='" + "" + "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                            }
                            else
                                sql1 = "DELETE FROM vLogistik_Ubicaciones WHERE id_ubicacion = " + IDUbica;
                            EjecutarQuerySQLWifi(sql1);
                        }
                        else
                            Toast.MakeText(this, "ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...", ToastLength.Short).Show();
                        if (Class1.TipoProd == 1)
                            sql1 = "insert into Logistik_Inventario_Temp(id_order, id_product, id_product_attribute, num_parte, cantidad, ubicacion, tipo, almacen, existe, tarima, embarque, oc, factura) values ('" +
                                   CrearBD.vgOrder + "','" + idProd + "','0','" + codigo + "','" + cant + "','" + UbicaOri + "','" + Class1.TipoProd.ToString() + "','" + UbicaOri + "','1','" + tarima + "','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                        if (Class1.TipoProd == 2 || Class1.TipoProd == 3)
                            sql1 = "insert into Logistik_Inventario_Temp(id_order, id_product, id_product_attribute, num_parte, cantidad, ubicacion, tipo, almacen, existe, tarima, embarque, oc, factura) values ('" +
                                   CrearBD.vgOrder + "','" + idProd + "','0','" + codigo + "','" + cant + "','" + UbicaOri + "','" + Class1.TipoProd.ToString() + "','" + UbicaOri + "','1','" + "" + "','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                        if (Class1.TipoProd == 4)
                            sql1 = "insert into Logistik_Inventario_Temp(id_order, id_product, id_product_attribute, num_parte, cantidad, ubicacion, tipo, almacen, existe, tarima, embarque, oc, factura) values ('" +
                                 CrearBD.vgOrder + "','" + idProd + "','0','" + codigo + "','" + cant + "','" + IDWharehouse + "','" + Class1.TipoProd.ToString() + "','" + IDWharehouse + "','1','" + tarima + "','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                        EjecutarQuerySQLWifi(sql1);
                        CantProducto = CantProducto + 1;
                    }
                }
                if (Class1.TipoProd == 1 || Class1.TipoProd == 4)
                {
                    //txtTarima.Text = ""
                    editTextCodRD.Text = "";
                    editTextCatRD.Text = "";
                    editTextCatRD.Enabled = false;
                    editTextCodRD.RequestFocus();
                }    
                if (Class1.TipoProd == 2)
                {
                    //txtTarima.Text = ""
                    editTextCodRD.Text = "";
                    editTextCatRD.Text = "";
                    editTextCatRD.Enabled = false;
                    editTextCodRD.RequestFocus();
                }    
                if (Class1.TipoProd == 3)
                {
                    editTextTarimaRD.Text = "";
                    editTextCodRD.Text = "";
                    editTextCatRD.Text = "";
                    editTextTarimaRD.RequestFocus();
                }
                imgbtnRegresarRD.Enabled = false;
                //dtOrdenCompra.Clear()
                //dsOrdenCompra.Clear()
                AgregarDatosLista();
            }
            else
            {
                Toast.MakeText(this, "Especifica una cantidad", ToastLength.Short).Show();
                editTextCatRD.Text = "";
                editTextCatRD.RequestFocus();
            }
                
       
        }
        private void GuardarDatos()
        {
            string sql1="";
            decimal dCant=0;
            string CodigoTemp="";
            string UbicaTemp="";
            int CantTemp=0;
            string vAutorizar = "S";
            string vSTATE = "8";
            bool FlagInicial = true;
            var cant = editTextCatRD.Text;
            var codigo = editTextCodRD.Text;
            if (CantProducto > 0)
            {
                //lblAguarde.Visible = true;
                textviewMgs.Visibility = ViewStates.Visible;
                if (cant == "")
                {
                    if (Class1.TipoProd == 1 && Class1.TipoProd == 2)
                    {
                        //' Abre la tabla de Inventario temporal para ir afectando cada renglon y generar tambien el traspaso
                        var sqllocal = "SELECT num_parte,id_product,tarima,cantidad,Ubicacion,embarque,oc,factura" +
                            " FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder  + "')";
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
                                idProd = (string)reader["id_product"];
                                UbicaTemp = (string)reader["tarima"];
                                CantTemp = (int)reader["cantidad"];
                                UbicaOri = (string)reader["Ubicacion"];
                                Class1.vEmbarque = (string)reader["embarque"];
                                Class1.vOC = (string)reader["oc"];
                                Class1.vFactura = (string)reader["factura"];

                                bool verifExist = false;

                                if (Class1.TipoProd == 1)
                                    verifExist = FnExisteArticuloInv(CodigoTemp, 1, "");
                                if (Class1.TipoProd == 3)
                                    verifExist = FnExisteArticuloInv(CodigoTemp, 1, UbicaTemp);
                                if (Class1.TipoProd == 2)
                                    //' Obtiene el ID de la ubicacion destino
                                    //'Verifica_Ubica(UbicaTemp, "1")
                                    verifExist = FnExisteArticuloInv(CodigoTemp, 1, "");
                                if (verifExist == false)
                                {
                                    if (Class1.TipoProd == 1)
                                        sql1 = "insert into vLogistik_Ubicaciones(" +
                                        "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + "1" +
                                        "','" + "" + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp + "','2','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                                    if (Class1.TipoProd == 3)
                                        sql1 = "insert into vLogistik_Ubicaciones(" +
                                       "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + UbicaOri +
                                       "','" + UbicaTemp + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp + "','3','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                                    if (Class1.TipoProd == 2)
                                        sql1 = "insert into vLogistik_Ubicaciones(" +
                                       "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + "1" +
                                       "','" + "" + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp + "','2','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";
                                }
                                else
                                {
                                    dCant = 0;
                                    if (UbicaOri != "")
                                        dCant = Convert.ToInt32(CantTemp) + Class1.Inv_cant;
                                    if (Class1.TipoProd == 1)
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() + " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='" + "1" + "' and tarima = '" + "" + "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                    if (Class1.TipoProd == 3)
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() + " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='" + "1" + "' and tarima = '" + UbicaTemp + "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                    if (Class1.TipoProd == 2)
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() + " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='" + "1" + "' and tarima = '' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                    else
                                        Toast.MakeText(this, "ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...", ToastLength.Short).Show();
                                }
                                EjecutarQuerySQLWifi(sql1);
                                if (FlagInicial)
                                {
                                    Verifica_Ubica(UbicaTemp, "2");
                                    //' Insertar el encabezado de la orden de traspaso
                                    sql1 = "INSERT INTO Logistik_Traspasos(id_order,reference, id_shop_group, id_shop, id_carrier, id_lang, id_customer, " +
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
                                    "0,'01/01/2015',getdate(),'" + "1" + "','" + IDWharehouse + "','U','" +
                                    Class1.vgID_employee.ToString() + "','" + CrearBD.vgOrder + "','" + vAutorizar.ToString() + "','58','06',1)";
                                    //' Originalmente estaba IDWharehouse en lugar de UbicaOri
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
                                         CrearBD.vgOrder + "','1','" + IDWharehouse + "','1','" + idProd + "','" +
                                         "0','" + vDescripcion + "'," + CantTemp + ",0,1,1," +
                                         "1,1,5000,0,0," +
                                         "0,0,0,0,''," +
                                         "'" + CodigoTemp + "','','',0,0," +
                                         "1,'IVA',1,1,1," +
                                         "1,'',1,'01/01/2015',16," +
                                         "0,0,0,116,0," +
                                         "0,0,0,0,'06')";
                                EjecutarQuerySQLWifi(sql1);
                                //' Marca la bandera para que se imprima la etiqueta de la tarima si acaba modificar
                                if (Class1.TipoProd == 1)
                                {
                                    sql1 = "update vLogistik_Ubicaciones set FlagPrint='1' where tarima = '" + UbicaTemp + "' and ubicacion='" + UbicaOri + "'";
                                    EjecutarQuerySQLWifi(sql1);
                                }

                                //' Genera el registro de la tabla del Kardex
                                sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " +
                                    "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " +
                                    "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product) VALUES ('" + Class1.vEmbarque + "','" + Class1.vgID_employee + "',getdate()," +
                                    "'" + CodigoTemp + "','" + CantTemp + "',NULL,'0','3','2','" + IDWharehouse.ToString() + "','1','" + UbicaTemp + "','',NULL,NULL,NULL,NULL,NULL,NULL,'" + Class1.vOC + "','" + Class1.vFactura + "'," + idProd + ")";

                                EjecutarQuerySQLWifi(sql1);
                            }//fin wile
                            //' Finalmente borra todo lo de Inv_Temp para limpiar la tabla
                            sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                            EjecutarQuerySQLWifi(sql1);

                            
                        }
                    }
                    if (Class1.TipoProd == 4)
                    {
                        //' Abre la tabla de Inventario temporal para ir afectando cada renglon y generar tambien el traspaso
                        
                        using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                        {
                            con.Open();
                            Class1.strSQL = "SELECT * FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                            SqlCommand command = new SqlCommand(Class1.strSQL, con);
                            SqlDataReader reader = command.ExecuteReader();
                            int VidProd = 0;
                            while (reader.Read())
                            {
                                CodigoTemp = (string)reader["num_parte"];
                                VidProd = (int)reader["id_product"];
                                idProd = Convert.ToString(VidProd);
                                
                                UbicaTemp = (string)reader["tarima"];
                                CantTemp = (int)reader["cantidad"];
                                UbicaOri = (string)reader["Ubicacion"];
                                Class1.vEmbarque = (string)reader["embarque"];
                                Class1.vOC = (string)reader["oc"];
                                Class1.vFactura = (string)reader["factura"];
                                //' Ahora guarda los datos en inventario *******_____________
                                //'verificar si existe
                                bool verifExist = false;
                                int UbOrig = Convert.ToInt32( UbicaOri);
                                verifExist = FnExisteArticuloInv(CodigoTemp, UbOrig, UbicaTemp);
                                if (verifExist == false)
                                    sql1 = "insert into vLogistik_Ubicaciones(" +
                                   "ubicacion, tarima, caja, id_product, codigo_producto, cantidad, estado, procesado_sae, embarque, oc, factura) values ('" + UbicaOri +
                                   "','" + UbicaTemp + "','','" + idProd + "','" + CodigoTemp + "','" + CantTemp.ToString() + "','3','N','" + Class1.vEmbarque + "','" + Class1.vOC + "','" + Class1.vFactura + "')";

                                else
                                {
                                    if (UbicaOri != "")
                                    {
                                        dCant = (CantTemp) + Class1.Inv_cant;
                                        sql1 = "update vLogistik_Ubicaciones set cantidad = " + dCant.ToString() +
                                        " where codigo_producto='" + CodigoTemp + "'" +
                                        " and ubicacion='" + UbicaOri + "' and tarima = '" + UbicaTemp + "' and embarque='" + Class1.vEmbarque + "' and oc='" + Class1.vOC + "' and factura='" + Class1.vFactura + "'";
                                    }
                                    else
                                        Toast.MakeText(this, "ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...", ToastLength.Short).Show();
                                }
                                EjecutarQuerySQLWifi(sql1);

                                //' Entra a reportar el movimiento de producto si es Ubicar Producto, o ubicar una tarima. 
                                if (FlagInicial)
                                {
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
                                    "0,'01/01/2015',getdate(),'" + UbicaOri + "','" + "1" + "','U','" + 
                                    Class1.vgID_employee.ToString() + "','" + CrearBD.vgOrder + "','" + vAutorizar.ToString() + "','58','06',1)";
                                    //' Originalmente estaba IDWharehouse en lugar de UbicaOri
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
                                    "0','" + vDescripcion + "'," + CantTemp.ToString() + ",0,1,1," + 
                                    "1,1,5000,0,0," + 
                                    "0,0,0,0,''," + 
                                    "'" + CodigoTemp + "','','',0,0," + 
                                    "1,'IVA',1,1,1," + 
                                    "1,'',1,'01/01/2015',16," + 
                                    "0,0,0,116,0," + 
                                    "0,0,0,0,'06')";
                                EjecutarQuerySQLWifi(sql1);
                                //' Genera el registro de la tabla del Kardex
                                sql1 = "INSERT INTO vLogistik_Kardex(embarque, id_usuario, fecha, codigo_producto, cantidad, cantidad_factura, merma, " + 
                                    "estado_inicial, estado_final, ubicacion_inicial, ubicacion_final, tarima_inicial, tarima_final, no_caja, observaciones_clasificacion, " + 
                                    "observaciones_reubicaciones, id_motivo_entrada, id_motivo_salida, pedido, oc, factura, id_product) VALUES ('" + Class1.vEmbarque + "','" + Class1.vgID_employee + "',getdate()," +
                                    "'" + CodigoTemp + "','" + CantTemp.ToString() + "',NULL,'0','2','3','" + UbicaOri + "','1','','" + UbicaTemp + "',NULL,NULL,NULL,NULL,NULL,NULL,'" + Class1.vOC + "','" + Class1.vFactura + "'," + idProd + ")";
                                EjecutarQuerySQLWifi(sql1);

                            }//fin while
                        //' Marca la bandera para que se imprima la etiqueta de la tarima si acaba modificar
                        if (UbicaTemp != "")
                        { 
                            sql1 = "update vLogistik_Ubicaciones set FlagPrint = '1'" + 
                            " where tarima = '" + UbicaTemp + "' and ubicacion='" + UbicaOri + "'";
                                EjecutarQuerySQLWifi(sql1); 
                        }
                        //' Finalmente borra todo lo de Inv_Temp para limpiar la tabla
                        sql1 = "DELETE FROM Logistik_Inventario_Temp WHERE (id_order = '" + CrearBD.vgOrder + "')";
                        EjecutarQuerySQLWifi(sql1);
                        }
                       

                    }
                    if (Class1.TipoProd == 3)
                    {
                        //' Inicia proceso para cambiar la tarima de ubicacion
                        //' Ahora va a llenar los datos de encabezado y detalle de traspasos de los productos en cuestion
                        //' Obtiene el numero de orden con el que se guardo el dertalle de esa tarima en la tabla de inventarios temporal
                        //'GetIDTarima(TxtCodigo.Text)
                        LlenaTraspasodeTarima();
                        //'Verifica_Ubica(txtTarima.Text, "1")
                        if (IDWharehoseDest != 0)
                        {
                            int vCod = editTextCodRD.Length();
                            if (vCod > 1)
                            {
                                //if (codigo(0) == "T")  //error 1
                                if (codigo == "T")
                                {
                                    //' Va y cambia la ubicacion de la tarima y limpia todo.
                                    if (editTextUbicaDest.Text == "PASILLO")
                                        sql1 = "update vLogistik_Ubicaciones set ubicacion = '" + IDWharehoseDest + "', estado='2'" +
                                        " where tarima='" + codigo + "'";
                                    else
                                        sql1 = "update vLogistik_Ubicaciones set ubicacion = '" + IDWharehoseDest + "', estado='3'" +
                                        " where tarima='" + codigo + "'";

                                    EjecutarQuerySQLWifi(sql1);

                                }
                                else
                                {
                                    Toast.MakeText(this,"Favor de indicar un numero de tarima valido!",ToastLength.Short).Show();
                                    editTextCodRD.Text = "";
                                    editTextCodRD.RequestFocus();
                                }
                            }
                            else
                            {
                                Toast.MakeText(this,"Favor de indicar un numero de tarima valido!", ToastLength.Short).Show();
                                editTextCodRD.Text = "";
                                editTextCodRD.RequestFocus();
                            }
                        }
                        else
                            Toast.MakeText(this,"ERROR CRITICO de SISTEMA !!!! Por favor Llame a su supervisor y no haga mas con la terminal...",ToastLength.Short).Show();

                    }
                    Toast.MakeText(this, "Datos guardados", ToastLength.Long).Show();
                    FlagTarima = false;
                    imgbtnRegresarRD.Enabled = true;
                    //PictureBox5.Enabled = enabled;
                    textviewMgs.Visibility = ViewStates.Invisible;
                    //lblAguarde.Visible = false;
                    StartActivity((typeof(ActivityReubicar)));
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, "No ha dado Enter para terminar de registrar la cantidad del producto que ya indicó...", ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "No ha capturado nada!", ToastLength.Short).Show();
            }
            
            
            
        }
         public bool FnExisteArticuloUbica(string codigo, int ubica, string tarima )
        {
            var sqllocal = "Select cantidad, embarque, oc, factura,id_ubicacion from vLogistik_Ubicaciones" +
            " where codigo_producto = '" + codigo.Trim() + "' and ubicacion='" + ubica.ToString().Trim() + "' and tarima='" + tarima.Trim() +
            "' and embarque='" + Class1.vEmbarque.Trim() + "' and oc='" + Class1.vOC.Trim() + "' and factura='" + Class1.vFactura.Trim() + "'";
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
                    Class1.vEmbarque = (string)reader["embarque"];
                    //vEmbarque = oFila.Item("embarque").ToString.Trim
                    Class1.vOC= (string)reader["oc"];
                    //vOC = oFila.Item("oc").ToString.Trim
                    Class1.vFactura = (string)reader["factura"];
                    //vFactura = oFila.Item("factura").ToString.Trim
                    IDUbica = (int)reader["id_ubicacion"];
                    //IDUbica = oFila.Item("id_ubicacion").ToString.Trim
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
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }
        }
        public int Verifica_Ubica(string ubica, string TipoU)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select * from Logistik_warehouse where name = '" + ubica + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    if (TipoU == "1")
                        IDWharehouse = 0;
                    else
                        IDWharehoseDest = 0;
                    while (reader.Read())
                    {
                        if (TipoU == "1")
                            IDWharehouse = (int)reader["id_warehouse"];
                        else
                            IDWharehoseDest = (int)reader["id_warehouse"];
                    }
                    return IDWharehouse;
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return IDWharehouse;
            }
        }
        private int Verifica_Tarima(string tarima)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select ubicacion from vLogistik_Ubicaciones where tarima = '" + tarima + "'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                   
                    IDWharehouse = 0;
                    string id = "";
                    while (reader.Read())
                    {
                        id = (string)reader["ubicacion"];
                        IDWharehouse = Convert.ToInt32(id);
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
        public bool FnExisteArticuloInv(string codigo, int ubica, string Tarima)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select cantidad from vLogistik_Ubicaciones" +
                    " where codigo_producto = '" + codigo + "' and ubicacion='" + ubica.ToString() + "' and tarima='" + Tarima +
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
        public string Get_Descri(string codigo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select * from Logistik_product_lang " + 
                                  "where id_product = '" + codigo + "'";
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
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "SELECT * FROM vLogistik_Ubicaciones WHERE (tarima = '" + editTextCodRD.Text.Trim() +
                                    "') AND (ubicacion='" + IDWharehouse + "') AND (estado= '3')";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    int VidProd = 0;
                    while (reader.Read())
                    {
                        CodigoTemp = (string)reader["codigo_producto"];
                        VidProd = (int)reader["id_product"];
                        idProd = Convert.ToString(VidProd);
                        UbicaTemp = (string)reader["tarima"];
                        CantTemp = (decimal)reader["cantidad"];
                        UbicaOri = (string)reader["Ubicacion"];
                        //' Ahora guarda los datos en inventario *******_____________
                        //'verificar si existe
                        bool verifExist;
                        if (Class1.TipoProd == 1 && Class1.TipoProd == 3)
                        {
                            int UbOrig = Convert.ToInt32(UbicaOri);
                            verifExist = FnExisteArticuloInv(CodigoTemp, UbOrig, UbicaTemp);
                        }

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
                                "0','" + vDescripcion + "'," + CantTemp.ToString() + ",0,1,1," + 
                                "1,1,5000,0,0," + 
                                "0,0,0,0,''," + 
                                "'" + CodigoTemp + "','','',0,0," + 
                                "1,'IVA',1,1,1," + 
                                "1,'',1,'01/01/2015',16," + 
                                "0,0,0,116,0," + 
                                "0,0,0,0,'06')";
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
    }
}