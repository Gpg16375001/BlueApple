using UnityEngine;


namespace Live2D.Cubism.Framework.MouthMovement
{
	/// <summary>
	/// Real-time <see cref="CubismMouthController"/> input from <see cref="AudioSource"/>s.
	/// </summary>
	[RequireComponent(typeof(CubismMouthController))]
	public sealed class CubismAudioMouthInputFromCri : MonoBehaviour
	{
		/// <summary>
		/// The analyzer.
		/// </summary>
		[SerializeField]
		public CriAtomExPlayerOutputAnalyzer Analyzer;

		/// <summary>
		/// Last root mean square.
		/// </summary>
		private float LastRms { get; set; }

		/// <summary>
		/// Targeted <see cref="CubismMouthController"/>.
		/// </summary>
		private CubismMouthController Target { get; set; }


		/// <summary>
		/// True if instance is initialized.
		/// </summary>
		private bool IsInitialized
		{
			get { return Analyzer != null; }
		}


		/// <summary>
		/// Makes sure instance is initialized.
		/// </summary>
		private void TryInitialize()
		{
			// Return early if already initialized.
			if (IsInitialized)
			{
				return;
			}

			// Cache target.
			Target = GetComponent<CubismMouthController>();
		}

		#region Unity Event Handling

		/// <summary>
		/// Samples audio input and applies it to mouth controller.
		/// </summary>
		private void LateUpdate()
		{
			// 'Fail' silently.
			if (Analyzer == null)
			{
				return;
			}

			// Clamp root mean square.
			var value = Analyzer.GetRms(0) * 3f;    // 値が小さすぎるので微調整.
			var rms = Mathf.Clamp(value, 0.0f, 1.0f);

			// Set rms as mouth opening and store it for next evaluation.
			Target.MouthOpeningValue = rms;

			LastRms = rms;
		}


		/// <summary>
		/// Initializes instance.
		/// </summary>
		private void OnEnable()
		{
			TryInitialize();
		}

		#endregion
	}
}
