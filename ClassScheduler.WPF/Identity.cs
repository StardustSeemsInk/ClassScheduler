﻿using KitX.Contract.CSharp;
using System.Collections.Generic;
using System;

namespace ClassScheduler.WPF;

public class Identity : IIdentityInterface
{
    public Identity()
    {
        Instances.Init();
    }

    public string GetName() => "ClassScheduler";

    public string GetVersion() => "0.3.1";

    public Dictionary<string, string> GetDisplayName() => new()
    {
        { "zh-cn", "潦草的课表日程" },
        { "zh-cnt", "顯示名稱" },
        { "en-us", "ClassScheduler" },
        { "ja-jp", "番組名" }
    };

    public string GetAuthorName() => "StarInk";

    public string GetPublisherName() => "船出";

    public string GetAuthorLink() => "作者链接";

    public string GetPublisherLink() => "发行者链接";

    public Dictionary<string, string> GetSimpleDescription() => new()
    {
        { "zh-cn", "一个简单的桌面课表" },
        { "zh-cnt", "簡單描述" },
        { "en-us", "Simple Description" },
        { "ja-jp", "簡単な説明" }
    };

    public Dictionary<string, string> GetComplexDescription() => new()
    {
        { "zh-cn", "复杂描述" },
        { "zh-cnt", "複雜描述" },
        { "en-us", "Complex Description" },
        { "ja-jp", "複雑な説明" }
    };

    public Dictionary<string, string> GetTotalDescriptionInMarkdown() => new()
    {
        { "zh-cn", "完整描述" },
        { "zh-cnt", "完整描述" },
        { "en-us", "Total Description" },
        { "ja-jp", "完全な説明" }
    };

    public string GetIconInBase64() => "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";

    public DateTime GetPublishDate() => DateTime.Parse("2022-12-26");

    public DateTime GetLastUpdateDate() => DateTime.Parse("2023-07-16");

    public IController GetController() => Instances.Controller ??
        throw new NullReferenceException(
            $"`{nameof(Instances)}.{nameof(Controller)}` is null."
        );

    public bool IsMarketVersion() => false;

    public IMarketPluginContract GetMarketPluginContract() => null;

    public string GetRootStartupFileName() => "ClassScheduler.WPF.dll";
}
