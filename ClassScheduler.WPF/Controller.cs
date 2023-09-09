using ClassScheduler.WPF.Data;
using KitX.Contract.CSharp;
using KitX.Web.Rules;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ClassScheduler.WPF;

public class Controller : IController
{
    private readonly MainWindow mainwin;

    public Controller(MainWindow mainwin)
    {
        this.mainwin = mainwin;
    }

    public void Start()
    {
        mainwin.Show();
    }

    public void Pause()
    {
        mainwin.WinPause();
    }

    public void End()
    {
        mainwin.WinExit();
    }

    public List<Function> GetFunctions() => new();

    public void Execute(Command cmd)
    {

    }

    public void SetRootPath(string path)
    {
        GlobalData.RootPath = path;

        Events.Invoke(nameof(Events.OnSetRootPath));
    }

    public void SetSendCommandAction(Action<Command> action) => mainwin.sendCommandAction = action;

    public void SetWorkPath(string path)
    {
        GlobalData.WorkPath = path;
    }
}
