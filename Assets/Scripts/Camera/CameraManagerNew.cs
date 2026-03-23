using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManagerNew : MonoBehaviour
{
    void Start()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain != null)
            StartCoroutine(SnapCameraToTargetThenRestoreSettings(brain));
    }

    IEnumerator SnapCameraToTargetThenRestoreSettings(CinemachineBrain brain)
    {
        // Disable blending and wait for the brain to elect an active camera
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);

        CinemachineCamera activeCam = null;
        while (activeCam == null)
        {
            activeCam = brain.ActiveVirtualCamera as CinemachineCamera;
            yield return null;
        }

        // Zero damping and discard position history so camera snaps instantly
        CinemachinePositionComposer composer = activeCam.GetComponent<CinemachinePositionComposer>();
        if (composer == null) yield break;

        Vector3 originalDamping = composer.Damping;
        composer.Damping = Vector3.zero;
        activeCam.PreviousStateIsValid = false;

        yield return null;

        // Restore everything
        composer.Damping = originalDamping;
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);
    }
}