using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Oracle.DataAccess.Client;
using System.Collections.Generic;

namespace IntranetSystem.Models
{
    public class NavigationMenu
    {
        public static string getAllNavBar(string url)
        {
            string navBar = string.Empty;
            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

            string query = "SELECT MID,NAME FROM I_MENU ORDER BY SORT";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows) //主選單
                {
                    while (reader.Read())
                    {
                        navBar += "<li class='dropdown'>"
                            + "<a href='#' class='dropdown-toggle' data-toggle='dropdown'>"
                            + reader["NAME"] + "<b class='caret'></b></a>";
                        string query1 = "SELECT NAME,CODE FROM I_PROGRAM WHERE MID=" + reader["MID"] + " ORDER BY SORT";
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = conn;
                        OracleDataReader reader1 = cmd1.ExecuteReader();
                        if (reader1.HasRows) //程式
                        {
                            navBar += "<ul class='dropdown-menu'>";
                            while (reader1.Read())
                            {
                                navBar += "<li><a href='"+ url + "/" + reader1["CODE"] + "/'>" + reader1["NAME"] + "</a></li>";
                            }
                            navBar += "</ul>";
                        }
                        navBar += "</li>";
                    }
                }
            }            
            return navBar;
        }

        public static string getNavMenu(string id, string url)
        {
            string menu = string.Empty;
            string navMenu = string.Empty;
            string navBar = string.Empty;
            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

            string query = "SELECT MID,NAME FROM I_MENU ORDER BY SORT";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows) //主選單
                {
                    while (reader.Read())
                    {
                        navMenu = "<li class='dropdown'>"
                            + "<a href='#' class='dropdown-toggle' data-toggle='dropdown'>"
                            + reader["NAME"] + "<b class='caret'></b></a>";

                        string query1 = "SELECT PID,NAME,CODE,EXTRA FROM I_PROGRAM WHERE MID=" + reader["MID"] + " ORDER BY SORT";
                        OracleCommand cmd1 = new OracleCommand(query1);
                        cmd1.Connection = conn;
                        OracleDataReader reader1 = cmd1.ExecuteReader();

                        
                        bool isExist = false;

                        // 開始判斷是否有程式權限
                        if (reader1.HasRows)
                        {                            
                            navBar = "<ul class='dropdown-menu'>";
                            while (reader1.Read())
                            {
                                string aid = id.ToString();
                                string pid = reader1["PID"].ToString();
                                bool hasNavBar = false;

                                // 1.檢查部門的群組是否有權限
                                string query2 = "SELECT A.AID FROM I_USER A, I_UD_REF01 B, I_DG_REF01 C, I_GP_REF01 D WHERE A.AID=" + aid +" AND A.AID=B.AID AND B.DID=C.DID AND C.GID=D.GID AND D.PID=" + pid;
                                OracleCommand cmd2 = new OracleCommand(query2);
                                cmd2.Connection = conn;
                                OracleDataReader reader2 = cmd2.ExecuteReader();
                                if (reader2.HasRows)
                                {
                                    hasNavBar = true;
                                    isExist = true;
                                }
                                else // 2.再檢查部門的程式是否有權限
                                {
                                    string query3 = "SELECT A.AID FROM I_USER A, I_UD_REF01 B, I_DP_REF01 C WHERE A.AID=" + aid + " AND A.AID=B.AID AND B.DID=C.DID AND C.PID=" + pid;
                                    OracleCommand cmd3 = new OracleCommand(query3);
                                    cmd3.Connection = conn;
                                    OracleDataReader reader3 = cmd3.ExecuteReader();
                                    if (reader3.HasRows)
                                    {
                                        hasNavBar = true;
                                        isExist = true;
                                    }
                                    else // 3.再檢查使用者的群組是否有權限
                                    {
                                        string query4 = "SELECT A.AID FROM I_USER A, I_UG_REF01 B, I_GP_REF01 C WHERE A.AID=" + aid + " AND A.AID=B.AID AND B.GID=C.GID AND C.PID=" + pid;
                                        OracleCommand cmd4 = new OracleCommand(query4);
                                        cmd4.Connection = conn;
                                        OracleDataReader reader4 = cmd4.ExecuteReader();
                                        if (reader4.HasRows)
                                        {
                                            hasNavBar = true;
                                            isExist = true;
                                        }
                                        else // 4.再檢查使用者的程式是否有權限
                                        {
                                            string query5 = "SELECT A.AID FROM I_USER A, I_UP_REF01 B WHERE A.AID=" + aid + " AND A.AID=B.AID AND B.PID=" + pid;
                                            OracleCommand cmd5 = new OracleCommand(query5);
                                            cmd5.Connection = conn;
                                            OracleDataReader reader5 = cmd5.ExecuteReader();
                                            if (reader5.HasRows)
                                            {
                                                hasNavBar = true;
                                                isExist = true;
                                            }
                                        }
                                    }
                                }
                                
                                //程式存在，才加入主選單
                                if (hasNavBar)
                                {
                                    //判斷是否外部連結
                                    if (string.IsNullOrEmpty(reader1["EXTRA"].ToString()))
                                    {
                                        navBar += "<li><a href='" + url + "/" + reader1["CODE"] + "/'>" + reader1["NAME"] + "</a></li>";
                                    }
                                    else
                                    {
                                        navBar += "<li><a href='" + url + "/Extra/Go/" + reader1["PID"] + "'>" + reader1["NAME"] + "</a></li>";
                                    }
                                }
                            }
                            navBar += "</ul>";
                        }

                        //有程式權限,才顯示主選單
                        if (isExist) menu += navMenu + navBar + "</li>";
                    }
                }
            }
            return menu;         
        }

        
        //檢查USER是否有權限
        public static bool chkUserAuth(string userid, string controller, string id="")
        {
            bool result = false;
            if (controller == "DepartMapping" || controller == "UserPermit")
            {
                controller = "User";
            }
            else if (controller == "DepartPermit")
            {
                controller = "Department";
            }
            else if (controller == "ProgramMapping")
            {
                controller = "Group";
            }
            else if (controller == "PrinterMapping" || controller == "PrinterDepartment")
            {
                controller = "Material";
            }

            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();

                //取得PID
                string pid = "0";
                if (controller == "Extra")
                {
                    pid = id;
                }
                else
                {
                    string query1 = "SELECT PID FROM I_PROGRAM WHERE CODE='" + controller + "'";
                    OracleCommand cmd1 = new OracleCommand(query1);
                    cmd1.Connection = conn;
                    OracleDataReader reader1 = cmd1.ExecuteReader();
                    if (reader1.HasRows) pid = reader1["PID"].ToString();
                }

                // 1.檢查部門的群組是否有權限
                string query2 = "SELECT A.AID FROM I_USER A, I_UD_REF01 B, I_DG_REF01 C, I_GP_REF01 D WHERE A.USERID='" + userid + "' AND A.AID=B.AID AND B.DID=C.DID AND C.GID=D.GID AND D.PID=" + pid;
                OracleCommand cmd2 = new OracleCommand(query2);
                cmd2.Connection = conn;
                OracleDataReader reader2 = cmd2.ExecuteReader();
                if (reader2.HasRows)
                {
                    result = true;
                }
                else // 2.再檢查部門的程式是否有權限
                {
                    string query3 = "SELECT A.AID FROM I_USER A, I_UD_REF01 B, I_DP_REF01 C WHERE A.USERID='" + userid + "' AND A.AID=B.AID AND B.DID=C.DID AND C.PID=" + pid;
                    OracleCommand cmd3 = new OracleCommand(query3);
                    cmd3.Connection = conn;
                    OracleDataReader reader3 = cmd3.ExecuteReader();
                    if (reader3.HasRows)
                    {
                        result = true;
                    }
                    else // 3.再檢查使用者的群組是否有權限
                    {
                        string query4 = "SELECT A.AID FROM I_USER A, I_UG_REF01 B, I_GP_REF01 C WHERE A.USERID='" + userid + "' AND A.AID=B.AID AND B.GID=C.GID AND C.PID=" + pid;
                        OracleCommand cmd4 = new OracleCommand(query4);
                        cmd4.Connection = conn;
                        OracleDataReader reader4 = cmd4.ExecuteReader();
                        if (reader4.HasRows)
                        {
                            result = true;
                        }
                        else // 4.再檢查使用者的程式是否有權限
                        {
                            string query5 = "SELECT A.AID FROM I_USER A, I_UP_REF01 B WHERE A.USERID='" + userid + "' AND A.AID=B.AID AND B.PID=" + pid;
                            OracleCommand cmd5 = new OracleCommand(query5);
                            cmd5.Connection = conn;
                            OracleDataReader reader5 = cmd5.ExecuteReader();
                            if (reader5.HasRows)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        //取得USER所屬部門
        public static List<string> getDepartmentData(string aid)
        {
            List<string> result = new List<string>();

            string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(constr))
            {
                conn.Open();
                string query = "SELECT A.NAME FROM I_DEPARTMENT A, I_UD_REF01 B WHERE A.DID=B.DID AND B.AID=" + aid;
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = conn;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(reader["NAME"].ToString());
                    }
                }
            }
            return result;
        }
    }
}


