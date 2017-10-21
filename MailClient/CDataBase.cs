using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
//using System.Data.SqlClient;

namespace MailClient
{
    class CDataBase
    {
        public static string connStr
            = "Server=localhost;Database=mailbox;Uid=root;Pwd=xzy03120201;";
        //public static MySqlConnection conn = new MySqlConnection(connStr);

        public static DataSet GetDataFromDB(string sqlStr)
        {
            //conn.Open();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlDataAdapter myAdapter = new MySqlDataAdapter(sqlStr, conn);
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
        }

        public static bool UpdateDB(string sqlStr)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                //conn.Open();
                MySqlCommand myCmd = new MySqlCommand(sqlStr, conn);
                conn.Open();
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
        }


    }
}
