using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private BotControl _botPrefab;
    [SerializeField] private PlayerControl _playerPrefab;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] private GameBoardHelper _gameBoardHelper;
    [SerializeField] private RulesManager _rulesManager;

    public List<PlayerControlBase> SpawnPlayers(SpawnPlayerData[] playersData)
    {
        var players = new List<PlayerControlBase>();
        foreach (var playerData in playersData.OrderBy(x=>x.Index))
        {
            if (playerData.PlayerType == PlayerType.Bot)
            {
                var bot = Instantiate(_botPrefab);
                bot.Init(playerData.Color, _checkerBoard, _gameBoardHelper, _rulesManager);
                players.Add(bot);
            }
            else
            {
                var realPlayer = Instantiate(_playerPrefab);
                realPlayer.Init(playerData.Color);
                players.Add(realPlayer);
            }
        }

        return players;
    }
    
}