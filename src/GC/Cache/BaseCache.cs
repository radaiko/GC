using System.IO;
using Microsoft.Data.Sqlite;

namespace GC.Cache;

public static class BaseCache {
  static BaseCache() {
    var dbPath = "cache.db";
    if (File.Exists(dbPath)) return;
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();
    using var command = connection.CreateCommand();
    // Create both cache tables if they do not exist
    command.CommandText = @"CREATE TABLE IF NOT EXISTS GourmetCache (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Key TEXT UNIQUE,
            Value TEXT
        );
        CREATE TABLE IF NOT EXISTS VentoCache (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Key TEXT UNIQUE,
            Value TEXT
        );";
    command.ExecuteNonQuery();
  }

  /// <summary>
  /// Writes a key-value pair to the specified cache table.
  /// </summary>
  public static void WriteToCache(string table, string key, string value) {
    using var connection = new SqliteConnection("Data Source=cache.db");
    connection.Open();
    using var command = connection.CreateCommand();
    command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, Value) VALUES (@key, @value)";
    command.Parameters.AddWithValue("@key", key);
    command.Parameters.AddWithValue("@value", value);
    command.ExecuteNonQuery();
  }

  /// <summary>
  /// Reads a value by key from the specified cache table.
  /// </summary>
  public static void ReadFromCache(string table, string key, out string? value) {
    using var connection = new SqliteConnection("Data Source=cache.db");
    connection.Open();
    using var command = connection.CreateCommand();
    command.CommandText = $"SELECT Value FROM {table} WHERE Key = @key";
    command.Parameters.AddWithValue("@key", key);
    using var reader = command.ExecuteReader();
    if (reader.Read()) {
      value = reader.GetString(0);
    } else {
      value = null;
    }
  }
}