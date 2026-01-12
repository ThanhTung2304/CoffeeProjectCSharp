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
    public partial class Menu : Form
    {
        string Status = "Reset";
        public Menu()
        {
            InitializeComponent();
            SetInterface("Reset");
            LoadData();
            LoadCategory();
        }

        private void SetInterface(string status)
        {
            bool edit = (status == "Add" || status == "Edit");

            txtName.Enabled = edit;
            nudPrice.Enabled = edit;
            cboCategory.Enabled = edit;

            dtpCreated.Enabled = false;
            dtpUpdated.Enabled = false;

            btnThem.Enabled = !edit;
            btnSua.Enabled = !edit;
            btnXoa.Enabled = !edit;
            btnLuu.Enabled = edit;
            btnHuy.Enabled = edit;
        }

        // ================= LOAD DATA =================
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da =
                    new SqlDataAdapter("SELECT * FROM Menu", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvMenu.AutoGenerateColumns = true;
                dgvMenu.DataSource = dt;
            }
        }

        // ================= LOAD LOẠI =================
        private void LoadCategory()
        {
            cboCategory.Items.Clear();
            cboCategory.Items.AddRange(new string[]
            {
                "Cà phê",
                "Trà",
                "Nước ép",
                "Bánh"
            });
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            ClearInput();
            dtpCreated.Value = DateTime.Now;
            SetInterface(Status);
            txtName.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvMenu.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn món cần sửa!");
                return;
            }

            DataGridViewRow r = dgvMenu.SelectedRows[0];
            txtID.Text = r.Cells["Id"].Value.ToString();
            txtName.Text = r.Cells["Name"].Value.ToString();
            nudPrice.Value = Convert.ToInt32(r.Cells["Price"].Value);
            cboCategory.Text = r.Cells["Category"].Value.ToString();

            dtpCreated.Value = Convert.ToDateTime(r.Cells["CreatedTime"].Value);
            if (r.Cells["UpdateTime"].Value != DBNull.Value)
                dtpUpdated.Value = Convert.ToDateTime(r.Cells["UpdateTime"].Value);

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvMenu.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvMenu.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd =
                        new SqlCommand("DELETE FROM Menu WHERE Id=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
                LoadData();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên món không được để trống!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Status == "Add")
                {
                    cmd.CommandText = @"INSERT INTO Menu
                        (Name, Price, Category, CreatedTime)
                        VALUES (@Name,@Price,@Category,@Created)";
                    cmd.Parameters.AddWithValue("@Created", dtpCreated.Value);
                }
                else
                {
                    cmd.CommandText = @"UPDATE Menu SET
                        Name=@Name,
                        Price=@Price,
                        Category=@Category,
                        UpdateTime=@Updated
                        WHERE Id=@ID";
                    cmd.Parameters.AddWithValue("@ID", txtID.Text);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.Now);
                }

                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Price", nudPrice.Value);
                cmd.Parameters.AddWithValue("@Category", cboCategory.Text);

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
                    @"SELECT * FROM Menu
                      WHERE Name LIKE @kw
                         OR Category LIKE @kw", conn);
                da.SelectCommand.Parameters.AddWithValue("@kw", "%" + key + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvMenu.DataSource = dt;
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
            nudPrice.Value = 0;
            cboCategory.SelectedIndex = -1;
            dtpCreated.Value = DateTime.Now;
            dtpUpdated.Value = DateTime.Now;
        }

        private void btnExcelMenu_Click(object sender, EventArgs e)
        {
            if (dgvMenu.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachMenu.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // HEADER
                        for (int i = 0; i < dgvMenu.Columns.Count; i++)
                        {
                            sw.Write(dgvMenu.Columns[i].HeaderText);
                            if (i < dgvMenu.Columns.Count - 1) sw.Write(",");
                        }
                        sw.WriteLine();

                        // DATA
                        foreach (DataGridViewRow row in dgvMenu.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvMenu.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvMenu.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel menu thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }
    }

}
