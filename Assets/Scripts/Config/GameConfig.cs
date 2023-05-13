public class GameConfig
{
    // Grid options
    public static readonly int Rows = 11;
    public static readonly int Cols = 7;
    public static readonly int MinSequenceLength = 3;
    public static readonly int MaxSequenceLength = 5;
    public static readonly float TileSpacing = 0.6f;
    public static readonly int MaxColorsCount = 5;
    public static readonly int BombAreaCoverage = 5;
    // Time variables
    public static readonly float AnimationDuration = 0.33f;
    public static readonly float GridCheckDelay = 0.033f;
    public static readonly float GridCheckDoneDelay = 0.033f;
    // Debug options
    public static readonly bool IsDebug = false;
    public static readonly int HistorySize = 100;
    public static readonly int RandomSeed = 42;
}