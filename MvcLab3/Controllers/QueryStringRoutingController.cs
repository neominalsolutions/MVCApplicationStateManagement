using Microsoft.AspNetCore.Mvc;

namespace MvcLab3.Controllers
{
  public class QueryStringRoutingController : Controller
  {
    //1.yöntem FromQuery attibute ile sayfaya gelen istek parametesi yakalanabilir
    public IActionResult QueryString([FromQuery(Name = "name")] string name)
    {

      // 2.yöntem 

      string _name = HttpContext.Request.Query["name"].ToString();

      return View();
    }


    // 1. yöntem attribute dinamik olarak code geçerek
    [HttpGet("routing/{code}/{name}")]
    public IActionResult Routing(string code, string name)
    {

      // 2.yöntem 

      string _code = HttpContext.Request.RouteValues["code"].ToString();

      return View();
    }
  }
}
