using System.Collections.Generic;
using System.Linq;

namespace PlayGen.Photon.Players
{
	public class PlayerManager
	{
		private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>(10);

		public List<Player> Players => _players.Values.ToList();
		public List<int> PlayersPhotonIds => _players.Values.Select(p => p.PhotonId).ToList();

		public PlayerStatus CombinedPlayerStatus
		{
			get
			{
				if (_players.Any(p => p.Value.Status == PlayerStatus.NotReady))
				{
					return PlayerStatus.NotReady;
				}
				else if (_players.All(p => p.Value.Status == PlayerStatus.Ready))
				{
					return PlayerStatus.Ready;
				}
				else if (_players.All(p => p.Value.Status == PlayerStatus.Initialized))
				{
					return PlayerStatus.Initialized;
				}
				else if (_players.All(p => p.Value.Status == PlayerStatus.Finalized))
				{
					return PlayerStatus.Finalized;
				}

				return PlayerStatus.Error;
			}
		}

		public void UpdatePlayer(Player player)
		{
			_players[player.PhotonId] = player;
		}

		public void ChangeAllStatuses(PlayerStatus status)
		{
			foreach (var player in _players.Values)
			{
				player.Status = status;
			}
		}

		public void Remove(int photonId)
		{
			_players.Remove(photonId);
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

			_players[photonId] = player;

			return player;
		}

		public Player Get(int photonId)
		{
			return _players[photonId];
		}
	}
}
