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
    public partial class Buscar : Form
    {
        string Server, Usuario, Password;
        public string IDproducto;
        public string Descripcion;
        public string Pventa;
        public string Pcompra;

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            IDproducto = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            Descripcion = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            Pventa = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            Pcompra = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            this.Close();
        }

        private void Buscar_Load(object sender, EventArgs e)
        {
            Movimientos mo = new Movimientos(Server, Usuario, Password);
            dataGridView1.DataSource = mo.ConsultarProductos();
        }

        public Buscar(string sServer, string sUsuario, string sPassword)
        {
            InitializeComponent();
            Server = sServer;
            Usuario = sUsuario;
            Password = sPassword;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Movimientos m = new Movimientos(Server, Usuario, Password);
            dataGridView1.DataSource = m.ProductosPorID(textBox1.Text);
            Movimientos y = new Movimientos(Server, Usuario, Password);
            dataGridView1.DataSource = y.ConsultarDescripcion(textBox1.Text);
        }
    }
}
