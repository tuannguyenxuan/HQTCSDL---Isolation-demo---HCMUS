using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Dangnhap
{
    public partial class Khachhang : Form
    {
        public Khachhang()
        {
            InitializeComponent();
        }


        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            hoten.Text = dataGridView1.Rows[0].Cells[0].Value.ToString();
            diachi.Text = dataGridView1.Rows[0].Cells[1].Value.ToString();
            dt.Text = dataGridView1.Rows[0].Cells[2].Value.ToString();
        }

        private void Update_KH_button_Click_1(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {

                SqlCommand cmd = new SqlCommand("EXEC SP_SUATHONGTINKH @MAKH, @TENKH, @DIACHIKH, @DIENTHOAIKH", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MAKH", Login._id));
                cmd.Parameters.Add(new SqlParameter("@TENKH", hoten.Text));
                cmd.Parameters.Add(new SqlParameter("@DIACHIKH", diachi.Text));
                cmd.Parameters.Add(new SqlParameter("@DIENTHOAIKH", dt.Text));

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                btnLoad.PerformClick();


                //SqlCommand cmd = new SqlCommand("sp_ChinhSuaThongTinKH", provider.Connect);
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Makh", Login._id);
                //cmd.Parameters.AddWithValue("@Tenkh", hoten.Text);
                //cmd.Parameters.AddWithValue("@Diachi", diachi.Text);
                //cmd.Parameters.AddWithValue("@SDT", dt.Text);
                //provider.Connect.Open();


                //cmd.ExecuteNonQuery();
                //MessageBox.Show("Cập nhật thông tin thành công");

                //provider.Connect.Close();
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

        private void btnLoad_Click(object sender, EventArgs e)
        {
            label3.Text = Login._id;
            Provider provider = new Provider();
            provider.getConnect();
            try
            {


                string sql = "SELECT TENKHACHHANG, DIACHIKHACHHANG, DIENTHOAIKHACHHANG FROM KHACH_HANG WHERE _USERNAME='" + Login._id + "'";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);

                provider.Connect.Open();


                cmd.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dataGridView1.DataSource = dt;
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

        private void dgvListNhaBan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button_TK_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                int count = 0;
                SqlCommand cmd = new SqlCommand("sp_TimKiemNha", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Loainha", Loainha.Text);
                cmd.Parameters.Add(new SqlParameter("@count", count)).Direction = ParameterDirection.Output;
               
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);
                int rownum = dt.Rows.Count;

                count = Convert.ToInt32(cmd.Parameters["@count"].Value);

                MessageBox.Show("Tìm thấy " + count.ToString() + " nhà");

                dt.Rows.Add(new Object[] { "So luong tim thay", count });
                dt.Rows.Add(new Object[] { "So luong xuat ra", rownum });
               
                dgvTK.DataSource = dt;
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
       

        private void fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                int count = 0;
                SqlCommand cmd = new SqlCommand("sp_fix_TimKiemNha", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Loainha", Loainha.Text);
                cmd.Parameters.Add(new SqlParameter("@count", count)).Direction = ParameterDirection.Output;
                
    
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);
                int rownum = dt.Rows.Count;

                count = Convert.ToInt32(cmd.Parameters["@count"].Value);
                dt.Rows.Add(new Object[] { "Count1", count });
                dt.Rows.Add(new Object[] { "Count2", rownum });

                dgvTK.DataSource = dt;
                MessageBox.Show("Fix thanh cong");
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

        private void button1_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                string query = string.Format("SELECT * FROM NHA WHERE TINHTRANGNHA = '0'");
                SqlCommand cmd = new SqlCommand(query, provider.Connect);
                provider.Connect.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvListNha.DataSource = dt;
                provider.Connect.Close();
            }
            catch
            {
                MessageBox.Show("Lỗi!");
            }
            finally
            {
                provider.disConnect();
            }
        }

        private void dgvListNhaThue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvTK_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Khachhang_Load(object sender, EventArgs e)
        {
            btnLoad.PerformClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {

                SqlCommand cmd = new SqlCommand("SP_UPDATENAMEKH_fix", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@maKH", SqlDbType.VarChar).Value = Login._id;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = hoten.Text;

                provider.Connect.Open();

                cmd.ExecuteNonQuery();
                provider.Connect.Close();

                MessageBox.Show("Thành Công");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                provider.disConnect();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {

                SqlCommand cmd = new SqlCommand("SP_UPDATENAMEKH", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@maKH", SqlDbType.VarChar).Value = Login._id;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = hoten.Text;
             
                provider.Connect.Open();
                     
                cmd.ExecuteNonQuery();
                provider.Connect.Close();

                MessageBox.Show("Thành Công");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                provider.disConnect();
            }
        }

        private void hoten_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvListNha_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
