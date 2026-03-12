
using Gems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using VRC.SDK3.UdonNetworkCalling;

namespace Gems
{
    namespace PostProcess
    {

        public class Options : EmeraldBehaviour
        {
            protected override string LogName => "Gems.PostProcessOptions";
            [Header("Post Processing")]
            [SerializeField] bool aoMode; // false = MSVO; true = SAO
            [SerializeField] bool colorMode; // false = Neutral; true = ACES
            [SerializeField, Range(0, 1)] float aoWeight = 0;
            [SerializeField, Range(0, 1)] float bloomWeight = 0;
            [SerializeField, Range(0, 1)] float grainWeight = 0;
            [SerializeField, Range(-1, 1)] float exposureWeight = 0;
            [SerializeField, Range(-1, 1)] float contrastWeight = 0;
            [SerializeField, Range(-1, 1)] float saturationWeight = 0;
            [SerializeField, Range(-1, 1)] float temperatureWeight = 0;

            [SerializeField] PostProcessVolume neutral;
            [SerializeField] PostProcessVolume aces;

            [SerializeField] PostProcessVolume msvoAo;
            [SerializeField] PostProcessVolume saoAo;

            [SerializeField] PostProcessVolume bloom;
            [SerializeField] PostProcessVolume grain;

            [SerializeField] PostProcessVolume[] exposure;
            [SerializeField] PostProcessVolume[] contrast;
            [SerializeField] PostProcessVolume[] saturation;
            [SerializeField] PostProcessVolume[] temperature;

            [Header("UI")]
            [SerializeField] Color enabledColor;
            [SerializeField] Color disabledColor;
            [SerializeField] BoxToggle aoToggle;
            [SerializeField] BoxToggle colorToggle;

            [SerializeField] Slider aoSlider;
            [SerializeField] Slider bloomSlider;
            [SerializeField] Slider grainSlider;

            [SerializeField] Slider exposureSlider;
            [SerializeField] Slider contrastSlider;
            [SerializeField] Slider saturationSlider;
            [SerializeField] Slider temperatureSlider;

            [SerializeField] TextMeshProUGUI aoSliderLabel;
            [SerializeField] TextMeshProUGUI bloomSliderLabel;
            [SerializeField] TextMeshProUGUI grainSliderLabel;
            [SerializeField] TextMeshProUGUI exposureSliderLabel;
            [SerializeField] TextMeshProUGUI contrastSliderLabel;
            [SerializeField] TextMeshProUGUI saturationSliderLabel;
            [SerializeField] TextMeshProUGUI temperatureSliderLabel;

            void Start()
            {
                InitializeUI();
            }

            void InitializeUI()
            {
                neutral.gameObject.SetActive(!colorMode);
                aces.gameObject.SetActive(colorMode);
                colorToggle.State = colorMode;

                msvoAo.gameObject.SetActive(!aoMode);
                saoAo.gameObject.SetActive(aoMode);
                aoToggle.State = aoMode;


                InitializePp(aoSliderLabel, aoSlider, msvoAo, aoWeight);
                InitializePp(aoSliderLabel, aoSlider, saoAo, aoWeight);

                InitializePp(bloomSliderLabel, bloomSlider, bloom, bloomWeight);
                InitializePp(grainSliderLabel, grainSlider, grain, grainWeight);

                InitializePp(exposureSliderLabel, exposureSlider, exposure, exposureWeight);
                InitializePp(contrastSliderLabel, contrastSlider, contrast, contrastWeight);
                InitializePp(saturationSliderLabel, saturationSlider, saturation, saturationWeight);
                InitializePp(temperatureSliderLabel, temperatureSlider, temperature, temperatureWeight);
            }

            void InitializePp(TextMeshProUGUI label, Slider slider, PostProcessVolume pp, float value)
            {
                slider.SetValueWithoutNotify(value);
                SetWeight(label, pp, value);
            }

            void InitializePp(TextMeshProUGUI label, Slider slider, PostProcessVolume[] pp, float value)
            {
                slider.SetValueWithoutNotify(value);
                SetWeight(label, pp, value);
            }

            void SetWeight(TextMeshProUGUI label, PostProcessVolume pp, float value)
            {
                pp.weight = value;

                bool active = Mathf.Abs(value) >= 0.01f;
                pp.gameObject.SetActive(active);
                label.color = active ? enabledColor : disabledColor;
            }

            void SetWeight(TextMeshProUGUI label, PostProcessVolume[] pp, float value)
            {
                if (value >= 0)
                {
                    pp[0].weight = value;
                    pp[1].weight = 0;
                }
                else
                {
                    pp[0].weight = 0;
                    pp[1].weight = Mathf.Abs(value);
                }

                bool active = Mathf.Abs(value) >= 0.01f;
                pp[0].gameObject.SetActive(active);
                pp[1].gameObject.SetActive(active);
                label.color = active ? enabledColor : disabledColor;
            }


            #region Ui Listeners
            [NetworkCallable]
            public void _ToggleAoMode(bool state)
            {
                aoMode = state;
                msvoAo.gameObject.SetActive(!aoMode);
                saoAo.gameObject.SetActive(aoMode);
            }

            [NetworkCallable]
            public void _ToggleColorMode(bool state)
            {
                colorMode = state;
                neutral.gameObject.SetActive(!colorMode);
                aces.gameObject.SetActive(colorMode);
            }

            public void _OnSliderAo()
            {
                aoWeight = aoSlider.value;

                SetWeight(aoSliderLabel, msvoAo, aoWeight);
                SetWeight(aoSliderLabel, saoAo, aoWeight);

                // Potentially gets easier if I just decided for one single AO mode
                bool active = Mathf.Abs(aoWeight) >= 0.01f;
                msvoAo.gameObject.SetActive(!aoMode && active);
                saoAo.gameObject.SetActive(aoMode && active);
                aoSliderLabel.color = active ? enabledColor : disabledColor;
            }

            public void _OnSliderBloom()
            {
                bloomWeight = bloomSlider.value;
                SetWeight(bloomSliderLabel, bloom, bloomWeight);
            }

            public void _OnSliderGrain()
            {
                grainWeight = grainSlider.value;
                SetWeight(grainSliderLabel, grain, grainWeight);
            }

            public void _OnSliderExposure()
            {
                exposureWeight = exposureSlider.value;
                SetWeight(exposureSliderLabel, exposure, exposureWeight);
            }

            public void _OnSliderContrast()
            {
                contrastWeight = contrastSlider.value;
                SetWeight(contrastSliderLabel, contrast, contrastWeight);
            }

            public void _OnSliderSaturation()
            {
                saturationWeight = saturationSlider.value;
                SetWeight(saturationSliderLabel, saturation, saturationWeight);
            }

            public void _OnSliderTemperature()
            {
                temperatureWeight = temperatureSlider.value;
                SetWeight(temperatureSliderLabel, temperature, temperatureWeight);
            }
            #endregion
        }
    }
}