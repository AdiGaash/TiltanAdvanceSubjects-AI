using UnityEngine;

[CreateAssetMenu(fileName = "New Protect Quest", menuName = "Quests/Protect Quest")]
public class ProtectQuestSO : QuestSO
{
    [Header("Protect Target Settings")]
    public string enemyType;  // e.g., "OrcWarrior"
    public int numberToSurvive = 1;
    public float timeToDefend = 30f; // Time in seconds

    private void OnEnable()
    {
        goalType = QuestGoalType.ProtectTarget;
    }
}