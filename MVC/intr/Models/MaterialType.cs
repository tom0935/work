using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace IntranetSystem.Models
{
    public class MaterialType
    {
        public decimal TID { get; set; }
        public string NAME { get; set; }
        public string REMARK { get; set; }
    }

    public class MaterialTypeDataContext
    {
        private string constr = ConfigurationManager.ConnectionStrings["OracleServices"].ConnectionString;

        public List<MaterialType> LoadMaterialType()
        {
            string query = "SELECT TID,NAME,REMARK FROM I_MATERIALTYPE WHERE ENABLE=1";

            List<MaterialType> mts = new List<MaterialType>();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MaterialType mt = new MaterialType();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = mt.GetType().GetProperty(reader.GetName(i));
                            property.SetValue(mt, (reader.IsDBNull(i)) ? "" : reader.GetValue(i), null);
                        }

                        mts.Add(mt);
                    }
                }
                reader.Close();
            }
            return mts;
        }


        //新增
        public void CreateMaterialType(MaterialType mt)
        {
            string query = "INSERT INTO I_MATERIALTYPE (NAME, REMARK) VALUES(:NAME, :REMARK)";

            using (OracleConnection conn = new OracleConnection(constr))
            {

                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = mt.NAME;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = mt.REMARK;
                cmd.ExecuteNonQuery();
            }
        }


        //部門編輯
        public MaterialType EditMaterialType(int id)
        {
            string query = "SELECT TID,NAME,REMARK FROM I_MATERIALTYPE WHERE TID=:TID";

            MaterialType mt = new MaterialType();
            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("TID", OracleDbType.Decimal).Value = id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    mt.TID = decimal.Parse(reader["TID"].ToString());
                    mt.NAME = reader["NAME"].ToString();
                    mt.REMARK = reader["REMARK"].ToString();
                }
                reader.Close();
            }
            return mt;
        }

        //修改部門資料
        public void UpdateMaterialType(MaterialType mt)
        {
            string query = "UPDATE I_MATERIALTYPE SET NAME=:NAME, REMARK=:REMARK WHERE TID=" + mt.TID.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {

                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = mt.NAME;
                cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = mt.REMARK;
                cmd.ExecuteNonQuery();
            }
        }

        //刪除部門資料
        public void DeleteMaterialType(int id)
        {
            string query = "DELETE FROM I_MATERIALTYPE WHERE TID=" + id.ToString();

            using (OracleConnection conn = new OracleConnection(constr))
            {
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }

}