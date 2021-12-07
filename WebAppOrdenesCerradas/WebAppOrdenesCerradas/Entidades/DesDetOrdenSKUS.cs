using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppOrdenesCerradas.Entidades
{
    public class DesDetOrdenSKUS
    {
    }

    public partial class detordenproductoshd 
    {
        public string Orden { get; set; }

        public int QTY { get; set; }

        public string FechaAlta { get; set; }

        public string FechaCierre { get; set; }

        public string SKUDescripcion { get; set; }

        public string OracleID { get; set; }

    }
}