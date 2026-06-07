using UnityEngine;

namespace ShoelaceStudios.Utilities.DataStore.Examples
{
    public class GameDataManager : MonoBehaviour
    {
        private DataStore<EnemyData> enemyStore = new();
        private DataStore<DialogueLine> dialogueStore = new();
        private string currentLanguage = "en";


        private void Start()
        {
            LoadData();

            if (enemyStore.TryGet("goblin", out EnemyData goblin))
            {
                Debug.Log($"Found {goblin.Name}: {goblin.Health} HP, {goblin.Speed} speed");
            }

            DialogueLine greeting = GetDialogue("greeting");
            if (greeting != null)
            {
                Debug.Log($"{greeting.Speaker}: {greeting.Text}");
                foreach (string response in greeting.Responses)
                {
                    Debug.Log($"  > {response}");
                }
            }
        }


        private void LoadData()
        {
            // Load both data stores
            // Files must be at: Assets/Resources/Enemies.json and Assets/Resources/Dialogue.json
            enemyStore.LoadFromResources("enemies");
            dialogueStore.LoadFromResources("dialogue");
        }


        private DialogueLine GetDialogue(string id)
        {
            // Construct composite key using the same pattern as DialogueLine.Key
            string key = $"{id}_{currentLanguage}";

            if (dialogueStore.TryGet(key, out DialogueLine line))
            {
                return line;
            }

            return null;
        }
    }
}
