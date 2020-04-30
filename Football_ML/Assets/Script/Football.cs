using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Football : MonoBehaviour
{
    public static bool goal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoalNet"))
        {
            goal = true;
        }
    }
}
