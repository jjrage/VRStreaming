using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrunkController : MonoBehaviour
{

    public MotionBlur motionBlur;
    public LensDistortion lensDistortion;
    public DepthOfField depthOfField;
    public Vignette vignette;
    public Volume volume;

    #region Public Fields
    #endregion

    #region Editor Fields
    [SerializeField]
    private List<DrunkState> m_drunkStates;
    #endregion

    #region Private Fields

    public float m_drinkedAmount = 0;
    private float m_soberingSpeed = 0.05f;
    public bool m_isDrunk = false;
    public DrunkState m_currentDrunkStage;
    #endregion

    #region MonoBehaviour

    void Start()
    {
        Drink.OnDrink += ChangeDrinkedAmount;
        if (volume.profile.TryGet<MotionBlur>(out MotionBlur mb))
        {
            motionBlur = mb;
        }
        if (volume.profile.TryGet<LensDistortion>(out LensDistortion ls))
        {
            lensDistortion = ls;
        }
        if (volume.profile.TryGet<DepthOfField>(out DepthOfField dof))
        {
            depthOfField = dof;
        }
        if (volume.profile.TryGet<Vignette>(out Vignette v))
        {
            vignette = v;
        }
    }

    #endregion

    #region Private Methods

    private void ChangeDrinkedAmount(float value)
    {
        ChangeDrinkedValue(value);

        if (!m_isDrunk)
        {
            m_isDrunk = true;
            StartCoroutine(SoberingRoutine());
        }

    }

    private void SetDrunkEffect(DrunkState drunkState)
    {
        if (m_currentDrunkStage == drunkState)
        {
            return;
        }

        switch (drunkState.state)
        {
            case DrunkState.State.Normal:
                motionBlur.intensity.value = 0;
                lensDistortion.intensity.value = 0;
                lensDistortion.xMultiplier.value = 0;
                lensDistortion.yMultiplier.value = 0;
                depthOfField.active = false;
                vignette.intensity.value = 0;
                break;
            case DrunkState.State.Tipsy:
                motionBlur.intensity.value = 0.25f;
                lensDistortion.intensity.value = -0.25f;
                lensDistortion.xMultiplier.value = 0.25f;
                lensDistortion.yMultiplier.value = 0.25f;
                depthOfField.active = true;
                depthOfField.gaussianStart.value = 10f;
                depthOfField.gaussianEnd.value = 30f;
                vignette.intensity.value = 0.99f;
                break;
            case DrunkState.State.Boozy:
                break;
            case DrunkState.State.Drunk:
                break;
            case DrunkState.State.Hammered:
                break;
        }
       

        m_currentDrunkStage = drunkState;
    }

    private void ChangeDrinkedValue(float amount)
    {
        m_drinkedAmount += amount;

        if (m_drinkedAmount < 0)
        {
            m_drinkedAmount = 0;
        }

        var drunkState = CheckDrunkStage(m_drinkedAmount);

        if (drunkState != null)
        {
            SetDrunkEffect(drunkState);
        }
        else
        {
            Debug.LogError($"[Drunk state] Drunk state is null when drunk amount is {m_drinkedAmount}");
        }
    }

    #endregion

    private IEnumerator SoberingRoutine()
    {
        while (m_drinkedAmount > 0)
        {
            yield return new WaitForSeconds(1f);
            ChangeDrinkedValue(-m_soberingSpeed);
        }
        m_isDrunk = false;
    }

    private DrunkState CheckDrunkStage(float drinkedAmount)
    {
        DrunkState drunkState = null;

        foreach (var state in m_drunkStates)
        {
            if (state.IsInBoundary(drinkedAmount))
            {
                drunkState = state;
            }
        }

        return drunkState;
    }

}
