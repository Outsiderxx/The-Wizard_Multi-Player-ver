using UnityEngine;
using UnityEngine.UI;

public class ResultPage : Page
{
    [SerializeField] private Text resultText;

    public void ShowResult(bool isWin)
    {
        this.Open();
        this.resultText.text = isWin ? "YOU WIN" : "YOU DEAD";
        this.resultText.color = isWin ? new Color(1, 0.5f, 0) : Color.red;
    }

    public void BackToMainScene()
    {
        GameManager.Instance.BackToMainScene();
    }
}
