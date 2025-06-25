using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using Mirror;
using Utils;

namespace EgorPlugin;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BreakDoorsCommand : ICommand, IUsageProvider
{
    public string Command { get; } = "breakdoors";
    public string[] Aliases { get; } = ["bd"];
    public string Description { get; } = "Даёт игроку возможность ломать все двери. (Полний разьиеб)";
    public string[] Usage { get; } = ["player"];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 1)
        {
            BreakDoorsFeature.GiveStatus(Player.Get(sender));
            response = "Успешно";
            return true;
        }
        var referenceHubList = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _);
        var players = referenceHubList.Select(Player.Get).ToArray();
        foreach (var player in players)
        {
            BreakDoorsFeature.GiveStatus(player);
        }
        
        response = "Успешно";
        return true;
    }
}