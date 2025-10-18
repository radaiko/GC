using System.Collections.Generic;
using GC.Core;
using GC.Models.Order;

namespace GC.Cache;

public static class GourmetCache {
  

  public static void GetFromWebApi() {
    var username = Base.Settings.GourmetUsername;
    var password = Base.Settings.GourmetPassword;
    if (username == null || password == null) {
      Log.Error("Gourmet username or password is not set.");
      return;
    }
    
    
  }
}