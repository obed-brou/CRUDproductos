using dbSqlServer;
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
    public partial class Login : Form
    {
        public Movimientos conexion;
        public String sLastError = "";

        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbUsuario.Text == "" || tbServer.Text == "" || tbPassword.Text == "")
            {
                MessageBox.Show("Rellene todos los datos");
                return;
            }

            conexion = new Movimientos(tbServer.Text, tbUsuario.Text, tbPassword.Text);

            if (conexion.AbrirConexion())
            {
                //MessageBox.Show("Conexion realizada!...");
                Menu menu = new Menu(tbServer.Text, tbUsuario.Text, tbPassword.Text);
                menu.Show();

                conexion.CerrarConexion();
            }
            else
            {
                MessageBox.Show($"Hay un error: {sLastError}");
            }
            
        }
    }
}
