using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GC.Models.Order;

public partial class Day : ObservableObject {
    [ObservableProperty] private DateTime _date;
    [ObservableProperty] private List<Menu> _menus = new();
}