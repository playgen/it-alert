using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Players.Extensions
{
    public class PlayerManager
    {
        private readonly List<Player> _players = new List<Player>(10);

        public Player[] Players => _players.ToArray();

        public PlayerStatuses CombinedPlayerStatuses
        {
            get
            {
                if (_players.Any(p => p.Status == PlayerStatuses.NotReady))
                {
                    return PlayerStatuses.NotReady;
                }
                else if (_players.All(p => p.Status == PlayerStatuses.Ready))
                {
                    return PlayerStatuses.Ready;
                }
                else if (_players.All(p => p.Status == PlayerStatuses.GameInitialized))
                {
                    return PlayerStatuses.GameInitialized;
                }
                else if (_players.All(p => p.Status == PlayerStatuses.GameFinalized))
                {
                    return PlayerStatuses.GameFinalized;
                }

                return PlayerStatuses.Error;
            }
        }

        public bool ChangeName(int id, string name)
        {
            var didChange = false;
            var player = _players.Single(p => p.Id == id);

            if (player.Name != name)
            {
                player.Name = name;
                didChange = true;
            }

            return didChange;
        }

        public bool ChangeColor(int id, string color)
        {
            var didChange = false;
            var player = _players.Single(p => p.Id == id);
            
            if (player.Color != color)
            {
                player.Color = color;
                didChange = true;
            }
            
            return didChange;
        }

        public bool ChangeStatus(int id, PlayerStatuses status)
        {
            var didChange = false;
            var player = _players.Single(p => p.Id == id);

            if (player.Status != status)
            {
                player.Status = status;
                didChange = true;
            }

            return didChange;
        }

        public void ChangeAllStatuses(PlayerStatuses status)
        {
            foreach (var player in _players)
            {
                player.Status = status;
            }
        }

        public void Remove(int id)
        {
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].Id == id)
                {
                    _players.RemoveAt(i);
                    break;
                }
            }
        }

        public Player Create(int playerId, string name, string color, PlayerStatuses status)
        {
            var player = new Player
            {
                Id = playerId,
                Name = name,
                Color = color,
                Status = status,
            };

            _players.Add(player);

            return player;
        }

        public Player Get(int id)
        {
            return _players.Single(p => p.Id == id);
        }
    }
}
