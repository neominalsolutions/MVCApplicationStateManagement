using Microsoft.AspNetCore.Mvc;

namespace MvcLab3.Controllers
{
  public class HiddenInputController : Controller
  {
    public IActionResult Sample()
    {
      return View();
    }

    [HttpPost]
    public IActionResult SendCode(string verificationCode)
    {
      // hidden Input ile formdan veriyi backend'e taşıdık

      return View("Sample");
    }
  }
}
