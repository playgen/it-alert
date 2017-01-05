namespace PlayGen.Photon.Players
{
    public class Player
    {
        public int PhotonId { get; set; }

        public int? ExternalId { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

		// todo set status as int and use same pattern as 
		// channels to modiy by defining an enum in the consuming, game specific project.
        public PlayerStatus Status { get; set; }
    }
}
