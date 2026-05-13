using System.Text.Json;
using ABLibrary.Interfaces;

namespace MyAbMobileApp.Services;

public class MobileFileStorage : ILocalStorage
{
    private readonly string _basePath;

    public MobileFileStorage()
    {
        _basePath = FileSystem.AppDataDirectory;
    }

    public void Save<T>(string key, T data)
    {
        var path = GetPath(key);

        var json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(path, json);
    }

    public T? Load<T>(string key)
    {
        var path = GetPath(key);

        if (!File.Exists(path))
            return default;

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<T>(json);
    }

    private string GetPath(string key)
    {
        return Path.Combine(_basePath, $"{key}.json");
    }
}