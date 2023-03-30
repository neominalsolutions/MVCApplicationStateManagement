using Microsoft.AspNetCore.Mvc;

namespace MvcLab3.Controllers
{
  public class CookieController : Controller
  {
    public IActionResult Index()
    {
      // önceki net versiyonlarında HttpCookie ile oluşuyordu
      CookieOptions cookie = new CookieOptions();
      cookie.HttpOnly = false; // http bazlı bir cookie olmasın güvenli değil
      cookie.Secure = true; // Https Only (Sadece Cookie SSL olduğu sayfalarda gönderilsin ssl bazlı çalışsın)

      cookie.SameSite = SameSiteMode.Strict; // client aynı domainde ike cookie request gönderebilir kısıtlatdık. Başka bir site yada postman üzerinden bir istek gönderildiğinde bu cookie bizim yapımız kullanmayacaktır.
      cookie.Expires = DateTime.Now.AddDays(30); // 30 gün boyunca geçerli olsun.
      Response.Cookies.Append("username", "mert.alptekin", cookie);



      return Redirect("/Cookie/GetCookies");
    }

    public IActionResult GetCookies()
    {

      var cookie = Request.Cookies["username"];
      //var cookie2 = Request.Cookies.Get("username");

      return View();
    }
  }
}
