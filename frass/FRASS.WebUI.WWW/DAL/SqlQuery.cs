using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FRASS.WebUI.WWW.DAL
{
	public interface ISqlQuery
	{
		IDataReader ExecuteReader(string commandText, params SqlParameter[] parameters);
		object ExecuteScalar(string commandText, params SqlParameter[] parameters);
		void ExecuteNonQuery(string commandText, params SqlParameter[] parameters);
	}

	public class SqlQuery : ISqlQuery
	{
		private readonly string ConnectionString;

		private SqlQuery(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public static ISqlQuery GetInstance(string connectionString)
		{
			return new SqlQuery(connectionString);
		}

		public IDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
		{
			return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, commandText, parameters);
		}

		public object ExecuteScalar(string commandText, params SqlParameter[] parameters)
		{
			return SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, commandText, parameters);
		}

		public void ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText, parameters);
		}
	}
}