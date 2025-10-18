using GC.Core;
using GC.WebApis;

namespace GC.UnitTests;

public class GourmetApiTests
{
    public GourmetApiTests()
    {
        // Loads .env file from project root
        DotNetEnv.Env.TraversePath().Load();
        Base.Settings.GourmetUsername = Environment.GetEnvironmentVariable("GourmetUsername");
        Base.Settings.GourmetPassword = Environment.GetEnvironmentVariable("GourmetPassword");
    }
    
    [Fact]
    public async Task LoginAsync_ReturnsTrue_WithValidCredentials()
    {
        // If not, this test will fail. For real unit tests, mock HTTP and settings.
        bool result = await GourmetApi.LoginAsync();
        Assert.True(result, "LoginAsync should return true with valid credentials.");
    }

    [Fact]
    public async Task GetMenusAsync_ReturnsMenus()
    {
        // This test assumes the endpoint is reachable and returns at least one menu
        List<GourmetApi.MenuInfo> menus = await GourmetApi.GetMenusAsync();
        Assert.NotNull(menus);
        Assert.True(menus.Count > 0, "GetMenusAsync should return at least one menu.");
    }
}

