using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MailClient
{
    class CDataBase
    {
        public static string connStr
            = "Data Source=.;Initial Catalog= mailSender;Integrated Security=True";
        public static SqlConnection conn = new SqlConnection(connStr);

        public static DataSet GetDataFromDB(string sqlStr)
        {
            conn.Open();
            SqlDataAdapter myAdapter = new SqlDataAdapter(sqlStr, conn);
            DataSet myDataSet = new DataSet();
            myDataSet.Clear();
            myAdapter.Fill(myDataSet);
            conn.Close();
            if (myDataSet.Tables[0].Rows.Count != 0)
            {
                return myDataSet;
            }
            else
            {
                return null;
            }
        }

        public static bool UpdateDB(string sqlStr)
        {
            conn.Open();
            SqlCommand myCmd = new SqlCommand(sqlStr, conn);
            myCmd.CommandType = CommandType.Text;
            myCmd.ExecuteNonQuery();
            conn.Close();
            return true;
        }


    }
}
