using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace ConexaoBanco.Util
{
    public class DatabaseHelper
    {
        public static SqlDataReader ExecuteReader(string query, SqlParameter[] parameters)
        {
            SqlDataReader dr = null;
            Connection conn = new Connection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn.OpenConnection();
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                if (dr != null) { dr.Close(); }
                throw ex;
            }
            return dr;
        }
        public static DataTable DataTable(string query, SqlParameter[] parameters)
        {
            SqlDataAdapter da = null;
            DataTable dataTable = null;
            Connection conn = new Connection();
            try
            {
                dataTable = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn.OpenConnection();
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);

                da = new SqlDataAdapter(cmd);
                da.Fill(dataTable); // preenche o datatable
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.closeConnection(); }
                da.Dispose();
            }
            return dataTable;
        }
        public static DataTable DataTableProcedure(string procedure, SqlParameter[] parameters)
        {
            SqlDataAdapter da = null;
            DataTable dataTable = null;
            Connection conn = new Connection();
            try
            {
                dataTable = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn.OpenConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedure;
                cmd.Parameters.AddRange(parameters);

                da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.closeConnection(); }
                da.Dispose();
            }
            return dataTable;
        }
        public static void ExecuteNonQueryStoredProcedure(string procedure, SqlParameter[] parameters)
        {
            Connection conn = new Connection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn.OpenConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedure;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.closeConnection(); }
            }
        }
        public static Object ExecuteScalar(string query, SqlParameter[] parameters)
        {
            Object obj = null;
            Connection conn = new Connection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn.OpenConnection();
                cmd.CommandText = query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.closeConnection(); }
            }
            return obj;
        }
        public static void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            Connection conn = new Connection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn.OpenConnection();
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.closeConnection(); }
            }
        }
        public static void ExecuteCommands(List<SqlCommand> commands)
        {
            Connection conn = new Connection();
            SqlTransaction transaction = null;
            try
            {
                //SqlConnection connection = conn.OpenConnection();
                transaction = conn.OpenConnection().BeginTransaction();
                foreach (var command in commands)
                {
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null) { transaction.Rollback(); }
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.closeConnection(); }
            }
        }
        public static object IsNullOrWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }
        public static string DataTable_JSON_StringBuilder(DataTable tabela)
        {
            var JSONString = new StringBuilder();
            if (tabela.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < tabela.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < tabela.Columns.Count; j++)
                    {
                        if (j < tabela.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + tabela.Columns[j].ColumnName.ToString() + "\":" + "\"" + tabela.Rows[i][j].ToString().Replace("\"", "'") + "\",");
                        }
                        else if (j == tabela.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + tabela.Columns[j].ColumnName.ToString() + "\":" + "\"" + tabela.Rows[i][j].ToString().Replace("\"", "'") + "\"");
                        }
                    }
                    if (i == tabela.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }
        public static string DataTable_JSON_JavaSerializer(DataTable tabela)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = Int32.MaxValue;
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            try
            {
                Dictionary<string, object> childRow;
                foreach (DataRow row in tabela.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in tabela.Columns)
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                    parentRow.Add(childRow);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsSerializer.Serialize(parentRow);
        }
    }
}