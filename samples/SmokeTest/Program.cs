using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AromaShooterWindowsSDK;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("=== AromaShooterWindowsSDK SmokeTest ===");

        // ---- USB ----
        var usb = AromaShooterControllerUSB.SharedInstance;
        usb.ScanAndConnect();
        List<string> usbDevices = usb.GetConnectedDevices();
        Console.WriteLine($"[USB] connected: {usbDevices.Count}");
        foreach (var name in usbDevices) Console.WriteLine($"  - {name}");
        if (usbDevices.Count > 0)
        {
            usb.ShootAllSimple(3000, new int[] { 1, 2 }, true);
            await Task.Delay(2500);
            usb.StopAllSimple();
            var chambers = new List<AromaChamber> { new AromaChamber { number = 1, concentration = 100 } };
            usb.ShootAllWithIntensity(3000, chambers, 100, 0);
            await Task.Delay(2500);
            usb.StopAllWithIntensity(new int[] { 1 }, true, false);
        }

        // ---- BLE ----
        var ble = AromaShooterControllerBLE.SharedInstance;
        await ble.ScanAndConnect();
        List<string> bleDevices = ble.GetConnectedDevices();
        Console.WriteLine($"[BLE] connected: {bleDevices.Count}");
        foreach (var name in bleDevices) Console.WriteLine($"  - {name}");
        if (bleDevices.Count > 0)
        {
            ble.ShootAllSimple(3000, new int[] { 1, 2 }, true);
            await Task.Delay(2500);
            ble.StopAllSimple();
            var chambers = new List<AromaChamber> { new AromaChamber { number = 1, concentration = 100 } };
            ble.ShootAllWithIntensity(3000, chambers, 100, 0);
            await Task.Delay(2500);
            ble.StopAllWithIntensity(new int[] { 1 }, true, false);
        }

        Console.WriteLine("Done. Press any key to exit...");
        Console.ReadKey();
    }
}
