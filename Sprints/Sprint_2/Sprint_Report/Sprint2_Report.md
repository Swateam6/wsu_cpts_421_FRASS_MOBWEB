# Sprint 2 Report (Feb. 20 – Mar. 20, 2026)

## YouTube link of Sprint 2 Video

<https://youtu.be/X1jUYrKJf18>

## What's New (User Facing)

* Data Entry Feature with Front to Backend implementatiom
* Gyroscope and Forestry Math Features

## Work Summary (Developer Facing)

This sprint, our primary focus was establishing a robust, highly decoupled architecture for the Data Entry and Forestry Math subsystems. Knowing that field applications require maximum UI responsiveness, we proactively designed the system around a strict MVVM pattern from the start to enforce a clear separation of concerns. We successfully implemented a centralized DataService to act as the active memory state manager for our hierarchical class structures (Stands, Plots, and Trees), and intentionally isolated our gyroscope-driven trigonometry algorithms into a dedicated math subsystem. By ensuring the heavy sensor data processing was completely separated from the view layer from day one, we successfully guaranteed a fluid, latency-free user experience during rapid data collection.

## Unfinished Work

Our mesic species, authentication, and navigation subsystems have not arrived at a 'ground level' implementation. We will be dividing work more broadly between ourselves for the coming sprints. Regardless, we are still confident in our current progress and view our project to be at a good pace.

## Completed Issues/User Stories

Here are links to the issues that we completed in this sprint:

* Prototype Tree Level Data Entry (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/22>)
* Create Gyro Demo  (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/32>)
* Prototype Gyroscope (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/24>)
* Test Gyroscope & Verify Height Calculation (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/25>)
* Refine Gyroscope Subsystem (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/27>)
* Create App Pages (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/16>)
* Research IOS Costs (<https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/30>)

## Incomplete Issues/User Stories

 Here are links to issues we worked on but did not complete in this sprint:

* Prototype Voice Recognition <https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/21>
  * The out-of-the-box models from the CommunityToolbox library performed very poorly. We decided to rework it to use VOSK to be much more deeply configureable albeit more complex to implement.

* Figma UI Prototypes <https://github.com/Swateam6/wsu_cpts_421_FRASS_MOBWEB/issues/23>

  * We have not reached this issue yet, mostly because we have been working on more fundimental subsystems, whereas UI can be done last.

## Code Files for Review

Please review the following code files, which were actively developed during this sprint, for quality:

* DataService.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Services/DataService.cs>)
* Defects.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Models/Defects.cs>)
* Plot.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Models/Plot.cs>)
* Stand.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Models/Stand.cs>)
* Tree.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Models/Tree.cs>)
* PlotEntry.xaml.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Screens/DataEntrySubsystems/PlotEntry.xaml.cs>)
* StandEntryData.xaml.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Screens/DataEntrySubsystems/StandEntryData.xaml.cs>)
* TreeEntryPage.xaml.cs (<https://github.com/Swateam6/FRASS-MOBWEB/blob/feature/tree-data-entry/Screens/DataEntrySubsystems/TreeEntryPage.xaml.cs>)

## Retrospective Summary

Here's what went well:

* Gyroscope implentation and demoing
* Weekly meetings with the client, presenting current progress

Here's what we'd like to improve:

* Bredth of subsystems being worked on
* Regular pull-requests / integrating each other's work
  
Here are changes we plan to implement in the next sprint:

* **SQLite Implementation:**
Complete Data Entry to link with Gyroscope, add more data fields that are not part of the metadata.
* **Mesic Seeking Interface**

* **Data Entry / Gyroscope Subsystem integration**
