using System;
using System.IO;
using ClassScheduler.WPF.Models.Types;

namespace ClassScheduler.WPF.Models;

public class ClassModel
{
    public string? Name { get; set; }

    public TeacherModel? Teacher { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime? BeginTime { get; set; }

    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 0b00000001 - 2 - Monday
    /// 0b00000010 - 4 - Tuesday
    /// 0b00000100 - 8 - Wendsday
    /// 0b00001000 - 16 - Thursday
    /// 0b00010000 - 32 - Friday
    /// 0b00100000 - 64 - Saturday
    /// 0b01000000 - 128 - Sunday
    /// </summary>
    public byte? WeekDay { get; set; }

    public int? DayOfWeek
    {
        get
        {
            var weekDay = (int)WeekDay!;
            var dividedCount = 0;
            while(weekDay != 1)
            {
                weekDay /= 2;
                ++dividedCount;
            }
            return dividedCount + 1;
        }
    }

    public RepeatType RepeatType { get; set; } = RepeatType.NoRepeat;

    public override string ToString()
    {
        var teacher = $"{(Teacher is null ? "" : $" by {Teacher.Name ?? "null"} ")}";
        var duration = $" from {BeginTime?.ToString("HH:mm")} to {EndTime?.ToString("HH:mm")}";
        return $"{Name}{teacher}{duration} on {DayOfWeek}";
    }
}
