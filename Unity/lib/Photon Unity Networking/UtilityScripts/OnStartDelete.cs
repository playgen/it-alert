using UnityEngine;

/// <summary>This component will destroy the GameObject it is attached to (in Start()).</summary>
public class OnStartDelete : MonoBehaviour 
{
	// Use this for initialization
	private void Start()
    {
		Destroy(this.gameObject);
	}
}