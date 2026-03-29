public enum GamePhase { Day, Unease, Night, Recovery }
public enum EntityState { Idle, Observing, Hunting, Escalating, Retreating }
public enum ObjectType { Book, Fixture, Prop, Record, Furniture, Switch }
public enum ObjectCondition { Normal, Degraded, Failed, Corrupted }
public enum EntryType { AutoDiscovery, AutoEvent, ManualNote }
public enum EntryCategory { Observation, Pattern, Clue, Record, Personal }
public enum PuzzleType { BookArrangement, CrossReference, Pattern, Code, Environmental }
public enum Difficulty { Simple, Moderate, Complex }
public enum SlotType { Shelf, Table, Wall, Floor, FixtureMount }
public enum ZoneId { MainFloor, Office, Storage, Basement, Apartment, Street, Alley, Forest, Rail }

[System.Flags]
public enum LogCategory
{
    None        = 0,
    Cycle       = 1 << 0,
    Environment = 1 << 1,
    Entity      = 1 << 2,
    Notebook    = 1 << 3,
    Puzzle      = 1 << 4,
    Save        = 1 << 5,
    Ending      = 1 << 6,
    Narrative   = 1 << 7,
    Performance = 1 << 8,
    All         = ~0
}
