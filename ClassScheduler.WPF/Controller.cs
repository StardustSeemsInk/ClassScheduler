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

    //private void AnalyzeCmd(Command cmd, Function kxpfunc, int commandType, string methodName)
    //{

    //    Command callBackCommand = cmd;
    //    if (cmd.Request == kxpfunc.Name)
    //    {
    //        if (commandType == 2)
    //        {
    //            Type type = mainwin.GetType();
    //            Object obj = Activator.CreateInstance(type);
    //            object[] parameters = new object[] { cmd.RequestArgs };
    //            var method = type.GetMethod(methodName, new Type[] { });
    //            var a = method.Invoke(obj, parameters);
    //            callBackCommand.Type = CommandType.CallBack;
    //            callBackCommand.Sender = cmd.Target;
    //            callBackCommand.Target = cmd.Sender;
    //            callBackCommand.Body = (byte[])a;
    //        }
    //    }
    //}

    public void Execute(Command cmd)
    {
        //AnalyzeCmd(cmd, ssv, 0, "KXPChangeVolume") ;
        //AnalyzeCmd(cmd, cm, 0, "KXPChangeMedia") ;
        //AnalyzeCmd(cmd, gsv, 2, "KXPGetSoundVolume") ;
        //AnalyzeCmd(cmd, gmp, 2, "KXPGetMediaPath") ;
        if  (cmd.Request == ssv.Name)
        {
            double volume;
            double.TryParse(cmd.RequestArgs[0], out volume);
            mainwin.KXPChangeVolume(volume);
            return;
        };

        if (cmd.Request == cm.Name)
        {
            mainwin.KXPChangeMedia(cmd.RequestArgs[0]);
            return;
        };

        if (cmd.Request == gsv.Name)
        {
            double sv = mainwin.KXPGetSoundVolume();
            Command callBackCommand = cmd;
            callBackCommand.Type = CommandType.CallBack;
            callBackCommand.Sender = cmd.Target;
            callBackCommand.Target = cmd.Sender;
            callBackCommand.Body = BitConverter.GetBytes(sv);
            mainwin.sendCommandAction(callBackCommand);

        };

        if (cmd.Request == gmp.Name)
        {
            string? mp = mainwin.KXPGetMediaPath();
            Command callBackCommand = cmd;
            callBackCommand.Type = CommandType.CallBack;
            callBackCommand.Sender = cmd.Target;
            callBackCommand.Target = cmd.Sender;
            callBackCommand.Body = System.Text.Encoding.Default.GetBytes(mp);
            mainwin.sendCommandAction(callBackCommand);

        };

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
