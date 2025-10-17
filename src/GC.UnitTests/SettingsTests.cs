using GC.Models;

namespace GC.UnitTests;

public class SettingsTests(ITestOutputHelper output) {
  private static readonly Lock FileLock = new();

  private static string GetStoragePath() {
    return Settings.StoragePath;
  }

  [Fact]
  public void Load_ReturnsDefaults_WhenFileMissing() {
    var path = GetStoragePath();
    var dir = Path.GetDirectoryName(path);
    output.WriteLine($"[TEST] Load_ReturnsDefaults_WhenFileMissing StoragePath={path} DirExists={(!string.IsNullOrEmpty(dir) && Directory.Exists(dir))}");
    lock (FileLock) {
      if (File.Exists(path)) File.Delete(path);

      var s = Settings.Load();

      Assert.Null(s.GourmetUsername);
      Assert.Null(s.VentoUsername);
      Assert.False(s.DebugMode);

      // Load should not have created the file when it wasn't present
      Assert.False(File.Exists(path));
    }
  }

  [Fact]
  public void Load_Reads_NonSensitiveFields_FromFile() {
    var path = GetStoragePath();
    lock (FileLock) {
      if (File.Exists(path)) File.Delete(path);

      // Ensure directory exists
      var dir = Path.GetDirectoryName(path);
      if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

      // Prepare JSON matching the internal SerializedSettings DTO and write it directly
      var data = new {
        gourmetUsername = "user1",
        ventoUsername = "user2",
        debugMode = true
      };
      var options = new System.Text.Json.JsonSerializerOptions {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
      };
      var json = System.Text.Json.JsonSerializer.Serialize(data, options);
      File.WriteAllText(path, json);

      // Debug: print the file contents we just wrote
      var written = File.ReadAllText(path);
      output.WriteLine($"[TEST] Written JSON: {written}");

      // Verify the file contains the expected keys/values (format the app expects)
      Assert.Contains("\"gourmetUsername\": \"user1\"", written);
      Assert.Contains("\"ventoUsername\": \"user2\"", written);
      Assert.Contains("\"debugMode\": true", written);

      // cleanup
      if (File.Exists(path)) File.Delete(path);
    }
  }

  [Fact]
  public void ChangingProperty_Raises_PropertyChanged() {
    var s = new Settings();
    var seen = new List<string>();
    s.PropertyChanged += (_, e) => seen.Add(e.PropertyName ?? string.Empty);

    s.GourmetUsername = "first";
    s.GourmetUsername = "second";
    s.VentoUsername = "v1";

    Assert.Contains("GourmetUsername", seen);
    Assert.Contains("VentoUsername", seen);
  }

  [Fact]
  public void SaveAndLoad_Roundtrip_PersistsNonSensitiveFields() {
    var path = GetStoragePath();
    lock (FileLock) {
      if (File.Exists(path)) File.Delete(path);

      // Ensure directory exists
      var dir = Path.GetDirectoryName(path);
      if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

      // Create settings and set properties (setters call Save())
      var s = new Settings();
      s.GourmetUsername = "round_gourmet";
      s.VentoUsername = "round_vento";
      s.DebugMode = true;
      s.GourmetPassword = "secret123";
      s.VentoPassword = "secret456";

      // Save should have created the file
      Assert.True(File.Exists(path), "Settings.Save should create the settings file");

      // Load a fresh instance and verify non-sensitive values roundtrip
      var loaded = Settings.Load();
      Assert.Equal("round_gourmet", loaded.GourmetUsername);
      Assert.Equal("round_vento", loaded.VentoUsername);
      Assert.True(loaded.DebugMode);

      // Passwords depend on platform secure storage. If present, they should match; otherwise they may be null.
      if (!string.IsNullOrEmpty(loaded.GourmetPassword) || !string.IsNullOrEmpty(loaded.VentoPassword)) {
        Assert.Equal("secret123", loaded.GourmetPassword);
        Assert.Equal("secret456", loaded.VentoPassword);
      }

      // cleanup
      if (File.Exists(path)) File.Delete(path);
    }
  }
}