using System.Collections.Generic;

public static class Setting
{
    public static int playerPrefabId;
    public static bool isNewGame = false;
    public static bool isWarping = false;

    public static int enterPointId;
    public static int partyCount;

    public static HashSet<int> recruitedHeroPrefabIds = new HashSet<int>();
}