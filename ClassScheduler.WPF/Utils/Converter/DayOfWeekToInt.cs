using System;

namespace ClassScheduler.WPF.Utils.Converter;

public static class DayOfWeekToInt
{
    public static int ToInt(this DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Sunday => 7,
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 3,
            DayOfWeek.Thursday => 4,
            DayOfWeek.Friday => 5,
            DayOfWeek.Saturday => 6,
            _ => -1,
        };
    }
}
