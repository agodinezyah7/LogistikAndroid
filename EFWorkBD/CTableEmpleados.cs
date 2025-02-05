using SQLite;
//using Mono.Data.Sqlite;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
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
namespace BilddenLogistik.EFWorkBD
{
	[Table("Empleados")]
    public class CTableEmpleados
    {
		//SELECT id, Codigo, Nombre, Clave, Rol FROM Empleados
		[PrimaryKey, NotNull, Column("id")]
        public int id { get; set; }
        [Column("Codigo"), MaxLength(15)]
        public string Codigo { get; set; }
		[Column("Nombre"), MaxLength(50)]
		public string Nombre { get; set; }
		[Column("Clave"), MaxLength(10)]
		public string Clave { get; set; }
		[Column("Rol"), MaxLength(5)]
		public string Rol { get; set; }

		public CTableEmpleados(string Id, string codigo, string nombre, string clave, string rol)
        {
            id = Convert.ToInt32(Id);
            Codigo = codigo;
			Nombre = nombre;
			Clave = clave;
			Rol = rol;
        }
    }
}
