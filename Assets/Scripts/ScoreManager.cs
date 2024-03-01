using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI firstPlayerScore;
    public TextMeshProUGUI secondPlayerScore;
    private readonly Dictionary<GameColor, TextMeshProUGUI> _playerScoreTexts = new();
    public Action<GameColor> PlayerHasWon;
    
   private void Start()
   {
       InitTextDictionaryValues();
       gameManager.PlayerBeatEnemyChecker += CountScore;
   }

   private void Update()
   {
       CheckIfPlayerHasBeatenAllCheckers();
   }

   private void CountScore(int number,GameColor playerColor)
    {
        if (_playerScoreTexts.TryGetValue(playerColor, out var playerScoreText))
        {
            playerScoreText.text = $"Score of {playerColor.ToString().ToLower()} player: {number}";
        }
    }
    
    private void InitTextDictionaryValues()
    {
        _playerScoreTexts.Add(GameColor.Red, firstPlayerScore);
        _playerScoreTexts.Add(GameColor.White, secondPlayerScore);
    }
    
    public readonly Dictionary<GameColor, int> PlayerScores = new()
    {
        { GameColor.White, 0},
        { GameColor.Red, 0}
    };

    private void CheckIfPlayerHasBeatenAllCheckers()
    {
        if (PlayerScores[GameColor.White] == 12 || PlayerScores[GameColor.Red] == 12)
        {
            PlayerHasWon?.Invoke(PlayerScores[GameColor.White] == 12 ? GameColor.White : GameColor.Red);
        }
    }
}
