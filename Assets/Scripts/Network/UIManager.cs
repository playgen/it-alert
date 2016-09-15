using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649


// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class UIManager : MonoBehaviour
{
	#region editor properties

	/// <summary>
	/// upgrade counter text component
	/// </summary>
	[SerializeField]
	private Text _upgradeText;

	/// <summary>
	/// repair counter text component
	/// </summary>
	[SerializeField]
	private Text _repairText;

	/// <summary>
	/// score counter text component
	/// </summary>
	[SerializeField]
	private Text _scoreText;

	/// <summary>
	/// timer text component
	/// </summary>
	[SerializeField]
	private Text _timerText;

	/// <summary>
	/// warning text component
	/// </summary>
	[SerializeField]
	private Text _warningText;


	[SerializeField]
	private GameObject _hintObject;

	//[SerializeField]
	//private SVGImage _hintObjectIcon;

	#endregion

	//TODO: why are these coroutines?
	private static Coroutine _warningFlash;
	//private static Coroutine _hintMove;
	private static int _flashCount;

	private void Awake()
	{

	}
	
	private void FixedUpdate () {
		//if (_upgradeGen != null)
		//{
		//	//_upgradeText.text = _upgradeGen.Amount.ToString();
		//}
		//if (_repairGen != null)
		//{
		//	//_repairText.text = _repairGen.Amount.ToString();
		//}

		SetScore();
		SetTimer();
	}

	#region read game params

	private void SetScore()
	{
		_scoreText.text = Director.GetScore();
	}

	private void SetTimer()
	{
		_timerText.text = Director.GetTimer();
	}

	#endregion

	#region Warnings

	private IEnumerator StartWarning()
	{
		while (_flashCount > 0)
		{
			if (_flashCount % 2 == 0)
			{
				_warningText.color += new Color(0, 0, 0, Time.fixedDeltaTime*2);
			}
			else
			{
				_warningText.color -= new Color(0, 0, 0, Time.fixedDeltaTime*2);
			}
			if (_warningText.color.a < 0 || _warningText.color.a > 1)
			{
				_flashCount--;
			}
			yield return new WaitForFixedUpdate();
		}
		_warningText.color = new Color(_warningText.color.r, _warningText.color.g, _warningText.color.b, 0);
		_warningFlash = null;
	}


	public static void DisplayWarning (string warning)
	{
		//TODO: apparently this can't be static
		_flashCount += 2;
		//_warningText.text = warning;
		if (_warningFlash == null)
		{
			_flashCount += (int)((4 - _flashCount) / 2) * 2;
			//_warningFlash = StartCoroutine(StartWarning());
		}
	}

	//public void DisplayHint(SVGAsset image, string message)
	//{
	//	_hintObject.GetComponentInChildren<Text>().text = message.ToUpper();
	//	_hintObjectIcon.vectorGraphics = image;
	//	if (_hintMove == null)
	//	{
	//		_hintMove = StartCoroutine(MoveHint());
	//	}
	//}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private IEnumerator MoveHint()
	{
		Vector2 pos = _hintObject.GetComponent<RectTransform>().anchoredPosition;
		bool upward = true;
		while (pos.y >= -75)
		{
			if (upward)
			{
				pos += new Vector2(0, 2);
			} else
			{
				pos -= new Vector2(0, 2);
			}
			if (upward && pos.y >= 75)
			{
				upward = false;
				yield return new WaitForSeconds(3);
			}
			_hintObject.GetComponent<RectTransform>().anchoredPosition = pos;
			yield return new WaitForFixedUpdate();
		}
		_hintObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
		//_hintMove = null;
	}
	
	#endregion
}

