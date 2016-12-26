using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace Kagekuri
{
    public class MakeStageFileWizard : ScriptableWizard
    {
        public string FileName;
        public StageData StageData = new StageData();
        [MenuItem("Kagekuri/StageFile")]
        public static void Open()
        {
            DisplayWizard<MakeStageFileWizard>("ステージの作成", "Save", "Load");
        }

        public void OnWizardCreate()
        {
            var json = JsonConvert.SerializeObject(StageData, Formatting.Indented);
            if (!string.IsNullOrEmpty(FileName))
            {
                File.WriteAllText(Application.dataPath + @"\Resources\Stage\StageData\" + FileName + ".stage.txt", json, System.Text.Encoding.UTF8);

                AssetDatabase.Refresh();
            }
        }

        public void OnWizardOtherButton()
        {
            var asset = Resources.Load<TextAsset>("Stage/StageData/" + FileName + ".stage");
            if (asset != null)
                StageData = JsonConvert.DeserializeObject<StageData>(asset.text);
        }
    }
}