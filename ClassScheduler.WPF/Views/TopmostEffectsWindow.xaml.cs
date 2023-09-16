using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    private bool isPlayingPrepareClassAlert = false;

    public TopmostEffectsWindow()
    {
        InitializeComponent();

        (Resources["Storyboard_PrepareClassAlert"] as Storyboard)!.Completed += (_, _) =>
        {
            isPlayingPrepareClassAlert = false;
        };
    }

    public void PlayPrepareClassAlert()
    {
        if (!isPlayingPrepareClassAlert)
        {
            (Resources["Storyboard_PrepareClassAlert"] as Storyboard)!.Begin();
        }
    }
}
