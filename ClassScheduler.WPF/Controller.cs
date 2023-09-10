using ClassScheduler.WPF.Data;
using KitX.Contract.CSharp;
using KitX.Web.Rules;
using System;
using System.Collections.Generic;

namespace ClassScheduler.WPF;

public class Controller : IController
{
    private Action<Command>? sendCommandAction;

    public void Start()
    {
        Instances.MainWindow?.Show();
    }

    public void Pause()
    {
        Instances.MainWindow?.Pause();
    }

    public void End()
    {
        Instances.MainWindow?.Exit();
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

    public void SetSendCommandAction(Action<Command> action) => sendCommandAction = action;

    public void SetWorkPath(string path)
    {
        GlobalData.WorkPath = path;
    }
}
