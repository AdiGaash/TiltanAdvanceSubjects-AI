[System.Serializable]
public class QuestData
{
    public string questId;
    public string title;
    public string description;
    public string goalType; // "KillEnemies", "CollectItems", etc.
    public int rewardXP;
    public string rewardItem;

    // Optional fields depending on type
    public string enemyType;
    public string itemType;
    public int amount;
}