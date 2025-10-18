using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using GC.Models.Order;

namespace GC.ViewModels;

public partial class OrderViewModel : ObservableObject {
  
  [ObservableProperty] private List<Menu> _menus = Menu.GetDummyData();
  
}