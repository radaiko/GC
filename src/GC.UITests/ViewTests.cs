// New: headless tests for Views (MainView and MainWindow)
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Xunit;
using GC.ViewModels;
using GC.Views;

namespace GC.UITests;

[Collection("AvaloniaHeadless")]
public class ViewTests
{
  [Fact]
  public void MainView_BindsGreeting_ToTextBlock()
  {
    var view = new MainView();
    var vm = new MainViewModel();
    view.DataContext = vm;

    // Attach the view to a Window (TopLevel) so templates/bindings are applied
    var window = new Window { Content = view };
    window.Show();

    // Layout so bindings update
    window.Measure(Size.Infinity);
    window.Arrange(new Rect(0, 0, 400, 100));

    // Find the TextBlock descendant inside the view
    // Try direct Content (common when UserControl contains a single root element)
    var textBlock = view.Content as TextBlock;
    // Fallback to visual tree search if Content isn't the TextBlock
    if (textBlock == null)
    {
      textBlock = window.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault();
    }
    Assert.NotNull(textBlock);
    Assert.Equal(vm.Greeting, textBlock.Text);
   }

   [Fact]
   public void MainWindow_ContainsMainView_AndDisplaysGreeting()
   {
     var window = new MainWindow();

     // Show the window so its visual tree is built
     window.Show();

     // Find MainView inside the window by checking the Content or visual descendants
     var mainView = window.Content as MainView ?? window.GetVisualDescendants().OfType<MainView>().FirstOrDefault();
     Assert.NotNull(mainView);

     var vm = new MainViewModel();
     // Set DataContext on the window so the contained MainView picks it up
     window.DataContext = vm;

     // Layout window and children
     window.Measure(Size.Infinity);
     window.Arrange(new Rect(0, 0, 800, 450));

     // Prefer direct content if MainView contains TextBlock directly
     var textBlock = mainView.Content as TextBlock ?? window.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault();
     Assert.NotNull(textBlock);
     Assert.Equal(vm.Greeting, textBlock.Text);
   }
 }
