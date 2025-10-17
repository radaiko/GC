using GC.Core;

namespace GC.UnitTests;

public class TestInitTests {
  [Fact]
  public void Hub_IsTesting_IsTrue() {
    Assert.True(Hub.IsTesting, "Hub.IsTesting should be true during unit tests (set by module initializer)");
  }
}

