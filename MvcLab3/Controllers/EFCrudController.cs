using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcLab3.Entities;
using System.ComponentModel;

namespace MvcLab3.Controllers
{
  public class EFCrudController : Controller
  {
    private NORTHWNDContext _db;

    public EFCrudController(NORTHWNDContext db)
    {
      _db = db;
    }
    public ActionResult Sample()
    {
      return View();
    }


    public IActionResult CreateSample()
    {

      var c = new Category();
      c.CategoryName = "c-test1";

      _db.Categories.Add(c);
      int result = _db.SaveChanges();

      if(result > 0)
      {
        ViewBag.Message = "Kayıt Başarılı";
      }

      return View();
    }


    public IActionResult UpdateSample()
    {
      var c = _db.Categories.FirstOrDefault(x => x.CategoryName == "c-test1");
      c.CategoryName = "c-test-21";
      int result = _db.SaveChanges();

      if (result > 0)
      {
        ViewBag.Message = "Kayıt Başarılı";
      }


      return View();
    }


    public IActionResult DeleteSample()
    {
      var c = _db.Categories.FirstOrDefault(x => x.CategoryName == "c-test-21");
      _db.Categories.Remove(c);
      int result = _db.SaveChanges();


      if (result > 0)
      {
        ViewBag.Message = "Kayıt Başarılı";
      }

      return View();
    }


    // EF Core Async methodlar ile kullanımı
    public async Task<IActionResult> DeleteSampleAsync()
    {
      var c = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryName == "c-test-21");
      _db.Categories.Remove(c);
      int result =  await _db.SaveChangesAsync();


      if (result > 0)
      {
        ViewBag.Message = "Kayıt Başarılı";
      }

      return View();
    }

    // aşağıdaki örneklerde ise EF Core tarafında async olarak çalışacabildiğimizi görmekteyiz.
    // async ile birlikte veritabanında eş zamanlı query çalıştırabilme yeteğine sahibiz bu sebep ile genlede async olarak savechanges işlemlerimizi yapmaktayız

    // EF tarafında Transaction örneği
    public async Task<IActionResult> TransactionSample()
    {
      // transaction, io read write ve api call db connection gibi işlemleri lütfen using ile yazalım
      using (var tran = _db.Database.BeginTransaction())
      {


        try
        {
          var c = new Category();
          c.CategoryName = "Kategori 451";

          var p = new Product();
          p.UnitPrice = 10;
          p.UnitsInStock = 10;
          p.ProductName = "P1041";
          p.Category = c;
          p.CategoryId = c.CategoryId;

           _db.Add(c);
          _db.Add(p);

          // aynı anda birden fazla kayıt gönderdik

          _db.SaveChanges();

          tran.Commit();
        }
        catch (Exception ex)
        {

          tran.Rollback();

          throw;
        }
      }

      return View();
    }

    // EF ile Store Procedure örneği
    public async Task<IActionResult> StoreProcedureSample()
    {

     
      SqlParameter Name = new SqlParameter("@name", "Test Kategori");

      // aşağıdaki procedure çalıştırıp sql de çalıştırıp öyle test edelim.
      /*
       * 
       * 
       *  create proc InsertCategoryProc(@name nvarchar(50))
         as
         begin
          insert into dbo.Categories(CategoryName) values(@name)
         end
       * 
       */

      var result = await _db.Database.ExecuteSqlRawAsync("exec InsertCategoryProc @name", Name);

      return View();

    }
  }
}
