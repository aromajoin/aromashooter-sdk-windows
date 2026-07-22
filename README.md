[English](#en) / [日本語](README-JP.md)

# Aroma Shooter SDK (Windows)

**Version 3.0.0**

A Windows .NET SDK for connecting to and controlling Aroma Shooter devices via **USB** and/or **BLE**.

> **Important — the internal booster is required for scent to be emitted.** Enable it on every shoot: pass `internalBooster: true` (simple API) or `internalBoosterIntensity > 0` (intensity API). With the internal booster off, no scent comes out.

This SDK ships as a **single DLL**:

-   `AromaShooterWindowsSDK.dll` — contains both the USB controller and the BLE controller, plus the shared model types.

There is no Core/plugin split anymore, and no unified `AromaShooterController` facade. Your app picks the controller(s) it needs and talks to that controller's own API directly:

-   `AromaShooterControllerUSB.SharedInstance` — for USB / serial devices
-   `AromaShooterControllerBLE.SharedInstance` — for BLE devices

Both controllers expose the same shooting/stopping API shape (simple + intensity); only connection setup differs (USB scan is synchronous, BLE scan is asynchronous).

---

<a id="en"></a>

## English

### Table of Contents

1. [Prerequisites](#prerequisites)
2. [Package layout](#package-layout)
3. [Installation](#installation)
4. [Sample (SmokeTest)](#sample-smoketest)
5. [Usage](#usage)
    - [0. Setup / discovery](#0-setup--discovery)
    - [1. Simple shooting API](#1-simple-shooting-api)
    - [2. Intensity shooting API](#2-intensity-shooting-api)
    - [3. Disconnecting / reconnecting](#3-disconnecting--reconnecting)
6. [Troubleshooting](#troubleshooting)
7. [License](#license)

---

<a id="prerequisites"></a>

### Prerequisites

-   OS: **Windows 10 / 11**
-   Runtime: **.NET Framework 4.7.2**
-   USB:
    -   The device must be recognized as a serial/COM device (install the proper USB-serial driver if required)
-   BLE:
    -   A Bluetooth LE capable adapter and Windows BLE support

---

<a id="package-layout"></a>

### Package layout

Recommended minimal distribution:

```
AromaShooterWindowsSDK/
├─ lib/
│  └─ net472/
│     └─ AromaShooterWindowsSDK.dll
├─ samples/
│  └─ SmokeTest/
│     ├─ AromaShooterWindowsSDK.SmokeTest.csproj
│     └─ Program.cs
└─ README.md
```

**Important**: `AromaShooterWindowsSDK.dll` must be located next to your executable at runtime (or copied to the output folder). It is the only DLL you need to deploy — USB and BLE support both live inside it, so there is nothing to enable/disable per transport at deploy time.

---

<a id="installation"></a>

### Installation

#### .NET (Desktop app)

1. Reference `AromaShooterWindowsSDK.dll`.
2. Use whichever controller(s) your app needs:
    - `AromaShooterControllerUSB.SharedInstance` for USB
    - `AromaShooterControllerBLE.SharedInstance` for BLE

There are no additional DLLs to copy next to your `.exe`.

#### Unity (Windows)

-   Place `AromaShooterWindowsSDK.dll` into:
    -   `Assets/Plugins/` (or `Assets/Plugins/x86_64/` depending on your setup)
-   Use **.NET 4.x** scripting runtime / API compatibility level as needed by your Unity version.

---

<a id="sample-smoketest"></a>

### Sample (SmokeTest)

A minimal console sample is provided under `samples/SmokeTest/`.

Build and run:

-   Open the sample `.csproj` in Visual Studio and run it, or
-   Build from command line using MSBuild.

The sample will:

1. Connect via `AromaShooterControllerUSB.SharedInstance.ScanAndConnect()` and/or `await AromaShooterControllerBLE.SharedInstance.ScanAndConnect()`
2. Print discovered devices
3. Run a simple shoot + stop sequence
4. Run an intensity shoot + stop sequence

---

<a id="usage"></a>

## Usage

There is no unified `AromaShooterController` facade — get the controller for the transport you want and call its own methods.

<a id="0-setup--discovery"></a>

### 0. Setup / discovery

USB (synchronous scan):

```csharp
using AromaShooterWindowsSDK;

var usb = AromaShooterControllerUSB.SharedInstance;
usb.ScanAndConnect();

var usbDevices = usb.GetConnectedDevices(); // List<string> of connected serials
```

BLE (asynchronous scan — must be awaited):

```csharp
using AromaShooterWindowsSDK;

var ble = AromaShooterControllerBLE.SharedInstance;
var found = await ble.ScanAndConnect(); // List<string> of devices found during this scan

var bleDevices = ble.GetConnectedDevices(); // List<string> of connected device names
```

Each controller's `GetConnectedDevices()` returns a plain `List<string>` of device serials/names — there is no `{Transport, Identifier, DisplayName}` object. Since you already chose the controller, you know the transport; if you need to correlate a serial across both transports, track that mapping yourself.

---

<a id="1-simple-shooting-api"></a>

### 1. Simple shooting API

Available on both `AromaShooterControllerUSB.SharedInstance` and `AromaShooterControllerBLE.SharedInstance` with identical method names (examples below use `usb`; swap in `ble` for BLE devices):

Shoot all connected devices:

```csharp
usb.ShootAllSimple(3000, new int[] { 1, 2, 5 }, internalBooster: true);
```

Shoot a specific device:

```csharp
usb.ShootSimple(3000, new int[] { 1, 2, 5 }, true, "ASN3A01192");
```

Stop all:

```csharp
usb.StopAllSimple();
```

Stop a specific device:

```csharp
usb.StopSimple("ASN3A01192");
```

---

<a id="2-intensity-shooting-api"></a>

### 2. Intensity shooting API

`AromaChamber`:

```csharp
public class AromaChamber
{
    public int number;        // 1..6
    public int concentration; // 0..100
}
```

Shoot all with intensity control:

```csharp
var chambers = new List<AromaChamber>
{
    new AromaChamber { number = 1, concentration = 50 },
    new AromaChamber { number = 2, concentration = 80 },
};

usb.ShootAllWithIntensity(
    3000,
    chambers,
    internalBoosterIntensity: 100,
    externalBoosterIntensity: 0
);
```

Shoot a specific device with intensity control:

```csharp
usb.ShootWithIntensity(
    3000,
    chambers,
    internalBoosterIntensity: 100,
    externalBoosterIntensity: 0,
    deviceSerial: "ASN3A01192"
);
```

Stop with intensity (per chamber):

```csharp
usb.StopAllWithIntensity(
    chambers: new[] { 1, 2 },
    stopInternalBooster: true,
    stopExternalBooster: false
);
```

Stop a specific device with intensity (per chamber):

```csharp
usb.StopWithIntensity(
    "ASN3A01192",
    chambers: new[] { 1, 2 },
    stopInternalBooster: true,
    stopExternalBooster: false
);
```

`AromaShooterControllerBLE.SharedInstance` exposes the identical intensity API (`ShootAllWithIntensity`, `ShootWithIntensity`, `StopAllWithIntensity`, `StopWithIntensity`) — BLE devices support intensity control just like USB devices.

---

<a id="3-disconnecting--reconnecting"></a>

### 3. Disconnecting / reconnecting

Use these to release a device so another application/project can take it over — e.g. switching between two apps that share the same AromaShooter **without physically unplugging the USB** (a USB/COM port can only be held by one application at a time).

USB:

```csharp
usb.DisconnectAll();               // stop output, then close & release every serial port
usb.Disconnect("ASN3A01192");      // disconnect a single device by name

usb.Reconnect();                   // fast switch: re-open the port(s) used before the last
                                   // DisconnectAll(); falls back to ScanAndConnect()

bool ok = usb.Connect("COM5");     // connect to a specific COM port without scanning every
                                   // port; retries briefly if the port is still busy
```

BLE:

```csharp
ble.DisconnectAll();               // release GATT services for all connected devices
ble.Disconnect("ASN3A01192");      // release a single device
await ble.Reconnect();             // re-discover & reconnect (equivalent to ScanAndConnect)
```

> **`Disconnect(name)` (USB and BLE).** `name` is the device serial from `GetConnectedDevices()` (e.g. `"ASN3A01192"`). Only that device is released; other connected devices stay connected. Exact match (case-sensitive); an unknown name is ignored silently.

> **Switching between two apps (USB).** The remembered port list is per process, so each app reconnects independently. Ensure the app that is leaving calls `DisconnectAll()` **before** the next one calls `Reconnect()`/`Connect()`, otherwise the port is still busy. `Connect()`/`Reconnect()` retry briefly to absorb a small timing gap.

---

<a id="troubleshooting"></a>

## Troubleshooting

-   No devices found:
    -   USB: call `usb.ScanAndConnect()` first, then check COM port / driver installation
    -   BLE: call `await ble.ScanAndConnect()` first; ensure the device is paired/available and Bluetooth LE is enabled
-   Calling the wrong controller:
    -   USB and BLE devices are handled by two separate controllers (`AromaShooterControllerUSB`, `AromaShooterControllerBLE`); make sure you're using the one that matches your device's connection
    -   Re-run `ScanAndConnect()` and confirm `GetConnectedDevices()` returns devices

---

<a id="license"></a>

## License

See [LICENSE](LICENSE).
