using System;

[Serializable]
public class SavedMonsterData
{
    public float currentHealthPoints;
    public float[] currentPosition;
}

[Serializable]
public class SavedLevelData
{
    public int currentLevelIndex;
    public SavedMonsterData[] monsters;
}

[Serializable]
public class SavedCharacterData
{
    public int chosenCharacterIndex;
    public float currentHealthPoints;
    public float currentMagicPoints;
    public float leftNormalAttackCD;
    public float leftSkillACD;
    public float leftSkillBCD;
    public float[] currentPositions;
}

[Serializable]
public class SavedRecord
{
    public long time;
    public SavedLevelData levelData;
    public SavedCharacterData characterData;
}
