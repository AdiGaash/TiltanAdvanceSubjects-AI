using UnityEngine;



// Base runtime quest class renamed to RealtimeQuest
public abstract class ActiveQuest
{
    // Reference to the existing Quest ScriptableObject instance
    public QuestSO questSO;

    // Reference to the quest receiver (player, NPC, etc.)
    public GameObject questReceiver;

    public bool IsActive;

    // Abstract methods to implement quest behavior
    public abstract void StartQuest();
    public abstract void CancelQuest();
}

// Protect quest derived from RealtimeQuest
public class ProtectActiveQuest : ActiveQuest
{
    public string targetName;

    public override void StartQuest()
    {
        IsActive = true;
        Debug.Log($"Protect RealtimeQuest started: {questSO.title} for {questReceiver.name}, protect {targetName}");
        // Additional start logic here
    }

    public override void CancelQuest()
    {
        IsActive = false;
        Debug.Log($"Protect RealtimeQuest canceled: {questSO.title} for {questReceiver.name}");
        // Cleanup logic here
    }
}

// Sabotage quest derived from RealtimeQuest
public class SabotageActiveQuest : ActiveQuest
{
    public string targetName;

    public override void StartQuest()
    {
        IsActive = true;
        Debug.Log($"Sabotage RealtimeQuest started: {questSO.title} for {questReceiver.name}, sabotage {targetName}");
        // Additional start logic here
    }

    public override void CancelQuest()
    {
        IsActive = false;
        Debug.Log($"Sabotage RealtimeQuest canceled: {questSO.title} for {questReceiver.name}");
        // Cleanup logic here
    }
}