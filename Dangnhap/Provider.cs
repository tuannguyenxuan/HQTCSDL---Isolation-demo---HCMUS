using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Dangnhap
{
    class Provider
    {
        SqlConnection con { get; set; }

        public SqlConnection Connect
        {
            set { con = value; }
            get { return con; }
        }

        public void getConnect()
        {
        
            string constring = @"Data Source=DESKTOP-G5N68VN;Initial Catalog=QLND;Integrated Security=True";

            try
            {
                if (con == null)
                {
                    con = new SqlConnection(constring);
                }

                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }

                //con.Open();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public void disConnect()
        {
            try
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}
