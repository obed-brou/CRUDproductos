using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbSqlServer
{ 
    public class datosInventarios
    {
        ArrayList renglonesDetalle = new ArrayList();

        public Int32 nFolio;
        public DateTime dtFecha;
        public Double dTotal;
        public String sTipoMovimiento;
        public Double dPrecio;
        public Int32 dCantidad;
        public string sProductoID;

        public void AddRow(String sProductoID, Double dPrecio, Double dCantidad)
        {
            ArrayList row = new ArrayList();

            row.Add(sProductoID);
            row.Add(dPrecio);
            row.Add(dCantidad);

            renglonesDetalle.Add(row);
        }
        public void GetRow(Int32 nNumRow, ref String sProductoID, ref Double dPrecio, ref Double dCantidad)
        {
            ArrayList row = (ArrayList)renglonesDetalle[nNumRow];

            sProductoID = row[0].ToString();
            dPrecio = Double.Parse(row[1].ToString());
            dCantidad = Double.Parse(row[2].ToString());
        }

        public Int32 NumRows()
        {
            return renglonesDetalle.Count;
        }
    }
    public class opInventarios
    {
        SqlConnection Conexion = new SqlConnection(@"Server=LAPTOP-1A844UJR\SQLEXPRESS; Database=InventariosBD; User Id=obed_brou; Password=230318;");
        public String sLastError = "";

        public bool Alta(datosInventarios datos)
        {
            bool bAllOk = true;

            try
            {
                using (Conexion)
                {
                    Conexion.Open();

                    SqlTransaction transaccion = Conexion.BeginTransaction();

                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Conexion;
                        cmd.CommandText = $"INSERT INTO Inventarios(Folio, Fecha, Total, TipoMovimineto) " +
                                          $"VALUES({datos.nFolio}, '{datos.dtFecha.ToString("dd/MM/yyyy")}', '{datos.dTotal}', '{datos.sTipoMovimiento}')";

                        cmd.Transaction = transaccion;
                        cmd.ExecuteNonQuery();

                        for (int i = 0; i < datos.NumRows(); i++)
                        {
                            string sProductoID = "";
                            double dPrecio = 0;
                            double dCantidad = 0;

                            datos.GetRow(i, ref sProductoID, ref dPrecio, ref dCantidad);

                            SqlCommand cmdDetalle = new SqlCommand();
                            cmdDetalle.Connection = Conexion;
                            cmdDetalle.CommandText = $"INSERT INTO InventarioDetalle(Folio,ProductoID, Cantidad) " +
                                                    $"VALUES({datos.nFolio},'{sProductoID}', '{dCantidad}')";

                            cmdDetalle.Transaction = transaccion;
                            cmdDetalle.ExecuteNonQuery();

                            if (!ExisteSaldo(sProductoID, cmd))
                            {
                                SqlCommand cmdSaldo = new SqlCommand();
                                cmdSaldo.Connection = Conexion;
                                cmdSaldo.Transaction = transaccion;
                                cmdSaldo.CommandText = $"INSERT INTO Productos(ProductoID, Saldo) " +
                                                      $"VALUES('{sProductoID}', '{dCantidad}')";

                                cmdSaldo.ExecuteNonQuery();
                            }
                            else
                            {
                                SqlCommand cmdSaldo = new SqlCommand();
                                cmdSaldo.Connection = Conexion;
                                cmdSaldo.Transaction = transaccion;

                                cmdSaldo.CommandText = $"UPDATE Productos " +
                                                      $"SET Saldo = Saldo + {dCantidad} " +
                                                      $"WHERE ProductoID = '{sProductoID}'";

                                cmdSaldo.ExecuteNonQuery();
                            }
                        }

                        transaccion.Commit();
                        Conexion.Close();
                    }
                    catch (Exception ex)
                    {
                        bAllOk = false;
                        sLastError = ex.Message;
                        transaccion.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                bAllOk = false;
                sLastError = ex.Message;
            }

            return bAllOk;
        }

        private bool ExisteSaldo(string sProductoId, SqlCommand cmd)
        {
            string strcmd = $"SELECT COUNT(*) FROM Productos WHERE ProductoID = '{sProductoId}'";

            cmd.CommandText = strcmd;
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

    }
}
