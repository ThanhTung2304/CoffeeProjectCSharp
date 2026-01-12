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
    public partial class QLHoaDon : Form
    {
        string Status = "Reset";
        public QLHoaDon()
        {
            InitializeComponent();
            LoadStatus();
            SetInterface("Reset");
            LoadData();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        // ================== LOAD STATUS ==================
        private void LoadStatus()
        {
            cboStatus.Items.Clear();
            cboStatus.Items.Add("Chờ xử lý");
            cboStatus.Items.Add("Đã thanh toán");
            cboStatus.Items.Add("Hủy");
        }

        // ================== GIAO DIỆN ==================
        private void SetInterface(string status)
        {
            bool edit = (status == "Add" || status == "Edit");

            txtCustomer.Enabled = edit;
            txtPhone.Enabled = edit;
            nudTotal.Enabled = edit;
            cboStatus.Enabled = edit;

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
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Invoice ORDER BY CreatedTime DESC", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvInvoice.AutoGenerateColumns = true;
                dgvInvoice.DataSource = dt;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            ClearInput();
            dtpCreated.Value = DateTime.Now;
            SetInterface(Status);
            txtCustomer.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvInvoice.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần sửa!");
                return;
            }

            DataGridViewRow r = dgvInvoice.SelectedRows[0];
            txtID.Text = r.Cells["Id"].Value.ToString();
            txtCustomer.Text = r.Cells["CustomerName"].Value.ToString();
            txtPhone.Text = r.Cells["Phone"].Value.ToString();
            nudTotal.Value = Convert.ToInt32(r.Cells["TotalAmount"].Value);
            cboStatus.Text = r.Cells["Status"].Value.ToString();
            dtpCreated.Value = Convert.ToDateTime(r.Cells["CreatedTime"].Value);
            if (r.Cells["UpdateTime"].Value != DBNull.Value)
                dtpUpdated.Value = Convert.ToDateTime(r.Cells["UpdateTime"].Value);

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvInvoice.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvInvoice.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa hóa đơn này?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Invoice WHERE Id=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
                LoadData();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomer.Text))
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
                    cmd.CommandText = @"INSERT INTO Invoice
                        (CustomerName, Phone, TotalAmount, Status, CreatedTime)
                        VALUES (@Name,@Phone,@Total,@Status,@Created)";
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = @"UPDATE Invoice SET
                        CustomerName=@Name,
                        Phone=@Phone,
                        TotalAmount=@Total,
                        Status=@Status,
                        UpdateTime=@Updated
                        WHERE Id=@ID";
                    cmd.Parameters.AddWithValue("@ID", txtID.Text);
                    cmd.Parameters.AddWithValue("@Updated", DateTime.Now);
                }

                cmd.Parameters.AddWithValue("@Name", txtCustomer.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@Total", nudTotal.Value);
                cmd.Parameters.AddWithValue("@Status", cboStatus.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Lưu hóa đơn thành công!");
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

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT * FROM Invoice
                      WHERE CustomerName LIKE @kw
                         OR Phone LIKE @kw", conn);
                da.SelectCommand.Parameters.AddWithValue("@kw", "%" + key + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvInvoice.DataSource = dt;
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
            txtCustomer.Clear();
            txtPhone.Clear();
            nudTotal.Value = 0;
            cboStatus.SelectedIndex = -1;
            dtpCreated.Value = DateTime.Now;
            dtpUpdated.Value = DateTime.Now;
        }

        private void dgvInvoice_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow r = dgvInvoice.Rows[e.RowIndex];

            txtID.Text = r.Cells["Id"].Value.ToString();
            txtCustomer.Text = r.Cells["CustomerName"].Value.ToString();
            txtPhone.Text = r.Cells["Phone"].Value.ToString();
            nudTotal.Value = Convert.ToDecimal(r.Cells["TotalAmount"].Value);
            cboStatus.Text = r.Cells["Status"].Value.ToString();

            dtpCreated.Value = Convert.ToDateTime(r.Cells["CreatedTime"].Value);
            if (r.Cells["UpdateTime"].Value != DBNull.Value)
                dtpUpdated.Value = Convert.ToDateTime(r.Cells["UpdateTime"].Value);
        }

        private void btnExcelInvoice_Click(object sender, EventArgs e)
        {
            if (dgvInvoice.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachHoaDon.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // HEADER
                        for (int i = 0; i < dgvInvoice.Columns.Count; i++)
                        {
                            sw.Write(dgvInvoice.Columns[i].HeaderText);
                            if (i < dgvInvoice.Columns.Count - 1) sw.Write(",");
                        }
                        sw.WriteLine();

                        // DATA
                        foreach (DataGridViewRow row in dgvInvoice.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvInvoice.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvInvoice.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel hóa đơn thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }
    }
}
