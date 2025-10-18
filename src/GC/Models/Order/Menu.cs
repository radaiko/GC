using System;
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