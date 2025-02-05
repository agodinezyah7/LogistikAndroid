using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using SQLite.Net.Attributes;
using SQLite;

namespace BilddenLogistik.EFWorkBD
{
    [Table("Configuracion")]
    public class Configuracion
    {
        [Column("id"), PrimaryKey, NotNull]
        public int id { get; set; }
        [Column("ip_data_source")]
        public string ipDataSource { get; set; }
        [Column("Port"), MaxLengthAttribute(50)]
        public string port { get; set; }
        [Column("User"), MaxLengthAttribute(50)]
        public string user { get; set; }
        [Column("Password"), MaxLengthAttribute(50)]
        public string password { get; set; }
        [Column("DBName"), MaxLengthAttribute(50)]
        public string dbName { get; set; }
        [Column("local_remote"), MaxLengthAttribute(50)]
        public string local_remote { get; set; }


        public Configuracion()
        {
        }
        ~Configuracion()
        {

        }
        public Configuracion(int id, string ipAdress, string portNumber, string dbNameAccess, string userName, string passwordDB, string remote)
        {
            this.id = id;
            ipDataSource = ipAdress;
            port = portNumber;
            dbName = dbNameAccess;
            user = userName;
            password = passwordDB;
            local_remote = remote;
        }


    }
}
