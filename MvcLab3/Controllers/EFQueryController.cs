using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLab3.Entities;

namespace MvcLab3.Controllers
{
  public class EFQueryController : Controller
  {

    private NORTHWNDContext _db;

    public EFQueryController(NORTHWNDContext db)
    {
      _db = db;
    }

    // Sql üzerinde Windows Tab SqlServer Profiler üzerinden bir kaç sorgu inceledylim.
    public IActionResult Sample()
    {

      var products = _db.Products.ToList(); // lambda expression linq
      var products2 = (from p in _db.Products select p).ToList(); // raw linq
                                                                  // karmaşık group by ve join işlemleri varsa yukarıdaki gibi linqtoSql olarak kullanabiliriz.
                                                                  // lazy loading products ile birlikte categories geliyor.
                                                                  // Include ile navigation property üzerinden bağlamış olduk.
                                                                  // Eager Loading her zaman lazy loading göre daha performanslı bir yöntemdir.

      var productIncludeWithCategory = _db.Products.Include(x => x.Category).ToList();
      // install-package Microsoft.EntityFrameworkCore.Proxies eğer lazy loading aktif hale getirmek istersek bu versiyonda yukarıdaki paketi kurmamız gerekiyor. 


      // fiyatı 43 ile 78 arasında olanlar

      var products3 = _db.Products.Where(x => x.UnitPrice >= 43 && x.UnitPrice <= 78).ToList();
      
      // ürünleri fiyatına göre artandan azalana sıralama
      var products4 = _db.Products.OrderByDescending(x => x.UnitPrice).ToList();

      // isiminde ürün geçen products
      var products5 = _db.Products.Where(x => x.ProductName.Contains("chai")).ToList();
      // 2.yöntem
      var products6 = _db.Products.Where(x => EF.Functions.Like(x.ProductName, "%chai%")).ToList();

      // ürün ismi ch ile başlayanlar
      var products7 = _db.Products.Where(x => EF.Functions.Like(x.ProductName, "%ch"));

      // ürün adı a ile başlayanlar
      var products8 = _db.Products.Where(x => x.ProductName.StartsWith("a")).ToList();

      // ismi s ile biten ürünler
      var products9 = _db.Products.Where(x => EF.Functions.Like(x.ProductName, "s%")).ToList();
      var products10 = _db.Products.Where(x => x.ProductName.EndsWith("s")).ToList();

      // ürün id sine göre getirme Find ve FirstOrDefault kaydı bulamazsa null döndürür. Bulursa nesneyi attached eder. yani veri tabanına bağlı bir hale getirir.
      var product11 = _db.Products.Find(11);

      // firstorDefault ile getirme
      var products12 = _db.Products.FirstOrDefault(x => x.ProductId == 12);
      // tek bir kayı deöndürür yukarıdakilerden farklı olarak bulamaz ise hata döner.exception düşer. o yüzden kullanmıyoruz.

      // 1200 nolu kayıt yok burada sorgu patlayacaktır
      var products13 = _db.Products.SingleOrDefault(x => x.ProductId == 1200);

      // ilk 5 ürünü çekme // ürün fiyatına göre asc olarak sıralanmış ilk 5 adet ürün. Take ile çalırken öncesinde orderBy sorgusu atalım
      var products14 = _db.Products.OrderBy(x => x.UnitPrice).Take(5).ToList();

      // sayfalam işlemleri için
      // skip methodu ile kayıt atlatma işlemleri yani sqldeki offset işlemleri yaparız.
      var products15 = _db.Products.OrderBy(x => x.UnitPrice).Skip(2).Take(2).ToList();
     

      // iki farklı alana göre kayıtlarımızı sırlamak için thenBy methodunu kullanırız.
      // veri tabanında aynı isimde çalışan varsa soy isimlerine göre azalandan artana sıralama yapmak için kullanılabilir.
      var products16 = _db.Products.OrderBy(x => x.ProductName).ThenBy(x => x.UnitPrice).ToList();


      // veri tabanında kayıt var mı yok mu diye sorgulamak için any methodu kullanılır
      var products17 = _db.Products.Any(x => x.ProductName.Contains("a")); // name alanında a geçen bir kayıt var mı. Any sonuç olarak true yada false değer döndürür.
                                                                    
      
      // select ile bir tablodaki belirli alanları çekebiliriz.
      var products18 = _db.Products.Select(x => x.ProductName).ToList(); 
      // sadece name alanlarını çektik. 
      
      
      // bir tablodan birden fazla alanı çekmek istersek bu durumda ise new keyword ile anonim bir class içerisine alırız.
      var product19 = _db.Products.Select(x => new
      {
        Name = x.ProductName,
        Price = x.UnitPrice
      }).ToList();


      // select many ile ise bireçok ilişkili tablolarda koleksiyon içerisinde bir işlem yapabiliriz.

      // kategor'nin altındaki ürünlere bağlanıp fiyatı 50 tl üstünde olan ürünlerin filtrelenmesini sağlar.
      var products20 = _db.Categories
      .Include(x => x.Products)
      .SelectMany(x => x.Products)
      .Where(x => x.UnitPrice > 50)
      .ToList();

   
      // stoğuna göre ürünleri gruplama
      var products21 = _db.Products.GroupBy(x => x.UnitsInStock).Select(a => new
      {
        Count = a.Count(), // 2
        Name = a.Key // 32

      }).ToList();

      // Lambda join çok kullanmıyoruz. bunu yerine complex join işlemlerinde ya linq raw kullanıyoruz.
      var query = _db.Products
  .Join(
      _db.Categories,
      product => product.CategoryId,
      category => category.CategoryId,
      (category, product) => new
      {
        CategoryName = category.Category.CategoryName,
        ProductName = category.ProductName
      }
  ).ToList();

      // IncludeYöntemi
      var query2 = _db.Products.Include(x => x.Category).Select(a => new
      {
        CategoryName = a.Category.CategoryName,
        ProductName = a.ProductName
      }).ToList();

      // Linq ile sorgulama yöntemi
      var query3 = (from product in _db.Products
                    join category in _db.Categories on product.Category.CategoryId equals category.CategoryId
                    select new
                    {
                      CategoryName = category.CategoryName,
                      ProductName = product.ProductName
                    }).ToList();


      // sum, count, avarage, max, min aggregate functions

      // tüm ürünlerin birim fiyatlarının toplamı
      var totalUnitPrice = _db.Products.Sum(x => x.UnitPrice);

      // fiyatı 50 den büyük olanların sayısını getir
      var totalCount = _db.Products.Where(x => x.UnitPrice > 50).Count();

      // ortalama 1 ürüne ait birim maliyeti bulur
      var avgs = _db.Products.Average(x => x.UnitPrice);

      // stoktaki ürünlerin ortalama maliyeti
      var totalavgs = _db.Products.Average(x => x.UnitPrice * x.UnitsInStock);

      // ortalama ürün birim fiyatın üstündeki ürünlerin listesi ort ürün birim fiyat 30=> sql de subquery ile yaparız.
      var products23 = _db.Products.Where(x => x.UnitPrice > _db.Products.Average(x => x.UnitPrice)).ToList();

      // ürün giyatı maksimum fiyat olan ürünü getir.
      var products24 = _db.Products.FirstOrDefault(x => x.UnitPrice == _db.Products.Max(x => x.UnitPrice));
     

      // kategori getir fakat ürünleri fiyatına göre artandan azalana sıralı getir.
      var categories10 = _db.Categories.Include(x => x.Products).Select(x => new Category
      {
        CategoryId = x.CategoryId,
        CategoryName = x.CategoryName,
        Products = x.Products.OrderByDescending(y => y.UnitPrice).ToList()

      }).ToList();

     

      return View();
    }
  }
}
