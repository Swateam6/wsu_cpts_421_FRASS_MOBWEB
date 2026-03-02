Test Branch for the FRASS Forest Cruiser Mobile Application

## Building
#### Targeting iOS simulator:
```bash
xcrun simctl shutdown all
xcrun simctl erase all
dotnet build \
    -t:Run \
    -f net9.0-ios \
    -p:RuntimeIdentifier=iossimulator-arm64
```