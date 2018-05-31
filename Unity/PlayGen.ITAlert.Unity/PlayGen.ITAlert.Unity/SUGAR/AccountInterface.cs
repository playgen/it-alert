using System;
using System.Linq;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

namespace PlayGen.ITAlert.Unity.Sugar
{
    using PlayGen.Unity.Utilities.Localization;

    public class AccountInterface : BaseAccountInterface
	{
	    private UILocalization[] _localizationComponents;

	    /// <summary>
		/// Trigger DoBestFit method and add event listener for when resolution changes to trigger DoBestFit.
		/// </summary>
		private void OnEnable()
		{
			DoBestFit();
			BestFit.ResolutionChange += DoBestFit;

		    _localizationComponents = GetComponentsInChildren<UILocalization>(true);
		    Array.ForEach(_localizationComponents, lc => lc.SetEvent += DoBestFit);
		}

		/// <summary>
		/// Remove event listener on disable.
		/// </summary>
		private void OnDisable()
		{
			BestFit.ResolutionChange -= DoBestFit;
            Array.ForEach(_localizationComponents, lc => lc.SetEvent -= DoBestFit);
        }

		/// <summary>
		/// Set the text of all the active buttons to be as big as possible and the same size.
		/// </summary>
		private void DoBestFit()
		{
			GetComponentsInChildren<Button>(true).Select(t => t.gameObject).Where(t => t.activeSelf).BestFit();
			GetComponentsInChildren<Text>().Where(t => !t.GetComponentsInParent<Button>(true).Any() && !t.GetComponentsInParent<InputField>(true).Any()).BestFit();
		}
	}
}