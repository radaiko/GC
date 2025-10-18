using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using GStyles = GC.Core.Styles;
using GMenu = GC.Models.Order.Menu;

namespace GC.Controls;

public class MenuDayPanel : UserControl {
  public List<GMenu> Menus { get; } = [];
  public DateTime Date { get; set; }
  
  public MenuDayPanel() {
    CreateMenuPanels();
  }
  
  public void SetData(List<GMenu> menus) {
    Menus.Clear();
    Menus.AddRange(menus);
    if (Menus.Count > 0) {
      Date = Menus[0].Date;
    }
    CreateMenuPanels();
  }
  
  private void CreateMenuPanels() {
    List<MenuPanel> menuPanels = [];
    menuPanels.AddRange(Menus.Select(menu => new MenuPanel(menu)));
    var stack = new StackPanel {
      Orientation = Avalonia.Layout.Orientation.Vertical,
      Margin = GStyles.StandardPadding,
      Children = {
        new TextBlock {
          Text = Date.ToString("dddd, dd.MM.yyyy"),
          FontSize = 20,
          FontWeight = Avalonia.Media.FontWeight.Bold,
          Foreground = GStyles.AccentBrush,
          Margin = GStyles.CompactPadding
        }
      }
    };
    foreach (var panel in menuPanels) {
      stack.Children.Add(panel);
    }
  }
  
}