using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LoadRecordPage : Page
{
    [SerializeField] private Transform recordRoot;
    [SerializeField] private GameObject recordInfoPrefab;

    private SavedRecord[] savedRecordCache;

    protected override void Awake()
    {
        this.OnOpen += this.UpdateContent;
    }

    private void UpdateContent()
    {
        this.savedRecordCache = RecordHelper.LoadAllRecord();
        this.savedRecordCache = this.savedRecordCache.OrderByDescending((savedRecord) => savedRecord.time).ToArray();

        // adjust display record info number
        if (this.recordRoot.childCount > this.savedRecordCache.Length)
        {
            int idx = 0;
            foreach (Transform child in this.recordRoot)
            {
                if (idx >= this.savedRecordCache.Length)
                {
                    this.recordRoot.GetChild(idx).gameObject.SetActive(false);
                }
                idx++;
            }
        }
        else if (this.recordRoot.childCount < this.savedRecordCache.Length)
        {
            int needCount = this.savedRecordCache.Length - this.recordRoot.childCount;
            for (int i = 0; i < needCount; i++)
            {
                Button button = Instantiate(this.recordInfoPrefab, this.recordRoot).GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    GameManager.Instance.ResumeSavedGame(this.savedRecordCache[button.transform.GetSiblingIndex()]);
                });
            }
        }

        // update record info content
        int index = 0;
        foreach (Transform child in this.recordRoot)
        {
            Text characterName = child.Find("ChosenCharacterName").GetComponent<Text>();
            Text stageName = child.Find("CurrentStage").GetComponent<Text>();
            Text time = child.Find("Time").GetComponent<Text>();
            SavedRecord savedRecord = this.savedRecordCache[index];
            child.gameObject.SetActive(true);
            characterName.text = savedRecord.characterData.chosenCharacterIndex == 0 ? "LUNA" : "LUCIFER";
            stageName.text = $"Stage {savedRecord.levelData.currentLevelIndex + 1}";
            time.text = DateTimeConverter.ToDateTime(savedRecord.time).ToString();
            index++;
        }
    }
}
