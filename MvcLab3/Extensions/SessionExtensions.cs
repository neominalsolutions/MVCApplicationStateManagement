using System.Text.Json;

// MVC Newton.Soft.JSON önermiyoruz. Bu lightweight bir kğütüphane değil.
// Net Core API System.Text.Json desteği ile gelidi.

namespace MvcLab3.Extensions
{
  public static class SessionExtensions
  {
    public static T GetObject<T>(this ISession session, string key)
    {
      // jsonString olarak nesneyi string json formatında tutmak 
      string jsonString = session.GetString(key);

      if (string.IsNullOrEmpty(jsonString))
      {
        throw new Exception("Session Bulunamadı");
      }

      // bunu T tipindeki bir nesneye Deserialize etmek
      var result = JsonSerializer.Deserialize<T>(jsonString);

      return result;
    }

    // string key verilen bir objeyi jsonString formatında sessionda tutmamızı sağlar.
    public static void SetObject<T>(this ISession session, string key, T value)
    {
     
      string jsonString = JsonSerializer.Serialize<T>(value);

      session.SetString(key, jsonString);
    }
  }


}
