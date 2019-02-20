using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace WindowsFormsApplication5
{
    class AccessDBLoader
    {
        private static string str = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password=;Persist Security Info=False;";
        public static DataSet LoadFromFile(string FileName)
        {
            DataSet result = new DataSet();
            result.DataSetName = Path.GetFileNameWithoutExtension(FileName);
            string ConnectionStr = string.Format(str, FileName);
            try
            {
                using (OleDbConnection cn = new OleDbConnection(ConnectionStr))
                {
                    cn.Open();
                    DataTable dt = cn.GetSchema("Tables");
                    List<string> tablesName = dt.AsEnumerable().Select(dr => dr.Field<string>("TABLE_NAME")).Where(dr => !dr.StartsWith("MSys")).ToList();
                    foreach (string tableName in tablesName)
                    {
                        using (OleDbCommand cmd = new OleDbCommand("select * from [" + tableName + "]", cn))
                        {
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                            {
                                DataTable buf = new DataTable(tableName);
                                adapter.Fill(buf);
                                result.Tables.Add(buf);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex) 
            {
                return null;
            }
            return result;
        }


        public static bool UploadToFile(string FilePath, DataSet ds) 
        {
            string ConnectionStr = string.Format(str, FilePath);
            OleDbCommand cmd = new OleDbCommand();
            OleDbCommand cmd1 = new OleDbCommand();

            using (OleDbConnection cn = new OleDbConnection(ConnectionStr))
            {
              
                cmd.Connection = cn;
                cmd.CommandType = CommandType.Text;
                cmd1.Connection = cn;
                cmd1.CommandType = CommandType.Text;
                cn.Open();

                try
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            cmd.CommandText = "INSERT INTO " + dt.TableName + "(" + dt.Columns[0].ColumnName.Trim() + ") VALUES (" + dt.Rows[i].ItemArray.GetValue(0) + ")";
                            cmd.ExecuteNonQuery();

                            for (int j = 1; j < dt.Columns.Count; j++)
                            {
                                cmd1.CommandText = "UPDATE " + dt.TableName + " SET [" + dt.Columns[j].ColumnName.Trim() + "] = '" + dt.Rows[i].ItemArray.GetValue(j) + "' WHERE [" + dt.Columns[0].ColumnName.Trim() + "]= " + (dt.Rows[i].ItemArray.GetValue(0));
                                cmd1.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }






    }
}
