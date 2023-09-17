using ClassScheduler.WPF.Data;
using ClassScheduler.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClassScheduler.WPF.Views;

public partial class TopmostEffectsWindow : Window
{
    private Timer? dateTimeTimer;

    private bool isPlayingPrepareClassAlert = false;
    private bool isPlayingDateTimeSlideAnimation = false;

    private bool dateTimeSlideInAnimationCompletedRegistered = false;
    private bool dateTimeSlideOutAnimationCompletedRegistered = false;

    public TopmostEffectsWindow()
    {
        InitializeComponent();

        (Resources["Storyboard_PrepareClassAlert"] as Storyboard)!.Completed += (_, _) =>
        {
            isPlayingPrepareClassAlert = false;

            IsHitTestVisible = true;
        };

        Loaded += TopmostEffectsWindow_Loaded;
    }

    private void TopmostEffectsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        BeginRefreshingDateTime();

        Task.Run(async () =>
        {
            await Task.Delay(1000);

            Dispatcher.Invoke(new(() =>
            {
                SetDateTimeVisibility(
                    Instances.AppConfig!.TopmostEffectsSettings.IsDateTimeVisible ?? false
                );
            }));
        });
    }

    private void BeginRefreshingDateTime()
    {
        dateTimeTimer = new()
        {
            Interval = 1 * (1000 / 10)
        };

        dateTimeTimer.Elapsed += (_, _) =>
        {
            Dispatcher.Invoke(new(() =>
            {
                TextBlock_DateTime.Text = DateTime.Now.ToString("HH:mm:ss");
            }));
        };

        dateTimeTimer.Start();
    }

    internal void SetDateTimeVisibility(bool visible)
    {
        var aniName = $"Storyboard_DateTimeSlide{(visible ? "In" : "Out")}";
        var ani = Resources[aniName] as Storyboard;

        if (!dateTimeSlideInAnimationCompletedRegistered && visible)
        {
            ani!.Completed += (_, _) => isPlayingDateTimeSlideAnimation = false;
            dateTimeSlideInAnimationCompletedRegistered = true;
        }

        if (!dateTimeSlideOutAnimationCompletedRegistered && !visible)
        {
            ani!.Completed += (_, _) => isPlayingDateTimeSlideAnimation = false;
            dateTimeSlideOutAnimationCompletedRegistered = true;
        }

        Instances.AppConfig!.TopmostEffectsSettings.IsDateTimeVisible = visible;
        Instances.AppConfig!.Save();

        if (!isPlayingDateTimeSlideAnimation) ani!.Begin();
    }

    public void PlayPrepareClassAlert()
    {
        if (!isPlayingPrepareClassAlert)
        {
            IsHitTestVisible = false;
            (Resources["Storyboard_PrepareClassAlert"] as Storyboard)!.Begin();
        }
    }

    private void Container_DateTime_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        SetDateTimeVisibility(false);

        NotifyIconManager.Rebuild();
    }
}
