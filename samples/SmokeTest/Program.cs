using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASControllerSDK;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("=== ASControllerSDK SmokeTest ===");

        var controller = AromaShooterController.SharedInstance;

        Console.WriteLine("[1] Setup() ...");
        await controller.Setup();

        var devices = controller.GetConnectedDevices();
        Console.WriteLine($"[2] Connected devices: {devices.Count}");
        foreach (var d in devices)
        {
            Console.WriteLine($" - {d.Transport} | {d.Identifier} | {d.DisplayName}");
        }

        if (devices.Count == 0)
        {
            Console.WriteLine("No devices found. Plug USB or pair BLE device, then run again.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Pick first device identifier (works as identifierOrName)
        var firstId = devices[0].Identifier;

        // --- Simple example ---
        Console.WriteLine("[3] Simple shoot: ports 1,2 for 3s (internal booster ON) on first device");
        controller.ShootSimple(3000, new[] { 1, 2 }, internalBooster: true, shooterNameOrId: firstId);

        await Task.Delay(2500);

        Console.WriteLine("[4] Stop simple on first device");
        controller.StopSimple(firstId);

        await Task.Delay(500);

        // --- Intensity example ---
        Console.WriteLine("[5] Intensity shoot: port1=100% for 3s (internal=100, external=100) on first device");
        var chambers = new List<AromaChamber>
        {
            new AromaChamber { number = 1, concentration = 100 }
        };

        controller.ShootWithIntensity(3000, chambers, internalBoosterIntensity: 100, externalBoosterIntensity: 100, shooterNameOrId: firstId);

        await Task.Delay(2500);

        Console.WriteLine("[6] Stop intensity: stop port1 + internal booster");
        controller.StopWithIntensity(firstId, chambers: new[] { 1 }, stopInternalBooster: true, stopExternalBooster: false);

        Console.WriteLine("Done. Press any key to exit...");
        Console.ReadKey();
    }
}
