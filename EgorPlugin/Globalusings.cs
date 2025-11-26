global using MapEvents = Exiled.Events.Handlers.Map;
global using ItemEvents = Exiled.Events.Handlers.Item;
global using ServerEvents = Exiled.Events.Handlers.Server;
global using SchematicEvents = ProjectMER.Events.Handlers;
global using Player = Exiled.API.Features.Player;
global using Log = Exiled.API.Features.Log;
global using PlayerEvents = Exiled.Events.Handlers.Player;

namespace EgorPlugin;

class GlobalProjectFunctions()
{
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}