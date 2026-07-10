[English](#en) / [日本語](README-JP.md)

# ASControllerSDK (Windows)

A Windows .NET SDK for connecting to and controlling Aroma Shooter devices via **USB** and/or **BLE**.

This SDK is split into:

-   **Core**: `ASControllerSDK.dll` (public API + plugin loader)
-   **Plugins** (optional):
    -   `ASControllerSDK.USB.dll`
    -   `ASControllerSDK.BLE.dll`

If a plugin DLL is missing, the SDK still works, but that transport is simply unavailable.

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
ASControllerSDK-Windows/
├─ lib/
│  └─ net472/
│     ├─ ASControllerSDK.dll
│     ├─ ASControllerSDK.USB.dll        (optional)
│     └─ ASControllerSDK.BLE.dll        (optional)
├─ samples/
│  └─ SmokeTest/
│     ├─ ASControllerSDK.SmokeTest.csproj
│     └─ Program.cs
└─ README.md
```

**Important**: Plugin DLLs must be located next to your executable at runtime
(or copied to the output folder), so the plugin loader can discover them.

---

<a id="installation"></a>

### Installation

#### .NET (Desktop app)

1. Reference the Core DLL: `ASControllerSDK.dll`
2. Copy plugin DLLs next to your `.exe`:
    - `ASControllerSDK.USB.dll` (USB support)
    - `ASControllerSDK.BLE.dll` (BLE support)

#### Unity (Windows)

-   Place all required DLLs into:
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

1. Call `Setup()` to initialize available plugins
2. Print discovered devices
3. Run a simple shoot + stop sequence
4. Run an intensity shoot + stop sequence

---

<a id="usage"></a>

## Usage

<a id="0-setup--discovery"></a>

### 0. Setup / discovery

```csharp
using ASControllerSDK;

var controller = AromaShooterController.SharedInstance;
await controller.Setup();

var devices = controller.GetConnectedDevices();
```

Each device returns:

-   `Transport`: USB or BLE
-   `Identifier`: canonical id (typically serial)
-   `DisplayName`: raw OS/device name

---

<a id="1-simple-shooting-api"></a>

### 1. Simple shooting API

Shoot all connected devices:

```csharp
controller.ShootSimple(3000, new int[] { 1, 2, 5 }, internalBooster: true);
```

Shoot a specific device:

```csharp
controller.ShootSimple(3000, new int[] { 1, 2, 5 }, true, "ASN3A01192");
```

Stop all:

```csharp
controller.StopSimple();
```

Stop a specific device:

```csharp
controller.StopSimple("ASN3A01192");
```

---

<a id="2-intensity-shooting-api"></a>

### 2. Intensity shooting API

`AromaChamber`:

```csharp
public sealed class AromaChamber
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

controller.ShootWithIntensity(
    3000,
    chambers,
    internalBoosterIntensity: 100,
    externalBoosterIntensity: 0
);
```

Stop with intensity (per port):

```csharp
controller.StopAllWithIntensity(
    chambers: new[] { 1, 2 },
    stopInternalBooster: true,
    stopExternalBooster: false
);
```

---

<a id="troubleshooting"></a>

## Troubleshooting

-   No devices found:
    -   Call `await controller.Setup()` first
    -   USB: check COM/driver installation
    -   BLE: ensure the device is paired/available and Bluetooth LE is enabled
-   USB/BLE features missing:
    -   Make sure `ASControllerSDK.USB.dll` / `ASControllerSDK.BLE.dll` are located next to your executable
    -   Re-run `Setup()` and confirm `GetConnectedDevices()` returns devices

---

<a id="license"></a>

## License

See [LICENSE.](LICENSE.md)
