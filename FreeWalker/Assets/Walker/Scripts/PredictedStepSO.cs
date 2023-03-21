using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


[CreateAssetMenu]

public class PredictedStepSO : ScriptableObject
{
    public GameObject predStep;

    public GameObject clone;


    public void CreateStep(Vector3 pos, Quaternion rot)
    {
        clone = Instantiate(predStep, pos, rot);
    }

    public void DestroyStep()
    {
        Destroy(clone);
    }

    public float Distance(Transform other)
    {
        return Vector3.Distance(clone.transform.position, other.position);
    }
    
}
