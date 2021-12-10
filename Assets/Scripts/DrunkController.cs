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
    [SerializeField]
    private Volume m_currentVolume;
    #endregion

    #region Private Fields

    public float m_drinkedAmount = 0;
    private float m_soberingSpeed = 0.05f;
    public bool m_isDrunk = false;
    public DrunkState m_currentDrunkStage;
    public VolumeProfile m_currentProfile;

    private float m_maxLensDistortionMultiplierX = 1;
    private float m_minLensDistortionMultiplierX = 0;
    private float m_maxLensDistortionMultiplierY = 1;
    private float m_minLensDistortionMultiplierY = 0;
    private float m_minLensDistortionIntensity = -0.65f;
    private float m_maxLensDistortionIntensity = 0.5f;
    private float m_maxDOFFocusDistance = 10f;
    private float m_minDOFFocusDistance = 0.1f;
    public bool isLDIncreasing = true;
    public float m_drunkProccessSpeed = 0.2f;
    private bool isLDXIncreasing= true;
    private bool isLDYIncreasing= false;

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
            StartCoroutine(DrunkProcess());
        }

    }

    public void SetHammered()
    {
       
            ChangeDrinkedAmount(5);
        
    }

    private void SetDrunkEffect(DrunkState drunkState)
    {
        if (m_currentDrunkStage == drunkState)
        {
            return;
        }
       
        m_currentProfile = drunkState.profile;
        m_currentVolume.profile = m_currentProfile;

        if (m_currentVolume.profile.TryGet<MotionBlur>(out MotionBlur motionBlur))
        {
            this.motionBlur = motionBlur;
        }

        if (m_currentVolume.profile.TryGet<LensDistortion>(out LensDistortion lensDistortion))
        {
            this.lensDistortion = lensDistortion;
        }

        if (m_currentVolume.profile.TryGet<DepthOfField>(out DepthOfField depthOfField))
        {
            this.depthOfField = depthOfField;
        }

        if (m_currentVolume.profile.TryGet<Vignette>(out Vignette vignette))
        {
            this.vignette = vignette;
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

    private IEnumerator DrunkProcess()
    {
        while (m_drinkedAmount > 0)
        {
            yield return null;
            ProcessDrunkEffect(m_currentDrunkStage);
        }
        m_isDrunk = false;
    }

    private void ProcessDrunkEffect(DrunkState drunkState)
    {
        float currentld = lensDistortion.intensity.value;
        float currentldX = lensDistortion.xMultiplier.value;
        float currentldY = lensDistortion.yMultiplier.value;
        float maxld;
        float ld;
        float ldX;
        float ldY;

        if (isLDIncreasing)
        {
            IncreaseLensDistortion(currentld, drunkState.maxLD);
        }
        else
        {
            DecreaseLensDistortion(currentld, drunkState.minLD);
        }

        if (isLDXIncreasing)
        {
            IncreaseXLensMultipliers(currentldX, 1);
        }
        else
        {
            DecreaseXLensMultipliers(currentldX, 0);
        }

        if (isLDYIncreasing)
        {
            IncreaseYLensMultipliers(currentldY, 1);
        }
        else
        {
            DecreaseYLensMultipliers(currentldY, 0);
        }
        //if (currentldX < 1)
        //{
        //    ldX = Mathf.Lerp(currentldX, 1, Time.deltaTime);
        //}
        //else
        //{

        //}

        //lensDistortion.intensity.value = ld;
    }

    private void IncreaseLensDistortion(float current, float max)
    {
        if(current < max)
        {
            float drunkspeed = Random.Range(0f, 0.5f);
            isLDIncreasing = true;
            float ldValue =current + Time.deltaTime * drunkspeed;
            lensDistortion.intensity.value = ldValue + 0.01f;
        }
        else
        {
            isLDIncreasing = false;
        }
    }

    private void DecreaseLensDistortion(float current, float min)
    {
        if (current > min)
        {
            float drunkspeed = Random.Range(0f, 0.5f);
            isLDIncreasing = false;
            float ldValue = current + -Time.deltaTime * drunkspeed;
            lensDistortion.intensity.value = ldValue - 0.01f;
        }
        else
        {
            isLDIncreasing = true;
        }
    }

    private void IncreaseXLensMultipliers(float currentX, float maxX)
    {
        if (currentX < maxX)
        {
            float drunkspeed = Random.Range(0f, 0.2f);
            isLDXIncreasing = true;
            float ldValue = currentX + Time.deltaTime * drunkspeed;
            lensDistortion.xMultiplier.value = ldValue;
        }
        else
        {
            isLDXIncreasing = false;
        }
    }

    private void DecreaseXLensMultipliers(float currentX, float minX)
    {
        if (currentX > minX)
        {
            float drunkspeed = Random.Range(0f, 0.2f);
            isLDXIncreasing = false;
            float ldValue = currentX + -Time.deltaTime * drunkspeed;
            lensDistortion.xMultiplier.value = ldValue;
        }
        else
        {
            isLDXIncreasing = true;
        }
    }
    private void IncreaseYLensMultipliers(float currentY, float maxY)
    {
        if (currentY < maxY)
        {
            float drunkspeed = Random.Range(0f, 0.2f);
            isLDYIncreasing = true;
            float ldValue =currentY+ Time.deltaTime * drunkspeed;
            lensDistortion.yMultiplier.value = ldValue;
        }
        else
        {
            isLDYIncreasing = false;
        }
    }
    private void DecreaseYLensMultipliers(float currentY, float minY)
    {
        if (currentY > minY)
        {
            float drunkspeed = Random.Range(0f, 0.2f);
            isLDYIncreasing = false;
            float ldValue = currentY+ -Time.deltaTime * drunkspeed;
            lensDistortion.yMultiplier.value = ldValue;
        }
        else
        {
            isLDYIncreasing = true;
        }
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
