using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;

namespace FRASS.DAL.DataManager
{
	public class UserDataManager
	{
		UserRepository db;
		private UserDataManager()
		{
			db = UserRepository.GetInstance();
		}
		public static UserDataManager GetInstance()
		{
			return new UserDataManager();
		}

		public void AddNewUser(User user, int createdByID)
		{
			db.AddNewUser(user, createdByID);
		}
		public User Login(string email, string password)
		{
			return db.Login(email, password);
		}

		public User GetUser(Int32 userid)
		{
			return db.GetUser(userid);
		}
		public User GetUserByHash(string hash)
		{
			return db.GetUserByHash(hash);
		}
		public User GetUser(string email)
		{
			return db.GetUser(email);
		}
		public User GetUser(string firstname, string lastname, string email)
		{
			return db.GetUser(firstname, lastname, email);
		}
		public User GetUser(string hash, Int32 userid)
		{
			return db.GetUser(hash, userid);
		}
		public void UpdateUser(User user)
		{
			db.UpdateUser(user);
		}
		public void UpdatePassword(User user)
		{
			db.UpdatePassword(user);
		}
		public void DeleteUser(int userID)
		{
			db.DeleteUser(userID);
		}
		public void SetUserStatusTypes(User user, UserStatusTypes status)
		{
			db.SetUserStatusTypes(user, status);
		}

		public void SetUserCreatedBy(User user, User createdBy)
		{
			db.SetUserCreatedBy(user, createdBy);
		}
		public IQueryable<User> GetUsers()
		{
			return db.GetUsers();
		}
	}
}
