// New: xUnit fixture to initialize Avalonia in headless mode per Avalonia headless xUnit docs
using System;
using Avalonia;
using Avalonia.Headless;
using Xunit;

namespace GC.UITests;

public class AvaloniaHeadlessFixture : IDisposable
{
  public AvaloniaHeadlessFixture()
  {
    // Configure Avalonia for headless testing. SetupWithoutStarting initializes platform services
    // so XAML loading and control instantiation works without starting an application loop.
    AppBuilder.Configure<GC.App>()
      .UseHeadless(new AvaloniaHeadlessPlatformOptions())
      .SetupWithoutStarting();
  }

  public void Dispose()
  {
    // No-op cleanup: avoid accessing Avalonia internals from tests. The process exit will reclaim resources.
  }
}

// Define xUnit collection so the fixture runs once per collection
[CollectionDefinition("AvaloniaHeadless")]
public class AvaloniaHeadlessCollection : ICollectionFixture<AvaloniaHeadlessFixture>
{
}
