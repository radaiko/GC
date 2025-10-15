using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Xunit;
using GC.Models;

namespace GC.UnitTests;

public class SettingsTests {t
  private static readonly object FileLock = new();

  private static string GetStoragePath() {
    var f = typeof(Settings).GetField("StoragePath", BindingFlags.NonPublic | BindingFlags.Static);
    if (f == null) throw new InvalidOperationException("StoragePath field not found");
    return f.GetValue(null) as string ?? throw new InvalidOperationException("StoragePath is null");
  }

  [Fact]
  public void Load_ReturnsDefaults_WhenFileMissing() {
    var path = GetStoragePath();
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
  public void Save_Persists_NonSensitiveFields_And_LoadRestoresThem() {
    var path = GetStoragePath();
    lock (FileLock) {
      if (File.Exists(path)) File.Delete(path);

      var s = new Settings();
      s.GourmetUsername = "user1";
      s.VentoUsername = "user2";
      s.DebugMode = true;

      // File should have been written by the property setters
      Assert.True(File.Exists(path));
      var json = File.ReadAllText(path);

      using var doc = JsonDocument.Parse(json);
      var root = doc.RootElement;

      Assert.True(root.TryGetProperty("gourmetUsername", out var g));
      Assert.Equal("user1", g.GetString());

      Assert.True(root.TryGetProperty("ventoUsername", out var v));
      Assert.Equal("user2", v.GetString());

      Assert.True(root.TryGetProperty("debugMode", out var d));
      Assert.True(d.GetBoolean());

      // Now call Load() and verify values round-trip
      var loaded = Settings.Load();
      Assert.Equal("user1", loaded.GourmetUsername);
      Assert.Equal("user2", loaded.VentoUsername);
      Assert.True(loaded.DebugMode);

      // cleanup
      if (File.Exists(path)) File.Delete(path);
    }
  }

  [Fact]
  public void ChangingProperty_UpdatesStorageFile() {
    var path = GetStoragePath();
    lock (FileLock) {
      if (File.Exists(path)) File.Delete(path);

      var s = new Settings();
      s.GourmetUsername = "first";
      var json1 = File.ReadAllText(path);
      using var doc1 = JsonDocument.Parse(json1);
      Assert.Equal("first", doc1.RootElement.GetProperty("gourmetUsername").GetString());

      s.GourmetUsername = "second";
      var json2 = File.ReadAllText(path);
      using var doc2 = JsonDocument.Parse(json2);
      Assert.Equal("second", doc2.RootElement.GetProperty("gourmetUsername").GetString());

      if (File.Exists(path)) File.Delete(path);
    }
  }
}