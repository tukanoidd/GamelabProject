using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent((typeof(Volume)))]
public class PostProcessAnimate : MonoBehaviour
{
    enum PostProcessVolumePartType
    {
        ChromaticAberration,
        HueShift
    }

    private bool _chromaticAberrationLoopStarted = false;
    private bool _chromaticAberrationLoopReverse = false;
    [SerializeField] private bool _loopChromaticAberration = true;
    [SerializeField] private float _chromaticAberrationLoopTime = 1f;

    private bool _hueShiftLoopStarted = false;
    [SerializeField] private bool _loopHueShift = true;
    [SerializeField] private float _hueShiftLoopTime = 1f;

    private Volume _postProcessVolume;

    private void Awake()
    {
        _postProcessVolume = GetComponent<Volume>();
    }

    void Update()
    {
        if (!_postProcessVolume) return;

        if (_loopChromaticAberration && !_chromaticAberrationLoopStarted)
            StartCoroutine(
                LoopPostProcessVolumePart(
                    _chromaticAberrationLoopTime,
                    PostProcessVolumePartType.ChromaticAberration
                )
            );

        if (_loopHueShift && !_hueShiftLoopStarted)
            StartCoroutine(
                LoopPostProcessVolumePart(
                    _hueShiftLoopTime,
                    PostProcessVolumePartType.HueShift
                )
            );
    }

    private IEnumerator LoopPostProcessVolumePart(float duration, PostProcessVolumePartType type)
    {
        switch (type)
        {
            case PostProcessVolumePartType.ChromaticAberration:
                _chromaticAberrationLoopStarted = true;
                break;
            case PostProcessVolumePartType.HueShift:
                _hueShiftLoopStarted = true;
                break;
        }

        float changedVal = 0;

        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;

            switch (type)
            {
                case PostProcessVolumePartType.ChromaticAberration:
                    ChromaticAberration chromaticAberrationComponent =
                        (ChromaticAberration) _postProcessVolume.profile.components.FirstOrDefault(
                            volComp => (volComp as ChromaticAberration) != null
                        );

                    if (chromaticAberrationComponent != null)
                    {
                        changedVal = Mathf.Clamp(
                            _chromaticAberrationLoopReverse ? 1 - normalizedTime : normalizedTime, 0, 1
                        );
                        chromaticAberrationComponent.intensity.value = changedVal;
                    }

                    break;
                case PostProcessVolumePartType.HueShift:
                    ColorAdjustments colorAdjustmentsComponent =
                        (ColorAdjustments) _postProcessVolume.profile.components.FirstOrDefault(
                            volComp => (volComp as ColorAdjustments) != null
                        );

                    if (colorAdjustmentsComponent != null)
                    {
                        colorAdjustmentsComponent.hueShift.value = Mathf.Clamp(
                            normalizedTime * 360 - 180, -180, 180
                        );
                    }

                    break;
            }

            yield return null;
        }

        switch (type)
        {
            case PostProcessVolumePartType.ChromaticAberration:
                if (Mathf.RoundToInt(changedVal) == 1) _chromaticAberrationLoopReverse = true;
                else _chromaticAberrationLoopReverse = false;

                _chromaticAberrationLoopStarted = false;
                break;
            case PostProcessVolumePartType.HueShift:
                _hueShiftLoopStarted = false;
                break;
        }
    }
}