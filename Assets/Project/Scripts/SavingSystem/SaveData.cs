using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<string> unlockedUpgradeIDs = new();
    public int savedBalance = 0;
}
