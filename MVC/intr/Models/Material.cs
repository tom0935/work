using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class Material
    {
        public decimal RN { get; set; }
        public decimal MID { get; set; }
        public decimal TID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string SPEC { get; set; }
        public string UNIT { get; set; }
        public decimal SAFETY { get; set; }
        public decimal INVENTORY { get; set; }
        public string REMARK { get; set; }
        public decimal PQTY { get; set; }
        public string TREF { get; set; }
    }

    public class MaterialDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<Material> LoadMaterial(int id = 1, int pageSize = 20, int page = 1)
        {
            int startRN = ((page - 1) * pageSize) + 1; //開始號
            int endRN = (page * pageSize); //結束號

            string query = "SELECT * FROM"
                + " (SELECT ROWNUM RN,MID,TID,CODE,NAME,SPEC,UNIT,SAFETY,REMARK FROM"
                + " (SELECT * FROM I_MATERIAL WHERE TID=" + id.ToString() + " AND ENABLE=1 ORDER BY CODE)"
                + " ) WHERE RN BETWEEN " + startRN + " AND " + endRN;

            List<Material> materials = new List<Material>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int inAmount = 0; //進貨數量
                        int outAmount = 0; //出貨數量
                        int inventory = 0; //剩餘=進貨-出貨

                        Material material = new Material();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = material.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(material, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        inAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_INCOMINGDETAILS A, I_INCOMINGORDER B", "A.MID=" + material.MID + " AND A.IID=B.IID AND A.ENABLE=1 AND B.STATUS>=2"));
                        outAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + material.MID + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS>=2"));
                        inventory = inAmount - outAmount;

                        material.INVENTORY = inventory;
                        material.PQTY = decimal.Parse(Models.Helper.getFieldValue("COUNT(1)", "I_DT_REF01", "MID=" + material.MID + " AND NOSHOW=1"));
                        if (id == 2) material.TREF = GetTonerRef(material.MID);
                        materials.Add(material);
                    }
                }
                reader.Close();
            }
            return materials;
        }

        //新增
        public void CreateMaterial(Material material)
        {
            string query = "INSERT INTO I_MATERIAL (TID, NAME, CODE, SPEC, UNIT, SAFETY, REMARK) VALUES(:TID, :NAME, :CODE, :SPEC, :UNIT, :SAFETY, :REMARK)";

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("TID", OracleDbType.Decimal).Value = material.TID;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = material.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = material.CODE;
                cmd.Parameters.Add("SPEC", OracleDbType.Varchar2).Value = material.SPEC;
                cmd.Parameters.Add("UNIT", OracleDbType.Varchar2).Value = material.UNIT;
                cmd.Parameters.Add("SAFETY", OracleDbType.Decimal).Value = material.SAFETY;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = material.REMARK;
                cmd.ExecuteNonQuery();
            }
        }



        //編輯程式
        public Material EditMaterial(int id)
        {
            string query = "SELECT TID, NAME, CODE, SPEC, UNIT, SAFETY, REMARK FROM I_MATERIAL WHERE MID=" + id.ToString();

            Material material = new Material();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    material.MID = (decimal)id;
                    material.TID = decimal.Parse(reader["TID"].ToString());
                    material.NAME = reader["NAME"].ToString();
                    material.CODE = reader["CODE"].ToString();
                    material.SPEC = reader["SPEC"].ToString();
                    material.UNIT = reader["UNIT"].ToString();
                    material.SAFETY = decimal.Parse(reader["SAFETY"].ToString());
                    material.REMARK = reader["REMARK"].ToString();
                }
                reader.Close();
            }
            return material;
        }

        //更新程式
        public void UpdateMaterial(Material material)
        {
            string query = "UPDATE I_MATERIAL SET TID=:TID, NAME=:NAME, CODE=:CODE, SPEC=:SPEC, UNIT=:UNIT, SAFETY=:SAFETY, REMARK=:REMARK WHERE MID=" + material.MID;

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                cmd.Parameters.Add("TID", OracleDbType.Decimal).Value = material.TID;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = material.NAME;
                cmd.Parameters.Add("CODE", OracleDbType.Varchar2).Value = material.CODE;
                cmd.Parameters.Add("SPEC", OracleDbType.Varchar2).Value = material.SPEC;
                cmd.Parameters.Add("UNIT", OracleDbType.Varchar2).Value = material.UNIT;
                cmd.Parameters.Add("SAFETY", OracleDbType.Decimal).Value = material.SAFETY;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = material.REMARK;
                cmd.ExecuteNonQuery();
            }
        }

        //刪除程式
        public void DeleteMaterial(int id)
        {
            string query = "DELETE FROM I_MATERIAL WHERE MID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        //取得歸屬選單SELECT，新增與修改時使用
        public string GetTypeList()
        {
            string val = string.Empty;
            string query = "SELECT TID,NAME FROM I_MATERIALTYPE WHERE ENABLE=1";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        val += "<option value=\"" + reader["TID"] + "\">" + reader["NAME"] + "</option>";
                    }
                }
            }
            return val;
        }

        //有預設值的取得歸屬選單SELECT
        public string GetTypeList(decimal tid)
        {
            string val = string.Empty;
            string query = "SELECT TID,NAME FROM I_MATERIALTYPE WHERE ENABLE=1";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader["TID"].ToString() == tid.ToString())
                            val += "<option value=\"" + reader["TID"] + "\" selected>" + reader["NAME"] + "</option>";
                        else
                            val += "<option value=\"" + reader["TID"] + "\">" + reader["NAME"] + "</option>";
                    }
                }
            }
            return val;
        }

        //取得碳粉對應的印表機
        public string GetTonerRef(decimal mid)
        {
            int i = 0;
            string result = string.Empty;
            string query = "SELECT B.CODE, B.NAME PNAME, D.NAME DNAME, C.TEAM FROM I_PT_REF01 A, I_MATERIAL B, I_DT_REF01 C, I_DEPARTMENT D "
                + "WHERE A.TONER=" + mid + " AND A.PRINTER=B.MID AND B.MID=C.MID AND C.DID=D.DID "
                + "ORDER BY B.CODE,D.NAME";
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        i++;
                        //if (result != "") result += "\n"; //加入換行符號
                        result += "<tr><td>" + i + "</td><td>" + reader["CODE"] + "</td><td>" + reader["PNAME"] + "</td><td>" + reader["DNAME"];
                        result += (string.IsNullOrEmpty(reader["TEAM"].ToString())) ? "" : "(" + reader["TEAM"] + ")";
                        result += "</td><td><tr> \n";
                    }
                }
            }
            result = (string.IsNullOrEmpty(result)) ? "沒有對應的印表機..." : result;
            return result;
        }

        //搜尋結果
        public List<Material> SearchMaterial(int id, string keywords)
        {
            string query = "SELECT MID,TID,CODE,NAME,SPEC,UNIT,SAFETY,REMARK FROM I_MATERIAL "
                + "WHERE TID=" + id.ToString() + " AND ENABLE=1 AND "
                + "(CODE LIKE '%" + keywords + "%' OR NAME LIKE '%" + keywords + "%')"
                + "ORDER BY CODE";

            List<Material> materials = new List<Material>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int inAmount = 0; //進貨數量
                        int outAmount = 0; //出貨數量
                        int inventory = 0; //剩餘=進貨-出貨

                        Material material = new Material();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = material.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(material, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        inAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_INCOMINGDETAILS A, I_INCOMINGORDER B", "A.MID=" + material.MID + " AND A.IID=B.IID AND A.ENABLE=1 AND B.STATUS>=2"));
                        outAmount = int.Parse(Models.Helper.getFieldValue("NVL(SUM(A.QTY),0)", "I_REQUISITIONDETAILS A, I_REQUISITIONORDER B", "A.MID=" + material.MID + " AND A.RID=B.RID AND A.ENABLE=1 AND B.STATUS>=2"));
                        inventory = inAmount - outAmount;

                        material.INVENTORY = inventory;
                        material.PQTY = decimal.Parse(Models.Helper.getFieldValue("COUNT(1)", "I_DT_REF01", "MID=" + material.MID + " AND NOSHOW=1"));
                        if (id == 2) material.TREF = GetTonerRef(material.MID);
                        materials.Add(material);
                    }
                }
                reader.Close();
            }
            return materials;
        }


    }

}