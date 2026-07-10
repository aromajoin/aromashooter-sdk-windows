Change Log
===
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