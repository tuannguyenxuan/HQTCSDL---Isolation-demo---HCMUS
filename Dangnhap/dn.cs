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
    public partial class Login : Form
    {
        public static String getName = "",_id="",_password="";
        public Login()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            
            Provider provider = new Provider();
            provider.getConnect();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT  _USERNAME FROM ACCOUNT WHERE _USERNAME ='" + txt_taikhoan.Text + "' AND PASSWORD='"+ txt_matkhau.Text+"'", provider.Connect);
                provider.Connect.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                dt.Clear();
                da.Fill(dt);
                da.Dispose();

                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu, Hãy Thử Lại");
                }
                else
                {
                    //Khai bao 1 bien kieu form can load
                    _id = txt_taikhoan.Text;
                    getName = _id.Substring(0, 2);
                    MessageBox.Show("Dang nhap " + _id + " thanh cong");
                  
                    if (getName=="nv"||getName=="NV")
                    {
                        Nhanvien NV = new Nhanvien();
                        NV.Show();
                    }
                    else if(getName=="KH"||getName=="kh")
                    {
                        Khachhang KH = new Khachhang();
                        KH.Show();
                    }
                    else if(getName=="cn"||getName=="CN")
                    {
                        Chunha CN = new Chunha();
                        CN.Show();
                    }
                    //this.Hide();
                }

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

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void txt_taikhoan_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_matkhau_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
