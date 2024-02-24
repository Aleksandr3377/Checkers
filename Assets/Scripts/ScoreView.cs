using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI firstPlayerScore;
    public TextMeshProUGUI secondPlayerScore;
   private void Start()
   {
       gameManager.PlayerBeatEnemyChecker += CountScore;
   }
    
    private void CountScore(int number,GameColor playerColor)
    {
        switch (playerColor)
        {
            case GameColor.Red:
                firstPlayerScore.text = $"Score of red player:{number}";
                break;
            case GameColor.White:
                secondPlayerScore.text = $"Score of white player:{number}";
                break;
        }
    }
}
