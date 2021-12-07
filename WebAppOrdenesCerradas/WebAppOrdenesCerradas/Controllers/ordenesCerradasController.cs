using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOrdenesCerradas.Entidades;
using System.Linq.Dynamic;
using System.Globalization;

namespace WebAppOrdenesCerradas.Controllers
{
    public class ordenesCerradasController : Controller
    {
        DB_A3F19C_OGEntities db = new DB_A3F19C_OGEntities();

        // GET: ordenesCerradas
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerOrdenes()
        {
            try
            {
                var Draw = Request.Form.GetValues("draw").FirstOrDefault();
                var Start = Request.Form.GetValues("start").FirstOrDefault();
                var Length = Request.Form.GetValues("length").FirstOrDefault();
                var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var fechaInicio = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var fechaFin = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

                int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
                int Skip = Start != null ? Convert.ToInt32(Start) : 0;
                int TotalRecords = 0;

                List<detordenproductoshd> Inventario = new List<detordenproductoshd>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_PIEZAS_ORDENESCERRADAS] @fechaFInicio, @fechaFFin";
                    var query = new SqlCommand(sql, con);

                    CultureInfo ci = new CultureInfo("es-MX");
                    ci = new CultureInfo("es-MX");

                    if (fechaInicio != "")
                    {
                        DateTime date = Convert.ToDateTime(fechaInicio, ci);
                        query.Parameters.AddWithValue("@fechaFInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFInicio", DBNull.Value);
                    }

                    if (fechaFin != "")
                    {
                        DateTime date = Convert.ToDateTime(fechaFin, ci);

                        query.Parameters.AddWithValue("@fechaFFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFFin", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var detalle = new detordenproductoshd();
                                                        
                            detalle.Orden = dr["Orden"].ToString();
                            detalle.OracleID = dr["User"].ToString();
                            detalle.QTY = Convert.ToInt32(dr["cantidad"]);
                            detalle.SKUDescripcion = dr["Sku"].ToString();
                            detalle.FechaAlta = dr["FechaAlta"].ToString();
                            detalle.FechaCierre = dr["FechaCierre"].ToString();

                            Inventario.Add(detalle);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    Inventario = Inventario.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = Inventario.ToList().Count();
                var NewItems = Inventario.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                var json1 = Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);

                json1.MaxJsonLength = 999999999;

                return json1;
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }
    }
}