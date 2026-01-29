using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FRASS.WebUI.WWW.DAL
{
	public class Repository
	{
		private ISqlQuery Querier;

		private Repository(ISqlQuery querier)
		{
			Querier = querier;
		}

		public static Repository GetInstance(ISqlQuery querier)
		{
			return new Repository(querier);
		}

		public int SaveOrder(string name, string street, string city, string state, string zip, int countryID, string email, int numberOfCopies, bool isTaxExempt, ShippingTypes shippingOption)
		{
			var sql = @"
			INSERT INTO [dbo].[Orders]
			   ([Name]
			   ,[Street]
			   ,[City]
			   ,[State]
			   ,[Zip]
			   ,[CountryID]
			   ,[Email]
			   ,[IsExempt]
			   ,[ShippingTypeID])
			VALUES
		   (@name
				, @Street
				, @City
				, @State
				, @Zip
				, @CountryID
				, @Email
				, @IsExempt
				, @ShippingTypeID)
			; Select Scope_Identity();";
			SqlParameter[] parms = new SqlParameter[]
			{
				new SqlParameter("@Name",name),
				new SqlParameter("@Street", street),
				new SqlParameter("@City", city),
				new SqlParameter("@State", state),
				new SqlParameter("@Zip", zip),
				new SqlParameter("@CountryID", countryID),
				new SqlParameter("@Email", email),
				new SqlParameter("@IsExempt", isTaxExempt),
				new SqlParameter("@ShippingTypeID", Convert.ToInt32(shippingOption))
			};
			return Convert.ToInt32(Querier.ExecuteScalar(sql, parms));
		}
	}
}