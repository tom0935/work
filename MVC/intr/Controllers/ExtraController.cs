using System.Web.Mvc;
using System.Configuration;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Controllers
{
    public class ExtraController : Controller
    {
        
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        //
        // GET: /Extra/Go/5

        public ActionResult Go(int id)
        {
            string query = "SELECT PID,NAME,EXTRA FROM I_PROGRAM WHERE PID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    ViewData.Add("PID", reader["PID"].ToString());
                    ViewData.Add("NAME", reader["NAME"].ToString());
                    ViewData.Add("EXTRA", reader["EXTRA"].ToString());
                }
            }
            return View();
        }

    }
}
