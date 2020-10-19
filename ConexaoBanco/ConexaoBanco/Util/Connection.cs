using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ConexaoBanco.Util
{
    public class Connection
    {
        private SqlConnection sqlConn;

        public SqlConnection OpenConnection()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ExemploConexaoBanco"].ConnectionString;
                SqlConnection sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                this.sqlConn = sqlConn;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível acessar a base de dados do sistema.");
            }
            return sqlConn;
        }

        public void closeConnection()
        {
            if (this.sqlConn != null) { sqlConn.Close(); }
        }
    }
}