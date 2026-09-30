using MOBWEB_TEST.Models;
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
        private const string DB_NAME = "mobweb.sql";
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
            await _connection.CreateTableAsync<defect_data>();
            await _connection.CreateTableAsync<tree_data>();
            await _connection.CreateTableAsync<plot_data>();
            await _connection.CreateTableAsync<stand_data>();
            await _connection.CreateTableAsync<parcel_data>();
            await _connection.CreateTableAsync<user_data>();
        }
        public async Task ResetIncrements()
        {
            await _connection.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='defect_data'");
            await _connection.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='tree_data'");
            await _connection.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='plot_data'");
            await _connection.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='stand_data'");
            await _connection.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='parcel_data'");
            await _connection.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='user_data'");
        }
        public string GetCurrentDatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
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

        public async Task<int> AddUserDataAsync(user_data data)
        {
            await _connection.InsertAsync(data);
            return data.Id;
        }

        public async Task UpdateUserDataAsync(user_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteUserDataAsync(user_data data)
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

        public async Task<List<parcel_data>> GetParcelsinUser(int userID)
        {
            return await _connection.Table<parcel_data>()
                .Where(p => p.parentUserId == userID)
                .ToListAsync();
        }

        public async Task<int> AddParcelDataAsync(parcel_data data)
        {
            await _connection.InsertAsync(data);
            return data.Id;
        }

        public async Task UpdateParcelDataAsync(parcel_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteParcelDataAsync(parcel_data data)
        {
            await _connection.DeleteAsync(data);
        }

        /// STAND FUNCTIONS -----------------------------------
        public async Task<List<stand_data>> GetAllStandDataAsync()
        {
            return await _connection.Table<stand_data>().ToListAsync();
        }

        public async Task<stand_data> GetStandDataByIdAsync(int standId) // Consolidated from GetStandByIdAsync
        {
            return await _connection.Table<stand_data>().Where(s => s.Id == standId).FirstOrDefaultAsync();
        }

        public async Task<List<stand_data>> GetStandsinParcels(int parcelID)
        {
            return await _connection.Table<stand_data>()
                .Where(s => s.ParcelID == parcelID)
                .ToListAsync();
        }

        public async Task<int> AddStandDataAsync(stand_data data)
        {
            await _connection.InsertAsync(data);
            return data.Id;
        }

        public async Task UpdateStandDataAsync(stand_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteStandDataAsync(stand_data data)
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

        public async Task<List<plot_data>> GetPlotsForStandAsync(int standId)
        {
            return await _connection.Table<plot_data>()
                                    .Where(p => p.ParentStandId == standId)
                                    .ToListAsync();
        }

        public async Task<int> AddPlotDataAsync(plot_data data)
        {
            await _connection.InsertAsync(data);
            return data.Id;
        }

        public async Task UpdatePlotDataAsync(plot_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeletePlotDataAsync(plot_data data)
        {
            await _connection.DeleteAsync(data);
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

        public async Task<List<tree_data>> GetTreesInPlotsID(int plotID)
        {
            return await _connection.Table<tree_data>()
                .Where(t => t.parentPlotId == plotID)
                .ToListAsync();
        }

        public async Task<int> AddTreeDataAsync(tree_data data)
        {
            await _connection.InsertAsync(data);
            return data.Id;
        }

        public async Task UpdateTreeDataAsync(tree_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteTreeDataAsync(tree_data data)
        {
            await _connection.DeleteAsync(data);
        }

        /// DEFECT FUNCTIONS -----------------------------------
        public async Task<List<defect_data>> GetAllDefectDataAsync()
        {
            return await _connection.Table<defect_data>().ToListAsync();
        }

        public async Task<defect_data> GetDefectDataByIdAsync(int parentTreeID)
        {
            return await _connection.Table<defect_data>().Where(d => d.parentTreeId == parentTreeID).FirstOrDefaultAsync();
        }

        public async Task<int> AddDefectDataAsync(defect_data data)
        {
            await _connection.InsertAsync(data);
            return data.Id;
        }

        public async Task UpdateDefectDataAsync(defect_data data)
        {
            await _connection.UpdateAsync(data);
        }

        public async Task DeleteDefectDataAsync(defect_data data)
        {
            await _connection.DeleteAsync(data);
        }


        /// DEMO SEEDER (Kamiak Butte) -----------------------------------
        public async Task SeedKamiakStand()
        {
            // 1. Safety Check: Don't seed if it's already in the database
            var existingStands = await GetAllStandDataAsync();
            if (existingStands.Any(s => s.HabitatType == "Kamiak Butte"))
            {
                return; // Already seeded, skip!
            }

            // 2. Create the Master Stand for Kamiak Butte
            var kamiakStand = new stand_data
            {
                Date = DateTime.Now,
                Acres = 159.27f,
                HabitatType = "Kamiak Butte"
            };

            // Insert and capture the new ID so the plots can link to it
            int newStandId = await AddStandDataAsync(kamiakStand);

            // 3. The extracted GeoJSON coordinates formatted as { Latitude, Longitude }
            double[,] kamiakCoordinates = new double[,]
            {
                { 46.866053, -117.169526 }, { 46.865504, -117.169524 }, { 46.864955, -117.169522 }, // Plots 28-30
                { 46.864407, -117.169521 }, { 46.863858, -117.169519 }, { 46.863310, -117.169517 }, // Plots 31-33
                { 46.862761, -117.169515 }, { 46.866054, -117.168726 }, { 46.865505, -117.168724 }, // Plots 34, 45, 46
                { 46.864957, -117.168723 }, { 46.864408, -117.168721 }, { 46.863859, -117.168719 }, // Plots 47-49
                { 46.863311, -117.168717 }, { 46.866055, -117.167926 }, { 46.865506, -117.167924 }, // Plots 50, 62, 63
                { 46.864958, -117.167923 }, { 46.864409, -117.167921 }, { 46.863861, -117.167919 }, // Plots 64-66
                { 46.863312, -117.167918 }, { 46.866056, -117.167126 }, { 46.865507, -117.167125 }, // Plots 67, 79, 80
                { 46.864959, -117.167123 }, { 46.864410, -117.167121 }, { 46.863862, -117.167120 }, // Plots 81-83
                { 46.863313, -117.167118 }, { 46.866057, -117.166326 }, { 46.865509, -117.166325 }, // Plots 84, 96, 97
                { 46.864960, -117.166323 }, { 46.864412, -117.166321 }, { 46.863863, -117.166320 }, // Plots 98-100
                { 46.863314, -117.166318 }, { 46.866058, -117.165527 }, { 46.865510, -117.165525 }, // Plots 101, 113, 114
                { 46.864961, -117.165523 }, { 46.864413, -117.165522 }, { 46.863864, -117.165520 }, // Plots 115-117
                { 46.866060, -117.164727 }, { 46.865511, -117.164725 }, { 46.864962, -117.164723 }, // Plots 130-132
                { 46.864414, -117.164722 }, { 46.863865, -117.164720 }, { 46.866061, -117.163927 }, // Plots 133, 134, 147
                { 46.865512, -117.163925 }, { 46.864964, -117.163924 }, { 46.864415, -117.163922 }, // Plots 148-150
                { 46.863866, -117.163920 }, { 46.866062, -117.163127 }, { 46.865513, -117.163125 }, // Plots 151, 164, 165
                { 46.864965, -117.163124 }, { 46.864416, -117.163122 }, { 46.866063, -117.162327 }, // Plots 166, 167, 181
                { 46.865514, -117.162326 }, { 46.864966, -117.162324 }, { 46.864417, -117.162322 }, // Plots 182-184
                { 46.866064, -117.161527 }, { 46.865516, -117.161526 }, { 46.864967, -117.161524 }, // Plots 198-200
                { 46.864418, -117.161523 }, { 46.867711, -117.160733 }, { 46.867162, -117.160731 }, // Plots 201, 212, 213
                { 46.866614, -117.160729 }, { 46.866065, -117.160728 }, { 46.865517, -117.160726 }, // Plots 214-216
                { 46.864968, -117.160724 }, { 46.864420, -117.160723 }, { 46.867712, -117.159933 }, // Plots 217, 218, 229
                { 46.867164, -117.159931 }, { 46.866615, -117.159929 }, { 46.866066, -117.159928 }, // Plots 230-232
                { 46.865518, -117.159926 }, { 46.864969, -117.159925 }, { 46.864421, -117.159923 }, // Plots 233-235
                { 46.870456, -117.159141 }, { 46.869907, -117.159139 }, { 46.869359, -117.159138 }, // Plots 241-243
                { 46.868810, -117.159136 }, { 46.868262, -117.159134 }, { 46.867713, -117.159133 }, // Plots 244-246
                { 46.867165, -117.159131 }, { 46.866616, -117.159130 }, { 46.866067, -117.159128 }, // Plots 247-249
                { 46.865519, -117.159126 }, { 46.864970, -117.159125 }, { 46.870457, -117.158341 }, // Plots 250, 251, 258
                { 46.869909, -117.158339 }, { 46.869360, -117.158338 }, { 46.868811, -117.158336 }, // Plots 259-261
                { 46.868263, -117.158335 }, { 46.867714, -117.158333 }, { 46.867166, -117.158331 }, // Plots 262-264
                { 46.866617, -117.158330 }, { 46.866069, -117.158328 }, { 46.865520, -117.158327 }, // Plots 265-267
                { 46.870458, -117.157541 }, { 46.869910, -117.157540 }, { 46.869361, -117.157538 }, // Plots 275-277
                { 46.868813, -117.157536 }, { 46.868264, -117.157535 }, { 46.867715, -117.157533 }, // Plots 278-280
                { 46.867167, -117.157531 }, { 46.866618, -117.157530 }, { 46.866070, -117.157528 }, // Plots 281-283
                { 46.865521, -117.157527 }, { 46.870459, -117.156741 }, { 46.869911, -117.156740 }, // Plots 284, 292, 293
                { 46.869362, -117.156738 }, { 46.868814, -117.156736 }, { 46.868265, -117.156735 }, // Plots 294-296
                { 46.867717, -117.156733 }, { 46.867168, -117.156732 }, { 46.866619, -117.156730 }, // Plots 297-299
                { 46.866071, -117.156728 }, { 46.870460, -117.155941 }, { 46.869912, -117.155940 }, // Plots 300, 309, 310
                { 46.869363, -117.155938 }, { 46.868815, -117.155937 }, { 46.868266, -117.155935 }, // Plots 311-313
                { 46.867718, -117.155933 }, { 46.867169, -117.155932 }, { 46.866620, -117.155930 }, // Plots 314-316
                { 46.871010, -117.155143 }, { 46.870462, -117.155141 }, { 46.869913, -117.155140 }, // Plots 325-327
                { 46.869364, -117.155138 }, { 46.868816, -117.155137 }, { 46.868267, -117.155135 }, // Plots 328-330
                { 46.867719, -117.155134 }, { 46.867170, -117.155132 }, { 46.871011, -117.154343 }, // Plots 331, 332, 342
                { 46.870463, -117.154342 }, { 46.869914, -117.154340 }, { 46.869365, -117.154338 }, // Plots 343-345
                { 46.868817, -117.154337 }, { 46.868268, -117.154335 }, { 46.867720, -117.154334 }, // Plots 346-348
                { 46.867171, -117.154332 }, { 46.871012, -117.153543 }, { 46.870464, -117.153542 }, // Plots 349, 359, 360
                { 46.869915, -117.153540 }, { 46.869367, -117.153538 }, { 46.868818, -117.153537 }, // Plots 361-363
                { 46.868269, -117.153535 }, { 46.867721, -117.153534 }, { 46.871013, -117.152743 }, // Plots 364, 365, 376
                { 46.870465, -117.152742 }, { 46.869916, -117.152740 }, { 46.869368, -117.152739 }, // Plots 377-379
                { 46.868819, -117.152737 }, { 46.868270, -117.152735 }, { 46.867722, -117.152734 }, // Plots 380-382
                { 46.871014, -117.151943 }, { 46.870466, -117.151942 }, { 46.869917, -117.151940 }, // Plots 393-395
                { 46.869369, -117.151939 }, { 46.868820, -117.151937 }, { 46.868272, -117.151936 }, // Plots 396-398
                { 46.871015, -117.151143 }, { 46.870467, -117.151142 }, { 46.869918, -117.151140 }, // Plots 410-412
                { 46.869370, -117.151139 }, { 46.868821, -117.151137 }, { 46.868273, -117.151136 }, // Plots 413-415
                { 46.871017, -117.150344 }, { 46.870468, -117.150342 }, { 46.869919, -117.150340 }, // Plots 427-429
                { 46.869371, -117.150339 }, { 46.868822, -117.150337 }, { 46.868274, -117.150336 }, // Plots 430-432
                { 46.871018, -117.149544 }, { 46.870469, -117.149542 }, { 46.869920, -117.149541 }, // Plots 444-446
                { 46.869372, -117.149539 }, { 46.868823, -117.149538 }, { 46.868275, -117.149536 }  // Plots 447-449
            };

            // 4. Generate the plots
            var plotsToInsert = new List<plot_data>();

            for (int i = 0; i < kamiakCoordinates.GetLength(0); i++)
            {
                plotsToInsert.Add(new plot_data
                {
                    ParentStandId = newStandId, // Links it to the stand we just made
                    Latitude = (float)kamiakCoordinates[i, 0],
                    Longitude = (float)kamiakCoordinates[i, 1],
                    Date = DateTime.Now,
                    size = 37.24 // Adjust this if you want a different default fixed-radius
                });
            }

            // 5. Save them all to the database
            foreach (var plot in plotsToInsert)
            {
                await AddPlotDataAsync(plot);
            }
        }





        public async Task<bool> SaveStandBatchAsync(Stand stand)
        {
            if (stand == null) return false;

            try
            {
                await _connection.RunInTransactionAsync(tran =>
                {
                    int standId = stand.StandId;

                    // 1. Locate existing Stand in SQLite or insert if new
                    if (standId > 0)
                    {
                        var existingStand = tran.Find<stand_data>(standId);
                        if (existingStand != null)
                        {
                            existingStand.Acres = (float)stand.Acres;
                            existingStand.Date = DateTime.Now;
                            tran.Update(existingStand);
                        }
                    }
                    else
                    {
                        var standRow = new stand_data
                        {
                            Date = DateTime.Now,
                            Acres = (float)stand.Acres,
                        };
                        tran.Insert(standRow);
                        standId = standRow.Id;
                    }

                    // 2. Iterate directly over the stand's ObservableCollection<Plot>
                    if (stand.PlotList != null)
                    {
                        foreach (var plot in stand.PlotList)
                        {
                            int plotId = plot.PlotNumber;

                            // Locate the seeded/existing plot row in SQLite
                            var existingPlot = tran.Find<plot_data>(plotId);
                            if (existingPlot != null)
                            {
                                existingPlot.Slope = plot.Slope;
                                existingPlot.Aspect = plot.Aspect;
                                existingPlot.Date = DateTime.Now;
                                tran.Update(existingPlot);
                            }
                            else
                            {
                                var plotRow = new plot_data
                                {
                                    ParentStandId = standId,
                                    Slope = plot.Slope,
                                    Aspect = plot.Aspect,
                                    Date = DateTime.Now
                                };
                                tran.Insert(plotRow);
                                plotId = plotRow.Id;
                            }

                            // 3. Save trees to THIS specific plot ID
                            if (plot.TreeList != null)
                            {
                                foreach (var tree in plot.TreeList)
                                {
                                    var treeRow = new tree_data
                                    {
                                        parentPlotId = plotId,
                                        Species = tree.Species,
                                        DiameterBreastHeight = (float)tree.Dbh,
                                        Height = tree.Height
                                    };
                                    tran.Insert(treeRow);

                                    // 4. Save defects to THIS tree
                                    if (tree.DefectList != null)
                                    {
                                        foreach (var defect in tree.DefectList)
                                        {
                                            var defectRow = new defect_data
                                            {
                                                parentTreeId = treeRow.Id,
                                                Description = defect.Description,
                                                bottomHeight = (float)defect.BaseAngle,
                                                TopAngle = (float)defect.TopAngle
                                            };
                                            tran.Insert(defectRow);
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }







    }
}


        
          
            