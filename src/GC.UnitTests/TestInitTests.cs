using GC.Core;

namespace GC.UnitTests;

public class TestInitTests {
  [Fact]
  public void Hub_IsTesting_IsTrue() {
    Assert.True(Base.IsTesting, "Base.IsTesting should be true during unit tests (set by module initializer)");
  }
}

