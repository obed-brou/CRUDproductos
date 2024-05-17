using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDproductos
{
    public partial class Menu : Form
    {
        string Server, Usuario, Password;
        
        private void button1_Click(object sender, EventArgs e)
        {
            Productos prod = new Productos(Server, Usuario, Password);
            
            prod.TopLevel = false;
            prod.FormBorderStyle = FormBorderStyle.None;
            prod.Dock = DockStyle.Fill;

            panel1.Controls.Clear(); // Limpia cualquier control existente en el panel
            panel1.Controls.Add(prod);
            prod.Show();
            btnLimpiar.Visible = false;
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            Form1 inv = new Form1(Server, Usuario, Password);
            inv.TopLevel = false;
            inv.FormBorderStyle = FormBorderStyle.None;
            inv.Dock = DockStyle.Fill;

            panel1.Controls.Clear(); // Limpia cualquier control existente en el panel
            panel1.Controls.Add(inv);
            inv.Show();
            btnLimpiar.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Form1 inv = new Form1(Server, Usuario, Password);
            inv.TopLevel = false;
            inv.FormBorderStyle = FormBorderStyle.None;
            inv.Dock = DockStyle.Fill;

            panel1.Controls.Clear(); // Limpia cualquier control existente en el panel
            panel1.Controls.Add(inv);
            inv.Show();
        }

        public Menu(string sServer, string sUsuario, string sPassword)
        {

            InitializeComponent();
            Server = sServer;
            Usuario = sUsuario;
            Password = sPassword;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            btnLimpiar.Visible = false;
        }
    }
}
