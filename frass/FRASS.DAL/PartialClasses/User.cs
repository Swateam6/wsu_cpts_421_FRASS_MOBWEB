using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Web;

namespace FRASS.DAL
{
	public partial class User
	{
		public string Password { get; set; }
		public string GetHash()
		{
			return GetHash("MAS: " + UserID.ToString() + ":" + Password);
		}
		public string GetHash(string stringToHash)
		{
			System.Text.UnicodeEncoding encoding = new UnicodeEncoding();
			StringBuilder mHash = new StringBuilder();
			byte[] ByteSourceText = encoding.GetBytes(stringToHash);
			SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
			byte[] ByteHash = sha.ComputeHash(ByteSourceText);
			for (int i = 0; i < ByteHash.Length; i++)
			{
				mHash.Append(ByteHash[i].ToString("X2"));
			}

			return mHash.ToString();
		}
	}

	public enum UserStatusTypes
	{
		Active = 1,
		Draft = 2,
		LockedOut = 3,
		NeedsPasswordReset = 4,
		Inactivated = 5
	}

	public enum UsersTypes
	{
		SuperUser = 1,
		Administrator = 2,
		EconomicManager = 3,
		PortfolioManager = 4,
		Viewer = 5,
		Guest = 6
	}
}
