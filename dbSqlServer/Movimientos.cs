using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbSqlServer
{
    public class Movimientos
    {
        SqlConnection Conexion = new SqlConnection(@"Server=LAPTOP-1A844UJR\SQLEXPRESS; Database=InventariosBD; User Id=obed_brou; Password=230318;");
        public String sLastError = "";

        public Movimientos(string sServer, string sUsuario, string sPassword)
        {
            SqlConnection Conexion = new SqlConnection($"Server={sServer};Database=Inventarios;User Id={sUsuario};Password={sPassword};");

        }
        public Boolean AbrirConexion()
        {
            Boolean bALLOk = true;

            try
            {
                Conexion.Open();
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
                bALLOk = false;
            }

            return bALLOk;
        }
        public void CerrarConexion()
        {
            Conexion.Close();
        }
        public Boolean AgregarProducto(String sProductoID, String sDescripcion, Double sPrecioVenta, Double sSaldo)
        {
            Boolean bAllOk = true;

            try
            {
                using (Conexion)
                {
                    Conexion.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Conexion;
                    cmd.CommandText = "Insert into Productos(ProductoID, Descripcion, PVenta, Saldo)" +
                                     $"Values({sProductoID}, '{sDescripcion}', '{sPrecioVenta}', '{sSaldo}')";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                bAllOk = false;
                sLastError = ex.Message;
            }

            return bAllOk;
        }
        public Boolean EliminarInventarios(string sFolio)
        {
            Boolean Eliminado = true;


            try
            {
                Conexion.Open();
                string Query = $"DELETE from InventarioDetalle where Folio ='{sFolio}'";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
                Eliminado = false;
            }
            finally
            {
                Conexion.Close();
            }

            return Eliminado;
        }
        public Boolean EliminarProductos(string sProductId)
        {
            Boolean Eliminado = true;


            try
            {
                Conexion.Open();
                string Query = $"DELETE from Productos where ProductoID ='{sProductId}'";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
                Eliminado = false;
            }
            finally
            {
                Conexion.Close();
            }

            return Eliminado;
        }

        public DataTable ConsultarProductos()
        {
            DataTable data = new DataTable();

            try
            {
                Conexion.Open();
                string Query = $" SELECT * FROM Productos ";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                data.Load(comando.ExecuteReader());
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
            }
            finally
            {
                Conexion.Close();
            }

            return data;
        }
        public DataTable ProductosPorID(string sProductId)
        {
            DataTable data = new DataTable();

            try
            {
                Conexion.Open();
                string Query = $" SELECT ProductoID, Descripcion, PVenta, Saldo FROM Productos where ProductoID ='{sProductId}'";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                data.Load(comando.ExecuteReader());
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
            }
            finally
            {
                Conexion.Close();
            }

            return data;
        }
        public bool ActualizarProductoPorID(string sFolio, string sDescripcion, string sPrecio, string sSaldo)
        {
            try
            {
                Conexion.Open();

                // Verificar si el producto existe
                string verificarQuery = $"SELECT COUNT(*) FROM Productos WHERE ProductoID = '{sFolio}'";
                SqlCommand verificarComando = new SqlCommand(verificarQuery, Conexion);

                int existeProducto = Convert.ToInt32(verificarComando.ExecuteScalar());

                if (existeProducto > 0)
                {
                    // El producto existe, realizar la actualización
                    string updateQuery = $"UPDATE Productos SET Descripcion = @Descripcion, PVenta = @Precio, Saldo = @Saldo WHERE ProductoID = '{sFolio}'";
                    SqlCommand updateComando = new SqlCommand(updateQuery, Conexion);

                    updateComando.Parameters.AddWithValue("@Descripcion", sDescripcion);
                    updateComando.Parameters.AddWithValue("@Precio", sPrecio);
                    updateComando.Parameters.AddWithValue("@Saldo", sSaldo);

                    int filasAfectadas = updateComando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        // La actualización fue exitosa
                        return true;
                    }
                    else
                    {
                        // No se actualizaron filas, puede indicar que el producto no existe
                        sLastError = "No se pudo actualizar el producto. Puede que el ProductoID no exista.";
                        return false;
                    }
                }
                else
                {
                    // El producto no existe, mostrar un mensaje de error
                    sLastError = "El ProductoID especificado no existe en la base de datos.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
                return false;
            }
            finally
            {
                Conexion.Close();
            }
        }

        public DataTable ConsultarDescripcion( string sDescripcion)
        {
            DataTable data = new DataTable();

            try
            {
                Conexion.Open();
                string Query = $" SELECT ProductoID, Descripcion, PVenta FROM Productos Where Descripcion like '%{sDescripcion}%'";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                data.Load(comando.ExecuteReader());
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
            }
            finally
            {
                Conexion.Close();
            }

            return data;
        }
        public DataTable ConsultarFolio()
        {

            DataTable data = new DataTable();

            try
            {
                Conexion.Open();
                string Query = $" select Folio from Inventarios order by Folio desc";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                data.Load(comando.ExecuteReader());
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
            }
            finally
            {
                Conexion.Close();
            }

            return data;
        }
        public DataTable ConsultarInventarioDetalle(string sFolio)
        {

            DataTable data = new DataTable();

            try
            {
                Conexion.Open();

                // Modificamos la consulta para obtener los detalles del inventario según el Folio
                string Query = $@"
            SELECT
                I.Fecha AS FechaInventario,
                I.TipoMovimineto AS TipoMovimiento,
                ID.ProductoID,
                P.Descripcion,
                P.PVenta AS PrecioProducto,
                ID.Cantidad,
                I.Total AS TotalInventario
            FROM
                Inventarios I
            JOIN
                InventarioDetalle ID ON I.Folio = ID.Folio
            JOIN
                Productos P ON ID.ProductoID = P.ProductoID
            WHERE
                I.Folio = {sFolio}";

                SqlCommand comando = new SqlCommand(Query, Conexion);
                data.Load(comando.ExecuteReader());
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
            }
            finally
            {
                Conexion.Close();
            }

            return data;
        }
        public DataTable ConsultarSaldo(string sProductId)
        {
            DataTable data = new DataTable();

            try
            {
                Conexion.Open();
                string Query = $" SELECT ProductoID, Descripcion, PVenta FROM Productos where ProductoID ='{sProductId}'";
                SqlCommand comando = new SqlCommand(Query, Conexion);
                data.Load(comando.ExecuteReader());
            }
            catch (Exception ex)
            {
                sLastError = ex.Message;
            }
            finally
            {
                Conexion.Close();
            }

            return data;
        }

    }

}
