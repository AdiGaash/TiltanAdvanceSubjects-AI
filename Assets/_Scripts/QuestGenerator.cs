using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QuestGenerator
{
    class EnemyOption { public string id; public string displayName; }
    class ItemOption { public string id; public string displayName; }

    [MenuItem("Tools/Quests/Generate Quests From JSON")]
    public static void GenerateQuests()
    {
        string path = EditorUtility.OpenFilePanel("Select Quest JSON", "", "json");
        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        List<QuestData> quests = JsonConvert.DeserializeObject<List<QuestData>>(json);

        // Load option data
        List<EnemyOption> enemies = LoadOptionList<EnemyOption>("Assets/Quests/enemies.json");
        List<ItemOption> items = LoadOptionList<ItemOption>("Assets/Quests/items.json");

        string savePath = "Assets/Quests/Generated/";
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        foreach (var data in quests)
        {
            QuestSO quest = null;

            if (data.goalType == "KillEnemies")
            {
                var killQuest = ScriptableObject.CreateInstance<KillQuestSO>();
                killQuest.amount = data.amount;
                killQuest.enemyType = ResolveDynamicField(data.enemyType, "@enemy", enemies);
                quest = killQuest;
            }
          

            if (quest == null)
            {
                Debug.LogWarning($"Unknown quest type: {data.goalType}");
                continue;
            }

            // Fill common fields
            quest.questId = data.questId;
            quest.title = data.title;
            quest.description = data.description;
            quest.goalType = (QuestGoalType)System.Enum.Parse(typeof(QuestGoalType), data.goalType);
            quest.rewardXP = data.rewardXP;
            quest.rewardItem = ResolveDynamicField(data.rewardItem, "@item", items);

            string fileName = $"{quest.questId}_{quest.title.Replace(" ", "_")}.asset";
            AssetDatabase.CreateAsset(quest, savePath + fileName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated {quests.Count} quest assets.");
    }

    private static string ResolveDynamicField<T>(string input, string token, List<T> options) where T : class
    {
        if (string.IsNullOrEmpty(input)) return "";
        if (!input.StartsWith(token)) return input;

        int index = Random.Range(0, options.Count);
        var option = options[index];
        var prop = option.GetType().GetProperty("id");
        return prop != null ? prop.GetValue(option).ToString() : "";
    }

    private static List<T> LoadOptionList<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Missing option file: {filePath}");
            return new List<T>();
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<T>>(json);
    }
}
