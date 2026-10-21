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
        // No Windows pairing needed: listen for advertisements, show the list, connect to the chosen device.
        var ble = AromaShooterControllerBLE.SharedInstance;
        List<AromaBleDevice> found = await ble.Discover(3000);
        Console.WriteLine($"[BLE] advertising: {found.Count}");
        for (int i = 0; i < found.Count; i++) Console.WriteLine($"  {i + 1}. {found[i]}");
        if (found.Count > 0)
        {
            int pick = 0;
            if (found.Count > 1 && !Console.IsInputRedirected)
            {
                Console.Write($"Connect to which device (1-{found.Count}, Enter = all)? ");
                string line = Console.ReadLine();
                int n;
                if (int.TryParse(line, out n) && n >= 1 && n <= found.Count) pick = n;
            }
            if (pick > 0)
            {
                bool ok = await ble.Connect(found[pick - 1]);
                Console.WriteLine($"[BLE] connect {found[pick - 1].Serial}: {(ok ? "ok" : "failed")}");
            }
            else
            {
                foreach (var device in found)
                {
                    bool ok = await ble.Connect(device);
                    Console.WriteLine($"[BLE] connect {device.Serial}: {(ok ? "ok" : "failed")}");
                }
            }
        }
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
            ble.DisconnectAll();
        }

        Console.WriteLine("Done.");
        if (!Console.IsInputRedirected)
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
