using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.DataVisualization.Charting;

namespace IntranetSystem.Controllers
{
    public class ChartsController : Controller
    {
        //
        // GET: /Charts/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Charts/TestLine

        public ActionResult TestLine()
        {
            //建立一個新的Chart
            Chart chart = new Chart();
            chart.Width = 400;
            chart.Height = 200;
            chart.ID = "Chart1";

            //建立一個ChartAreas，並對ChartAreas設定格式
            chart.ChartAreas.Add("ChartAreas1");
            
            chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = false;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = false;
            chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            
            //新增一個Series
            chart.Series.Add("Series1");
            chart.Series["Series1"].Points.AddXY("Taipei1", 1);
            chart.Series["Series1"].Points.AddXY("Taipei2", 10);
            chart.Series["Series1"].Points.AddXY("Taipei3", 100);

            //顯示長條圖的數據
            chart.Series["Series1"].IsValueShownAsLabel = true;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            return File(ms, "image/png");
        }

        //
        // GET: /Charts/Create

        public ActionResult TonerLineChart()
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            Dictionary<string, int> toners = Models.Charts.getTonerCount(aid);

            //建立一個Chart
            Chart chart = new Chart();
            chart.Width = 800;
            chart.Height = 200;
            chart.ID = "Chart1";

            //建立一個ChartAreas，並對ChartAreas設定格式
            chart.ChartAreas.Add("ChartAreas1");

            chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = false;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = false;
            chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.Interval = 1;
            chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);

            //新增一個Series
            chart.Series.Add("Series1");
            foreach (var item in toners)
            {
                chart.Series["Series1"].Points.AddXY(item.Key, item.Value);
            }
            //顯示長條圖的數據
            chart.Series["Series1"].IsValueShownAsLabel = true;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            return File(ms, "image/png");
        }

    }
}
