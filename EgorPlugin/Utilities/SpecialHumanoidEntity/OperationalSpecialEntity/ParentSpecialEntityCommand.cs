using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ParentSpecialEntityCommand : ParentCommand
{
    public ParentSpecialEntityCommand()
    {
        LoadGeneratedCommands();
    }
    public override string Command { get; } = "specialentity";
    public override string[] Aliases { get; } = ["sex"];
    public override string Description { get; } = "Гуманоидные угрожающие сущности";
    
    private readonly List<ICommand> _subCommands =
    [
        new RemoveSpecialEntityCommand(),
        new SetSpecialEntityCommand()
    ];
    public override void LoadGeneratedCommands()
    {
        foreach (var command in _subCommands)
        {
            RegisterCommand(command);
        }
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("rp.playerutils"))
        {
            response = "Нет прав.";
            return false;
        }
        
        StringBuilder sb = new($"<b>Суб-команды:</b>");
    
        foreach(var command in _subCommands)
        {
            sb.Append($"<i>specialentity {command.Command}</i> - {command.Description}\n");
        }
        response = sb.ToString();
        return true;
        
    }

    
    
}