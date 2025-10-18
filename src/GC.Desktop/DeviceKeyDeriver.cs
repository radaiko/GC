using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

// Windows only

namespace GC.Desktop;

public static class DeviceKeyDeriver {
  public static string GetDevicePassphrase() {
    var uniqueId = GetUniqueDeviceId();
    using (var sha = SHA256.Create()) {
      var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(uniqueId));
      return Convert.ToBase64String(hash); // 44-char passphrase
    }
  }

  private static string GetUniqueDeviceId() {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      return GetWindowsId();
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      return GetMacId();
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")))
        return GetLinuxId();
    }
    throw new PlatformNotSupportedException();
  }

  private static string GetWindowsId() => (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography")?.GetValue("MachineGuid") ?? Environment.MachineName;

  private static string GetMacId() {
    var p = new Process { StartInfo = new ProcessStartInfo { FileName = "ioreg", Arguments = "-rd1 -c IOPlatformExpertDevice", RedirectStandardOutput = true } };
    p.Start();
    var output = p.StandardOutput.ReadToEnd();
    p.WaitForExit();
    var start = output.IndexOf("IOPlatformSerialNumber", StringComparison.Ordinal) + "\"IOPlatformSerialNumber\" = \"".Length;
    var end = output.IndexOf("\"", start, StringComparison.Ordinal);
    return output.Substring(start, end - start);
  }

  private static string GetLinuxId() {
    try {
      var path = "/sys/class/dmi/id/product_serial";
      return File.Exists(path) ? File.ReadAllText(path).Trim() : Environment.MachineName;
    }
    catch {
      return Environment.MachineName;
    }
  }
}