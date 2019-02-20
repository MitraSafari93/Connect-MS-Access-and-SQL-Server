using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace WindowsFormsApplication5
{
    class SQLDBLoader
    {
        private static string connectionString = "Data Source=s-ranjgar;Initial Catalog=AryaInstrumentERPDB4;User ID=sa;Password=sa123";
    
    public static DataSet RunQuery(string queryString,DataSet ds)
    {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(queryString, cn);
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    if (ds.Tables.Count != 0)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            da.Fill(dt);
                            da.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ds = null;
                }
                cn.Close();
            }
            return ds;
        }
    }
}
