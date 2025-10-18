using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GC.Core;
using HtmlAgilityPack;

namespace GC.WebApis;

public static class GourmetApi {
  private const string WebUrl = "https://alaclickneu.gourmet.at/";
  private const string StartPage = "start";

  // Shared CookieContainer and HttpClient for session persistence
  private static readonly CookieContainer CookieContainer = new();
  private static readonly HttpClientHandler Handler = new() { CookieContainer = CookieContainer, AllowAutoRedirect = true };
  private static readonly HttpClient Client = new(Handler);
  private static bool _isLoggedIn;

  public static void Logout() {
    foreach (Cookie cookie in CookieContainer.GetAllCookies())
    {
      cookie.Expired = true;
    }
    _isLoggedIn = false;
  }
  
  public static async Task<bool> LoginAsync() {
    if (_isLoggedIn) return true;
    // 1. Check if already logged in
    var startPageResponse = await Client.GetAsync(WebUrl + StartPage + "/");
    var startPageHtml = await startPageResponse.Content.ReadAsStringAsync();
    if (startPageHtml.Contains("navbar-link") && startPageHtml.Contains("einstellungen")) {
      _isLoggedIn = true;
      return true;
    }
    // 2. Get login page and extract ufprt
    var loginPageResponse = await Client.GetAsync(WebUrl + StartPage + "/");
    var loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
    var doc = new HtmlAgilityPack.HtmlDocument();
    doc.LoadHtml(loginPageHtml);
    var ufprtNode = doc.DocumentNode.SelectSingleNode("//input[@name='ufprt']");
    var ufprt = ufprtNode?.GetAttributeValue("value", "");
    // 3. Prepare form data
    var formData = new Dictionary<string, string> {
      { "Username", Base.Settings.GourmetUsername ?? "" },
      { "Password", Base.Settings.GourmetPassword ?? "" },
      { "RememberMe", "true" },
      { "ufprt", ufprt ?? "" }
    };
    // 4. Post login form
    var response = await Client.PostAsync(WebUrl, new FormUrlEncodedContent(formData));
    var responseHtml = await response.Content.ReadAsStringAsync();
    // 5. Check for login success
    if (responseHtml.Contains("navbar-link") && responseHtml.Contains("einstellungen")) {
      _isLoggedIn = true;
      return true;
    }
    return false;
  }

  public class MenuInfo {
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Allergens { get; set; }
    public string PriceInfo { get; set; }
    public MenuType Type { get; set; }

  }
  
  public enum MenuType {
    Menu1,
    Menu2,
    Menu3,
    SoupAndSalad
  }

  public static async Task<List<MenuInfo>> GetMenusAsync() {
    // Ensure logged in before fetching menus
    if (!_isLoggedIn) await LoginAsync();
    if (!_isLoggedIn) {
      throw new InvalidOperationException("Not logged in to Gourmet API.");
    }
    var i = 0;
    List<MenuInfo> result = new();
    var seen = new HashSet<string>(); // To track unique menus by a composite key
    while (i < 4) {
      var doc = await GetPageAsync(WebUrl + $"menus/?page={i}");
      var menus = await ExtractMenusFromPage(doc);
      foreach (var menu in menus) {
        // Use a composite key of date, title, and type to prevent duplicates
        var key = $"{menu.Date:yyyy-MM-dd}|{menu.Title}|{menu.Type}";
        if (!seen.Contains(key)) {
          seen.Add(key);
          result.Add(menu);
        }
      }
      if (menus.Count == 0) {
        break;
      }
      i++;
    }
    return result;
  }
  
  private static async Task<HtmlDocument> GetPageAsync(string url) {
    var response = await Client.GetAsync(url);
    response.EnsureSuccessStatusCode();
    var html = await response.Content.ReadAsStringAsync();
    var doc = new HtmlDocument();
    doc.LoadHtml(html);
    return doc;
  }
  
  private static async Task<List<MenuInfo>> ExtractMenusFromPage(HtmlDocument doc) {
    var result = new List<MenuInfo>();
    var menuNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'meal')]");
    if (menuNodes != null) {
      foreach (var node in menuNodes) {
        var id = node.SelectSingleNode(".//div[@class='title']");
        // Find the allergen <li> element inside the meal node
        var allergeneNode = node.SelectSingleNode(".//li[contains(@class, 'allergen')]");
        var allergene = allergeneNode != null ? allergeneNode.InnerText.Trim() : null;
        // Find the open_info div with menu-article-detail class and get data-date
        var openInfoNode = node.SelectSingleNode(".//div[contains(@class, 'open_info') and contains(@class, 'menu-article-detail')]");
        var dateAttr = openInfoNode?.GetAttributeValue("data-date", null);
        DateTime? date = null;
        if (!string.IsNullOrEmpty(dateAttr)) {
          // Try parsing MM-dd-yyyy
          if (DateTime.TryParseExact(dateAttr, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate)) {
            date = parsedDate;
          } else if (DateTime.TryParse(dateAttr, out var fallbackDate)) {
            date = fallbackDate;
          }
        }
        var titleNode = node.SelectSingleNode(".//div[@class='subtitle']");
        // Find the price <div class="price"><span>...</span></div>
        var priceNode = node.SelectSingleNode(".//div[contains(@class, 'price')]/span");
        var price = priceNode != null ? priceNode.InnerText.Trim() : null;
        // Find the menu type from the title div (e.g., MENÜ I, MENÜ II, etc.)
        var titleDiv = node.SelectSingleNode(".//div[contains(@class, 'title')]");
        MenuType menuType = MenuType.Menu1; // Default
        if (titleDiv != null)
        {
          var titleText = titleDiv.InnerText.Trim();
          if (titleText.Contains("SUPPE") || titleText.Contains("SUPPE &amp; SALAT"))
            menuType = MenuType.SoupAndSalad;
          else if (titleText.Contains("MENÜ III") || titleText.Contains("MEN&#220; III"))
            menuType = MenuType.Menu3;
          else if (titleText.Contains("MENÜ II") || titleText.Contains("MEN&#220; II"))
            menuType = MenuType.Menu2;
          else if (titleText.Contains("MENÜ I") || titleText.Contains("MEN&#220; I"))
            menuType = MenuType.Menu1;
        }
        if (titleNode != null && date.HasValue) {
          result.Add(new MenuInfo {
            Date = date.Value,
            Title = titleNode.InnerText.Trim(),
            Allergens = allergene,
            PriceInfo = price,
            Type = menuType
          });
        }
      }
    }
    return result;
  }
}