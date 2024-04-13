using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
   [SerializeField] private Button _playerVsPlayer;
   [SerializeField] private Button _playerVsBot;
   [SerializeField] private Button _botVsBot;

   private void Start()
   {
      _playerVsPlayer.onClick.AddListener(() =>
      {
         var players = new[]
         {
            new SpawnPlayerData()
            {
               Color = GameColor.White,
               PlayerType = PlayerType.RealPlayer,
               Index = 0
            },
            new SpawnPlayerData()
            {
               Color = GameColor.Red,
               PlayerType = PlayerType.RealPlayer,
               Index = 1
            }
         };
         LoadSceneWithPlayers(players);
      });
      
      _playerVsBot.onClick.AddListener(() =>
      {
         var players = new[]
         {
            new SpawnPlayerData()
            {
               Color = GameColor.White,
               PlayerType = PlayerType.RealPlayer,
               Index = 0
            },
            new SpawnPlayerData()
            {
               Color = GameColor.Red,
               PlayerType = PlayerType.Bot,
               Index = 1
            }
         };
         LoadSceneWithPlayers(players);
      });
      
      _botVsBot.onClick.AddListener(() =>
      {
         var players = new[]
         {
            new SpawnPlayerData()
            {
               Color = GameColor.White,
               PlayerType = PlayerType.Bot,
               Index = 0
            },
            new SpawnPlayerData()
            {
               Color = GameColor.Red,
               PlayerType = PlayerType.Bot,
               Index = 1
            }
         };
         LoadSceneWithPlayers(players);
      });
   }
   
   private void LoadSceneWithPlayers(SpawnPlayerData[] playersData)
   {
      GameGlobalData.SpawnPlayerData = playersData;
      SceneManager.LoadScene("Main");
   }
}
