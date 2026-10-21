Change Log
===
Version 4.1.0 *(2026-10-21)*
----------------------------
* BLE: no Windows pairing needed any more. The controller listens to the Aroma Shooters' advertisements and connects directly
  * `Discover(durationMillisec = 3000)` returns the devices heard as `List<AromaBleDevice>` (`Serial`, `Address`, `AddressText`, `Rssi`) without connecting; `GetDiscoveredDevices()` returns the last result
  * `Connect(AromaBleDevice)` / `Connect(string deviceSerial)` connect to the device the user picked (`Task<bool>`)
  * `ScanAndConnect()` / `ScanAndConnect(int durationMillisec)` discover and connect to every device heard; devices already connected stay connected (a connected Aroma Shooter stops advertising); a device heard again while its link is down is reconnected. The returned list is now every connected serial, not only the ones added by this call
  * after `Disconnect` / `DisconnectAll`, Windows needs a few seconds to drop the link before the device advertises again; wait before `Discover` / `Reconnect`
  * the link is held open (`GattSession.MaintainConnection`) so writes do not pay for a reconnect
* BLE fixes
  * `StopAllWithIntensity` / `StopWithIntensity` now really stop the listed chambers and boosters (they sent NO CHANGE instead of OFF); the intensity frame uses the same OFF / ON / NO CHANGE states as USB
  * `DisconnectAll` / `Disconnect` send stop frames before releasing the link (a dropped link alone does not stop a running shoot)
  * shoot/stop calls on an unknown serial or a dropped link no longer throw inside the async call
* `Windows.winmd` updated to the Windows 10 SDK 10.0.20348 metadata (needed for the GATT session API); .NET Framework 4.7.2 unchanged
* rebuild `AromaShooterWindowsSDK.dll`


Version 4.0.0 *(2026-10-21)*
----------------------------
* BREAKING: booster parameters renamed internal/external -> inner/outer on both controllers (USB + BLE)
  * `internalBooster` -> `innerBooster`, `internalBoosterIntensity` -> `innerBoosterIntensity`, `externalBoosterIntensity` -> `outerBoosterIntensity`, `stopInternalBooster` -> `stopInnerBooster`, `stopExternalBooster` -> `stopOuterBooster`
  * Identifiers only — no wire-protocol or behavioral change. Positional calls are unaffected; only named-argument callers need updating. No deprecated aliases.
  * Same rename ships across every AromaShooter SDK and firmware 2.2.2 (Wi-Fi JSON keys)
* rebuild `AromaShooterWindowsSDK.dll`


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