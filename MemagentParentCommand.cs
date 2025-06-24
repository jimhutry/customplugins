using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommandSystem;
using Exiled.Permissions.Extensions;
using MapEditorReborn.Commands.UtilityCommands;
using Mirror;

namespace EgorPlugin;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MemagentParentCommand : ParentCommand
{
    public new List<ICommand> Commands { get; } = [new MemagentStartAttack(), new MemagentStopAttack()];
    
    public MemagentParentCommand()
    {
        LoadGeneratedCommands();
    }
    public override void LoadGeneratedCommands()
    {
        foreach (ICommand command in Commands)
        {
            RegisterCommand(command);
        }
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
        [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("mem.attack"))
        {
            response = "Недостаточно прав.";
            return false;
        }

        var b = new StringBuilder("Введите суб-команду. Доступные суб-команды:\n");
        foreach (ICommand command in Commands)
        {
            b.Append(command.Command + "\n");
        }
        response = b.ToString();
        return true;
    }

    public override string Command { get; } = "memagent";
    public override string[] Aliases { get; } = ["mem"];
    public override string Description { get; } = "Родительская команда для использования меметических атак."; 
}
