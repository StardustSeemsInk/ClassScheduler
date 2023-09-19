using ClassScheduler.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ClassScheduler.WPF.Data;

public class Classes
{
    public string? Name { get; set; }

    public List<ClassModel> ClassesList { get; set; } = new()
    {
        new()
        {
            Name = "name",
            BeginTime = DateTime.Now,
            EndTime = DateTime.Now + new TimeSpan(1,0,0),
            WeekDay = 1,
        }
    };

    public Classes Sort()
    {
        ClassesList.Sort(
            (x, y) =>
            {
                if (x.WeekDay == y.WeekDay)
                {
                    var x_begin = DateTime.Parse(x.BeginTime?.ToString("HH:mm")!);
                    var y_begin = DateTime.Parse(y.BeginTime?.ToString("HH:mm")!);
                    var x_end = DateTime.Parse(x.EndTime?.ToString("HH:mm")!);
                    var y_end = DateTime.Parse(y.EndTime?.ToString("HH:mm")!);

                    if (x_begin == y_begin && x_end == y_end)
                        return x.Name!.CompareTo(y.Name);
                    else return x_begin.CompareTo(y_end);
                }
                else return x.WeekDay > y.WeekDay ? 1 : -1;
            }
        );

        return this;
    }
}

public static class ClassesExtensions
{
    public static void Save(
        this Classes classes,
        string path = "./Data/Classes.json",
        Action<JsonSerializerOptions>? optionsProcessor = null)
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            WriteIndented = true,
        };
        optionsProcessor?.Invoke(options);

        var json = JsonSerializer.Serialize(classes, options);
        File.WriteAllText(path, json);
    }

    public static Classes? LoadAsClasses(
        this string path,
        Action<JsonSerializerOptions>? optionsProcessor = null)
    {
        if (!Path.Exists(path)) return null;

        var options = new JsonSerializerOptions();
        optionsProcessor?.Invoke(options);

        var json = File.ReadAllText(path);
        var classes = JsonSerializer.Deserialize<Classes>(json, options);

        return classes;
    }
}
