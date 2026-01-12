using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeProjectCSharp
{
    public partial class Customer : Form
    {
        string Status = "Reset";   // biến status có 3 giá trị chính"reset: chỉ xem- add: đang thêm- edit: đang sửa"
        public Customer()
        {
            InitializeComponent();
            SetInterface("Reset");       // khóa các ô nhập
            LoadData();    //hiển thị/ tải dsach kh từ DB
        }
        //------------ĐIỀU KHIỂN GIAO DIỆN--------------
        private void SetInterface(string status)
        {
            bool edit = (status == "Add" || status == "Edit");

            txtName.Enabled = edit;
            txtPhone.Enabled = edit;
            txtEmail.Enabled = edit;
            nudPoints.Enabled = edit;

            dtpCreated.Enabled = false;
            dtpUpdated.Enabled = false;

            btnThem.Enabled = !edit;
            btnSua.Enabled = !edit;
            btnXoa.Enabled = !edit;
            btnLuu.Enabled = edit;
            btnHuy.Enabled = edit;
        }

        // ================== LOAD DATA ==================
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))     // KẾT Nối Db
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Customer", conn);             // lấy toàn bộ danh sách khách hàng

                DataTable dt = new DataTable();        // đổ dl vào datatable
                da.Fill(dt);
                dgvCustomer.AutoGenerateColumns = true;    //
                dgvCustomer.DataSource = dt;                 //hiển thị trên dgv
            }
        }



        private void Customer_Load(object sender, EventArgs e)
        {

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            ClearInput();   //dl đã đc sửa đổi
            dtpCreated.Value = DateTime.Now;
            SetInterface(Status);      //nhập biểu mẫu
            txtName.Focus();                          //tập trung vào ô tên
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.SelectedRows.Count == 0)        // không chọn vào hàng/ cột khách hàng báo lỗi
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!");
                return;
            }

            DataGridViewRow r = dgvCustomer.SelectedRows[0];   // trỏ vào dong đang chọn
            txtID.Text = r.Cells["Id"].Value.ToString();
            txtName.Text = r.Cells["Name"].Value.ToString();
            txtPhone.Text = r.Cells["Phone"].Value.ToString();
            txtEmail.Text = r.Cells["Email"].Value.ToString();
            nudPoints.Value = Convert.ToInt32(r.Cells["Points"].Value);
            // lấy lại dl từ biểu mẫu

            dtpCreated.Value = Convert.ToDateTime(r.Cells["CreatedTime"].Value);
            if (r.Cells["UpdateTime"].Value != DBNull.Value)
                dtpUpdated.Value = Convert.ToDateTime(r.Cells["UpdateTime"].Value);

            Status = "Edit";
            SetInterface(Status);     // chuyển sang chế độ chỉnh sửa
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvCustomer.SelectedRows[0].Cells["Id"].Value);        //click id khách hàng

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd =
                        new SqlCommand("DELETE FROM Customer WHERE Id=@ID", conn);   // xóa trong db
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
                LoadData();// load lại danh sách
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))   // khoogn thể save nếu để trống tên
            {
                MessageBox.Show("Tên khách hàng không được để trống!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Status == "Add")
                {
                    cmd.CommandText = @"INSERT INTO Customer
                        (Name, Phone, Email, Points, CreatedTime)
                        VALUES (@Name,@Phone,@Email,@Points,@Created)";
                    cmd.Parameters.AddWithValue("@Created", dtpCreated.Value);
                }
                else
                {
                    cmd.CommandText = @"UPDATE Customer SET
                        Name=@Name,
                        Phone=@Phone,
                        Email=@Email,
                        Points=@Points,
                        UpdateTime=@Updated
                        WHERE Id=@ID";
                    cmd.Parameters.AddWithValue("@ID", txtID.Text);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.Now);
                }

                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@Points", nudPoints.Value);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Lưu thành công!");
            Status = "Reset";
            SetInterface(Status);
            LoadData();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Status = "Reset";
            SetInterface(Status);
            ClearInput();
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow r = dgvCustomer.Rows[e.RowIndex];
            txtID.Text = r.Cells["Id"].Value.ToString();
            txtName.Text = r.Cells["Name"].Value.ToString();
            txtPhone.Text = r.Cells["Phone"].Value.ToString();
            txtEmail.Text = r.Cells["Email"].Value.ToString();
            nudPoints.Value = Convert.ToInt32(r.Cells["Points"].Value);

            dtpCreated.Value = Convert.ToDateTime(r.Cells["CreatedTime"].Value);
            if (r.Cells["UpdateTime"].Value != DBNull.Value)
                dtpUpdated.Value = Convert.ToDateTime(r.Cells["UpdateTime"].Value);
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string key = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(key))
            {
                LoadData();
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT * FROM Customer
                      WHERE Name LIKE @kw
                         OR Phone LIKE @kw
                         OR Email LIKE @kw", conn);
                da.SelectCommand.Parameters.AddWithValue("@kw", "%" + key + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvCustomer.DataSource = dt;
            }
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            Mainframe m = new Mainframe();
            m.Show();
            this.Hide();
        }

        private void ClearInput()
        {
            txtID.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            nudPoints.Value = 0;
            dtpCreated.Value = DateTime.Now;
            dtpUpdated.Value = DateTime.Now;
        }

        private void btnExcelCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachKhachHang.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // HEADER
                        for (int i = 0; i < dgvCustomer.Columns.Count; i++)
                        {
                            sw.Write(dgvCustomer.Columns[i].HeaderText);
                            if (i < dgvCustomer.Columns.Count - 1) sw.Write(",");
                        }
                        sw.WriteLine();

                        // DATA
                        foreach (DataGridViewRow row in dgvCustomer.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvCustomer.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvCustomer.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel khách hàng thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
