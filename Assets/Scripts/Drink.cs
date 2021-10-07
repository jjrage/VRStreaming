using System.Collections;
using UnityEngine;

public class Drink : MonoBehaviour
{
    public delegate void DrinkAction(float value);
    public static event DrinkAction OnDrink;

    [SerializeField]
    protected float m_volume;
    private bool m_isDrinking = false;
    private const float DRINK_STEP = 0.1f;
    private IEnumerator m_drinkRoutine;

    private void Awake()
    {
        m_drinkRoutine = DrinkRoutine();
    }

    private void SipDrink()
    {
        m_volume -= DRINK_STEP;
        if (m_volume < 0)
        {
            m_volume = 0;
        }
        OnDrink(DRINK_STEP);
    }

    protected void StartDrink()
    {
        if (m_volume > 0)
        {
            m_isDrinking = true;
            StartCoroutine(m_drinkRoutine);
        }
    }

    protected void StopDrink()
    {
        m_isDrinking = false;
        StopCoroutine(m_drinkRoutine);
    }

    protected IEnumerator DrinkRoutine()
    {
        while (m_volume > 0)
        {
            yield return new WaitForSeconds(0.5f);
            SipDrink();
        }

        StopDrink();
    }
}
