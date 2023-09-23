using System;

namespace ClassScheduler.WPF.Utils.Converter;

public static class DayOfWeekToInt
{
    public static int ToInt(this DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Sunday:
                return 7;
            case DayOfWeek.Monday:
                return 1;
            case DayOfWeek.Tuesday:
                return 2;
            case DayOfWeek.Wednesday:
                return 3;
            case DayOfWeek.Thursday:
                return 4;
            case DayOfWeek.Friday:
                return 5;
            case DayOfWeek.Saturday:
                return 6;
        }

        return -1;
    }
}
