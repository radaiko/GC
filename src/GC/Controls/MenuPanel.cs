using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using GStyles = GC.Core.Styles;
using GMenu = GC.Models.Order.Menu;

namespace GC.Controls;

public class MenuPanel : UserControl {
  public MenuPanel(GMenu menu) {
    var stack = new StackPanel {
      Orientation = Orientation.Vertical,
      Margin = GStyles.StandardPadding
    };

    // Title
    stack.Children.Add(new TextBlock {
      Text = menu.Title,
      FontSize = 18,
      FontWeight = FontWeight.Bold,
      Foreground = GStyles.AccentBrush,
      Margin = GStyles.CompactPadding
    });

    // Allergens
    stack.Children.Add(new TextBlock {
      Text = $"Allergens: {(menu.Allergens.Length > 0 ? string.Join(", ", menu.Allergens) : "None")}",
      Foreground = GStyles.SecondaryTextBrush,
      Margin = GStyles.CompactPadding
    });

    // Price
    stack.Children.Add(new TextBlock {
      Text = $"Price: {menu.Price:C}",
      Foreground = GStyles.TextBrush,
      Margin = GStyles.CompactPadding
    });

    // Status
    stack.Children.Add(new TextBlock {
      Text = $"Status: {menu.AppStatus}",
      Foreground = GStyles.SecondaryTextBrush,
      Margin = GStyles.CompactPadding
    });

    Content = stack;
  }
}