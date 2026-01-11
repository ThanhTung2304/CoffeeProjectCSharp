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

        private void button3_Click(object sender, EventArgs e)
        {

        }


        private void button6_Click_1(object sender, EventArgs e)
        {
            Product product = new Product();
            product.Show();
            this.Hide();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Inventory inventory = new Inventory();
            inventory.Show();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Recipe recipe = new Recipe();
            recipe.Show();
            this.Hide();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Hide();
        }
    }
}
