using System;
using ClassScheduler.WPF.Models.Types;

namespace ClassScheduler.WPF.Models;

public class ClassModel
{
    public string? Name { get; set; }

    public TeacherModel? Teacher { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime? BeginTime { get; set; }

    public DateTime? EndTime { get; set; }

    public RepeatType RepeatType { get; set; } = RepeatType.NoRepeat;
}
