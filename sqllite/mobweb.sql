DROP TABLE IF EXISTS `user_data`;
DROP TABLE IF EXISTS `parcel_data`;
DROP TABLE IF EXISTS `stand_data`;
DROP TABLE IF EXISTS `plot_data`;
DROP TABLE IF EXISTS `tree_data`;

CREATE TABLE `user_data` (
    `id` INTEGER PRIMARY KEY,
    `parcel_list` TEXT NOT NULL
);

CREATE TABLE `parcel_data` (
    `id` INTEGER PRIMARY KEY,
    `date_last_entry` TEXT NOT NULL,
    `parcel_acres` REAL NOT NULL,
    `stand_list` TEXT NOT NULL
);

CREATE TABLE `stand_data` (
    `id` INTEGER PRIMARY KEY,
    `date_last_entry` TEXT NOT NULL,
    `fvs_variant` TEXT NOT NULL,
    `site_index` TEXT NOT NULL,
    `habitat_type` TEXT NOT NULL,
    `acres` REAL NOT NULL,
    `stand_latitude` REAL NOT NULL,
    `stand_longitude` REAL NOT NULL,
    `stand_aspect(degrees)` REAL NOT NULL,
    `stand_slope(degrees)` REAL NOT NULL,
    `stand_elevation(ft)` REAL NOT NULL,
    `plot_list` TEXT NOT NULL
);

CREATE TABLE `plot_data` (
    `id` INTEGER PRIMARY KEY,
    `date_last_entry` TEXT NOT NULL,
    `plot_latitude` REAL NOT NULL,
    `plot_longitude` REAL NOT NULL,
    `plot_aspect(degrees)` INTEGER NOT NULL,
    `plot_slope(degrees)` INTEGER NOT NULL,
    `plot_elevation(ft)` INTEGER NOT NULL,
    `plot_image_filepath` TEXT NOT NULL,
    `most_mesic_tree_species` TEXT NOT NULL,
    `most_mesic_bush_species` TEXT NOT NULL,
    `tree_list` TEXT NOT NULL
);

CREATE TABLE `tree_data` (
    `id` INTEGER PRIMARY KEY,
    `date_last_entry` TEXT NOT NULL,
    `tree_height(ft)` INTEGER NOT NULL,
    `tree_species` TEXT NOT NULL,
    `diameter_breast_height(in)` REAL NOT NULL,
    `stump_height(in)` REAL NOT NULL,
    `base_of_live_crown(ft)` REAL NOT NULL,
    `crown_ratio(%)` REAL NOT NULL,
    `defect_description` TEXT NOT NULL,
    `defect_base(ft)` REAL NOT NULL,
    `defect_top(ft)` REAL NOT NULL
);