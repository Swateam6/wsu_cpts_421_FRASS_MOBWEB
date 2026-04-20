-- Drop tables in reverse order of inheritance to avoid constraint conflicts
DROP TABLE IF EXISTS `tree_data`;
DROP TABLE IF EXISTS `plot_data`;
DROP TABLE IF EXISTS `stand_data`;
DROP TABLE IF EXISTS `parcel_data`;
DROP TABLE IF EXISTS `user_data`;

-- 1. USER TABLE (The Top Level)
CREATE TABLE `user_data` (
    `id` INTEGER PRIMARY KEY AUTOINCREMENT
    -- Add Cruiser Name, Email, etc. here later
);

-- 2. PARCEL TABLE (Child of User)
CREATE TABLE `parcel_data` (
    `id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `parent_user_id` INTEGER NOT NULL,
    `date_last_entry` DATETIME,
    `parcel_acres` REAL NOT NULL,
    
    -- This enforces the relationship back to the User
    FOREIGN KEY(`parent_user_id`) REFERENCES `user_data`(`id`) ON DELETE CASCADE
);

-- 3. STAND TABLE (Child of Parcel)
CREATE TABLE `stand_data` (
    `id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `parent_parcel_id` INTEGER NOT NULL,
    `date_last_entry` DATETIME,
    `fvs_variant` TEXT,
    `site_index` TEXT,
    `habitat_type` TEXT,
    `acres` REAL NOT NULL,
    `stand_latitude` REAL NOT NULL,
    `stand_longitude` REAL NOT NULL,
    `stand_aspect(degrees)` REAL NOT NULL,
    `stand_slope(degrees)` REAL NOT NULL,
    `stand_elevation(ft)` REAL NOT NULL,
    `IsFixedPlot` INTEGER NOT NULL, -- SQLite stores booleans as 0 (false) or 1 (true)
    
    FOREIGN KEY(`parent_parcel_id`) REFERENCES `parcel_data`(`id`) ON DELETE CASCADE
);

-- 4. PLOT TABLE (Child of Stand)
CREATE TABLE `plot_data` (
    `id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `parent_stand_id` INTEGER NOT NULL,
    `date_last_entry` DATETIME,
    `plot_latitude` REAL NOT NULL,
    `plot_longitude` REAL NOT NULL,
    `plot_aspect(degrees)` INTEGER NOT NULL,
    `plot_slope(degrees)` INTEGER NOT NULL,
    `plot_elevation(ft)` INTEGER NOT NULL,
    `plot_image_filepath` TEXT,
    `most_mesic_tree_species` TEXT,
    `most_mesic_bush_species` TEXT,
    `size` REAL NOT NULL, -- Stores your BAF or Radius
    
    FOREIGN KEY(`parent_stand_id`) REFERENCES `stand_data`(`id`) ON DELETE CASCADE
);

-- 5. TREE TABLE (Child of Plot)
CREATE TABLE `tree_data` (
    `id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `parent_plot_id` INTEGER NOT NULL,
    `date_last_entry` DATETIME,
    `tree_height(ft)` INTEGER NOT NULL,
    `tree_species` TEXT,
    `diameter_breast_height(in)` REAL NOT NULL,
    `stump_height(in)` REAL NOT NULL,
    `base_of_live_crown(ft)` REAL NOT NULL,
    `crown_ratio(%)` REAL NOT NULL,
    `latitude` REAL NOT NULL,
    `longitude` REAL NOT NULL,
    
    FOREIGN KEY(`parent_plot_id`) REFERENCES `plot_data`(`id`) ON DELETE CASCADE
);

-- --------------------------------------------------------
-- HIGH-PERFORMANCE INDEXES
-- These match your [Indexed] attributes in C# and make queries lightning fast
-- --------------------------------------------------------
CREATE INDEX `idx_);