// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

public class Reward : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Individual individual = other.GetComponent<Individual>();
        if (individual != null)
        {
            individual.RegenerateLifeTime(Configuration.Singleton.LifeTimeRegeneration);
            gameObject.SetActive(false);
        }
    }
}
