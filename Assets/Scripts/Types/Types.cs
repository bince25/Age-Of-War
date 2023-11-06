using System;
using System.Collections.Generic;

public class GameStartBody
{
    public string UserId { get; set; }
}

public class GameEndBody
{
    public int GameSessionId { get; set; }
    public string TargetSide { get; set; }
}

public class CreateUnitBody
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
    public string UnitType { get; set; }
}

public class UpdateAgeBody
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
    public int NewAge { get; set; }
}

public class HandleAttackBody
{
    public string GameId { get; set; }
    public string AttackerId { get; set; }
    public string TargetUnitId { get; set; }
    public int Damage { get; set; }
}

public class WebSocketContext
{
    public string Id { get; set; }
    public object Data { get; set; } // You might want to specify a more specific type for Data, depending on how it's used.
}

public class Unit
{
    public string Id { get; set; }
    public string Type { get; set; }
    public int Health { get; set; }
    public string Side { get; set; }
    public string PlayerId { get; set; }
}

public class GameState
{
    public List<Unit> Units { get; set; }
    public List<string> Players { get; set; }
    public string GameId { get; set; }
    public DateTime StartTime { get; set; }
    public Dictionary<string, CastleController> Castles { get; set; }
}

public class Castle
{
    public int Health { get; set; }
    public string Type { get; set; } = "CASTLE";
    public string PlayerId { get; set; }
    public string Id { get; set; }
}

public class Building
{
    public string Id { get; set; }
    public int Level { get; set; }
    public string Type { get; set; }
    public string Side { get; set; }
}

public enum Side
{
    Left,
    Right
}
