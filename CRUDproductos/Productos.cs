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
    public partial class Productos : Form
    {
        string Server, Usuario, Password;

        public Productos(string sServer, string sUsuario, string sPassword)
        {

            InitializeComponent();
            Server = sServer;
            Usuario = sUsuario;
            Password = sPassword;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbProducto.Text))
            {
                MessageBox.Show("Por favor, Seleccione el ProductoID para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si hay campos vacíos
            }
            Movimientos delete = new Movimientos(Server, Usuario, Password);
            if (delete.EliminarProductos(tbProducto.Text))
            {
                MessageBox.Show("Se a Eliminado con Exito.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(delete.sLastError);
            }
            Limpiar();
        }

        private void Productos_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void tbProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                Buscar buscarxproducto = new Buscar(Server, Usuario, Password);
                buscarxproducto.ShowDialog();
                tbProducto.Text = buscarxproducto.IDproducto;
                tbVenta.Text = buscarxproducto.Pventa;
                tbDescripcion.Text = buscarxproducto.Descripcion;
                tbSaldo.Text = buscarxproducto.Pcompra;
            }
            if (e.KeyCode == Keys.Enter)
            {
                tbDescripcion.Focus();   
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbProducto.Text))
            {
                MessageBox.Show("Por favor, Seleccione el ProductoID para Actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si hay campos vacíos
            }
            Movimientos Actualizar = new Movimientos(Server, Usuario, Password);  
            Actualizar.ActualizarProductoPorID(tbProducto.Text,tbDescripcion.Text,tbVenta.Text,tbSaldo.Text);
        }

        private void tbProducto_TextChanged(object sender, EventArgs e)
        {
            Movimientos Prod = new Movimientos(Server, Usuario, Password);
            DataTable detalles = Prod.ProductosPorID(tbProducto.Text);

            if (detalles.Rows.Count > 0)
            {
                
                DataRow detalle = detalles.Rows[0];

                tbProducto.Text = detalle["ProductoID"].ToString();
                tbDescripcion.Text = detalle["Descripcion"].ToString();
                tbVenta.Text = detalle["PVenta"].ToString();
                tbSaldo.Text = detalle["Saldo"].ToString();
            }
            else
            {
                tbSaldo.Text = "";
                tbDescripcion.Text = "";
                tbVenta.Text = "";
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

        private void tbDescripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbVenta.Focus();
            }

        }

        private void tbVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbSaldo.Focus();
            }

        }

        private void tbSaldo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(tbProducto.Text) || string.IsNullOrEmpty(tbDescripcion.Text) || string.IsNullOrEmpty(tbVenta.Text) || string.IsNullOrEmpty(tbSaldo.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Salir del método si hay campos vacíos
                }

                // Confirmar con el usuario antes de realizar la acción
                DialogResult confirmacion = MessageBox.Show("¿Está seguro de guardar los cambios?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    Movimientos realizarMovimientos = new Movimientos(Server, Usuario, Password);

                    // Verificar si el producto ya existe en la base de datos
                    DataTable productoExistente = realizarMovimientos.ProductosPorID(tbProducto.Text);

                    if (productoExistente.Rows.Count > 0)
                    {
                        // El producto ya existe, realizar actualización
                        if (realizarMovimientos.ActualizarProductoPorID(tbProducto.Text, tbDescripcion.Text, tbVenta.Text, tbSaldo.Text))
                        {
                            MessageBox.Show("Se ha actualizado el producto con éxito.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(realizarMovimientos.sLastError);
                        }
                    }
                    else
                    {
                        // El producto no existe, realizar inserción
                        if (realizarMovimientos.AgregarProducto(tbProducto.Text, tbDescripcion.Text, Convert.ToDouble(tbVenta.Text), Convert.ToDouble(tbSaldo.Text)))
                        {
                            MessageBox.Show("Se ha guardado el nuevo producto con éxito.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(realizarMovimientos.sLastError);
                        }
                    }

                    Limpiar();
                }

            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbProducto.Text) || string.IsNullOrEmpty(tbDescripcion.Text) || string.IsNullOrEmpty(tbVenta.Text) || string.IsNullOrEmpty(tbSaldo.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Salir del método si hay campos vacíos
            }

            Movimientos realizarMovimientos = new Movimientos(Server, Usuario, Password);

            // Verificar si el producto ya existe en la base de datos
            DataTable productoExistente = realizarMovimientos.ProductosPorID(tbProducto.Text);

            if (productoExistente.Rows.Count > 0)
            {
                // El producto ya existe, realizar actualización
                if (realizarMovimientos.ActualizarProductoPorID(tbProducto.Text, tbDescripcion.Text, tbVenta.Text, tbSaldo.Text))
                {
                    MessageBox.Show("Se ha actualizado el producto con éxito.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(realizarMovimientos.sLastError);
                }
            }
            else
            {
                // El producto no existe, realizar inserción
                if (realizarMovimientos.AgregarProducto(tbProducto.Text, tbDescripcion.Text, Convert.ToDouble(tbVenta.Text), Convert.ToDouble(tbSaldo.Text)))
                {
                    MessageBox.Show("Se ha guardado el nuevo producto con éxito.", "Realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(realizarMovimientos.sLastError);
                }
            }

            Limpiar();
        }
        private void Limpiar()
        {
            tbSaldo.Text = "";
            tbProducto.Text = "";
            tbDescripcion.Text = "";
            tbVenta.Text = "";
        }
    }
}
