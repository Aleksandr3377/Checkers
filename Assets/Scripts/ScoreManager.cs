using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private List<GameColorToScoreView> _scoreViews;

    private void Awake()
    {
        ActivateCountScoresImages();
    }

    private void ActivateCountScoresImages()
    {
        foreach (var view in _scoreViews)
        {
            view.PlayerColorImage.gameObject.SetActive(true);
        }
    }

    private void DisplayScore(int number, GameColor playerColor)
    {
        var findPlayerScore = _scoreViews.FirstOrDefault(view => view.GameColor == playerColor);
        findPlayerScore.View.text = $"{number}";
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
    public TextMeshProUGUI View; 
    public Image PlayerColorImage;
}