using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private List<GameColorToScoreView> _scoreViews;
    
    private void DisplayScore(int number, GameColor playerColor)
    {
        var findPlayerScore = _scoreViews.FirstOrDefault(x => x.GameColor == playerColor);
        findPlayerScore.ScoreView.View.text = $"{number}";
    }
    
    public readonly Dictionary<GameColor, int> PlayerScores = new()
    {
        { GameColor.White, 0 },
        { GameColor.Red, 0 }
    };
    
    public void AddScore(GameColor playerColor, int score)
    {
        var newScore = PlayerScores[playerColor] + score;
        PlayerScores[playerColor] = newScore;
        DisplayScore(newScore, playerColor);
    }
}

[Serializable]
public struct GameColorToScoreView
{
    public GameColor GameColor;
    public ScoreView ScoreView;
}