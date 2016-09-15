using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugBehaviour : MonoBehaviour
{
	[SerializeField]
	private Toggle _autoTickToggle;

	public bool AutoTickOn { get { return _autoTickToggle.isOn; } }

	[SerializeField]
	private Slider _autoTickInterval;

	[SerializeField]
	private Button _spawnVirusButton;

	private float _elapsedInterval;

	[SerializeField]
	private Text _fpsText;

	[SerializeField]
	private Text _tpsText;

	private float _lastTickTime;

	// Use this for initialization
	void Start () {
		
	}

	void Awake()
	{
		_spawnVirusButton.onClick.AddListener(SpawnVirus);
		_autoTickToggle.onValueChanged.AddListener(OnAutoTickToggled);
	}
	
	// Update is called once per frame
	void Update () {
		if (AutoTickOn)
		{
			DebugAutoTick();
		}
		_fpsText.text = (1 / Time.deltaTime).ToString("N1") + "FPS";
	}

	void FixedUpdate()
	{
	}

	private void OnAutoTickToggled(bool isOn)
	{
		if (isOn == false)
		{
			_elapsedInterval = 0;
		}
	}

	public void DebugAutoTick()
	{
		_elapsedInterval += Time.deltaTime;
		if (_elapsedInterval > (1 - _autoTickInterval.value))
		{
			DebugTick();
		}
	}

	public void DebugTick()
	{
		_elapsedInterval = 0;
		Director.Tick();
		_tpsText.text = (1 / (Time.time - _lastTickTime)).ToString("N1") + "TPS";
		_lastTickTime = Time.time;
	}

	private void SpawnVirus()
	{
		DebugCommands.SpawnVirus();
	}
}
