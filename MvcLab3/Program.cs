using Microsoft.EntityFrameworkCore;
using MvcLab3.Entities;
using MvcLab3.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); // HttpContext classlardan veya viewden eri�mek i�in bu servisi ekledik.

#region SessionSample
// session servis ekledik
builder.Services.AddSession();
#endregion

#region cacheSample

// StackExchange.Redis bu paket ile redis cache i�lemleri yap�l�yor.
// Buradaki cache y�ntemimiz bir redis sunucunda tutuldu�unda distributed cache olarak kullan�l�yor.
// session bir inmemory cache (sunucu �zerinde) yani �uan projede �al��an sunucu �zerinde tututur

// Not: Session State Sql session bilgilerini sql tarafta tutma ile ilgili herhangi bir �rnek olmayacak. Bu altyap�sal bir konu net core ile alakas� olmayan bir konu. 

// E�er session bilgilerini uygulama i�erisinde tutmayacaksak. Session kayb� ya�amak istemiyorsak yine redis ile session bilgilerini bir sunucu �zerinde tutabiliriz.Session kayb� ya�amak istemezsek redis e atarak da kullanabiliriz. 

// Redis ayr�ca Docker �zerinde Docker komutlar� ile �al��t�r�labiliyor.

IConfiguration configuration = builder.Configuration;
var multiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer); // connection y�neten service

// redis sunucuya ba�lanmak i�in kullan�yoruz.
// redise ba�lant� kurup redis cache �zerindeki methodlara eri�memiz sa�layan service
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

#endregion

#region DbScaffold Sample

// dbContext olarak tan�ma i�lemi
builder.Services.AddDbContext<NORTHWNDContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn")));

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

#region CookieSample
// cookie middleware ile cookie s�recini devreye soktuk
app.UseCookiePolicy();
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

#region SessionSample
// session middleware �a��rd�k
app.UseSession();

#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
