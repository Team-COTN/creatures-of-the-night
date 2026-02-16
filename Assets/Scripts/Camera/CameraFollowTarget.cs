using System;
using UnityEngine;
using UnityEngine.Serialization;
using Player;

[ExecuteAlways]
public class CameraFollowTarget : MonoBehaviour
{
    private PlayerCharacterController character;
    private TempEye eye;
    Transform _target;
    public bool trackPlayerX = true;
    public bool trackPlayerY = true;
    public bool trackPlayerRot = true;
    private void OnEnable()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        eye = GameObject.FindGameObjectWithTag("Eye").GetComponent<TempEye>();
        eye.AddEyeStateChangeObserver(TrackTarget);
    }
    private void OnDisable()
    {
        eye.RemoveEyeStateChangeObserver(TrackTarget);
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
        float x = trackPlayerX ? _target.position.x : transform.position.x;
        float y = trackPlayerY ? _target.position.y : transform.position.y;
        Vector3 rot = trackPlayerRot ? _target.rotation.eulerAngles : Vector3.zero;
        
        transform.position = new Vector3(x, y, transform.position.z);
        transform.rotation = Quaternion.Euler(rot);
    }
}
