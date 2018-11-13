/*
 * Copyright(c) Live2D Inc. All rights reserved.
 * 
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at http://live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using Live2D.Cubism.Core;
using UnityEngine;


namespace Live2D.Cubism.Framework.MouthMovement
{
    /// <summary>
    /// Controls <see cref="CubismMouthParameter"/>s.
    /// </summary>
    public sealed class CubismMouthController : MonoBehaviour
    {
        /// <summary>
        /// The blend mode.
        /// </summary>
        [SerializeField]
        public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Multiply;


        // 2018/2/8 public変数MouthOpeningに値を直接代入すると原因不明の外的要因(Animationなど)で値が上書きされることがあるためスクリプト上から操作する用のpropatyを設置 yoshida.
        public float MouthOpeningValue
        {
            get {
                return MouthOpening;
            }
            set {
                if (Destinations == null) {
                    return;
                }
                Destinations.BlendToValue(BlendMode, value);
                MouthOpening = value;
            }
        }
        /// <summary>
        /// The opening of the eyes.
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        public float MouthOpening = 1f;


        /// <summary>
        /// Eye blink parameters.
        /// </summary>
        private CubismParameter[] Destinations { get; set; }


        /// <summary>
        /// Refreshes controller. Call this method after adding and/or removing <see cref="CubismEyeBlinkParameter"/>s.
        /// </summary>
        public void Refresh()
        {
            var model = this.FindCubismModel();


            // Fail silently...
            if (model == null) {
                return;
            }


            // Cache destinations.
            var tags = model
                .Parameters
                .GetComponentsMany<CubismMouthParameter>();


            Destinations = new CubismParameter[tags.Length];


            for (var i = 0; i < tags.Length; ++i) {
                Destinations[i] = tags[i].GetComponent<CubismParameter>();
            }
        }

        #region Unity Events Handling

        /// <summary>
        /// Called by Unity. Makes sure cache is initialized.
        /// </summary>
        private void Start()
        {
            // Initialize cache.
            Refresh();
        }


        /// <summary>
        /// Called by Unity. Updates controller.
        /// </summary>
        /// <remarks>
        /// Make sure this method is called after any animations are evaluated.
        /// </remarks>
        private void LateUpdate()
        {
            // Fail silently.
            if (Destinations == null) {
                return;
            }


            // Apply value.
            Destinations.BlendToValue(BlendMode, MouthOpening);
        }

        private void OnDisable()
        {
            // Disable時は口を必ず閉じる. 2018/2/8 yoshida.
            this.MouthOpeningValue = 0f;
        }
        #endregion
    }
}
