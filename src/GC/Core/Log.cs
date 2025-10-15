using System;

namespace GC.Core;

public static class Log {
  public static void Info(string message) {
    Write("INFO", message);
  }

  public static void Warn(string message) {
    Write("WARN", message);
  }

  public static void Error(string message) {
    Write("ERROR", message);
  }

  public static void Debug(string message) {
    if (!Base.Settings.DebugMode) return;
    Write("DEBUG", message);
  }
  
  private static void Write(string level, string message) {
    Console.WriteLine($"[{level}] [{Caller}] [{Timestamp}] {message}");
  }

  private static string Caller => new System.Diagnostics.StackTrace().GetFrame(3)?.GetMethod()?.DeclaringType?.Name ?? "Unknown";

  private static string Timestamp => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
}