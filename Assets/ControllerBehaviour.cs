using UnityEngine;
using System.Collections;

public class ControllerBehaviour : MonoBehaviour
{

	private StateController _stateController;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start ()
	{
	
	}
	
	void Update ()
	{
		_stateController.Tick(Time.deltaTime);
	}

}
