using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SerreVeure.Data
{
    public abstract class DataConnectorBase : IDisposable
    {
        protected SqlConnection cnx = null;

        protected DataConnectorBase()
        {
            cnx = new SqlConnection("server=DESKTOP-CLS\\SQLEXPRESS; Initial Catalog=SuperFlux; Integrated Security=true;");

            cnx.Open();
        }

        protected void ExecuteNonQuery(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, cnx);
            cmd.ExecuteNonQuery();
        }

        protected DataTable ExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand(sql, cnx);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(dt);

            return dt;
        }

        protected List<T> ConvertDataTableToList<T>(DataTable dt, Func<DataRow, T> convert)
        {
            List<T> result = null;

            if (convert == null)
                throw new InvalidOperationException("convert parameter cannot be null");

            if (dt != null)
            {
                result = new List<T>();
                foreach (DataRow dr in dt.Rows)
                    result.Add(convert(dr));
            }

            return result;
        }

        #region Disposable

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            if (disposing)
                cnx.Close();
        }

        #endregion
    }
}
