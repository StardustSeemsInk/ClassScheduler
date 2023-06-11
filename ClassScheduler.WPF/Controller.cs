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

    #region 声明函数
    private Function ssv = new()
    {
        Name = "SetSoundVolume",
        DisplayNames = new Dictionary<string, string>()
            {
                { "zh-cn", "设置音量" },
                { "en-us", "Set Sound Volume" }
            },
        Parameters = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "volume",
                    new Dictionary<string, string>()
                    {
                        { "zh-cn", "音量" },
                        { "en-us", "Volume" }
                    }
                }
            },
        ParametersType = new List<string>()
            {
                "double"
            },
        HasAppendParameters = false,
        ReturnValueType = "void"
    };

    private Function cm = new()
    {
        Name = "ChangeMedia",
        DisplayNames = new Dictionary<string, string>()
        {
            { "zh-cn", "更换播放内容" },
            { "en-us", "Change Media" }
        },
        Parameters = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                "path",
                new Dictionary<string, string>()
                {
                    { "zh-cn", "文件路径" },
                    { "en-us", "FilePath" }
                }
            }
        },
        ParametersType = new List<string>()
        {
            "string"
        },
        HasAppendParameters = false,
        ReturnValueType = "void"
    };

    private Function gsv = new()
    {
        Name =  "GetSoundVolume",
        DisplayNames = new Dictionary<string, string>()
        {
            { "zh-cn", "获取音量数据" },
            { "en-us", "Get Sound Volume" }
        },
        HasAppendParameters = false,
        ReturnValueType = "double"
    };

    private Function gmp = new()
    {
        Name = "GetMediaPath",
        DisplayNames = new Dictionary<string, string>()
        {
            { "zh-cn", "获取当前播放的媒体文件的路径" },
            { "en-us", "Get the Path of the Current Media File" }
        },
        HasAppendParameters = false,
        ReturnValueType = "string"
    };
    #endregion

    public List<Function> GetFunctions()
    {


        return new List<Function>()
        {
            new Function()
            {
                Name = "HelloWorld",
                DisplayNames = new Dictionary<string, string>()
                {
                    { "zh-cn", "你好, 世界!" },
                    { "en-us", "Hello, World!" }
                },
                Parameters = new Dictionary<string, Dictionary<string, string>>()
                {
                    {
                        "par1",
                        new Dictionary<string, string>()
                        {
                            { "zh-cn", "参数1" },
                            { "en-us", "Parameter1" }
                        }
                    }
                },
                ParametersType = new List<string>()
                {
                    "void"
                },
                HasAppendParameters = false,
                ReturnValueType = "void"
            },

            ssv,

            cm,

            gsv,

            gmp

        };
    }

    private void AnalyzeCmd(Command cmd, Function kxpfunc, Action action)
    {
        if (cmd.Request == kxpfunc.Name) action.Invoke();
    }

    public void Execute(Command cmd)
    {
        AnalyzeCmd(cmd, ssv, delegate () { mainwin.KXPChangeVolume(Convert.ToDouble(cmd.RequestArgs[0])); }) ;
        AnalyzeCmd(cmd, cm, delegate () { mainwin.KXPChangeMedia(cmd.RequestArgs[0]); }) ;
        AnalyzeCmd(cmd, gsv, delegate () { mainwin.KXPGetSoundVolume(); }) ;
        AnalyzeCmd(cmd, gmp, delegate () { mainwin.KXPGetMediaPath(); }) ;
    }

    public void SetRootPath(string path)
    {
        Global.RootPath = path;

        Events.Invoke(nameof(Events.OnSetRootPath));
    }

    public void SetSendCommandAction(Action<Command> action) => mainwin.sendCommandAction = action;

    public void SetWorkPath(string path)
    {
        MessageBox.Show($"SetWorkPath({path})");
    }

}
