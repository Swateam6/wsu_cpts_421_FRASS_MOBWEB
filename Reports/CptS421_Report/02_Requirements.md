# Requirements

## 2. Non-Functional Requirements

The following table defines the non-functional requirements (NFRs) for the FRASS MOBWEB Forest Cruiser application. Each requirement includes a measurable acceptance criterion.

| ID     | Category            | Requirement Description                                                               | Measurable Acceptance Criterion                                     | Priority |
|--------|---------------------|---------------------------------------------------------------------------------------|---------------------------------------------------------------------|----------|
| NFR-01 | Performance         | The app shall respond to user input without noticeable delay.                         | UI response latency ≤ 0.5 seconds for all standard interactions.    | High     |
| NFR-02 | Performance         | Data upload to the FRASS server shall complete in a reasonable time.                  | A full plot dataset (≤ 1 MB) uploads in ≤ 10 seconds on LTE.        | High     |
| NFR-03 | Accuracy            | GPS-based plot navigation shall locate plot center within acceptable field tolerances. | Plot center located within ±3 meters of true position.              | High     |
| NFR-04 | Accuracy            | Tree height measurements via gyroscope shall be sufficiently precise for forestry.    | Height measurement error ≤ 5% of actual height.                     | High     |
| NFR-05 | Accuracy            | Voice recognition shall correctly transcribe spoken numeric data entries.             | Voice transcription accuracy ≥ 90% for numeric field entries.       | Medium   |
| NFR-06 | Reliability         | The app shall remain stable during a full field session.                              | App crash rate ≤ 1 crash per 100 field sessions.                    | High     |
| NFR-07 | Reliability         | Data transfers shall handle network interruptions gracefully.                         | No data loss during upload if connectivity drops and reconnects within 60 seconds. | High |
| NFR-08 | Portability         | The app shall run on both Android and iOS without platform-specific bugs.             | All core features pass functional tests on Android 12+ and iOS 16+. | High     |
| NFR-09 | Security            | All data transmitted between the app and the server shall be encrypted.               | All network communications use TLS 1.3 or higher.                  | High     |
| NFR-10 | Offline Capability  | The app shall allow full data collection without an active internet connection.        | 100% of data-entry features remain available when offline.          | High     |
| NFR-11 | Usability           | Field users shall be able to complete core tasks without prior training.              | New users complete a standard plot entry in ≤ 5 minutes on first use. | Medium  |
| NFR-12 | Battery Efficiency  | The app shall minimize power consumption during continuous field use.                 | Battery draw ≤ 10% per hour of active use on a modern smartphone.  | Medium   |
| NFR-13 | Storage             | Locally cached data shall not exceed device storage limits for typical field work.    | Local cache size ≤ 50 MB for up to 100 recorded plots.             | Medium   |
| NFR-14 | Maintainability     | The codebase shall be structured to enable updates or bug fixes to be deployed quickly. | A critical bug fix can be built, tested, and deployed within 2 business days. | Low |
