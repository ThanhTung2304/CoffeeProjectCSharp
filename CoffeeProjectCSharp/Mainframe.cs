using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeProjectCSharp
{
    public partial class Mainframe : Form
    {
        public Mainframe()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)      //quản lí khách hàng
        {
            Customer customer = new Customer();
            customer.Show();
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Hide();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Hide();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Hide();
        }

        private void btnQLHD_Click(object sender, EventArgs e)
        {
            QLHoaDon qLHoaDon = new QLHoaDon();
            qLHoaDon.Show();
            this.Hide();
        }
    }
}
