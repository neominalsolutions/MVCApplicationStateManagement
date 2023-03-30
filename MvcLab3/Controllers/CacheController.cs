using Microsoft.AspNetCore.Mvc;
using MvcLab3.Models;
using MvcLab3.Services;

namespace MvcLab3.Controllers
{

  // PostMan ile test edelim

  public class CacheController : Controller
  {
    private readonly ICacheService _cacheService;

    public CacheController(ICacheService cacheService)
    {
      _cacheService = cacheService;
    }

    [HttpGet("{key}/{value}")]
    public async Task<IActionResult> Get(string key, string value)
    {
      // key value set ettik
      await _cacheService.SetValueAsync(key,value);
      // value redisten keye göre çektik
      var data = await _cacheService.GetValueAsync(key);

      return Json(data);
    }

  }
}
