using System;
using System.Security.Cryptography;
using System.Text;
using Android.Provider; // Android only

namespace GC.Android;

public static class DeviceKeyDeriver
{
    public static string GetDevicePassphrase()
    {
        const string uniqueId = Settings.Secure.AndroidId;;
        using SHA256 sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(uniqueId));
        return Convert.ToBase64String(hash); // 44-char passphrase
    }
}