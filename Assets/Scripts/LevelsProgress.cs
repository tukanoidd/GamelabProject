using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsProgress", menuName = "ScriptableObjects/LevelsProgress", order = 1)]
public class LevelsProgress : ScriptableObject
{
    public List<string> levelsUnlocked;
    public List<string> allLevels;

    public LevelsProgressData ToLevelsProgressData()
    {
        LevelsProgressData lProgData = new LevelsProgressData();
        
        lProgData.allLevels = allLevels;
        lProgData.levelsUnlocked = levelsUnlocked;

        return lProgData;
    }
}