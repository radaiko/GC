// New: runtime headless tests following Avalonia headless xUnit docs
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Xunit;

namespace GC.UITests;

[Collection("AvaloniaHeadless")]
public class AvaloniaHeadlessRuntimeTests
{
  [Fact]
  public void App_CanBeInitialized_Headlessly()
  {
    var app = new GC.App();
    // Should not throw when loading XAML and resources in headless setup
    app.Initialize();
  }

  [Fact]
  public void Button_CanBeMeasuredArrangedAndRendered_Headlessly()
  {
    var button = new Button { Content = "Hi", Width = 120, Height = 40 };

    // Measure & arrange the control
    button.Measure(Size.Infinity);
    button.Arrange(new Rect(0, 0, button.Width, button.Height));

    // Create a render target and render the control
    var pixelSize = new PixelSize((int)button.Width, (int)button.Height);
    var dpi = new Vector(96, 96);

    using var rtb = new RenderTargetBitmap(pixelSize, dpi);
    rtb.Render(button);

    // Basic assertion: render target has non-zero size
    Assert.True(rtb.PixelSize.Width > 0 && rtb.PixelSize.Height > 0);
  }
}
