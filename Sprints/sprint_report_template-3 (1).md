# Sprint x Report (March 25,2026 - May 1 2026)

## YouTube link of Sprint * Video (Make this video unlisted)

## What's New (User Facing)
Plot coordinate selection: Users can now manually select and refine specific points on the interface for more accurate data mapping.

Geolocation: Integrated real-time location services to automatically tag entries with geographic data.

Data to SQLite: Implemented local database persistence, ensuring all user data is saved locally and remains available across sessions.

Navigation Extra:Showing current distance to plot using geolocation

GyroScope into data field integration
## Work Summary (Developer Facing)
This sprint, we focused on moving past our SQLite mock data and finally getting the real data entry integration hooked up. It was a big step to shift from static test cases to a dynamic system where user inputs actually drive the database. We also implemented GeoLocation that showed current location and added a feature that shows both direction and distance to the current plot center. We also spent time ensuring the plot coordinate selection was fully validated before saving anything to the local DB to avoid corrupting our new tables with empty data.

## Unfinished Work
We didn't quite wrap up the Mesic subsystem integration this sprint. While we made a start on the core logic, hooking it into the main application proved more time-consuming than we initially planned. We also hit some snags with the UI screen flow; because we added several new screens, we realized we had overlooked some of the navigation transitions and logic needed to tie them all together.
To keep things organized, we have:

(a) Tracked our progress by updating the acceptance criteria checkboxes in each issue.

(b) Added comments to the unfinished stories explaining that we simply ran out of time or hit unexpected complexity or communicated with each other about issues we run into.

(c) Moved both the subsystem integration and the flow fixes into the next sprint for immediate attention.

## Completed Issues/User Stories
https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/12
https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/42
https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/43
https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/38
https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/46
 
 ## Incomplete Issues/User Stories
 Here are links to issues we worked on but did not complete in this sprint:
 
https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/47<<time constrainsts>>
 

## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
 * [ForestyMath.cs](https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Services/ForestyMath.cs)

  [DataService.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Services/DataService.cs

  [DefectScren.xaml.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/DataEntrySubsystems/GyroscopeSubsystem/DefectScreen.xaml.cs

  [PlotSlopeScreen.xaml.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/DataEntrySubsystems/GyroscopeSubsystem/PlotSlopeScreen.xaml.cs

  [PlotCoordinateSet.xaml.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/PlotCoordinateSet.xaml.cs

  [LocationDemo.xaml.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/LocationDemo.xaml.cs

  [DataBaseTestScreen.xaml.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/DatabaseTestScreen.xaml.cs

  [DeviceLocation.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Location/DeviceLocation.cs

  [LocationDisplayBacked.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Location/DeviceLocation.cs

  [LocationService.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Location/LocationService.cs

  [DeviceLocationMessage.cs ]https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Location/DeviceLocationMessage.cs

  [DefectGyroScopeController.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/DataEntrySubsystems/GyroscopeSubsystem/DefectGyroscopeController.cs

  [GyroscopeController.cs] https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/plot-navigation-with-geoLoc/Screens/DataEntrySubsystems/GyroscopeSubsystem/GyroscopeController.cs
 
## Retrospective Summary
Here's what went well:
We successfully tracked down and squashed the ghost data creation bug that was cluttering our tables; now, the database only saves intentional entries from the UI.

The compass accuracy is much more reliable after some recalibration, which has made the geolocation and mapping features feel significantly more precise.

Integrating the plot coordinate selection with the live database worked out great, allowing for real-time updates without any noticeable lag.


Here's what we'd like to improve:
To avoid version confusion, we will clearly identify the active development branch so everyone knows exactly where the most recent code lives.

We are implementing a "test-first" rule where every team member must run a local test build before committing to prevent broken code from hitting the repo.



Here are changes we plan to implement in the next sprint:
We plan to refactor the codebase to strip out redundant logic and "extra" code that was added during the push for new features, making the project much cleaner and easier to maintain.

We will focus heavily on UI usability to fix the navigation issues due to new screens being added and make sure screen flow is up to par with our plans.

Finalizing the Mesic subsystem integration is a top priority so that the backend logic is fully connected to the main application interface.

We intend to map out a clear screen flow diagram before adding any more UI elements to avoid the navigation oversights we encountered this time.

Not uploading only after finished stand to SQLite
