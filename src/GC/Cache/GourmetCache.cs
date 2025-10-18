using System.Collections.Generic;
using GC.Core;
using GC.Models.Order;

namespace GC.Cache;

public static class GourmetCache {


  public static List<Day> Get(bool forceRefresh = false) {
    // TODO: Implement caching logic here
    // Check if cache exists and is valid
    // if not valid get from webapi
    // if forceRefresh is true, bypass cache and get from webapi
    return new List<Day>();
  }

  public static void GetFromWebApi() {
    var username = Base.Settings.GourmetUsername;
    var password = Base.Settings.GourmetPassword;
    if (username == null || password == null) {
      Log.Error("Gourmet username or password is not set.");
      return;
    }
    
    
  }
}