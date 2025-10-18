using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using GC.WebApis;

namespace GC.Models.Order;

public partial class Menu : ObservableObject {
  [ObservableProperty] private MenuType _type;
  [ObservableProperty] private string _title;
  [ObservableProperty] private char[] _allergens;
  [ObservableProperty] private decimal _price;
  [ObservableProperty] private DateTime _date;
  [ObservableProperty] private Status _appStatus;
  [ObservableProperty] private Status _webStatus;

  public Menu(MenuType type, string title, char[] allergens, decimal price, DateTime date) {
    _date = date;
    _type = type;
    _title = title;
    _allergens = allergens;
    _price = price;
  }
  
  public static List<Menu> GetDummyData() =>
    new() {
      // Day 0
      new Menu(MenuType.Menu1, "Grilled Chicken with Vegetables", ['A', 'C'], 8.50m, DateTime.Now),
      new Menu(MenuType.Menu2, "Spaghetti Bolognese", ['A', 'G'], 7.00m, DateTime.Now),
      new Menu(MenuType.Menu3, "Vegetarian Stir Fry", ['A', 'C', 'G'], 7.50m, DateTime.Now),
      new Menu(MenuType.SoupAndSalad, "Tomato Soup and Caesar Salad", ['A', 'D', 'G'], 6.00m, DateTime.Now),
      // Day 1
      new Menu(MenuType.Menu1, "Roast Beef with Potatoes", ['B', 'C'], 9.00m, DateTime.Now.AddDays(1)),
      new Menu(MenuType.Menu2, "Pasta Carbonara", ['A', 'G'], 7.50m, DateTime.Now.AddDays(1)),
      new Menu(MenuType.Menu3, "Quinoa Salad", ['A', 'C', 'G'], 8.00m, DateTime.Now.AddDays(1)),
      new Menu(MenuType.SoupAndSalad, "Lentil Soup and Garden Salad", ['A', 'D', 'G'], 6.50m, DateTime.Now.AddDays(1)),
      // Day 2
      new Menu(MenuType.Menu1, "Fish Tacos", ['A', 'F'], 8.00m, DateTime.Now.AddDays(2)),
      new Menu(MenuType.Menu2, "Lasagna", ['A', 'G'], 8.50m, DateTime.Now.AddDays(2)),
      new Menu(MenuType.Menu3, "Tofu Curry", ['A', 'C', 'G'], 7.00m, DateTime.Now.AddDays(2)),
      new Menu(MenuType.SoupAndSalad, "Mushroom Soup and Mixed Greens", ['A', 'D', 'G'], 6.00m, DateTime.Now.AddDays(2))
    };
}

public enum MenuType {
  Menu1,
  Menu2,
  Menu3,
  SoupAndSalad
}

public enum Status {
  Unknown,
  Available,
  SoldOut,
  Ordered,
  MarkedForOrder,
  MarkedForCancellation
}