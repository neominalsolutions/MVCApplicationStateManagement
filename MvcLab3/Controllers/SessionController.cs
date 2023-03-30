using Microsoft.AspNetCore.Mvc;
using MvcLab3.Extensions;
using MvcLab3.Models;

namespace MvcLab3.Controllers
{
  public class SessionController : Controller
  {


    public IActionResult SetSession()
    {
      // Normalde sessionda string,int,byte tipinde veri tutabiliriz.
      // Object tipinde tutmak için kendi SessionExtensions yazdık
      HttpContext.Session.SetString("deneme", HttpContext.Session.Id);

      // Net Core session mekanizması sadece String,Int,Byte
      
     
      // kendi extensionımız üzerinden session değerini obje tipinde set ettik
      HttpContext.Session.SetObject<Product>("ProductSession", new Product
      {
        Name = "P1",
        Description = "Deneme"
      });

      return Redirect("/Session/GetSession");
    }


    public IActionResult GetSession()
    {

      // kendi extensionımız üzerinden session değerini obje tipinde get ettik

      
      ViewBag.Session = HttpContext.Session.GetString("deneme");
      // HttpContext.Session.Remove("deneme");

      ViewBag.SessionProduct = HttpContext.Session.GetObject<Product>("ProductSession");


      return View();
    }
  }
}
