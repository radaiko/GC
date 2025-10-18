using CommunityToolkit.Mvvm.ComponentModel;

namespace GC.Models.Order;

public partial class Menu : ObservableObject {
  [ObservableProperty] private int _id;
  [ObservableProperty] private string _name;
  [ObservableProperty] private string _description;
  [ObservableProperty] private char[] _allergens;
  [ObservableProperty] private Status _appStatus;
  [ObservableProperty] private Status _webStatus;

  public Menu(string name, string description, char[] allergens) {
    _name = name;
    _description = description;
    _allergens = allergens;
  }
}

public enum Status {
  Unknown,
  Available,
  SoldOut,
  Ordered,
  MarkedForOrder,
  MarkedForCancellation
}