using System.Collections.Generic;
using Avalonia.Controls;
using GC.Models.Order;
using GCMenu = GC.Models.Order.Menu;

namespace GC.Controls;

public partial class MenuDay : UserControl {
  
  public List<GCMenu> Menus { get; set; } = [];
  public string FirstMenuDateString => Menus.Count > 0 ? Menus[0].Date.ToString("dddd, dd.MM.yyyy") : string.Empty;

  public MenuDay() {
    InitializeComponent();
    GetDummyData();
  }

  private void GetDummyData() {
    Menus.Add(new GCMenu(MenuType.Menu1, "Grilled Chicken with Vegetables", ['A', 'C'], 8.50m, System.DateTime.Now));
    Menus.Add(new GCMenu(MenuType.Menu2, "Spaghetti Bolognese", ['A', 'G'], 7.00m, System.DateTime.Now));
    Menus.Add(new GCMenu(MenuType.Menu3, "Vegetarian Stir Fry", ['A', 'C', 'G'], 7.50m, System.DateTime.Now));
    Menus.Add(new GCMenu(MenuType.SoupAndSalad, "Tomato Soup and Caesar Salad", ['A', 'D', 'G'], 6.00m, System.DateTime.Now));
  }
}