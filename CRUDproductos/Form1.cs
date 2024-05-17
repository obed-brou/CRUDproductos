using dbSqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CRUDproductos
{
    public partial class Form1 : Form
    {
        string Server, Usuario, Password;
        public Form1(string sServer, string sUsuario, string sPassword)
        {
            InitializeComponent();
            Server = sServer;
            Usuario = sUsuario;
            Password = sPassword;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            
            Movimientos Prod = new Movimientos(Server, Usuario, Password);
            DataTable detalles = Prod.ProductosPorID(tbProducto.Text);

            if (detalles.Rows.Count > 0)
            {
               
                Double totalActual = 0;
                string Producto = tbProducto.Text;
                double Precio = Convert.ToDouble(tbPventa.Text);
                double Cantidad = Convert.ToDouble(tbCantidad.Text);
                String Descripcion = tbDescripcion.Text;
                double total = Precio * Cantidad;
                dataGridView1.Rows.Add(Producto, Descripcion, Cantidad, Precio, total);

                try
                {
                    totalActual = Convert.ToDouble(tbTotal.Text);
                }
                catch (Exception)
                {
                    
                }

                tbTotal.Text = Convert.ToString(totalActual + total);
                tbProducto.Text = "";
                tbDescripcion.Text = "";
                tbCantidad.Text = "";
                tbPventa.Text = "";
            }
            else
            {
                // El producto no existe, mostrar mensaje y cancelar la operación
                MessageBox.Show("El producto no existe. No puedes agregarlo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validación: Verificar si los campos obligatorios están vacíos
            if (string.IsNullOrEmpty(cbTipoMovimiento.Text) || string.IsNullOrEmpty(tbTotal.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Salir del método si hay campos vacíos
            }
            opInventarios opInv = new opInventarios();
            datosInventarios datosInv = new datosInventarios();

            datosInv.dtFecha = dateTimePicker1.Value;
            datosInv.nFolio = int.Parse(tbFolio.Text);
            datosInv.sTipoMovimiento = cbTipoMovimiento.Text;
            datosInv.dTotal = Convert.ToInt32(tbTotal.Text);

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string Product = dataGridView1.Rows[i].Cells[0].Value.ToString();
                Double Price = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString());
                Double Quantity = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString());
                datosInv.AddRow(Product, Price, Quantity);
            }
            if (opInv.Alta(datosInv))
            {
                MessageBox.Show("Se ha Completado correctamente.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show(opInv.sLastError);
            }
            tbFolio.Text = "";
            dataGridView1.Rows.Clear();
            DataTable ti;
            Movimientos mi = new Movimientos(Server, Usuario, Password);
            ti = mi.ConsultarFolio();
            DataRow fila = ti.Rows[0];
            tbFolio.Text = (Convert.ToInt32(fila[0]) + 1).ToString();

        }

        private void tbDescripcion_Leave(object sender, EventArgs e)
        {
            Movimientos m = new Movimientos(Server, Usuario, Password);
            dataGridView1.DataSource = m.ProductosPorID(tbDescripcion.Text);
            Movimientos y = new Movimientos(Server, Usuario, Password);
            dataGridView1.DataSource = y.ConsultarDescripcion(tbDescripcion.Text);
           
        }

        private void tbProducto_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.F1)
            {
                Buscar buscarxproducto = new Buscar(Server, Usuario, Password);
                buscarxproducto.ShowDialog();
                tbProducto.Text = buscarxproducto.IDproducto;
                tbPventa.Text = buscarxproducto.Pventa;
                tbDescripcion.Text = buscarxproducto.Descripcion;
            }
            if (e.KeyCode == Keys.Enter)
            {
                tbCantidad.Focus();
            }
        }

        private void tbFolio_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
            }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Movimientos mi = new Movimientos(Server, Usuario, Password);
            if (mi.EliminarInventarios(tbFolio.Text)){

                MessageBox.Show("Se ha Eliminado correctamente.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(mi.sLastError);
            }
        }

        private void tbDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            cbTipoMovimiento.Text = "";
            tbTotal.Text = "";
            tbProducto.Text = "";
            tbDescripcion.Text = "";
            tbCantidad.Text = "";
            tbPventa.Text = "";
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Asegurarse de que se hizo clic en una fila, no en los encabezados
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Aquí puedes obtener los valores de las celdas según las columnas de tu DataGridView
                string producto = selectedRow.Cells[0].Value.ToString();
                string descripcion = selectedRow.Cells[1].Value.ToString();
                string cantidad = selectedRow.Cells[2].Value.ToString();
                string precio = selectedRow.Cells[3].Value.ToString();
                //string total = selectedRow.Cells["Total"].Value.ToString();

                // Ahora, puedes implementar la lógica para eliminar la fila o realizar otras acciones
                EliminarFilaSeleccionada(e.RowIndex);
            }
        }
        private void EliminarFilaSeleccionada(int rowIndex)
        {
            // Aquí puedes implementar la lógica para eliminar la fila del DataGridView y de la base de datos

            // Ejemplo de eliminación de la fila en el DataGridView
            dataGridView1.Rows.RemoveAt(rowIndex);

            // Además, si deseas eliminar la fila de la base de datos, debes implementar la lógica adecuada.
            // Puedes utilizar el folio, producto o cualquier otro identificador único para realizar la eliminación en tu base de datos.
        }

        private void tbFolio_TextChanged(object sender, EventArgs e)
        {
            string folioSeleccionado = tbFolio.Text; // Asumiendo que los folios están en un ComboBox
            MostrarDetalleInventario(folioSeleccionado);

            Movimientos mi = new Movimientos(Server, Usuario, Password);
            DataTable detalles = mi.ConsultarInventarioDetalle(tbFolio.Text);

            if (detalles.Rows.Count > 0)
            {
                btnAgregar.Enabled = false;
                tbProducto.Enabled = false;
                tbTotal.Enabled = false;
                tbDescripcion.Enabled = false;
                cbTipoMovimiento.Enabled = false;
                tbCantidad.Enabled = false;
                dateTimePicker1.Enabled = false;
                tbPventa.Enabled = false;
                btnGuardar.Enabled = false;
                
            }
            else
            {
                btnAgregar.Enabled = true;
                tbProducto.Enabled = true;
                tbTotal.Enabled = true;
                tbDescripcion.Enabled = true;
                cbTipoMovimiento.Enabled = true;
                tbCantidad.Enabled = true;
                dateTimePicker1.Enabled = true;
                tbPventa.Enabled = true;
                btnGuardar.Enabled = true;
                
            }
        }

        private void tbProducto_TextChanged(object sender, EventArgs e)
        {
            Movimientos Prod = new Movimientos(Server, Usuario, Password);
            DataTable detalles = Prod.ProductosPorID(tbProducto.Text);
            if (detalles.Rows.Count > 0)
            {
                // Mostrar detalles en los controles
                DataRow detalle = detalles.Rows[0];

                tbProducto.Text = detalle["ProductoID"].ToString();
                tbDescripcion.Text = detalle["Descripcion"].ToString();
                tbPventa.Text = detalle["PVenta"].ToString();


            }
            else
            {
                tbDescripcion.Text = "";
                tbPventa.Text = "";
            }
        }

        private void tbProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Ignora la tecla no válida.

                // Muestra un mensaje de advertencia solo si no es Enter ni Tab.
                if (e.KeyChar != (char)Keys.Enter && e.KeyChar != (char)Keys.Tab)
                {
                    MessageBox.Show("Por favor, ingrese solo números.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable t;
            Movimientos mi = new Movimientos(Server, Usuario, Password);
            t = mi.ConsultarFolio();
            DataRow fila = t.Rows[0];
            tbFolio.Text = (Convert.ToInt32(fila[0]) + 1).ToString();
            cbTipoMovimiento.Items.Add("Entradas");
            cbTipoMovimiento.Items.Add("Salidas");

            // Seleccionar la primera opción por defecto
            cbTipoMovimiento.SelectedIndex = 0;
        }
        private void MostrarDetalleInventario(string sFolio)
        {
            Movimientos mi = new Movimientos(Server, Usuario, Password);
            DataTable detalles = mi.ConsultarInventarioDetalle(sFolio);

            if (detalles.Rows.Count > 0)
            {
                // Mostrar detalles en los controles
                DataRow detalle = detalles.Rows[0];

                dateTimePicker1.Value = Convert.ToDateTime(detalle["FechaInventario"]);
                cbTipoMovimiento.Text = detalle["TipoMovimiento"].ToString();
                tbProducto.Text = detalle["ProductoID"].ToString();
                tbDescripcion.Text = detalle["Descripcion"].ToString();
                tbPventa.Text = detalle["PrecioProducto"].ToString();
                tbCantidad.Text = detalle["Cantidad"].ToString();
                tbTotal.Text = detalle["TotalInventario"].ToString();

                // Mostrar detalles en el DataGridView
                dataGridView1.DataSource = detalles;

                // Personalizar las columnas específicas en el DataGridView
                for (int i = 0; i < detalles.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = detalles.Rows[i]["ProductoID"];
                    dataGridView1.Rows[i].Cells[1].Value = detalles.Rows[i]["Descripcion"];
                    dataGridView1.Rows[i].Cells[2].Value = detalles.Rows[i]["Cantidad"];
                    dataGridView1.Rows[i].Cells[3].Value = detalles.Rows[i]["PrecioProducto"];
                }
                //dataGridView1.DataSource = detalles;
                //Agregar fila al DataGridView
                //dataGridView1.Rows.Add(
                //    detalle["ProductoID"],     // Columna 0: Producto
                //    detalle["Descripcion"],    // Columna 1: Descripcion
                //    detalle["Cantidad"],        // Columna 2: Cantidad
                //    detalle["PrecioProducto"],  // Columna 3: Precio
                //    detalle["TotalInventario"]  // Columna 4: Total
                //);
            }
            else
            {
                //MessageBox.Show("Aun no se encuentran detalles registrados para el folio especificado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



    }
}
