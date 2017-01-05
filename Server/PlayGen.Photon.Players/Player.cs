namespace PlayGen.Photon.Players
{
    public class Player
    {
        public int PhotonId { get; set; }

        public int? ExternalId { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public PlayerStatus Status { get; set; }
    }
}
