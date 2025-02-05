using System.IO;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using SQLite;
using System.Data.Sql;
using System.Data.SqlClient;
using System;
using System.Data;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
//using System.Reflection;
using BilddenLogistik.EFWorkBD;
using BilddenLogistik.MainActivities;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;

using Java.Net;
using Java.Util;
namespace BilddenLogistik.EFWorkBD
{
  
    public class CrearBD
    {
        #region LocalVar
        static List<string> rutas = new List<string>();
        static public string vgWindows;
        static public string vgUser2018;
        static public string vgsuc2018;
        static public string ex;
        static public string Msgerror;
        static public int vgOrder;
        //Rutas de acceso a Archivos y BD
        //static public string dbPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/Inventar.sqlite3");
        //public string DirPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden");
        //public static string settPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/MODB.txt");
        //static public string productoPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/sku.txt");
        //static public string inventarioTxtPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/Inventario.txt");
        //static public string inventarioTsvPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/Inventario.txt");
        //static public string inventarioCsvPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/Inventario.csv");
        //static public string parametrosPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/parametros.ini");
        //static public string configuracionPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/config.txt");
        //static public string nProductPath = Path.GetFullPath("/sdcard/My Documents/LogistikBildden/nuevosProductos.txt");
        //palma cola de pato es para prostata cocida en 2 litros de agua; cola de vaca 

        static public string dbPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/Inventar.sqlite3");
        public string DirPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden");
        public static string settPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/MODB.txt");
        static public string productoPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/sku.txt");
        static public string inventarioTxtPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/Inventario.txt");
        static public string inventarioTsvPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/Inventario.txt");
        static public string inventarioCsvPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/Inventario.csv");
        static public string parametrosPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/parametros.ini");
        static public string configuracionPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/config.txt");
        static public string nProductPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden/nuevosProductos.txt");
        SqliteConnection conexion = new SqliteConnection("Data Source=" + dbPath);
        SqliteConnection conexion2 = new SqliteConnection("Data Source=" + dbPath);
        SqliteConnection conn = new SqliteConnection("Data Source=" + dbPath);

        #endregion
        //    ConexionController con = new ConexionController();
        //    cTripleDES decry = new cTripleDES();
        //    SqlConnection miConexion = new SqlConnection("data source = snare.arvixe.com; initial catalog=logins; user id=******; password= ******");
        //    public int Login(string user, string password)
        //    {
        //        try
        //        {
        //            if (miConexion.State == ConnectionState.Closed)
        //            {
        //                miConexion.Open();
        //            }
        //            SqlCommand comando = new SqlCommand("select email, clave from CrmCustomers where email = '" + user + "'and clave = '" + password + "' ", miConexion);
        //            comando.ExecuteNonQuery();
        //            DataSet ds = new DataSet();
        //            SqlDataAdapter da = new SqlDataAdapter(comando);
        //            da.Fill(ds, "CrmCustomers");
        //            DataRow DR;
        //            DR = ds.Tables["CrmCustomers"].Rows[0];
        //            if ((user == DR["email"].ToString()) && (password == DR["clave"].ToString()))
        //            {
        //                miConexion.Close();
        //                return 1;
        //            }
        //            else
        //            {
        //                miConexion.Close();
        //                return 2;
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            miConexion.Close();
        //            return 3;
        //        }
        //    }

