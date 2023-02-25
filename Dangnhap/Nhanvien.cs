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
using Microsoft.VisualBasic;


namespace Dangnhap
{
    public partial class Nhanvien : Form
    {
        public Nhanvien()
        {
            InitializeComponent();
        }

        private void congDiemKH_NV_btn_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                if (string.IsNullOrEmpty(sdtKH_NV_txtBox.Text))
                {

                    MessageBox.Show("Chưa nhập SĐT");
                    throw new Exception();
                }
                SqlCommand cmd = new SqlCommand("SP_TANGDIEMKHMUANHA", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 10).Value = sdtKH_NV_txtBox.Text;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thành Công");
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

        private void thongKeKHTiemNang_NV_btn_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                int SoLuongL1 = 0, SoLuongL2 = 0;
                string sql = "EXEC SP_THONGKEKHTIEMNANG @SoLuongL1 OUTPUT ,@SoLuongL2 OUTPUT";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@SoLuongL1", SoLuongL1)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@SoLuongL2", SoLuongL2)).Direction = ParameterDirection.Output;

                provider.Connect.Open();



                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                SoLuongL1 = Convert.ToInt32(cmd.Parameters["@SoLuongL1"].Value);
                SoLuongL2 = Convert.ToInt32(cmd.Parameters["@SoLuongL2"].Value);

                dt.Rows.Add(new Object[] { "Count1", SoLuongL1 });
                dt.Rows.Add(new Object[] { "Count2", SoLuongL2 });
                khachHang_NV_dtgv.DataSource = dt;

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

                SqlCommand cmd = new SqlCommand("SP_TANGDIEMKHACHHANG", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@maKH", SqlDbType.VarChar, 10).Value = makh.Text;
                cmd.Parameters.Add("@diem", SqlDbType.SmallInt).Value = textBox1.Text;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thành Công");
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



        private void xemKH_NV_btn_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_XEMTHONGTINKH @MAKH", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MAKH", makh.Text));
                provider.Connect.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                khachHang_NV_dtgv.DataSource = dt;
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
        string usernameID = "";
        private void khachHang_NV_dtgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = khachHang_NV_dtgv.Rows[index];
            usernameID = selectedRow.Cells[5].Value.ToString();
        }


        private void btnThongKeNVXuatSac_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                int SoLuongL1 = 0, SoLuongL2 = 0;
                string sql = "EXEC SP_THONGKENVXUATSAC @SoLuongL1 OUTPUT ,@SoLuongL2 OUTPUT";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@SoLuongL1", SoLuongL1)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@SoLuongL2", SoLuongL2)).Direction = ParameterDirection.Output;

                provider.Connect.Open();

                cmd.ExecuteNonQuery();
                SoLuongL1 = Convert.ToInt32(cmd.Parameters["@SoLuongL1"].Value);
                SoLuongL2 = Convert.ToInt32(cmd.Parameters["@SoLuongL2"].Value);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt.Rows.Add(new Object[] { "Count1", SoLuongL1 });
                dt.Rows.Add(new Object[] { "Count2", SoLuongL2 });
                nhanVien_NV_dtgv.DataSource = dt;

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

        private void btnXemNhanVien_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                string sql = "SELECT MANV,TENNV,DIEM_THUONG,LUONGNV FROM NHAN_VIEN WHERE MANV = @MANV";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);

                cmd.Parameters.Add(new SqlParameter("@MANV", txtIDNV.Text));
                provider.Connect.Open();


                cmd.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                nhanVien_NV_dtgv.DataSource = dt;

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

        private void themDiemNV_NV_btn_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                String error = "";
                SqlCommand cmd = new SqlCommand("EXEC SP_TANGDIEMNV @MANV ,@DIEMCONG, @ERROR", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV", txtIDNV.Text));
                cmd.Parameters.Add(new SqlParameter("@DIEMCONG", int.Parse((txtDiemCong.Text))));
                cmd.Parameters.Add(new SqlParameter("@ERROR", error));

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                btnXemNhanVien.PerformClick();

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

        private void themDiemNV_NV_btn_undo_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                String error = "";
                SqlCommand cmd = new SqlCommand("EXEC SP_TANGDIEMNV @MANV ,@DIEMCONG, @ERROR", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV", txtIDNV.Text));
                cmd.Parameters.Add(new SqlParameter("@DIEMCONG", -int.Parse((txtDiemCong.Text))));
                cmd.Parameters.Add(new SqlParameter("@ERROR", error));

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                btnXemNhanVien.PerformClick();

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

        private void themDiemNV_NV_btn_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                String error = "";
                SqlCommand cmd = new SqlCommand("EXEC SP_TANGDIEMNV_fix @MANV ,@DIEMCONG, @ERROR", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV", txtIDNV.Text));
                cmd.Parameters.Add(new SqlParameter("@DIEMCONG", int.Parse((txtDiemCong.Text))));
                cmd.Parameters.Add(new SqlParameter("@ERROR", error)).Direction = ParameterDirection.Output;

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                btnXemNhanVien.PerformClick();

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

        private void btnThongKeNVXuatSac_undo_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                string sql = "UPDATE NHAN_VIEN SET DIEM_THUONG = 450 WHERE MANV = @MANV";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV", txtIDNV.Text));
                provider.Connect.Open();

                cmd.ExecuteNonQuery();
                btnXemNhanVien.PerformClick();
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

        private void btnThongKeNVXuatSac_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                int SoLuongL1 = 0, SoLuongL2 = 0;
                string sql = "EXEC SP_THONGKENVXUATSAC_fix @SoLuongL1 OUTPUT ,@SoLuongL2 OUTPUT";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@SoLuongL1", SoLuongL1)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@SoLuongL2", SoLuongL2)).Direction = ParameterDirection.Output;

                provider.Connect.Open();

                cmd.ExecuteNonQuery();
                SoLuongL1 = Convert.ToInt32(cmd.Parameters["@SoLuongL1"].Value);
                SoLuongL2 = Convert.ToInt32(cmd.Parameters["@SoLuongL2"].Value);
                Console.WriteLine(SoLuongL1);
                Console.WriteLine(SoLuongL2);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dt.Rows.Add(new Object[] { "Count1", SoLuongL1 });
                dt.Rows.Add(new Object[] { "Count2", SoLuongL2 });
                nhanVien_NV_dtgv.DataSource = dt;

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

        private void btnChuyenQuyenQuanLy_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            String error = "";
            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_CHUYENQUYENQUANLYNHA @MANV1,@MANV2,@ERROR", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV1", txtIDNV1.Text));
                cmd.Parameters.Add(new SqlParameter("@MANV2", txtIDNV2.Text));
                cmd.Parameters.Add(new SqlParameter("@ERROR", error)).Direction = ParameterDirection.Output;

                error = cmd.Parameters["@ERROR"].Value.ToString();

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                xemNha_NV_btn.PerformClick();

            }


            catch (Exception)
            {
                MessageBox.Show(error);
            }
            finally
            {
                provider.disConnect();
            }
        }

        private void xemNha_NV_btn_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_XEMTHONGTINNHA @MANHA", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANHA", txtIDNha.Text));
                provider.Connect.Open();
                cmd.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                nha_NV_dtgv.DataSource = dt;
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

        private void btnChuyenQuyenQuanLy_undo_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                String error = "";
                SqlCommand cmd = new SqlCommand("EXEC SP_CHUYENQUYENQUANLYNHA @MANV2,@MANV1,@ERROR", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV2", txtIDNV2.Text));
                cmd.Parameters.Add(new SqlParameter("@MANV1", txtIDNV1.Text));
                cmd.Parameters.Add(new SqlParameter("@ERROR", error)).Direction = ParameterDirection.Output;

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                xemNha_NV_btn.PerformClick();

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

        private void btnChuyenQuyenQuanLy_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                String error = "";
                SqlCommand cmd = new SqlCommand("EXEC SP_CHUYENQUYENQUANLYNHA_fix @MANV1,@MANV2,@ERROR", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV1", txtIDNV1.Text));
                cmd.Parameters.Add(new SqlParameter("@MANV2", txtIDNV2.Text));
                cmd.Parameters.Add(new SqlParameter("@ERROR", error)).Direction = ParameterDirection.Output;

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                xemNha_NV_btn.PerformClick();

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

        private void xemLuong_NV_btn_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("dbo.SP_THONG_KE_LUONG", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@TONGL1", SqlDbType.Money).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@TONGL2", SqlDbType.Money).Direction = ParameterDirection.Output;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                double tongL1 = Convert.ToDouble(cmd.Parameters["@TONGL1"].Value);
                double tongL2 = Convert.ToDouble(cmd.Parameters["@TONGL2"].Value);

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("So lan");
                dt.Columns.Add("Tong luong 1 thang");
                dt.Rows.Add(new Object[] { "1", tongL1 });
                dt.Rows.Add(new Object[] { "2", tongL2 });
                nhanVien_NV_dtgv.DataSource = dt;
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

        private void suaKH_NV_btn_Click(object sender, EventArgs e)
        {

        }

        private void themNV_NV_btn_Click(object sender, EventArgs e)
        {
            string inputTxt = Interaction.InputBox("Mã nv, tên, giới tính, lương, tên đăng nhập, mật khẩu.", "Nhập thông tin nhân viên (cách nhau bởi dấu phẩy)", "", 300, 300);
            string[] ret = inputTxt.Split(',');
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("dbo.SP_THEM_NV", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@MANV", SqlDbType.NVarChar, 10).Value = ret[0];
                cmd.Parameters.Add("@TEN", SqlDbType.NVarChar, 50).Value = ret[1];
                cmd.Parameters.Add("@GIOITINH", SqlDbType.NVarChar, 3).Value = ret[2];
                cmd.Parameters.Add("@LUONG", SqlDbType.Money).Value = Double.Parse(ret[3]);
                cmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar, 10).Value = ret[4];
                cmd.Parameters.Add("@PASS", SqlDbType.NVarChar, 10).Value = ret[5];


                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm nhân viên thành công");
                provider.Connect.Close();
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

        private void themNV_NV_btn_undo_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                string query = string.Format("DELETE FROM NHAN_VIEN WHERE _USERNAME = 'truong69'");
                string query2 = string.Format("DELETE FROM ACCOUNT WHERE _USERNAME = 'truong69'");
                SqlCommand cmd = new SqlCommand(query, provider.Connect);
                SqlCommand cmd2 = new SqlCommand(query2, provider.Connect);
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
                provider.disConnect();
                MessageBox.Show("undo thành công");
            }
            catch
            {
                MessageBox.Show("Lỗi!");
            }
        }

        private void xemLuong_NV_btn_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("dbo.SP_THONG_KE_LUONG_FIX", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@TONGL1", SqlDbType.Money).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@TONGL2", SqlDbType.Money).Direction = ParameterDirection.Output;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                double tongL1 = Convert.ToDouble(cmd.Parameters["@TONGL1"].Value);
                double tongL2 = Convert.ToDouble(cmd.Parameters["@TONGL2"].Value);

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("So lan");
                dt.Columns.Add("Tong luong 1 thang");
                dt.Rows.Add(new Object[] { "1", tongL1 });
                dt.Rows.Add(new Object[] { "2", tongL2 });
                nhanVien_NV_dtgv.DataSource = dt;
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int a = tabControl1.SelectedIndex;
            if (a == 2)
            {
                Provider provider = new Provider();
                provider.getConnect();

                SqlCommand cmd = new SqlCommand("dbo.CHECK_NV_TRUONG", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar, 50).Value = Login._id;
                cmd.Parameters.Add("@FLAG", SqlDbType.Int).Direction = ParameterDirection.Output;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                int flag = Convert.ToInt32(cmd.Parameters["@FLAG"].Value);
                if (flag == 0)
                {
                    MessageBox.Show("Phải là nhân viên trưởng mới vào được mục này.");
                    ((Control)this.nhanVien_NV_tab).Enabled = false;
                    return;
                }
            }
        }

        private void TKKH_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                
                string message = "";
                SqlCommand cmd = new SqlCommand("exec sp_XemThongTinKhachHang @Tenkh , @out out", provider.Connect);
             
                cmd.Parameters.Add(new SqlParameter("@Tenkh", textBox2.Text));

                cmd.Parameters.Add(new SqlParameter("@out",SqlDbType.VarChar,50)).Direction = ParameterDirection.Output;
                cmd.Parameters["@out"].Value = message;
               

                cmd.Parameters["@out"].Direction = ParameterDirection.Output;

                provider.Connect.Open();

         
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                message = Convert.ToString(cmd.Parameters["@out"].Value);
                MessageBox.Show(message);
                khachHang_NV_dtgv.DataSource = dt;

             

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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


        private void CNTTNha_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_Bannha", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manha", txtIDNha.Text);

                provider.Connect.Open();

                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công");
                provider.Connect.Close();
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

        private void khachHang_NV_tab_Click(object sender, EventArgs e)
        {

        }

        private void makh_TextChanged(object sender, EventArgs e)
        {

        }

        private void ff_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {

                string message = "";
                SqlCommand cmd = new SqlCommand("exec sp_fix_XemThongTinKhachHang @Tenkh , @out out", provider.Connect);

                cmd.Parameters.Add(new SqlParameter("@Tenkh", textBox2.Text));

                cmd.Parameters.Add(new SqlParameter("@out", SqlDbType.VarChar, 50)).Direction = ParameterDirection.Output;
                cmd.Parameters["@out"].Value = message;


                cmd.Parameters["@out"].Direction = ParameterDirection.Output;

                provider.Connect.Open();


                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                message = Convert.ToString(cmd.Parameters["@out"].Value);
                MessageBox.Show(message);
                khachHang_NV_dtgv.DataSource = dt;



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

        private void button2_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_fix_Bannha", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manha", txtIDNha.Text);

                provider.Connect.Open();


                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công");
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

        private void button3_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_Capnhatdl", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manha", txtIDNha.Text);

                provider.Connect.Open();


                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công");
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

        private void xemNha_NV_btn_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_XEMTHONGTINNHA_FIX @MANHA", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANHA", txtIDNha.Text));
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                nha_NV_dtgv.DataSource = dt;
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

        private void xemKH_NV_btn_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC SP_XEMTHONGTINKH_FIX @MAKH", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MAKH", makh.Text));
                provider.Connect.Open();
                cmd.ExecuteNonQuery();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                nha_NV_dtgv.DataSource = dt;
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

        private void btnXemLuongCN_Click_1(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                double tongL = 0;
                int tongDT = 0;
                string sql = "EXEC sp_XemLuongVaDiemThuong @maChiNhanh ,@tongL OUTPUT, @tongDT OUTPUT";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@maChiNhanh", txtMaCN.Text));
                cmd.Parameters.Add(new SqlParameter("@tongL", tongL)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@tongDT", tongDT)).Direction = ParameterDirection.Output;

                provider.Connect.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                tongL = Convert.ToDouble(cmd.Parameters["@tongL"].Value);
                tongDT = Convert.ToInt32(cmd.Parameters["@tongDT"].Value);
                dt.Rows.Add(new Object[] { "Tong:", " ", tongL, tongDT });
                nhanVien_NV_dtgv.DataSource = dt;

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

        private void btnXemLuongCN_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                double tongL = 0;
                int tongDT = 0;
                string sql = "EXEC sp_XemLuongVaDiemThuong_Fix @maChiNhanh ,@tongL OUTPUT, @tongDT OUTPUT";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@maChiNhanh", txtMaCN.Text));
                cmd.Parameters.Add(new SqlParameter("@tongL", tongL)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@tongDT", tongDT)).Direction = ParameterDirection.Output;

                provider.Connect.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                tongL = Convert.ToDouble(cmd.Parameters["@tongL"].Value);
                tongDT = Convert.ToInt32(cmd.Parameters["@tongDT"].Value);
                dt.Rows.Add(new Object[] { "Tong:", " ", tongL, tongDT });
                nhanVien_NV_dtgv.DataSource = dt;

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

        private void btnChuyenCN_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("EXEC sp_ChuyenChiNhanh @MANV ,@MACN", provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@MANV", txtIDNV.Text));
                cmd.Parameters.Add(new SqlParameter("@MACN", txtMaCN.Text));

                provider.Connect.Open();
                cmd.ExecuteNonQuery();
                provider.Connect.Close();
                MessageBox.Show("Cập nhật thành công");
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

        private void congDiemKH_NV_btn_undo_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();
            try
            {
                string query = string.Format("UPDATE KHACH_HANG SET DIEM_HOAT_DONG = 650 WHERE MAKHACHHANG = 'KH001'");
                SqlCommand cmd = new SqlCommand(query, provider.Connect);

                provider.Connect.Open();
                cmd.ExecuteNonQuery();

                provider.disConnect();
                MessageBox.Show("undo thành công");
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

        private void thongKeKHTiemNang_NV_btn_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                int SoLuongL1 = 0, SoLuongL2 = 0;
                string sql = "EXEC SP_THONGKEKHTIEMNANG_fix @SoLuongL1 OUTPUT ,@SoLuongL2 OUTPUT";
                SqlCommand cmd = new SqlCommand(sql, provider.Connect);
                cmd.Parameters.Add(new SqlParameter("@SoLuongL1", SoLuongL1)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@SoLuongL2", SoLuongL2)).Direction = ParameterDirection.Output;

                provider.Connect.Open();



                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                SoLuongL1 = Convert.ToInt32(cmd.Parameters["@SoLuongL1"].Value);
                SoLuongL2 = Convert.ToInt32(cmd.Parameters["@SoLuongL2"].Value);

                dt.Rows.Add(new Object[] { "Count1", SoLuongL1 });
                dt.Rows.Add(new Object[] { "Count2", SoLuongL2 });
                khachHang_NV_dtgv.DataSource = dt;

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

        private void themNV_NV_btn_fix_Click(object sender, EventArgs e)
        {

        }

        private void xemLuong_NV_btn_undo_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                if (string.IsNullOrEmpty(makh.Text))
                {

                    MessageBox.Show("Chưa nhập Mã KH");
                    throw new Exception();
                }
                SqlCommand cmd = new SqlCommand("SP_TANGDIEMKHACHHANG", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@maKH", SqlDbType.VarChar, 10).Value = makh.Text;
                cmd.Parameters.Add("@diem", SqlDbType.SmallInt).Value = textBox1.Text;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
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

        private void congDiemKH_NV_btn_fix_Click(object sender, EventArgs e)
        {
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                if (string.IsNullOrEmpty(makh.Text))
                {

                    MessageBox.Show("Chưa nhập Mã KH");
                    throw new Exception();
                }
                SqlCommand cmd = new SqlCommand("SP_TANGDIEMKHACHHANG_fix", provider.Connect);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@maKH", SqlDbType.VarChar, 10).Value = makh.Text;
                cmd.Parameters.Add("@diem", SqlDbType.SmallInt).Value = textBox1.Text;
                provider.Connect.Open();
                cmd.ExecuteNonQuery();
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
    }
}
