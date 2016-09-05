using UnityEngine;
using GameWork.States;

public class ControllerBehaviour : MonoBehaviour
{

	private StateController _stateController;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		_stateController = new StateController(  
			new LoginState(new LoginStateInterface()), 
			new LoadingState(new LoadingStateInterface()) 
			);
		_stateController.Initialize();
	}

	void Start ()
	{
		_stateController.SetState(LoadingState.StateName);
	}
	
	void Update ()
	{
		_stateController.Tick(Time.deltaTime);
	}

}
