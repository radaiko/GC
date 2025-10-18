using System;
using System.Diagnostics;
using System.IO;
using System.Management; // Windows only
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32; // Windows only
using Android.Provider; // Android only
using UIKit; // iOS only

public static class DeviceKeyDeriver
{
    public static string GetDevicePassphrase()
    {
        string uniqueId = GetUniqueDeviceId();
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(uniqueId));
            return Convert.ToBase64String(hash); // 44-char passphrase
        }
    }

    private static string GetUniqueDeviceId()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return GetWindowsId();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return GetMacId();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")))
                return GetAndroidId();
            else
                return GetLinuxId();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")))
            return GetIosId();
        throw new PlatformNotSupportedException();
    }

    private static string GetWindowsId()
    {
        return (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography")?.GetValue("MachineGuid") ?? Environment.MachineName;
    }

    private static string GetMacId()
    {
        Process p = new Process { StartInfo = new ProcessStartInfo { FileName = "ioreg", Arguments = "-rd1 -c IOPlatformExpertDevice", RedirectStandardOutput = true } };
        p.Start();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        int start = output.IndexOf("IOPlatformSerialNumber") + "\"IOPlatformSerialNumber\" = \"".Length;
        int end = output.IndexOf("\"", start);
        return output.Substring(start, end - start);
    }

    private static string GetLinuxId()
    {
        try
        {
            string path = "/sys/class/dmi/id/product_serial";
            return File.Exists(path) ? File.ReadAllText(path).Trim() : Environment.MachineName;
        }
        catch { return Environment.MachineName; }
    }

    private static string GetAndroidId()
    {
#if ANDROID
        return Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Settings.Secure.AndroidId);
#else
        return "unknown";
#endif
    }

    private static string GetIosId()
    {
#if IOS
        return UIDevice.CurrentDevice.IdentifierForVendor?.ToString() ?? "unknown";
#else
        return "unknown";
#endif
    }
}