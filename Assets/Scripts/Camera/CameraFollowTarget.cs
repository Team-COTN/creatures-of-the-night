using System;
using UnityEngine;
using UnityEngine.Serialization;
using Player;
using Player.Eye;

[ExecuteAlways]
public class CameraFollowTarget : MonoBehaviour
{
    private PlayerCharacterController character;
    private EyeController eye;
    
    Transform _target;
    public bool trackPlayerX = true;
    public bool trackPlayerY = true;
    public bool trackPlayerRot = true;

    private void OnEnable()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        eye = GameObject.FindGameObjectWithTag("Eye").GetComponent<EyeController>();
    }

    public void TrackTarget(Transform target)
    {
        _target = target;
    }
    
    private void Start()
    {
        TrackTarget(character.transform);
    }

    private void Update()
    {
        if (!character || !eye) return;
        
        float x = trackPlayerX ? _target.position.x : transform.position.x;
        float y = trackPlayerY ? _target.position.y : transform.position.y;
        Vector3 rot = trackPlayerRot ? _target.rotation.eulerAngles : Vector3.zero;
        
        transform.position = new Vector3(x, y, transform.position.z);
        transform.rotation = Quaternion.Euler(rot);

        if (eye.EyeActive)
        {
            TrackTarget(eye.transform);
        } else
        {
            TrackTarget(character.transform);
        }
    }
}
