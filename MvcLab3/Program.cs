using Microsoft.EntityFrameworkCore;
using MvcLab3.Entities;
using MvcLab3.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); // HttpContext classlardan veya viewden eriþmek için bu servisi ekledik.

#region SessionSample
// session servis ekledik
builder.Services.AddSession();
#endregion

#region cacheSample

// StackExchange.Redis bu paket ile redis cache iþlemleri yapýlýyor.
// Buradaki cache yöntemimiz bir redis sunucunda tutulduðunda distributed cache olarak kullanýlýyor.
// session bir inmemory cache (sunucu üzerinde) yani þuan projede çalýþan sunucu üzerinde tututur

// Not: Session State Sql session bilgilerini sql tarafta tutma ile ilgili herhangi bir örnek olmayacak. Bu altyapýsal bir konu net core ile alakasý olmayan bir konu. 

// Eðer session bilgilerini uygulama içerisinde tutmayacaksak. Session kaybý yaþamak istemiyorsak yine redis ile session bilgilerini bir sunucu üzerinde tutabiliriz.Session kaybý yaþamak istemezsek redis e atarak da kullanabiliriz. 

// Redis ayrýca Docker üzerinde Docker komutlarý ile çalýþtýrýlabiliyor.

IConfiguration configuration = builder.Configuration;
var multiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer); // connection yöneten service

// redis sunucuya baðlanmak için kullanýyoruz.
// redise baðlantý kurup redis cache üzerindeki methodlara eriþmemiz saðlayan service
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

#endregion

#region DbScaffold Sample

// dbContext olarak tanýma iþlemi
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
// cookie middleware ile cookie sürecini devreye soktuk
app.UseCookiePolicy();
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

#region SessionSample
// session middleware çaðýrdýk
app.UseSession();

#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
