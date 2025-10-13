using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    //this should be singleton instead

    private bool waiting;

    private void Awake()
    {
    }

    public void Stop(float duration)
    {
        if (waiting)
        {
            return;
        }

        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }

}
