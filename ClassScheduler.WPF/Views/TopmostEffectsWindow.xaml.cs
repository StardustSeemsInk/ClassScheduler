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
    private bool isDragingDateTime = false;

    private Point clickPosition_DateTime;

    private bool dateTimeSlideInAnimationCompletedRegistered = false;
    private bool dateTimeSlideOutAnimationCompletedRegistered = false;

    private FixedSizedQueue<Point> previousPoints_dateTime = new(6);

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
            ani!.Completed += (_, _) =>
                isPlayingDateTimeSlideAnimation = false;
            dateTimeSlideInAnimationCompletedRegistered = true;
        }

        if (!dateTimeSlideOutAnimationCompletedRegistered && !visible)
        {
            ani!.Completed += (_, _) =>
                isPlayingDateTimeSlideAnimation = false;
            dateTimeSlideOutAnimationCompletedRegistered = true;
        }

        Instances.AppConfig!.TopmostEffectsSettings.IsDateTimeVisible = visible;
        Instances.AppConfig!.Save();

        if (!isPlayingDateTimeSlideAnimation)
        {
            isPlayingDateTimeSlideAnimation = true;
            ani!.Begin();
        }
    }

    public void PlayPrepareClassAlert()
    {
        if (!isPlayingPrepareClassAlert)
        {
            IsHitTestVisible = false;
            (Resources["Storyboard_PrepareClassAlert"] as Storyboard)!.Begin();
        }
    }

    private void Container_DateTime_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Right)
        {
            SetDateTimeVisibility(false);

            NotifyIconManager.Rebuild();

            return;
        }

        Container_DateTime.CaptureMouse();

        isDragingDateTime = true;

        clickPosition_DateTime = e.GetPosition(Container_DateTime);

        var aniIn = Resources["Storyboard_DateTimeSlideIn"] as Storyboard;
        var aniOut = Resources["Storyboard_DateTimeSlideOut"] as Storyboard;

        aniIn?.Stop();
        aniOut?.Stop();
    }

    private void Container_DateTime_MouseUp(object sender, MouseButtonEventArgs e)
    {
        isDragingDateTime = false;

        Container_DateTime.ReleaseMouseCapture();

        BeginDecreaseSpeedMovement();
    }

    private void Container_DateTime_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragingDateTime && !isPlayingDateTimeSlideAnimation)
        {
            var currentPosition = e.GetPosition(Container_MainCanvas);

#if DEBUG   // Draw mouse movement line
            var ellipse = new Ellipse()
            {
                Width = 3,
                Height = 3,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Container_MainCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, currentPosition.X);
            Canvas.SetTop(ellipse, currentPosition.Y);
#endif

            var x = currentPosition.X - clickPosition_DateTime.X;
            var y = currentPosition.Y - clickPosition_DateTime.Y;

            var margin = 30;

            if (x + Container_DateTime.ActualWidth >= Width - margin)
                x = Width - Container_DateTime.ActualWidth - margin;

            if (x <= 0 + margin) x = 0 + margin;

            if (y + Container_DateTime.ActualHeight >= Height - margin)
                y = Height - Container_DateTime.ActualHeight - margin;

            if (y <= 0 + margin) y = 0 + margin;

            Canvas.SetLeft(Container_DateTime, x);
            Canvas.SetTop(Container_DateTime, y);

            previousPoints_dateTime.Enqueue(new(x, y));
        }
    }

    private void BeginDecreaseSpeedMovement()
    {
        if (previousPoints_dateTime.Count < 6)
            return;

        var cx = Canvas.GetLeft(Container_DateTime);
        var cy = Canvas.GetTop(Container_DateTime);

        var W = Width;
        var H = Height;
        var w = Container_DateTime.ActualWidth;
        var h = Container_DateTime.ActualHeight;

        var m = 30.0;

        var x = new double[6];
        var y = new double[6];

        var index = 0;

        while (index <= 5)
        {
            if (previousPoints_dateTime.TryDequeue(out var point))
            {
                x[index] = point.X;
                y[index] = point.Y;
            }
            ++index;
        }

        var vx = (x[5] - x[2] + x[4] - x[1] + x[3] - x[0]) / 3;
        var vy = (y[5] - y[2] + y[4] - y[1] + y[3] - y[0]) / 3;

        var af = 2.0;
        var afx = Math.Sqrt((af * af) / (Math.Pow(vy / vx, 2) + 1));
        var afy = afx * (vy / vx);

        var ttx = vx / afx;
        var tty = vy / afy;

        Task.Run(async () =>
        {
            var ct = 0;

            while (ct <= (ttx + tty) / 2)
            {
                ct += 1;
                vx -= afx;
                vy -= afy;

                cx += vx;
                cy += vy;

                Dispatcher.Invoke(new(() =>
                {
                    Canvas.SetLeft(Container_DateTime, cx);
                    Canvas.SetTop(Container_DateTime, cy);

                    if (cx + w >= W - m || cx <= m)
                    {
                        vx = -vx;
                        afx = -afx;
                    }

                    if (cy + h >= H - m || cy <= m)
                    {
                        vy = -vy;
                        afy = -afy;
                    }
                }));

                await Task.Delay(10);
            }

            Dispatcher.Invoke(new(() => SetDateTimeVisibility(true)));
        });
    }
}