        #region Usuarios
        //USUARIOS
        public string ValidarUsuario(string nombreUsuario, string contrasenia)
        {
            if (ExisteUsuario(nombreUsuario))
            {
                if (ContraseniaCorrecta(nombreUsuario, contrasenia))
                {
                    return "S";
                }
                return "N";
            }
            return null;
        }
        public bool ExisteUsuario(string nombreUsuario)//comprueba la credencial del nombre del usuario.
        {
            try
            {
                conexion.Open();
                var cmd = conexion.CreateCommand();
                SqliteDataReader reader;
                cmd.CommandText = "Select * from Empleados WHERE Codigo='" + nombreUsuario + "'";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    conexion.Close();
                    return true;
                }
                conexion.Close();
            }
            catch (SqliteException ex)
            {
                Msgerror = ex.Message;
                conexion.Close();
                //Tener cuidado con no confundir la excepcion con la inexistencia en BD.
                return false;
            }
            return false;
        }
        public bool ContraseniaCorrecta(string nombreUsuario, string contraseniaUsuario)//en caso de existir el usuario, comprueba contrase馻.
        {
            try
            {
                conexion.Open();
                SqliteCommand cmd = conexion.CreateCommand();
                SqliteDataReader reader;
                cmd.CommandText = "SELECT * from Empleados WHERE Codigo = '" + nombreUsuario + "' and Clave = '" + contraseniaUsuario + "'";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    conexion.Close();
                    return true;
                }
                conexion.Close();
            }
            catch (SqliteException ex)
            {
                Msgerror = ex.Message;
                conexion.Close();
            }
            return false;
        }
        public string RegistrarUsuario(CTableEmpleados nuevoUsuario)
        {
            try
            {
                SQLiteConnection conectar = new SQLiteConnection(dbPath);
                conectar.Insert(nuevoUsuario);
                conectar.Close();
                return ">Usuario: " + nuevoUsuario.Nombre.ToString() + "\nha sido registrado.";
            }
            catch (SqliteException sqlex)
            {
                return sqlex.ToString();
            }
        }
        public string BorrarUsuario(string nombreUsuario)
        {
            try
            {
                conexion.Open();
                SqliteCommand cmd = new SqliteCommand(conexion);
                cmd.CommandText = "DELETE FROM Empleados WHERE Nombre = '" + nombreUsuario + "'";
                SqliteDataReader reader;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return ">Usuario: " + nombreUsuario + "\nha sido eliminado.";
                }
                else
                {
                    return "No se encontro Usuario: " + nombreUsuario + "\n en el registro.";
                }
            }
            catch (SqliteException sqlex)
            {
                return sqlex.ToString();
            }
        }
        #endregion

        //Operaciones sobre Archivos
        public bool BorrarRutas()//Borrar el listado de rutas de acceso
        {
            if (rutas.Count > 0)
            {
                rutas.Clear();
                return true;
            }
            else
                return false;
        }

        public void CargarRutasAcceso()//Crear la lista de rutas de acceso a los archivos de trabajo.
        {
            rutas.Add(dbPath);
            rutas.Add(settPath);
            rutas.Add(productoPath);
            rutas.Add(parametrosPath);
            rutas.Add(configuracionPath);
        }

        public bool ComprobarArchivos()//Comprueba la existencia de los archivos.
        {
            int busqueda = 0;
            foreach (string ruta in rutas)
            {
                if (!File.Exists(ruta))
                {
                    busqueda = 1;
                }
            }
            if (busqueda == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string CrearArchivos()//Crear los archivos necesarios en nuevo directorio en memoria interna.
        {
            var dbnPath = Path.GetFullPath("/storage/emulated/O/My Documents/LogistikBildden");
            var resultado = "";
            try
            {
                if (!Directory.Exists(dbnPath))
                {
                    Directory.CreateDirectory(dbnPath);

                    foreach (string ruta in rutas)
                    {
                        using (FileStream fs = new FileStream(ruta, FileMode.OpenOrCreate))
                        {
                            if (ruta == configuracionPath)
                            {
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.Write("Presentarlo en caja...");
                                    sw.Close();
                                }
                            }
                            if (ruta == parametrosPath)
                            {
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.Write("192.168.1.68");
                                    sw.Close();
                                }
                            }


                            if (ruta == settPath)
                            {
                                using (StreamWriter sr = new StreamWriter(fs))
                                {
                                    sr.WriteLine("192.168.1.150");
                                    sr.WriteLine("1433");
                                    sr.WriteLine("TransferDB");
                                    sr.WriteLine("sa");
                                    sr.WriteLine("PASS");
                                    sr.WriteLine("0");//1 Valor de conexion Local, 0 Para conexion remota.
                                    sr.Close();
                                }
                            }


                            fs.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return resultado += "ERROR0: No se pudieron en crear algunos archivos: " + ex.ToString();
            }
            try
            {
                SQLiteConnection dbTable = new SQLiteConnection(dbPath);
                //dbTable.CreateTable<CTableDatos>();
                //dbTable.CreateTable<CTableInventarios>();
                //dbTable.CreateTable<CTableInventariosT>();
                //dbTable.CreateTable<CTableUbicaciones>();
                //dbTable.CreateTable<CTableMarbetes>();
                //dbTable.CreateTable<CTableIDDispositivos>();
                //dbTable.CreateTable<CTableArticulos>();
                //dbTable.CreateTable<CTableAlmacenes>();
                //dbTable.CreateTable<CTableConteos>();
                dbTable.CreateTable<CTableEmpleados>();
                dbTable.CreateTable<Configuracion>();
                //INSERT INTO [Empleados] ([Codigo], [Nombre], [Clave], [Rol]) VALUES (@p1, @p2, @p3, @p4);
                CTableEmpleados newUser = new CTableEmpleados("999", "123", "Operador", "123", "1");
                dbTable.Insert(newUser);
                //CTableIDDispositivos newUser2 = new CTableIDDispositivos("Centro", "0");
                //dbTable.Insert(newUser2);
                //CTableConfiguracion newConf = new CTableConfiguracion("nombreimpresora", "macimpresora", "1","2024","Mensaje");
                dbTable.CreateTable<CTableConfiguracion>();
                //dbTable.Insert(newConf);
                dbTable.Close();

            }
            catch (SqliteException ex)
            {
                return resultado += "ERROR: Fallo en diseño de Esquema de Base de Datos: " + ex.ToString();
                //return false;
            }
            return resultado += "Archivos creados en ruta: " + dbnPath;
        }


        //public static List<ClassListaCompras> ObtenerEmpleados()
        //{
        //    List<ClassListaCompras> listaEmpleados = new List<ClassListaCompras>();
        //    string sql = "SELECT * FROM Empleados";

        //    using (SqlConnection con = new SqlConnection(Class1.strSQL))
        //    {
        //        con.Open();

        //        using (SqlCommand comando = new SqlCommand(sql, con))
        //        {
        //            using (SqlDataReader reader = comando.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    ClassListaCompras empleado = new ClassListaCompras()
        //                    {
        //                        id_supply_order = reader.GetString(0), // reader.GetInt32(0),
        //                        Nombre = reader.GetString(1),
        //                        Salario = reader.GetDecimal(2)
        //                    };

        //                    listaEmpleados.Add(empleado);
        //                }
        //            }
        //        }

        //        con.Close();

        //        return listaEmpleados;
        //    }
        //}

        //public static void AgregarEmpleado(ClassListaCompras empleado)
        //{
        //    string sql = "INSERT INTO Empleados (Nombre,Salario) VALUES(@nombre, @salario)";

        //    using (SqlConnection con = new SqlConnection(Class1.strSQL))
        //    {
        //        con.Open();

        //        using (SqlCommand comando = new SqlCommand(sql, con))
        //        {
        //            comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = empleado.Nombre;
        //            comando.Parameters.Add("@salario", SqlDbType.Decimal).Value = empleado.Salario;
        //            comando.CommandType = CommandType.Text;
        //            comando.ExecuteNonQuery();
        //        }

        //        con.Close();
        //    }
        //}
        public static void updateUbica(string ubica,string cantidad)
        {
            var sqllocal = "Update Marbetes set cantidad ='" + cantidad + "'" +
            " WHERE ClaveInv = '" + Class1.FacturaAlmRec + "'" +
            " AND ubica = '" + ubica + "'";
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                SqlCommand sqlcmd2 = new SqlCommand(sqllocal, con);
                sqlcmd2.ExecuteNonQuery();
            }

         }
        public static void updateInventario(string maininfo2, string CodCorto, string CodSKU, string txtVTotalMarbete, string editTextCantCompras)
        {
            
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                
                try
                {
                    con.Open();
                    var sqllocal = "insert into conteos(JOBN, CEDTN, CEDLN, CITM, CLOCN, CEDSP, CMCU, CTQOH, Flag) VALUES ('" +
                    Class1.EmbarqueAlmRec + "','" + Class1.FacturaAlmRec + "','" + Class1.FacturaAlmRec + "','" +
                    Class1.EmbarqueAlmRec + "','" + Class1.FacturaAlmRec + "','" + Class1.FacturaAlmRec + "','" +
                    Class1.EmbarqueAlmRec + "','" + Class1.FacturaAlmRec + "','" + Class1.FacturaAlmRec + "','";
                    SqlCommand sqlcmd2 = new SqlCommand(sqllocal, con);
                    sqlcmd2.ExecuteNonQuery();


                }
                catch(SqlException sqlex)
                {
                    string h = sqlex.Message;
                }
            }
        }
        //public static void ModificarEmpleado(ClassListaCompras empleado)
        //{
        //    string sql = "UPDATE Empleados set Nombre = @nombre, Salario = @salario WHERE ID = @id";

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(Class1.strSQL))
        //        {
        //            con.Open();

        //            using (SqlCommand comando = new SqlCommand(sql, con))
        //            {
        //                comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = empleado.Nombre;
        //                comando.Parameters.Add("@salario", SqlDbType.Decimal).Value = empleado.Salario;
        //                comando.Parameters.Add("@id", SqlDbType.Int).Value = empleado.ID;
        //                comando.CommandType = CommandType.Text;
        //                comando.ExecuteNonQuery();
        //            }

        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();

        //    }
        //}

        //public static void EliminarEmpleado(ClassListaCompras empleado)
        //{
        //    string sql = "DELETE FROM Empleados WHERE id_supply_order = @id_supply_order";

        //    using (SqlConnection con = new SqlConnection(Class1.strSQL))
        //    {
        //        con.Open();

        //        using (SqlCommand comando = new SqlCommand(sql, con))
        //        {
        //            comando.Parameters.Add("@id_supply_order", SqlDbType.Int).Value = empleado.id_supply_order;
        //            comando.CommandType = CommandType.Text;
        //            comando.ExecuteNonQuery();
        //        }

        //        con.Close();
        //    }
        //}
        public void NextDocument()
        {
            using (SqlConnection con = new SqlConnection(Class1.cnSQL))
            {
                con.Open();
                //Toast.MakeText(this, "Se abrió la conexión con el servidor SQL Server y se seleccionó la base de datos", ToastLength.Short).Show();
                Class1.strSQL = "SELECT num_traspaso FROM Logistik_Consecutivo_SAE where num_doc=1";
                SqlCommand command = new SqlCommand(Class1.strSQL, con);
                SqlDataReader reader = command.ExecuteReader();
                vgOrder = 1;
                while (reader.Read())
                {
                    vgOrder = reader.GetInt32(0);
                    vgOrder = vgOrder + 1;
                }
                Class1.strSQL = "UPDATE Logistik_Consecutivo_SAE SET num_traspaso=" + vgOrder.ToString();
                SqlCommand sqlcmd5 = new SqlCommand(Class1.strSQL, con);
                sqlcmd5.ExecuteNonQuery();
                //EjecutarQuerySQL(sqlt)
                //EjecutarQuerySQLWifi(sqlt)
            }
        }
        
    }
}
