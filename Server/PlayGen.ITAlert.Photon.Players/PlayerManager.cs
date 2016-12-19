using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Players
{
    public class PlayerManager
    {
        private readonly List<Player> _players = new List<Player>(10);

        public Player[] Players => _players.ToArray();

        public PlayerStatus CombinedPlayerStatus
        {
            get
            {
                if (_players.Any(p => p.Status == PlayerStatus.NotReady))
                {
                    return PlayerStatus.NotReady;
                }
                else if (_players.All(p => p.Status == PlayerStatus.Ready))
                {
                    return PlayerStatus.Ready;
                }
                else if (_players.All(p => p.Status == PlayerStatus.GameInitialized))
                {
                    return PlayerStatus.GameInitialized;
                }
                else if (_players.All(p => p.Status == PlayerStatus.GameFinalized))
                {
                    return PlayerStatus.GameFinalized;
                }

                return PlayerStatus.Error;
            }
        }

        public bool ChangeName(int id, string name)
        {
            var didChange = false;
            var player = _players.Single(p => p.PhotonId == id);

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
            var player = _players.Single(p => p.PhotonId == id);
            
            if (player.Color != color)
            {
                player.Color = color;
                didChange = true;
            }
            
            return didChange;
        }

        public bool ChangeStatus(int id, PlayerStatus status)
        {
            var didChange = false;
            var player = _players.Single(p => p.PhotonId == id);

            if (player.Status != status)
            {
                player.Status = status;
                didChange = true;
            }

            return didChange;
        }

        public bool ChangeExternalId(int id, int externalId)
        {
            var didChange = false;
            var player = _players.Single(p => p.PhotonId == id);

            if (player.ExternalId != externalId)
            {
                player.ExternalId= externalId;
                didChange = true;
            }

            return didChange;
        }

        public void ChangeAllStatuses(PlayerStatus status)
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
                if (_players[i].PhotonId == id)
                {
                    _players.RemoveAt(i);
                    break;
                }
            }
        }

        public Player Create(int photonId, int? externalId, string name, string color, PlayerStatus status)
        {
            var player = new Player
            {
                PhotonId = photonId,
                ExternalId = externalId,
                Name = name,
                Color = color,
                Status = status,
            };

            _players.Add(player);

            return player;
        }

        public Player Get(int id)
        {
            return _players.Single(p => p.PhotonId == id);
        }
    }
}
