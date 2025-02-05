using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Net.Wifi;
using Android.Telephony;


using Java.Lang;
using Java.IO;
using Java.Net;

using System.Data;
using System.Data.SqlClient;
using BilddenLogistik.EFWorkBD;

using BilddenLogistik.EFWorkBD;

namespace BilddenLogistik.MainActivities
{
    [Activity(Label = "Menu Principal", MainLauncher = false, Theme = "@style/AppTheme")]
    public class Activitymenu : Activity
    {
        private Button ButtonCompras, ButtonUbicarProd, ButtonPedido, ButtonReubicar;
        private Button ButtonConsulta, ButtonEmpaques, Buttonentrada, ButtonSalida;
        private Button ButtonInventario, ButtonConfiguracion, ButtonSalir;
        private ImageView imageViewWifi;
        private TextView textViewBateriaP;
        private const string UNKNOWNSSID = "<unknown ssid>";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_menu);
            imageViewWifi = FindViewById<ImageView>(Resource.Id.imageViewWifi);
            textViewBateriaP = FindViewById<TextView>(Resource.Id.textViewBateriaP);
            ButtonCompras = FindViewById<Button>(Resource.Id.ButtonCompras);
            ButtonUbicarProd = FindViewById<Button>(Resource.Id.ButtonUbicarProd);
            ButtonPedido = FindViewById<Button>(Resource.Id.ButtonPedido);
            ButtonReubicar = FindViewById<Button>(Resource.Id.ButtonReubicar);
            ButtonSalir = FindViewById<Button>(Resource.Id.ButtonSalir);
            ButtonConsulta = FindViewById<Button>(Resource.Id.ButtonConsulta);
            ButtonEmpaques = FindViewById<Button>(Resource.Id.ButtonEmpaques);
            Buttonentrada = FindViewById<Button>(Resource.Id.Buttonentrada);
            ButtonSalida = FindViewById<Button>(Resource.Id.ButtonSalida);
            ButtonInventario = FindViewById<Button>(Resource.Id.ButtonInventario);
            ButtonConfiguracion = FindViewById<Button>(Resource.Id.ButtonConfiguracion);
            if (Class1.vgEmployee_Perfil == 8)
                ButtonPedido.Text = "Virificacion";
            else
                ButtonPedido.Text = "Surtir Pedido";
            ButtonConfiguracion.Click += delegate
                {
                    ButtonConfiguracion.Text = "OK";
                    StartActivity((typeof(ActivityConfig)));
                //this.Close();
                //Me.Close()
                Finish();
                };
            ButtonInventario.Click += delegate
            {
                ButtonInventario.Text = "OK";
                bool vInvet = verifyInvFisicoInic();
                if (vInvet == true)
                {
                    StartActivity((typeof(ActivityInventario)));
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, "No se ha iciniado el proceso del Inventario, verificar con su supervisor.", ToastLength.Short).Show();
                }
                
            };
            ButtonSalida.Click += delegate
            {
                ButtonSalida.Text = "OK";
                StartActivity((typeof(ActivitySalida)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            Buttonentrada.Click += delegate
            {
                Buttonentrada.Text = "OK";
                StartActivity((typeof(ActivityEntrada)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonEmpaques.Click += delegate
            {
                ButtonEmpaques.Text = "OK";
                StartActivity((typeof(ActivityEmpaques)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonConsulta.Click += delegate
            {
                ButtonConsulta.Text = "OK";
                StartActivity((typeof(ActivityConsultaVerif)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonSalir.Click += delegate
            {
                ButtonSalir.Text = "OK";
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonCompras.Click += delegate
            {
                ButtonCompras.Text = "OK";
                StartActivity((typeof(ActivityComprasE)));
                //this.Close();
                //Me.Close()
                Finish();
            };

            ButtonPedido.Click += delegate
            {
                ButtonPedido.Text = "OK";
                StartActivity((typeof(ActivityPedidosE)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonUbicarProd.Click += delegate
            {
                ButtonUbicarProd.Text = "OK";
                StartActivity((typeof(ActivityUbicar)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            ButtonReubicar.Click += delegate
            {
                ButtonReubicar.Text = "OK";
                StartActivity((typeof(ActivityReubicar)));
                //this.Close();
                //Me.Close()
                Finish();
            };
            RunUpdateLoop();


        }

        public bool verifyInvFisicoInic()
        {
            //try
            //{
                using (SqlConnection con = new SqlConnection(Class1.cnSQL))
                {
                    con.Open();
                    Class1.strSQL = "Select * from vLogistik_Inventarios where Status = '1'";
                    SqlCommand command = new SqlCommand(Class1.strSQL, con);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Class1.NumeroInventario = (string)reader["clave"];
                    }
                    return true;
                }
            //}
            //catch (Exception ex)
            //{
            //    Toast.MakeText(this, ex.Message.ToString(), ToastLength.Short).Show();
            //    return false;
            //}
        }
        protected async void RunUpdateLoop()
        {
            //int count = 1;
            while (true)
            {
                await Task.Delay(1000);
                //textViewBateriaP.Text = string.Format("{0} ticks!", count++);
                float nivel = getBatteryLevel();
                textViewBateriaP.Text = Convert.ToString(System.Math.Round(nivel)) + "%";
                //imageViewWifi.SetImageResource(Resource.Drawable.wifired);
                bool statuswifi = IsWifiEnabled();
                bool cnnwifi = IsWifiConnected();
                int sennal = 0;
                if (statuswifi == true && cnnwifi == true)
                    sennal = WifiSignalPorcen();
                if (sennal <= -75) // red
                    imageViewWifi.SetImageResource(Resource.Drawable.wifired);
                if (sennal <= -50 && sennal >= -75) // amarillo
                    imageViewWifi.SetImageResource(Resource.Drawable.wifiyelow);
                if (sennal >= -50 && sennal <= -45) // verde
                    imageViewWifi.SetImageResource(Resource.Drawable.wifigreen);
            }
        }

        public float getBatteryLevel()
        {
            Intent batteryIntent = RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
            int level = batteryIntent.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = batteryIntent.GetIntExtra(BatteryManager.ExtraScale, -1);
            return ((float)level / (float)scale) * 100.0f;
        }
        protected bool IsWifiEnabled()
        {
            var wifiManager = Application.Context.GetSystemService(Context.WifiService) as WifiManager;

            if (wifiManager != null)
                return wifiManager.IsWifiEnabled;
            else
                return false;
        }
        public bool IsWifiConnected()
        {
            var wifiManager = Application.Context.GetSystemService(Context.WifiService) as WifiManager;

            if (wifiManager != null)
            {
                // Check state is enabled.
                return wifiManager.IsWifiEnabled &&
                    // Check for network id equal to -1
                    (wifiManager.ConnectionInfo.NetworkId != -1
                    // Check for SSID having default value of "<unknown SSID>"
                    && wifiManager.ConnectionInfo.SSID != UNKNOWNSSID);
            }
            return false;
        }

        //public bool IsMobileDataEnabled()
        //{
        //    var connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
        //    var networkInfo = connectivityManager?.ActiveNetworkInfo;
        //    return networkInfo?.Type == ConnectivityType.Mobile;
        //}

        public int WifiSignalPorcen()
        {
            var wifiManager = Application.Context.GetSystemService(Context.WifiService) as WifiManager;
            if (wifiManager != null)
            {
                int rssi = wifiManager.ConnectionInfo.Rssi.GetHashCode();
                //WifiState wifiState = wifiManager.ConnectionInfo.Rssi.GetTypeCode();
                var kinkSpeed = wifiManager.ConnectionInfo.LinkSpeed.GetTypeCode();
                //int linkSpeed = wifiManager.getConnectionInfo().getLinkSpeed();
                //string rssi11 = Convert.ToString(rssi);
                int rssi11 = rssi;
                int kinkSpeed1 = Convert.ToInt32(kinkSpeed);
                int wifiLevel = WifiManager.CalculateSignalLevel(rssi, 5);
            //Toast.MakeText(this, "el rssi es:" + rssi11.ToString() + "LinkSpped:" + kinkSpeed1.ToString() + " Nivel:" + wifiLevel.ToString(), ToastLength.Short).Show();
                return rssi11;
            }
            return 0;
            //int rssi = wifiManager.getConnectionInfo().getRssi();
            //int level = WifiManager.calculateSignalLevel(rssi, 5);
            //System.out.println("Level is " + level + " out of 5");

            //WifiManager wifiManager1 = (WifiManager)GetSystemService(Context.WifiService);
            //WifiInfo info = wifiManager1.getConnectionInfo();
            //String ssid = info.getSSID();
            //int rssi = info.getRssi();
        }
    }
}