using System;
using System.Security.Cryptography;
using System.Text;
using UIKit;

namespace GC.iOS;

public static class DeviceKeyDeriver
{
    public static string GetDevicePassphrase()
    {
        var uniqueId = UIDevice.CurrentDevice.IdentifierForVendor?.ToString() ?? "unknown";
        using SHA256 sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(uniqueId));
        return Convert.ToBase64String(hash); // 44-char passphrase
    }
}