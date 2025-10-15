// New test: load and validate App.axaml headlessly (no Avalonia runtime required)
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace GC.UITests;

public class AppAxamlHeadlessTests {
  [Fact]
  public void AppAxaml_IsPresent_AndWellFormed() {
    // Find App.axaml by walking upward from the current directory
    var start = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
    var file = FindAppAxaml(start);
    Assert.True(File.Exists(file), $"App.axaml not found at '{file}'");

    // Parse as XML to verify it's well-formed XAML
    XDocument doc = XDocument.Load(file);
    Assert.NotNull(doc.Root);
    // Root local name should be Application
    Assert.Equal("Application", doc.Root.Name.LocalName);

    // Check that styles or FluentTheme is present
    var hasStyles = doc.Root.Descendants().Any(e => e.Name.LocalName == "Application.Styles" || e.Name.LocalName == "FluentTheme");
    Assert.True(hasStyles, "App.axaml should contain Application.Styles or a FluentTheme element");
  }

  private static string FindAppAxaml(string startDirectory) {
    var dir = new DirectoryInfo(startDirectory);
    while (dir != null) {
      var candidate = Path.Combine(dir.FullName, "GC", "App.axaml");
      if (File.Exists(candidate)) return candidate;
      // also check directly in this directory in case tests run from repo root
      candidate = Path.Combine(dir.FullName, "App.axaml");
      if (File.Exists(candidate)) return candidate;
      dir = dir.Parent;
    }
    throw new FileNotFoundException("Could not find GC/App.axaml by walking parent directories starting at " + startDirectory);
  }
}

