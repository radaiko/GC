using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GC.Core;
using GC.Models.Order;
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
    var username = Base.Settings.GourmetUsername;
    var password = Base.Settings.GourmetPassword;
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
      return false;
    }
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
    var doc = new HtmlDocument();
    doc.LoadHtml(loginPageHtml);
    var ufprtNode = doc.DocumentNode.SelectSingleNode("//input[@name='ufprt']");
    var ufprt = ufprtNode.GetAttributeValue("value", "");
    // 3. Prepare form data
    var formData = new Dictionary<string, string> {
      { "Username", username },
      { "Password", password },
      { "RememberMe", "true" },
      { "ufprt", ufprt }
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

  public static async Task<List<Menu>> GetMenusAsync() {
    // Ensure logged in before fetching menus
    if (!_isLoggedIn) await LoginAsync();
    if (!_isLoggedIn) {
      throw new InvalidOperationException("Not logged in to Gourmet API.");
    }
    var i = 0;
    List<Menu> result = new();
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
  
  private static string ExtractTitle(HtmlNode node)
  {
    var titleNode = node.SelectSingleNode(".//div[@class='subtitle']");
    return titleNode.InnerText.Trim();
  }

  private static string ExtractAllergens(HtmlNode node)
  {
    var allergeneNode = node.SelectSingleNode(".//li[contains(@class, 'allergen')]");
    return allergeneNode.InnerText.Trim();
  }

  private static string ExtractPrice(HtmlNode node)
  {
    var priceNode = node.SelectSingleNode(".//div[contains(@class, 'price')]/span");
    return priceNode.InnerText.Trim();
  }

  private static DateTime? ExtractDate(HtmlNode node)
  {
      var openInfoNode = node.SelectSingleNode(".//div[contains(@class, 'open_info') and contains(@class, 'menu-article-detail')]");
      var dateAttr = openInfoNode.GetAttributeValue("data-date", "");
      if (!string.IsNullOrEmpty(dateAttr)) {
        if (DateTime.TryParseExact(dateAttr, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate)) {
              return parsedDate;
        }
        if (DateTime.TryParse(dateAttr, out var fallbackDate)) {
          return fallbackDate;
        }
      }
      return null;
  }

  private static MenuType ExtractMenuType(HtmlNode node)
  {
    var titleDiv = node.SelectSingleNode(".//div[contains(@class, 'title')]");
    var titleText = titleDiv.InnerText.Trim();
    if (titleText.Contains("SUPPE") || titleText.Contains("SUPPE &amp; SALAT"))
      return MenuType.SoupAndSalad;
    if (titleText.Contains("MENÜ III") || titleText.Contains("MEN&#220; III"))
      return MenuType.Menu3;
    if (titleText.Contains("MENÜ II") || titleText.Contains("MEN&#220; II"))
      return MenuType.Menu2;
    return MenuType.Menu1;
  }

  private static Task<List<Menu>> ExtractMenusFromPage(HtmlDocument doc) {
    var result = new List<Menu>();
    var menuNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'meal')]");
    foreach (var node in menuNodes) {
      if (!node.InnerHtml.Contains("open_info menu-article-detail")) continue;
      var date = ExtractDate(node);
      var title = ExtractTitle(node);
      var allergene = ExtractAllergens(node);
      var price = ExtractPrice(node);
      var menuType = ExtractMenuType(node);
      if (date.HasValue) {
        result.Add(new Menu(
          menuType,
          title,
          allergene.Where(char.IsLetter).ToArray(),
          decimal.TryParse(price.Replace("€", "").Trim(), out var parsedPrice) ? parsedPrice : 0,
          date.Value
        ));
      }
    }
    return Task.FromResult(result);
  }
}