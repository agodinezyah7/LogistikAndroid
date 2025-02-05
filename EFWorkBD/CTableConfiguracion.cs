using SQLite;
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
	[Table("ConfigImpresora")]
    class CTableConfiguracion
    {
		static public string configuracionPath = Path.GetFullPath("/sdcard/My Documents/La_Europea/config.txt");
        static public string swnombreimpresoara { get; set; }
        static public string swmacimpresora { get; set; }
        static public string swnumeroterminal { get; set; }
        static public string swSucursal { get; set; }
        static public string swMensaje { get; set; }

        //[PrimaryKey, AutoIncrement, NotNull, Column("id")]
        //public int id { get; set; }

        [Column("nombreimpresoara")]
        public string nombreimpresoara { get; set; }
        [Column("macimpresora")]
        public string macimpresora { get; set; }
        [Column("numeroterminal")]
        public string numeroterminal { get; set; }
        [Column("Sucursal")]
        public string Sucursal { get; set; }
        [Column("Mensaje")]
        public string Mensaje { get; set; }


        public CTableConfiguracion(string totalizar, string validar, string totalizar2, string validar2, string totalizar3)
        {
            swnombreimpresoara = totalizar;
            swmacimpresora = validar;
            swnumeroterminal = totalizar2;
            swSucursal = validar2;
            swMensaje = totalizar3;

        }

        public CTableConfiguracion()

        {

        }



    }
}
