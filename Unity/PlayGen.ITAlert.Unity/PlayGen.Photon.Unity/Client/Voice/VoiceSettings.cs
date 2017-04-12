namespace PlayGen.Photon.Unity.Client.Voice
{
	public class VoiceSettings
	{
		public static VoiceSettings Instance { get; } = new VoiceSettings();

		public bool Enabled { get; set; } = true;

		public string RecordDevice { get; set; }

		public float PlaybackLevel { get; set; } = 1.0f;
	}
}
