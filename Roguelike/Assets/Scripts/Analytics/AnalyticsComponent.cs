using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsComponent : MonoBehaviour
{
    public void OnPlayerDead(int level)
    {
        Analytics.CustomEvent("Player Dead", new Dictionary<string, object>()
        {
            { "Level Number", level },
        });
    }
}
