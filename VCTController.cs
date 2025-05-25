using Newtonsoft.Json.Linq;
using SoruxBot.SDK.Attribute;
using SoruxBot.SDK.Model.Attribute;
using SoruxBot.SDK.Model.Message;
using SoruxBot.SDK.Model.Message.Entity;
using SoruxBot.SDK.Plugins.Basic;
using SoruxBot.SDK.Plugins.Model;
using SoruxBot.SDK.Plugins.Service;
using SoruxBot.SDK.QQ;
using SoruxBot.SDK.QQ.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace SoruxBot.VCT.Plugins;
public class VCTController(ILoggerService loggerService, ICommonApi bot) : PluginController
{
    readonly string baseUrl = "https://vlrggapi.vercel.app";
    readonly List<string> regions = new List<string>() { "na", "eu", "ap", "la", "la-s", "la-n", "oce", "kr", "mn", "gc", "br", "cn", "jp", "col" };
    public static bool ContainsSubstring(string str, string substring)
    {
        return str.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    [MessageEvent(MessageType.PrivateMessage)]
    [Command(CommandPrefixType.Single, "vct-commands")]
    public PluginFlag VCTCommandsInPrivate(MessageContext ctx)
    {
        loggerService.Info("VCTCommandsInPrivate", $"now return commands");
        var msgBuilder = QqMessageBuilder.PrivateMessage(ctx.TriggerId);
        msgBuilder = msgBuilder.Text($"VCT插件所有指令如下\n")
                               .Text($"#vct-commands\n")
                               .Text($"返回所有指令\n")
                               .Text($"无参数\n")
                               .Text($"\n")
                               .Text($"#vct-regions\n")
                               .Text($"返回所有地区\n")
                               .Text($"无参数\n")
                               .Text($"\n")
                               .Text($"#vct-news [num]\n")
                               .Text($"返回比赛新闻\n")
                               .Text($"可选参数num（新闻条数）\n")
                               .Text($"\n")
                               .Text($"#vct-match <q> [mEvent]\n")
                               .Text($"返回比赛相关信息\n")
                               .Text($"必要参数q（upcoming未来比赛，live_score当前比赛，results过去比赛结果）\n")
                               .Text($"可选参数mEvent（赛事名）\n")
                               .Text($"\n")
                               .Text($"#vct-rankings <region>\n")
                               .Text($"返回队伍排名\n")
                               .Text($"必要参数region（地区简写，可从#vct-regions中获得）\n");
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }
    [MessageEvent(MessageType.GroupMessage)]
    [Command(CommandPrefixType.Single, "vct-commands")]
    public PluginFlag VCTCommandsInGroup(MessageContext ctx)
    {
        loggerService.Info("VCTCommandsInGroup", $"now return commands");
        var msgBuilder = QqMessageBuilder.GroupMessage(ctx.TriggerPlatformId);
        msgBuilder = msgBuilder.Text($"VCT插件所有指令如下\n")
                               .Text($"#vct-commands\n")
                               .Text($"返回所有指令\n")
                               .Text($"无参数\n")
                               .Text($"\n")
                               .Text($"#vct-regions\n")
                               .Text($"返回所有地区\n")
                               .Text($"无参数\n")
                               .Text($"\n")
                               .Text($"#vct-news [num]\n")
                               .Text($"返回比赛新闻\n")
                               .Text($"可选参数num（新闻条数）\n")
                               .Text($"\n")
                               .Text($"#vct-match <q> [mEvent]\n")
                               .Text($"返回比赛相关信息\n")
                               .Text($"必要参数q（upcoming未来比赛，live_score当前比赛，results过去比赛结果）\n")
                               .Text($"可选参数mEvent（赛事名）\n")
                               .Text($"\n")
                               .Text($"#vct-rankings <region>\n")
                               .Text($"返回队伍排名\n")
                               .Text($"必要参数region（地区简写，可从#vct-regions中获得）\n");
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.PrivateMessage)]
    [Command(CommandPrefixType.Single, "vct-news [num]")]
    public PluginFlag VCTNewsInPrivate(MessageContext ctx,string num)
    {
        int number;
        if(!int.TryParse(num, out number))
        {
            number = 30;
        }
        var url = baseUrl + "/news";
        loggerService.Info("VCTNewsInPrivate", $"now return vct-news");
        var msgBuilder = QqMessageBuilder.PrivateMessage(ctx.TriggerId);
        using HttpClient client = new HttpClient();
        string response =client.GetStringAsync(url).Result;
        JObject jsonObj = JObject.Parse(response);
        JArray segments = (JArray)jsonObj["data"]["segments"];
        for(int i = 0; i < number; i++)
        {
            string title = segments[i]["title"]?.ToString();
            string description = segments[i]["description"]?.ToString();
            string date = segments[i]["date"]?.ToString();
            string auther = segments[i]["auther"]?.ToString();
            string urlPath = segments[i]["url_path"]?.ToString();
            msgBuilder = msgBuilder.Text("第" + (i+1) + "条\n")
                                   .Text("标题："+title+"\n")
                                   .Text("简述："+description + "\n")
                                   .Text("日期："+date + "\n")
                                   .Text("作者："+auther + "\n")
                                   .Text("链接："+urlPath + "\n")
                                   .Text("\n");
        }
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.GroupMessage)]
    [Command(CommandPrefixType.Single, "vct-news [num]")]
    public PluginFlag VCTNewsInGroup(MessageContext ctx,string num)
    {
        int number;
        if (!int.TryParse(num, out number))
        {
            number = 30;
        }
        var url = baseUrl + "/news";
        loggerService.Info("VCTNewsInGroup", $"now return vct-news");
        var msgBuilder = QqMessageBuilder.GroupMessage(ctx.TriggerPlatformId);
        using HttpClient client = new HttpClient();
        string response = client.GetStringAsync(url).Result;
        JObject jsonObj = JObject.Parse(response);
        JArray segments = (JArray)jsonObj["data"]["segments"];
        msgBuilder = msgBuilder.Text($"瓦区新闻：\n");
        for (int i = 0; i < number; i++)
        {
            string title = segments[i]["title"]?.ToString();
            string description = segments[i]["description"]?.ToString();
            string date = segments[i]["date"]?.ToString();
            string auther = segments[i]["auther"]?.ToString();
            string urlPath = segments[i]["url_path"]?.ToString();
            msgBuilder = msgBuilder.Text("第" + (i+1) + "条\n")
                                   .Text("标题：" + title + "\n")
                                   .Text("简述：" + description + "\n")
                                   .Text("日期：" + date + "\n")
                                   .Text("作者：" + auther + "\n")
                                   .Text("链接：" + urlPath + "\n")
                                   .Text("\n");
        }
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.PrivateMessage)]
    [Command(CommandPrefixType.Single, "vct-match <q> [mEvent]")]
    public PluginFlag VCTMatchInPrivate(MessageContext ctx, string q ,string? mEvent)
    {
        bool eveFlag = true;
        if(mEvent is null)
        {
            eveFlag = false;
        }
        
        loggerService.Info("VCTNewsInPrivate", $"now return vct-match {q}");
        var msgBuilder = QqMessageBuilder.PrivateMessage(ctx.TriggerId);
        using HttpClient client = new HttpClient();
        switch (q)
        {
            case "upcoming":
                {
                    var url = baseUrl + "/match?q="+q;
                    string response = client.GetStringAsync(url).Result;
                    JObject jsonObj = JObject.Parse(response);
                    JArray segments = (JArray)jsonObj["data"]["segments"];

                    msgBuilder = msgBuilder.Text($"即将到来的比赛：\n");
                    foreach (var segment in segments)
                    {
                        string team1 = segment["team1"]?.ToString();
                        string team2 = segment["team2"]?.ToString();
                        string timeUntilMatch = segment["time_until_match"]?.ToString();
                        string matchSeries = segment["match_series"]?.ToString();
                        string matchEvent = segment["match_event"]?.ToString();
                        if ((eveFlag&&ContainsSubstring(matchEvent, mEvent))||!eveFlag)
                        {
                            msgBuilder = msgBuilder.Text($"比赛队伍：{team1} VS {team2} \n")
                                                   .Text($"距离现在时间：{timeUntilMatch} \n")
                                                   .Text($"比赛信息：{matchSeries}\n")
                                                   .Text($"赛段：{matchEvent}\n")
                                                   .Text("\n");
                        } 
                    }
                    break;
                }
            case "live_score":
                {
                    var url = baseUrl + "/match?q=" + q;
                    string response = client.GetStringAsync(url).Result;
                    JObject jsonObj = JObject.Parse(response);
                    JArray segments = (JArray)jsonObj["data"]["segments"];

                    msgBuilder = msgBuilder.Text($"正在进行的比赛：\n");
                    foreach (var segment in segments)
                    {
                        string team1 = segment["team1"]?.ToString();
                        string team2 = segment["team2"]?.ToString();
                        string score1 = segment["score1"]?.ToString();
                        string score2 = segment["score2"]?.ToString();
                        string team1RoundCt = segment["team1_round_ct"]?.ToString();
                        string team1RoundT  = segment["team1_round_t"]?.ToString();
                        string team2RoundCt = segment["team2_round_ct"]?.ToString();
                        string team2RoundT  = segment["team2_round_t"]?.ToString();
                        string mapNumber = segment["map_number"]?.ToString();
                        string currentMap= segment["current_map"]?.ToString();
                        string timeUntilMatch = segment["time_until_match"]?.ToString();
                        string matchSeries = segment["match_series"]?.ToString();
                        string matchEvent = segment["match_event"]?.ToString();
                        int team1win=0;
                        int team2win = 0;
                        int num = 0;
                        if(int.TryParse(team1RoundCt,out num))
                        {
                            team1win += num;
                        }
                        if (int.TryParse(team1RoundT, out num))
                        {
                            team1win += num;
                        }
                        if (int.TryParse(team2RoundCt, out num))
                        {
                            team2win += num;
                        }
                        if (int.TryParse(team2RoundT, out num))
                        {
                            team2win += num;
                        }
                        if ((eveFlag && ContainsSubstring(matchEvent, mEvent)) || !eveFlag)
                        {
                            msgBuilder = msgBuilder.Text($"比赛队伍：{team1} VS {team2} \n")
                                               .Text($"大比分： {score1} : {score2} \n")
                                               .Text($"比赛地图：图{mapNumber}——{currentMap} \n")
                                               .Text($"当前地图比分：{team1win} : {team2win} \n")
                                               .Text($"距离现在时间：{timeUntilMatch} \n")
                                               .Text($"比赛信息：{matchSeries}\n")
                                               .Text($"赛段：{matchEvent}\n")
                                               .Text("\n");
                        }
                    }
                    break;
                }
            case "results":
                {
                    var url = baseUrl + "/match?q=" + q;
                    string response = client.GetStringAsync(url).Result;
                    JObject jsonObj = JObject.Parse(response);
                    JArray segments = (JArray)jsonObj["data"]["segments"];

                    msgBuilder = msgBuilder.Text($"比赛结果：\n");
                    foreach (var segment in segments)
                    {
                        string team1 = segment["team1"]?.ToString();
                        string team2 = segment["team2"]?.ToString();
                        string score1 = segment["score1"]?.ToString();
                        string score2 = segment["score2"]?.ToString();
                        string timeCompleted = segment["time_completed"]?.ToString();
                        string matchSeries = segment["round_info"]?.ToString();
                        string matchEvent = segment["tournament_name"]?.ToString();
                        if ((eveFlag && ContainsSubstring(matchEvent, mEvent)) || !eveFlag)
                        {
                            msgBuilder = msgBuilder.Text($"比赛队伍：{team1} VS {team2} \n")
                                               .Text($"比分：{score1} : {score2} \n")
                                               .Text($"总时间：{timeCompleted} \n")
                                               .Text($"比赛信息：{matchSeries}\n")
                                               .Text($"赛段：{matchEvent}\n")
                                               .Text("\n");
                        }
                    }
                    break;
                }
            default:
                {
                    msgBuilder = msgBuilder.Text($"请输入正确指令 \n");
                    break;
                }
        }
        var msgChain = msgBuilder.Build();
        if (msgChain.Messages.Count == 0)
        {
            msgChain = QqMessageBuilder.PrivateMessage(ctx.TriggerId)
                                       .Text("未找到任何结果")
                                       .Build();   
        }
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.GroupMessage)]
    [Command(CommandPrefixType.Single, "vct-match <q> [mEvent]")]
    public PluginFlag VCTMatchInGroup(MessageContext ctx, string q,string mEvent)
    {
        bool eveFlag = true;
        if (mEvent is null)
        {
            eveFlag = false;
        }

        loggerService.Info("VCTNewsInGroup", $"now return vct-match {q}");
        var msgBuilder = QqMessageBuilder.GroupMessage(ctx.TriggerPlatformId);
        using HttpClient client = new HttpClient();

        switch (q)
        {
            case "upcoming":
                {
                    var url = baseUrl + "/match?q=" + q;
                    string response = client.GetStringAsync(url).Result;
                    JObject jsonObj = JObject.Parse(response);
                    JArray segments = (JArray)jsonObj["data"]["segments"];

                    msgBuilder = msgBuilder.Text($"即将到来的比赛：\n");
                    foreach (var segment in segments)
                    {
                        string team1 = segment["team1"]?.ToString();
                        string team2 = segment["team2"]?.ToString();
                        string timeUntilMatch = segment["time_until_match"]?.ToString();
                        string matchSeries = segment["match_series"]?.ToString();
                        string matchEvent = segment["match_event"]?.ToString();
                        if ((eveFlag && ContainsSubstring(matchEvent, mEvent)) || !eveFlag)
                        {
                            msgBuilder = msgBuilder.Text($"比赛队伍：{team1} VS {team2} \n")
                                               .Text($"距离现在时间：{timeUntilMatch} \n")
                                               .Text($"比赛信息：{matchSeries}\n")
                                               .Text($"赛段：{matchEvent}\n")
                                               .Text("\n");
                        }
                    }
                    break;
                }
            case "live_score":
                {
                    var url = baseUrl + "/match?q=" + q;
                    string response = client.GetStringAsync(url).Result;
                    JObject jsonObj = JObject.Parse(response);
                    JArray segments = (JArray)jsonObj["data"]["segments"];

                    msgBuilder = msgBuilder.Text($"正在进行的比赛：\n");
                    foreach (var segment in segments)
                    {
                        string team1 = segment["team1"]?.ToString();
                        string team2 = segment["team2"]?.ToString();
                        string score1 = segment["score1"]?.ToString();
                        string score2 = segment["score2"]?.ToString();
                        string team1RoundCt = segment["team1_round_ct"]?.ToString();
                        string team1RoundT = segment["team1_round_t"]?.ToString();
                        string team2RoundCt = segment["team2_round_ct"]?.ToString();
                        string team2RoundT = segment["team2_round_t"]?.ToString();
                        string mapNumber = segment["map_number"]?.ToString();
                        string currentMap = segment["current_map"]?.ToString();
                        string timeUntilMatch = segment["time_until_match"]?.ToString();
                        string matchSeries = segment["match_series"]?.ToString();
                        string matchEvent = segment["match_event"]?.ToString();
                        int team1win = 0;
                        int team2win = 0;
                        int num = 0;
                        if (int.TryParse(team1RoundCt, out num))
                        {
                            team1win += num;
                        }
                        if (int.TryParse(team1RoundT, out num))
                        {
                            team1win += num;
                        }
                        if (int.TryParse(team2RoundCt, out num))
                        {
                            team2win += num;
                        }
                        if (int.TryParse(team2RoundT, out num))
                        {
                            team2win += num;
                        }
                        if ((eveFlag && ContainsSubstring(matchEvent, mEvent)) || !eveFlag)
                        {
                            msgBuilder = msgBuilder.Text($"比赛队伍：{team1} VS {team2} \n")
                                               .Text($"大比分： {score1} : {score2} \n")
                                               .Text($"比赛地图：图{mapNumber}——{currentMap} \n")
                                               .Text($"当前地图比分：{team1win} : {team2win} \n")
                                               .Text($"距离现在时间：{timeUntilMatch} \n")
                                               .Text($"比赛信息：{matchSeries}\n")
                                               .Text($"赛段：{matchEvent}\n")
                                               .Text("\n");
                        }
                    }
                    break;
                }
            case "results":
                {
                    var url = baseUrl + "/match?q=" + q;
                    string response = client.GetStringAsync(url).Result;
                    JObject jsonObj = JObject.Parse(response);
                    JArray segments = (JArray)jsonObj["data"]["segments"];

                    msgBuilder = msgBuilder.Text($"比赛结果：\n");
                    foreach (var segment in segments)
                    {
                        string team1 = segment["team1"]?.ToString();
                        string team2 = segment["team2"]?.ToString();
                        string score1 = segment["score1"]?.ToString();
                        string score2 = segment["score2"]?.ToString();
                        string timeCompleted = segment["time_completed"]?.ToString();
                        string matchSeries = segment["round_info"]?.ToString();
                        string matchEvent = segment["tournament_name"]?.ToString();
                        if ((eveFlag && ContainsSubstring(matchEvent, mEvent)) || !eveFlag)
                        {
                            msgBuilder = msgBuilder.Text($"比赛队伍：{team1} VS {team2} \n")
                                               .Text($"大比分：{score1} : {score2} \n")
                                               .Text($"总时间：{timeCompleted} \n")
                                               .Text($"比赛信息：{matchSeries}\n")
                                               .Text($"赛段：{matchEvent}\n")
                                               .Text("\n");
                        }
                    }
                    break;
                }
            default:
                {
                    msgBuilder = msgBuilder.Text($"请输入正确指令 \n");
                    break;
                }
        }
        var msgChain = msgBuilder.Build();
        if (msgChain.Messages.Count == 0)
        {
            msgChain = QqMessageBuilder.PrivateMessage(ctx.TriggerId)
                                       .Text("未找到任何结果")
                                       .Build();
        }
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.PrivateMessage)]
    [Command(CommandPrefixType.Single, "vct-regions")]
    public PluginFlag VCTRegionsInPrivate(MessageContext ctx)
    {
        loggerService.Info("VCTRegionsInPrivate", $"now return regions");
        var msgBuilder = QqMessageBuilder.PrivateMessage(ctx.TriggerId);
        msgBuilder = msgBuilder.Text($"所有榜单及简称如下\n")
                               .Text($"北美：na\n")
                               .Text($"欧洲：eu\n")
                               .Text($"亚太：ap\n")
                               .Text($"拉丁美洲：la\n")
                               .Text($"拉丁美洲南：la-s\n")
                               .Text($"拉丁美洲北：la-n\n")
                               .Text($"大洋洲：oce\n")
                               .Text($"韩国：kr\n")
                               .Text($"中东北非：mn\n")
                               .Text($"改变者：gc\n")
                               .Text($"巴西：br\n")
                               .Text($"中国：cn\n")
                               .Text($"日本：jp\n")
                               .Text($"大学：col\n");
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.GroupMessage)]
    [Command(CommandPrefixType.Single, "vct-regions")]
    public PluginFlag VCTRegionsInGroup(MessageContext ctx)
    {
        loggerService.Info("VCTRegionsInGroup", $"now return regions");
        var msgBuilder = QqMessageBuilder.GroupMessage(ctx.TriggerPlatformId);
        msgBuilder = msgBuilder.Text($"所有榜单及简称如下\n")
                               .Text($"北美：na\n")
                               .Text($"欧洲：eu\n")
                               .Text($"亚太：ap\n")
                               .Text($"拉丁美洲：la\n")
                               .Text($"拉丁美洲南：la-s\n")
                               .Text($"拉丁美洲北：la-n\n")
                               .Text($"大洋洲：oce\n")
                               .Text($"韩国：kr\n")
                               .Text($"中东北非：mn\n")
                               .Text($"改变者：gc\n")
                               .Text($"巴西：br\n")
                               .Text($"中国：cn\n")
                               .Text($"日本：jp\n")
                               .Text($"大学：col\n");
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.PrivateMessage)]
    [Command(CommandPrefixType.Single, "vct-rankings <region>")]
    public PluginFlag VCTRankingsInPrivate(MessageContext ctx, string region)
    {
        var url = baseUrl + "/rankings??region=" + region;
        loggerService.Info("VCTRankingsInPrivate", $"Fetching rankings for region: {region}");
        var msgBuilder = QqMessageBuilder.PrivateMessage(ctx.TriggerId);        
        if(regions.Contains(region))
        {
            using HttpClient client = new HttpClient();
            string response = client.GetStringAsync(url).Result;
            JObject jsonObj = JObject.Parse(response);
            JArray rankings = (JArray)jsonObj["data"];

            if (rankings == null || rankings.Count == 0)
            {
                msgBuilder = msgBuilder.Text($"未找到 {region} 地区的排名数据。");
            }
            else
            {
                msgBuilder = msgBuilder.Text($"{region} 地区的排名：\n");
                for (int i = 0; i < 15; i++)
                {
                    var teamData = rankings[i];
                    string rank = teamData["rank"]?.ToString();
                    string team = teamData["team"]?.ToString();
                    string country = teamData["country"]?.ToString();
                    string lastPlayed = teamData["last_played"]?.ToString();
                    string lastPlayedTeam = teamData["last_played_team"]?.ToString();
                    string record = teamData["record"]?.ToString();
                    msgBuilder = msgBuilder.Text($"第 {rank} 名\n")
                                           .Text($"战队：{team} ({country})\n")
                                           .Text($"上场时间：{lastPlayed}\n")
                                           .Text($"上场对手：{lastPlayedTeam}\n")
                                           .Text($"战绩：{record}\n")
                                           .Text("\n");
                }
            }
        }
        else
        {
            msgBuilder = msgBuilder.Text($"未找到指定地区榜单\n");
        } 
        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

    [MessageEvent(MessageType.GroupMessage)]
    [Command(CommandPrefixType.Single, "vct-rankings <region>")]
    public PluginFlag VCTRankingsInGroup(MessageContext ctx, string region)
    {
        var url = baseUrl + "/rankings??region=" + region;
        loggerService.Info("VCTRankingsInGroup", $"Fetching rankings for region: {region}");
        var msgBuilder = QqMessageBuilder.GroupMessage(ctx.TriggerPlatformId);

        if (regions.Contains(region))
        {
            using HttpClient client = new HttpClient();
            string response = client.GetStringAsync(url).Result;
            JObject jsonObj = JObject.Parse(response);
            JArray rankings = (JArray)jsonObj["data"];

            if (rankings == null || rankings.Count == 0)
            {
                msgBuilder = msgBuilder.Text($"未找到 {region} 地区的排名数据。");
            }
            else
            {
                msgBuilder = msgBuilder.Text($"{region} 地区的排名：\n");
                for (int i = 0; i < 15; i++)
                {
                    var teamData = rankings[i];
                    string rank = teamData["rank"]?.ToString();
                    string team = teamData["team"]?.ToString();
                    string country = teamData["country"]?.ToString();
                    string lastPlayed = teamData["last_played"]?.ToString();
                    string lastPlayedTeam = teamData["last_played_team"]?.ToString();
                    string record = teamData["record"]?.ToString();
                    msgBuilder = msgBuilder.Text($"第 {rank} 名\n")
                                           .Text($"战队：{team} ({country})\n")
                                           .Text($"上场时间：{lastPlayed}\n")
                                           .Text($"上场对手：{lastPlayedTeam}\n")
                                           .Text($"战绩：{record}\n")
                                           .Text("\n");
                }
            }
        }
        else
        {
            msgBuilder = msgBuilder.Text($"未找到指定地区榜单\n");
        }

        var msgChain = msgBuilder.Build();
        MessageContext newctx = MessageContextHelper.WithNewMessageChain(ctx, msgChain);
        bot.SendMessage(newctx);
        return PluginFlag.MsgIntercepted;
    }

}


