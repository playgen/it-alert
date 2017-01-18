using System.Collections.Generic;
using System.Linq;

namespace PlayGen.Photon.Players
{
    public class PlayerManager
    {
        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>(10);

        public List<Player> Players => _players.Values.ToList();
        public List<int> PlayersPhotonIds => _players.Values.Select(p => p.PhotonId).ToList();

        public void UpdatePlayer(Player player)
        {
            _players[player.PhotonId] = player;
        }

        public void ChangeAllState(int state)
        {
            foreach (var player in _players.Values)
            {
                player.State = state;
            }
        }

        public void Remove(int photonId)
        {
            _players.Remove(photonId);
        }

        public Player Create(int photonId, int? externalId, string name, string color, int state)
        {
            var player = new Player
            {
                PhotonId = photonId,
                ExternalId = externalId,
                Name = name,
                Color = color,
                State = state,
            };

            _players[photonId] = player;

            return player;
        }

        public Player Get(int photonId)
        {
            return _players[photonId];
        }
    }
}
