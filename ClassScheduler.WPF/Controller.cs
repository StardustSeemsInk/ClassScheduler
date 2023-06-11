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

            new Function()
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
            },

            new Function()
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
            },

            new Function()
            {
                Name =  "GetSoundVolume",
                DisplayNames = new Dictionary<string, string>()
                {
                    { "zh-cn", "获取音量数据" },
                    { "en-us", "Get Sound Volume" }
                },
                HasAppendParameters = false,
                ReturnValueType = "double"
            },

            new Function()
            {
                Name = "GetMediaPath",
                DisplayNames = new Dictionary<string, string>()
                {
                    { "zh-cn", "获取当前播放的媒体文件的路径" },
                    { "en-us", "Get the Path of the Current Media File" }
                },
                HasAppendParameters = false,
                ReturnValueType = "string"
            },

        };
    }

    public void Execute(Command cmd)
    {

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
