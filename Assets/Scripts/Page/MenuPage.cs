using UnityEngine;

public class MenuPage : Page
{
    [SerializeField] private LoadRecordPage loadRecordPage;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (this.IsOpen)
            {
                this.Close();
                this.loadRecordPage.Close();
            }
            else
            {
                this.Open();
            }
        }
    }

    public void SaveRecord()
    {
        RecordHelper.SaveRecord(GameObject.Find("LevelManager").GetComponent<LevelManager>().GetCurrentState());
        this.Close();
    }

    public void BackToMainScene()
    {
        GameManager.Instance.BackToMainScene();
    }
}
