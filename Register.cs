using SoruxBot.SDK.Plugins.Ability;
using SoruxBot.SDK.Plugins.Basic;

namespace SoruxBot.VCT.Plugins;

/// <summary>
/// 插件注册
/// </summary>
public class Register : SoruxBotPlugin, ICommandPrefix
{
    public override string GetPluginName() => "VCT Plugin";

    public override string GetPluginVersion() => "1.0.0";

    public override string GetPluginAuthorName() => "Open SoruxBot Project";

    public override string GetPluginDescription() => "这是一个VCT插件";
    
    public string GetPluginPrefix() => "#";
}