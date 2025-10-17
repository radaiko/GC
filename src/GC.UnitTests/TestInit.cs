using System.Runtime.CompilerServices;
using GC.Core;

namespace GC.UnitTests;

internal static class TestInit {
  [ModuleInitializer]
  public static void Initialize() {
    // Ensure Hub knows we're running under tests
    Hub.IsTesting = true;
  }
}

