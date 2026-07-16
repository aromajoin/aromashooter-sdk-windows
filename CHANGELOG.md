Change Log
===
Version 3.0.0 *(2026-07-16)*
----------------------------
* BREAKING: rename SDK assembly & namespace `ASControllerSDK` -> `AromaShooterWindowsSDK`
  * DLL `ASControllerSDK.dll` -> `AromaShooterWindowsSDK.dll`; update `using ASControllerSDK;` -> `using AromaShooterWindowsSDK;`
  * Aligns Windows with the `AromaShooter{Platform}SDK` naming used across the SDK family; class names unchanged
* BREAKING: rename the device-identifier parameter `shooterName` -> `deviceSerial` on `ShootSimple`/`StopSimple`/`ShootWithIntensity`/`StopWithIntensity`/`Disconnect` (USB and BLE). The value is the device serial (e.g. `ASN3A01192`). Positional calls are unaffected; only named-argument callers need updating. `GetConnectedDevices()` and `Connect(portName)` are unchanged.
* Add connection management on both controllers
  * USB: `DisconnectAll()`, `Disconnect(string deviceSerial)`, `Connect(string portName)`, `Reconnect()`
    * `Connect` opens a specific COM port without scanning every port and retries briefly if the port is momentarily busy
    * `Reconnect` re-opens the port(s) used before the last `DisconnectAll()` (fast switch), falling back to `ScanAndConnect()`
  * BLE: `DisconnectAll()`, `Disconnect(string deviceSerial)` (release GATT services), `Reconnect()` (re-scan)
* Use case: releasing the AromaShooter so another application/project can take it over without physically unplugging the USB
* Fix (USB): `ScanAndConnect()` now closes previously opened ports before re-scanning (no leaked handles)
* rebuild `AromaShooterWindowsSDK.dll`


Version 2.0.0 *(2026-07-10)*
----------------------------
* BREAKING: modernized API — Diffuse -> ShootSimple/ShootWithIntensity, Stop -> StopSimple/StopAllWithIntensity/StopWithIntensity
* BREAKING: AromaPort -> AromaChamber; field intensity -> concentration
* booster/fan -> internal/external booster terminology
* Single DLL with two public controllers (AromaShooterControllerUSB + AromaShooterControllerBLE); removed the unified AromaShooterController facade
* BLE promoted to public and gained the intensity API (ShootWithIntensity / StopWithIntensity), ported from the OBO9 BLE frame
* Unified device discovery to GetConnectedDevices() on both controllers
* Distribution ships a single ASControllerSDK.dll (dropped the USB/BLE plugin DLLs)
* AromaChamber moved to its own file; project retargeted to .NET Framework 4.7.2


Version 1.2.0 *(2025-12-)*
----------------------------
* Fix bugs


Version 1.1.1 *(2019-01-31)*
----------------------------
* Fix bugs

Version 1.1.0 *(2018-07-18)*
----------------------------
* Fix bugs
* Add 'stop' function

Version 1.0.1 *(2017-10-06)*
----------------------------
* Unset the limitation of aroma diffusing duration

Version 1.0.0 *(2017-02-15)*
----------------------------
* Initial release