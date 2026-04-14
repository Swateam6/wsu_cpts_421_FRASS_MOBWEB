using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MOBWEB_TEST.sqllite
{
    public class LocalDbService
    {
        // Mobile-safe file extension
        private const string DB_NAME = "mobweb.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            // Guaranteed read/write directory for Android/iOS
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
            _connection = new SQLiteAsyncConnection(dbPath);
        }

        // Safe initialization method to call when the app starts
        public async Task InitAsync()
        {
            await _connection.CreateTableAsync<tree_data>();
            await _connection.CreateTableAsync<plot_data>();
            await _connection.CreateTableAsync<stand_data>();
            await _connection.CreateTableAsync<parcel_data>();
            await _connection.CreateTableAsync<user_data>();
        }

        public string GetCurrentDatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
        }

        /// TREE FUNCTIONS -----------------------------------
        public async Task<List<tree_data>> GetAllTreeDataAsync()
        {
            return await _connection.Table<tree_data>().ToListAsync();
        }

        public async Task<tree_data> GetTreeDataByIdAsync(int id)
        {
            return await _connection.Table<tree_data>().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddTreeDataAsync(tree_data data)
        {
            await _connection.InsertAsync(data);
        }

        public async Task UpdateTreeDataAsync(tree_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteTreeDataAsync(tree_data data)
        {
            await _connection.DeleteAsync(data);
        }

        /// PLOT FUNCTIONS -----------------------------------
        public async Task<List<plot_data>> GetAllPlotDataAsync()
        {
            return await _connection.Table<plot_data>().ToListAsync();
        }

        public async Task<plot_data> GetPlotDataByIdAsync(int id)
        {
            return await _connection.Table<plot_data>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddPlotDataAsync(plot_data data)
        {
            await _connection.InsertAsync(data);
        }

        public async Task UpdatePlotDataAsync(plot_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeletePlotDataAsync(plot_data data)
        {
            await _connection.DeleteAsync(data);
        }

        /// STAND FUNCTIONS -----------------------------------
        public async Task<List<stand_data>> GetAllStandDataAsync()
        {
            return await _connection.Table<stand_data>().ToListAsync();
        }

        public async Task<stand_data> GetStandDataByIdAsync(int id)
        {
            return await _connection.Table<stand_data>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddStandDataAsync(stand_data data)
        {
            await _connection.InsertAsync(data);
        }

        public async Task UpdateStandDataAsync(stand_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteStandDataAsync(stand_data data)
        {
            await _connection.DeleteAsync(data);
        }

        /// PARCEL FUNCTIONS -----------------------------------
        public async Task<List<parcel_data>> GetAllParcelDataAsync()
        {
            return await _connection.Table<parcel_data>().ToListAsync();
        }

        public async Task<parcel_data> GetParcelDataByIdAsync(int id)
        {
            return await _connection.Table<parcel_data>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddParcelDataAsync(parcel_data data)
        {
            await _connection.InsertAsync(data);
        }

        public async Task UpdateParcelDataAsync(parcel_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteParcelDataAsync(parcel_data data)
        {
            await _connection.DeleteAsync(data);
        }

        /// USER FUNCTIONS -----------------------------------
        public async Task<List<user_data>> GetAllUserDataAsync()
        {
            return await _connection.Table<user_data>().ToListAsync();
        }

        public async Task<user_data> GetUserDataByIdAsync(int id)
        {
            return await _connection.Table<user_data>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddUserDataAsync(user_data data)
        {
            await _connection.InsertAsync(data);
        }

        public async Task UpdateUserDataAsync(user_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteUserDataAsync(user_data data)
        {
            await _connection.DeleteAsync(data);
        }
        public async Task<stand_data> GetStandByIdAsync(int standId)
        {
            return await _connection.Table<stand_data>()
                                    .Where(s => s.Id == standId)
                                    .FirstOrDefaultAsync();
        }

        // 2. Get all Plots that belong to that Stand
        public async Task<List<plot_data>> GetPlotsForStandAsync(int standId)
        {
            // This looks at the child table and grabs only the rows 
            // where the foreign key matches the parent!
            return await _connection.Table<plot_data>()
                                    .Where(p => p.ParentStandId == standId)
                                    .ToListAsync();
        }

        public async Task<List<tree_data>> GetTreesInPlotsID(int plotID)
        {
            return await _connection.Table<tree_data>()
                .Where(t=>t.parentPlotId==plotID)
                .ToListAsync();
        }
        public async Task <List<stand_data>> GetStandsinParcels(int parcelID)
        {
            return await _connection.Table<stand_data>()
                .Where(s => s.ParcelID == parcelID)
                .ToListAsync();
        }
        public async Task<List<parcel_data>> GetParcelsinUser(int userID)
        {
            return await _connection.Table<parcel_data>()
                .Where(p => p.parentUserId == userID)
                .ToListAsync();
        }

    }
}