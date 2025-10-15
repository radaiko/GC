using System.Text.Json;
using GC.Models;

namespace GC.Core;

public static class Base {
  public static bool IsDarkMode { get; set; } = false;
  public static Settings Settings { get; set; } = Settings.Load();
  
  public static JsonSerializerOptions JsonOptions { get; } = new() {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
  };
}