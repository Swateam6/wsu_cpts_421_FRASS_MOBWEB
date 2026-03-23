using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBWEB_TEST.sqllite
{
    class LocalDbService
    {
        private const string DB_NAME = "FRASS_MOBWEB.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory,DB_NAME));
            _connection.CreateTableAsync<tree_data>().Wait();
        }

        public async Task<List<tree_data>> GetAllTreeDataAsync()
        {
            return await _connection.Table<tree_data>().ToListAsync();
        }

        public async Task<tree_data > GetTreeDataByIdAsync(int id)
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
    }
}
