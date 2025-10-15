using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using GC.Core;
using Microsoft.Maui.Storage;

namespace GC.Models;

public partial class Settings : ObservableObject {
  private static readonly string StoragePath = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
  private const string GourmetPasswordKey = "settings_gourmet_password";
  private const string VentoPasswordKey = "settings_vento_password";
  // When true, Save() will no-op so we can set properties during Load without persisting repeatedly
  private bool _loading;
  
  [ObservableProperty] private string? _gourmetUsername;
  [ObservableProperty] private string? _gourmetPassword;
  [ObservableProperty] private string? _ventoUsername;
  [ObservableProperty] private string? _ventoPassword;
  [ObservableProperty] private bool _debugMode;

  // Attach on all properties to save on change
  partial void OnGourmetUsernameChanged(string? _) => Save();
  partial void OnGourmetPasswordChanged(string? _) => Save();
  partial void OnVentoUsernameChanged(string? _) => Save();
  partial void OnVentoPasswordChanged(string? _) => Save();
  partial void OnDebugModeChanged(bool _) => Save();
  
  // Private DTO used for JSON serialization so we don't trigger property-change side effects
  private sealed class SerializedSettings {
    public string? GourmetUsername { get; set; }
    public string? VentoUsername { get; set; }
    public bool DebugMode { get; set; }
  }

  public static Settings Load() {
    try {
      if (!File.Exists(StoragePath)) {
        Log.Info("Settings file not found, using defaults");
        // Try to read passwords if available, otherwise empty defaults
        var defaults = new Settings {
          _loading = true
        };
        try {
          defaults.GourmetPassword = SecureStorage.GetAsync(GourmetPasswordKey).GetAwaiter().GetResult();
        }
        catch {
          defaults.GourmetPassword = null;
        }
        try {
          defaults.VentoPassword = SecureStorage.GetAsync(VentoPasswordKey).GetAwaiter().GetResult();
        }
        catch {
          defaults.VentoPassword = null;
        }
        defaults._loading = false;

        Log.Debug("Settings loaded (defaults)");
        return defaults;
      }

      var json = File.ReadAllText(StoragePath);
      var data = JsonSerializer.Deserialize<SerializedSettings>(json, Base.JsonOptions) ?? new SerializedSettings();

      // Create instance and set backing fields directly to avoid firing change handlers during load
      var settings = new Settings();
      settings._loading = true;
      // Use the generated properties so we don't reference backing fields directly
      settings.GourmetUsername = data.GourmetUsername;
      settings.VentoUsername = data.VentoUsername;
      settings.DebugMode = data.DebugMode;

      // Load passwords from secure storage (may throw on unsupported platforms)
      try {
        settings.GourmetPassword = SecureStorage.GetAsync(GourmetPasswordKey).GetAwaiter().GetResult();
      }
      catch {
        settings.GourmetPassword = null;
      }

      try {
        settings.VentoPassword = SecureStorage.GetAsync(VentoPasswordKey).GetAwaiter().GetResult();
      }
      catch {
        settings.VentoPassword = null;
      }
      settings._loading = false;

      Log.Debug("Settings loaded");
      return settings;
    }
    catch (System.Exception ex) {
      Log.Error($"Failed to load settings: {ex.Message}");
      return new Settings();
    }
  }
  
  private void Save() {
    if (_loading) return;
    try {
      // Ensure directory exists
      var dir = Path.GetDirectoryName(StoragePath);
      if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

      // Prepare DTO with non-sensitive data
      var data = new SerializedSettings {
        GourmetUsername = this.GourmetUsername,
        VentoUsername = this.VentoUsername,
        DebugMode = this.DebugMode
      };

      var json = JsonSerializer.Serialize(data, Base.JsonOptions);
      File.WriteAllText(StoragePath, json);

      // Store passwords in platform secure storage. Call synchronously to keep this method signature simple.
      try {
        if (!string.IsNullOrEmpty(this.GourmetPassword))
          SecureStorage.SetAsync(GourmetPasswordKey, this.GourmetPassword).GetAwaiter().GetResult();
        else
          SecureStorage.Remove(GourmetPasswordKey);
      }
      catch {
        // ignore secure storage failures but log
        Log.Warn("Failed to save gourmet password to secure storage");
      }

      try {
        if (!string.IsNullOrEmpty(this.VentoPassword))
          SecureStorage.SetAsync(VentoPasswordKey, this.VentoPassword).GetAwaiter().GetResult();
        else
          SecureStorage.Remove(VentoPasswordKey);
      }
      catch {
        Log.Warn("Failed to save vento password to secure storage");
      }

      Log.Debug("Settings saved");
    }
    catch (System.Exception ex) {
      Log.Error($"Failed to save settings: {ex.Message}");
    }
  }
}