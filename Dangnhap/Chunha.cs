using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dangnhap
{
    public partial class Chunha : Form
    {
        public Chunha()
        {
            InitializeComponent();
        }

        private void btnXemNha_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                string sql = "SELECT * FROM NHA WHERE MANHA = @MANHA";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANHA", txtMaNha.Text));
                provider.Connect.Open();

                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                tableChuNha.DataSource = dt;

                provider.Connect.Close();

            }


            catch (Exception)
            {
                MessageBox.Show("Lỗi!");
            }
            finally
            {
                provider.disConnect();
            }
        }

        private void bntSuaPhong_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            String error = "";

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_CAPNHATSOPHONG @MANHA, @SOPHONG,@ERROR OUTPUT", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANHA", txtMaNha.Text));
                cmd.Parameters.Add(new SqlParameter("@SOPHONG", int.Parse((Phong.Text))));
                provider.Connect.Open();
                cmd.Parameters.Add("@ERROR", SqlDbType.VarChar, -1).Value = error;
                cmd.Parameters["@ERROR"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                provider.Connect.Close();

                btnXemNha.PerformClick();

            }


            catch (Exception exp)
            {
                MessageBox.Show("Exeption throw: " + exp.Message);
            }
            finally
            {
                provider.disConnect();
            }
        }

        private void bntDangNha_Click_1(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            SqlCommand comm = new SqlCommand("sp_DangNha", provider.Connect);
            comm.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                provider.Connect.Open();
                comm.Parameters.AddWithValue("@SoPhong", Phong.Text);
                comm.Parameters.AddWithValue("@Duong", Duong.Text);
                comm.Parameters.AddWithValue("@Quan", Quan.Text);
                comm.Parameters.AddWithValue("@Gia", Gia.Text);
                comm.Parameters.AddWithValue("@Loainha", Loainha.Text);

                comm.ExecuteNonQuery();

                MessageBox.Show("Đăng nhà thành công!!!");
                provider.Connect.Close();

            }
            catch (Exception)
            {
                MessageBox.Show("Đăng nhà thất bại!Vui lòng kiểm tra lại!!");
            }
            provider.disConnect();
        }

        private void bntSuaNha_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_SUATHONGTINNHA @MANHA, @DUONG, @QUAN, @SOPHONG", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANHA", txtMaNha.Text));
                cmd.Parameters.Add(new SqlParameter("@DUONG", Duong.Text));
                cmd.Parameters.Add(new SqlParameter("@QUAN", Quan.Text));
                cmd.Parameters.Add(new SqlParameter("@SOPHONG", int.Parse((Phong.Text))));

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Sửa thông tin nhà thành công");
                provider.Connect.Close();
                btnXemNha.PerformClick();
                //SqlCommand cmd = new SqlCommand("sp_SuaThongTinNha", provider.Connect);

                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@maNha", txtMaNha.Text);
                //cmd.Parameters.AddWithValue("@duong", Duong.Text);
                //cmd.Parameters.AddWithValue("@quan", Quan.Text);
                //cmd.Parameters.AddWithValue("@soPhong", Phong.Text);

                //provider.Connect.Open();


                //cmd.ExecuteNonQuery();
                
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                provider.disConnect();
            }

        }

        private void bntFix_DangNha_Click(object sender, EventArgs e)
        {

        }
    }
}
