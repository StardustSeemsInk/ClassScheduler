using ClassScheduler.WPF.Utils;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassScheduler.WPF.Data;

public class AppConfig
{
    public TopmostEffectsSettings TopmostEffectsSettings { get; set; } = new();

    public WallPaperSettings WallPaperSettings { get; set; } = new();

    public ApiSettings ApiSettings { get; set; } = new();

    public WindowsSettings WindowsSettings { get; set; } = new();
}

public class TopmostEffectsSettings
{
    public bool? IsDateTimeVisible { get; set; } = true;
}

public class WallPaperSettings
{
    public string? WallPapersPath { get; set; }

    public int CurrentWallPaperIndex { get; set; } = 0;

    public bool? WallPapersEnabled { get; set; } = false;

    public WallPaperStyle? WallPaperStyle { get; set; } = Utils.WallPaperStyle.Stretched;
}

public class ApiSettings
{
    public WeatherApiSettings WeatherApiSettings { get; set; } = new();
}

public class WeatherApiSettings
{
    public string? BaseUrl { get; set; } = "https://devapi.qweather.com/v7/weather/3d?location={0}&key={1}";

    public string? LocationId { get; set; }

    public string? Key { get; set; }

    [JsonIgnore]
    public string? FullUrl => BaseUrl is null
        ? null
        : string.Format(BaseUrl, LocationId, Key);
}

public class WindowsSettings
{
    public ScheduleWindowSettings ScheduleWindowSettings { get; set; } = new();
}

public class ScheduleWindowSettings
{
    private bool isHitTestVisible = true;

    public bool IsBottomMost { get; set; } = false;

    public bool IsHitTestVisible
    {
        get => isHitTestVisible;
        set
        {
            isHitTestVisible = value;

            if (Instances.ScheduleWindow is not null)
                Instances.ScheduleWindow.IsHitTestVisible = isHitTestVisible;
        }
    }
}

public static class AppConfigExtensions
{
    public static void Save(
        this AppConfig config,
        string path = "./Data/AppConfig.json",
        Action<JsonSerializerOptions>? optionsProcessor = null)
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            WriteIndented = true,
        };
        optionsProcessor?.Invoke(options);

        var json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(path, json);
    }

    public static AppConfig? LoadAsAppConfig(
        this string path,
        Action<JsonSerializerOptions>? optionsProcessor = null)
    {
        if (!Path.Exists(path)) return null;

        var options = new JsonSerializerOptions();
        optionsProcessor?.Invoke(options);

        var json = File.ReadAllText(path);
        var appConfig = JsonSerializer.Deserialize<AppConfig>(json, options);

        return appConfig;
    }
}
