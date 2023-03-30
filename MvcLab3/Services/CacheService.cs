namespace MvcLab3.Services
{


  public interface ICacheService
  {
    // key göre redis cache üzerinden veri getirdik
    Task<string> GetValueAsync(string key);

    // key göre redis cachede string value tuttuk
    Task<bool> SetValueAsync(string key, string value);

    // cahceden serilize edilmiş obje tipinde veri getirdik.
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action) where T : class;
    T GetOrAdd<T>(string key, Func<T> action) where T : class;
    Task Clear(string key);
    void ClearAll();
  }
}
