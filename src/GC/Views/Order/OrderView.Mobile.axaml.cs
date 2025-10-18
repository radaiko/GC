using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using GC.Controls;
using GMenu = GC.Models.Order.Menu;

namespace GC.Views.Order;

public partial class OrderViewMobile : UserControl {
  public List<GMenu> Menus { get; } = [];
  
  public OrderViewMobile() {
    InitializeComponent();
    Setup();
  }

  private void Setup() {
    Menus.Clear();
    Menus.AddRange(GMenu.GetDummyData()); 
    // Split the menus by day
    var daysAvailable = Menus.Select(m => m.Date.Date).Distinct().OrderBy(d => d).ToList().Count;
    List<GMenu>[] menusByDay = new List<GMenu>[daysAvailable];
    for (int i = 0; i < daysAvailable; i++) {
      var day = Menus.Select(m => m.Date.Date).Distinct().OrderBy(d => d).ToList()[i];
      menusByDay[i] = Menus.Where(m => m.Date.Date == day).ToList();
    }
    List<MenuDayPanel> menuDayPanels = [];
    foreach (var dayMenus in menusByDay) {
      menuDayPanels.Add(new MenuDayPanel());
      menuDayPanels.Last().SetData(dayMenus);
    }
    DayCarousel.Items = menuDayPanels;
  }
}