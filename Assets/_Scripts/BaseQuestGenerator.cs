// Assets/Editor/QuestGenerator.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BaseQuestGenerator
{
    [MenuItem("Tools/Quests/Generate Quests From JSON")]
    public static void GenerateQuests()
    {
        string path = EditorUtility.OpenFilePanel("Select Quest JSON", "", "json");
        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        List<QuestData> quests = JsonConvert.DeserializeObject<List<QuestData>>(json);

        string savePath = "Assets/Quests/Generated/";
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        foreach (var data in quests)
        {
            QuestSO quest = null;

            if (data.goalType == "KillEnemies")
            {
                var killQuest = ScriptableObject.CreateInstance<KillQuestSO>();
                killQuest.enemyType = data.enemyType;
                killQuest.amount = data.amount;
                quest = killQuest;
            }
           
            else
            {
                Debug.LogWarning($"Unsupported quest type: {data.goalType}");
                continue;
            }

            // Fill base data
            quest.questId = data.questId;
            quest.title = data.title;
            quest.description = data.description;
            quest.goalType = (QuestGoalType)System.Enum.Parse(typeof(QuestGoalType), data.goalType);
            quest.rewardXP = data.rewardXP;
            quest.rewardItem = data.rewardItem;

            string fileName = $"{quest.questId}_{quest.title.Replace(" ", "_")}.asset";
            AssetDatabase.CreateAsset(quest, savePath + fileName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated {quests.Count} quest assets.");
    }
}