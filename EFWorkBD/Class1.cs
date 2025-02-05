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
using System.Windows.Input;

namespace BilddenLogistik.EFWorkBD
{

    public class Class1
    {
        public static Boolean ctrlprocedimiento = false;
        //---------------ARTICULO LOTE Y PEDIMENTO
        public static string vgArtLotePed = "N";
        //---------------COMBINACION PEDIMENTOS, LOTES Y NUM SERIES
        public static Boolean vgFaltanPedim;
        //---------------COMBINACION LOTES Y NUM SERIES
        public static Boolean vgFaltanLotes;
        public static int TipoProd = 0;
        //public static string cnSQL = @"data source=minimac\sqlexpress;initial catalog=LogistikMasterValvulerias06;user id=sa;password=Mexpym*01;Connect Timeout=600;MultipleActiveResultSets=true";
        //public static string cnSQL = @"data source=http://minimac01.ddns.net;initial catalog=LogistikMasterValvulerias06;user id=sa;password=Mexpym*01;Connect Timeout=600;MultipleActiveResultSets=true";
        public static string cnSQL = @"data source=192.168.1.117,1433;initial catalog=LogistikMasterValvulerias06;user id=sa;password=Aspel2020$;Connect Timeout=600;MultipleActiveResultSets=true";

        //public static string cnSQL = @"data source=192.168.1.151,1433;initial catalog=LogistikMasterValvulerias06;user id=love;password=controlnet;Connect Timeout=600;MultipleActiveResultSets=true";
        //public static string cnSQL = @"data source=192.168.1.68,1433;initial catalog=LogistikMasterValvulerias06;user id=sa;password=controlnet;Connect Timeout=600;MultipleActiveResultSets=true";
        public static string strSQL;
        public static int TotalOrdenes=0;
        public static int TotalOrdenesD = 0;
        public static int conta=0;
        public static string OC;
        public static string Pedido;
        public static string EmbarqueAlmRec;
        public static string FacturaAlmRec;
        public static string ContenidoAlmRec;
        //public static string vgDocRecepcion;
        public static string NombreUsuario;
        public static string ClaveUsuario;
        public static string oldNombreUsuario;
        public static string oldClaveUsuario;
        public static Boolean FlagCodForzado;
        public static int vgTeclado = 1;
        //'----------------lotes
        public static string vgLoteOC;
        public static string vgLoteOC_FechaProd;
        public static string vgLoteOC_FechaCaduc;
        public static string vgLoteRem;
        public static string vgLoteRem_FechaProd;
        public static string vgLoteRem_FechaCaduc;
        public static string vgCtrlLotes;       //'control ventana lotes
        public static string vgParcialLotes;
        //'------------Numeros de serie
        public static int vgNumSer2;
        public static int vFinOrden = 0;
        public static string vgCtrlNumSer;       //'control ventana numeros de serie
        public static string vgNumSer;
        public static string vgAlmOrdCompra="1";     //'control ventana numeros de serie del almacen
        public static decimal vgCantPedida;
        //'-------------articulos tienen pedimento
        public static string vgPedimentoArt = "N";
        public static string vgPedimentoArtComp = "N";
        //'------------------
        public static int vlFinal = 0;
        public static int vlFinalSalida = 0;
        //'--------------------- pedimento de salida = remisiones o pedido
        public static string vgDocPedido;
        public static string vgArtidulo_remi;
        public static string vgLote_remi;
        public static string vgPedimento_remi;
        public static string vgPedCant_remi;
        public static string vgPedFecha_remi;
        public static string vgPedAduana_remi;
        public static string vgPedCiudad_remi;
        public static string vgPedFrontera_remi;
        public static string vgPedAlmacen_remi;
        //  '--------------------- pedimento de entrada = recepciones u orden de compra
        public static string vgDocRecepcion;
        public static string vgPedimento;
        public static string vgPedFecha;
        public static string vgPedAduana;
        public static string vgPedCiudad;
        public static string vgPedFrontera;
        //'----------------------
        public static string vgAlmRemision;  //'almacen de la remision de venta del SAE
        public static string vgNoExiste;
        public static string vgTipoOrdSalidas = "R";
        public static string vgIPServer;
        public static string xmlBatch;
        public static string vgUsuarioProd;
        public static int vgID_employee;
        public static string vgEmployee_first;
        public static string vgEmployee_last;
        public static Boolean vOrdenesActiva;
        public static string vAlamcenSAE;
        public static string vgNumEmpresa;
        public static string vbNomEmpresa;
        public static Boolean vgEspecifDoc;
        public static Boolean vgCodigoAlterno;
        public static Boolean vgTotalizar;
        public static string vgEnt_Sal;
        public static int vgEnt_Sal_Datos=1;
        public static string vgOrder;
        public static string vgOrdCodBar;
        public static string vgNomProv;
        public static string vgOrdenImporte;
        public static string vgPagoEfectivo;
        public static string vgRefeEfectivo;
        public static string vgPagoCheque;
        public static string vgRefeCheque;
        public static string vgPagoChequeCer;
        public static string vgRefeChequeCer;
        public static string vgPagoTransfer;
        public static string vgRefeTransfer;
        public static string vgAlm_Ori;
        public static string vgAlm_Des;
        public static string vgDocTraspasos;
        public static string vgAlm_Ori_Nom;
        public static string vgAlm_Des_Nom;
        public static Boolean FlagVerificador;
        public static string vgEmpresaSelect = "06";
        //'------------Agregado por Carlos
        public static string vbID_Supplier;
        public static decimal Inv_cant;
        public static string vEmbarque;
        public static string vFactura;
        public static string vOC;
        public static int TipoUbica;
        public static string vgOrdCodID = "0";
        public static string vgNTarima;
        public static string vgEmbarque;
        public static string NumeroInventario;
        public static bool FlagIniciaInvFisico;
        public static string UbicaSelect;
        public static bool FlagIniciaUbica = false;
        public static bool FlagListaInvF;
        public static string CaducaIF;
        public static string CodTarimaIF;
        public static bool FlagDetalleInvF;
        public static bool FlagInvFisicoInic;
        public static int vgIDProducto;
        public static string vbUPCProducto;
        public static string vbNomProd;
        public static int NumeroToma;
        public static bool FlagUbica;
        public static bool FlagReubica;
        public static bool FlagSalidaM;
        public static DateTime FechaEmbarque;
        public static decimal CantOriEmbarque;
        public static int vgEmployee_Perfil = 1; //perfil 1 o 8
        public static decimal vgCant_Surtida;
        public static string vgctrlNumSer;
        public static decimal vgCant_Validada;
        public static bool FlagOKSalidaDet;
        public static bool FlagOrdenesEmbarqueCheck;
        public static bool FlagOrdenesEmbarqueDet;
        public static bool FlagLeyoSalidaManual = false;
        public static string vgFechaEmpaque;
        public static string vNumEmpresa;
        public static string vIFEmbarque;    //' Datos para el inventario Fisico
        public static string vIFOC;          // Datos para el inventario Fisico
        public static string vIFFactura;     // Datos para el inventario Fisico
        public static string BanderaClave;   // =1 Para cuando va de un inventario fisico, para autorizar cerrar la ubicacion. 
        public static string vEstado;
        public static bool GeneroTraspasos;   // Para verificar que se hayan hecho los traspasos OK

        //public static SoundEffectConstants sound new Sound("\Windows\alarm3.wav");
        public static double iInvtGlobal;        // Total de productos de Inventario
        public static double iRackGlobal;        // Total de productos de la ubicacion
        public static double iRackcodigo;        // Total de producto
        public static string iInvtDesc;          //' Descripcion del inventario
        public static int iRackNum;          //' Numero de ubicacion actual

        public static bool bTerminal = true;  //' True = Terminal Bitatek, False = Catchwell   CAMBIAR DIRECTORIOS ABAJO!!

        public static bool bInventarioc = true;  // ' True = Inventario, False = Inventario con cantidad
        //'Public flagEditaAgc As Boolean = False      ' Bandera para saber si edito AGC
        public static bool flagControl = false;
        public static string CantidadS;
        public static bool FlagValida = false;      //' Bandera para validar o no contra base de datos
        public static bool FlagPaquete = false;     //' Bandera para validar o no contra base de datos

        public static bool FlagTotaliza = false;      //' Bandera para totalizar o no las lecturas
        public static bool flagControl2 = false;
        public static string MSGerror;
        public static bool FlagCaptura = false;
    }
}