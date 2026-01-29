using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;

namespace FRASS.DAL.Repositories
{
	internal class UserRepository
	{
		private FRASSDataContext db;
		private UserRepository()
		{
			db = new FRASSDataContext();
		}
		public static UserRepository GetInstance()
		{
			return new UserRepository();
		}
		public void AddNewUser(User user, int createdByID)
		{
			User u = new User();
			u.FirstName = user.FirstName;
			u.LastName = user.LastName;
			u.Email = user.Email;
			u.Company = user.Company;
			u.PhoneNumber = user.PhoneNumber;
			u.Hash = "";
			u.UserTypeID = user.UserTypeID;
			u.UserStatusID = user.UserStatusID;
			if (user.UserStatusID == Convert.ToInt32(UserStatusTypes.NeedsPasswordReset))
			{
				user.PasswordExpires = DateTime.Now.AddDays(1);
			}
			u.CreatedBy = createdByID;
			u.DateCreated = System.DateTime.Now;
			db.Users.InsertOnSubmit(u);
			db.SubmitChanges();
			User u2 = GetUser(u.Email);
			u2.Password = user.Password;
			u2.Hash = u2.GetHash();
			db.SubmitChanges();
		}
		public User Login(string email, string password)
		{
			User user = (from u in db.Users where u.Email.ToLower() == email.ToLower() && (u.UserStatusID == 1 || u.UserStatusID == 4) select u).FirstOrDefault();
			if (user != null)
			{
				if (user.PasswordExpires < DateTime.Now)
				{
					user.PasswordExpires = null;
					user.UserStatusID = Convert.ToInt32(UserStatusTypes.LockedOut);
					db.SubmitChanges();
					return null;
				}
				else
				{
					user.Password = password;
					if (user.Hash == user.GetHash())
					{
						user.WrongPasswordCount = 0;
						db.SubmitChanges();
						return user;
					}
					else
					{
						user.WrongPasswordCount = user.WrongPasswordCount + 1;
						if (user.WrongPasswordCount > 3)
						{
							user.UserStatusID = 3;
						}
						db.SubmitChanges();
						return null;
					}
				}
			}
			else
			{
				return null;
			}
		}

		public User GetUser(Int32 userid)
		{
			var user = (from u in db.Users where u.UserID == userid select u).FirstOrDefault();
			return user;
		}
		public User GetUserByHash(string hash)
		{
			var user = (from u in db.Users where u.Hash == hash select u).FirstOrDefault();
			return user;
		}
		public User GetUser(string email)
		{
			var user = (from u in db.Users where u.Email == email select u).FirstOrDefault();
			return user;
		}
		public User GetUser(string firstname, string lastname, string email)
		{
			var user = (from u in db.Users where u.FirstName == firstname && u.LastName == lastname && u.Email == email select u).FirstOrDefault();
			return user;
		}
		public User GetUser(string hash, Int32 userid)
		{
			User u = (from user in db.Users where user.Hash == hash && user.UserID == userid select user).FirstOrDefault();
			return u;
		}
		public void UpdateUser(User user)
		{
			var u = (from us in db.Users where us.UserID == user.UserID select us).FirstOrDefault();
			u.FirstName = user.FirstName;
			u.LastName = user.LastName;
			u.Email = user.Email;
			u.UserTypeID = user.UserTypeID;
			db.SubmitChanges();
		}
		public void UpdatePassword(User user)
		{
			user.PasswordExpires = null;
			user.Hash = user.GetHash();
			db.SubmitChanges();
		}
		public void SetUserStatusTypes(User user, UserStatusTypes status)
		{
			user.UserStatusID = Convert.ToInt32(status);
			if (status == UserStatusTypes.NeedsPasswordReset)
			{
				user.PasswordExpires = DateTime.Now.AddDays(1);
			}
			db.SubmitChanges();
		}

		public void SetUserCreatedBy(User user, User createdBy)
		{
			user.CreatedBy = createdBy.UserID;
			db.SubmitChanges();
		}
		public IQueryable<User> GetUsers()
		{
			var u = from user in db.Users select user;
			return u;
		}
		public void DeleteUser(Int32 userid)
		{
			var user = GetUser(userid);
			var smpRepo = StumpageMarketModelRepository.GetInstance();
			foreach (var smp in user.StumpageModelPortfolios)
			{
				smpRepo.DeleteStumpageModelPortfolio(smp);
			}
		
			user = GetUser(userid);
			db.Logs.DeleteAllOnSubmit(user.Logs);
			db.SubmitChanges();

			user = GetUser(userid);
			db.Reports.DeleteAllOnSubmit(user.Reports);
			db.SubmitChanges();

			var mmpRepo = DeliveredLogMarketModelRepository.GetInstance();
			var db3 = new FRASSDataContext();
			var mmps = db3.MarketModelPortfolios.Where(uu => uu.UserID == userid);
			foreach (var mmp in mmps)
			{
				mmpRepo.DeleteMarketModelPortfolio(mmp);
			}

			var rpaRepository = RPAPortfolioRepository.GetInstance();
			//rpaRepository.DeleteRPAPortfolio(rpa);

			var db4 = new FRASSDataContext();
			db4.Users.DeleteAllOnSubmit(from u in db4.Users where u.UserID == userid select u);
			db4.SubmitChanges();
		}
	}
}
