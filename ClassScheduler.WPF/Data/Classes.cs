using ClassScheduler.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ClassScheduler.WPF.Data;

public class Classes
{
    public string? Name { get; set; }

    public List<ClassModel> ClassesList { get; set; } = new();

    public Classes Sort()
    {
        ClassesList.Sort(
            new Comparison<ClassModel>(
                (x, y) =>
                {
                    if (x.WeekDay == y.WeekDay)
                    {
                        if (x.BeginTime == y.BeginTime)
                            return 0;
                        else
                            return x.BeginTime > y.BeginTime ? 1 : -1;
                    }
                    else
                        return x.WeekDay > y.WeekDay ? 1 : -1;
                }
            )
        );

        return this;
    }
}

public static class ClassesExtensions
{
    public static void Save(
        this Classes classes,
        string path,
        Action<JsonSerializerOptions>? optionsProcessor = null)
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            WriteIndented = true,
        };
        optionsProcessor?.Invoke(options);

        var json = JsonSerializer.Serialize(classes);
        File.WriteAllText(path, json);
    }

    public static Classes? Load(
        this string path,
        Action<JsonSerializerOptions>? optionsProcessor = null)
    {
        if (!Path.Exists(path)) return null;

        var options = new JsonSerializerOptions();
        optionsProcessor?.Invoke(options);

        var json = File.ReadAllText(path);
        var classes = JsonSerializer.Deserialize<Classes>(json);

        return classes;
    }
}
