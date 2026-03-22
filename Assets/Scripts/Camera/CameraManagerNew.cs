using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManagerNew : MonoBehaviour
{
    void Start()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
        {
            brain.DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Styles.Cut, 0f
            );

            StartCoroutine(RestoreBlend(brain));
        }
    }

    IEnumerator RestoreBlend(CinemachineBrain brain)
    {
        yield return null;
        brain.DefaultBlend = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut, 2f
        );
    }
}