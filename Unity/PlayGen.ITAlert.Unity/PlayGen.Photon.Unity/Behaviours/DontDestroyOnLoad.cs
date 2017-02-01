using Photon;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void OnEnable()
		{
			DontDestroyOnLoad(this);
		}
	}
}
