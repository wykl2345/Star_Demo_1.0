using System;
using System.Collections;
using System.Collections.Generic;
using MapStateManager;
using UnityEngine;

public class CameraWith : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform _playerTransform;
    public Transform PlayerTransform => _playerTransform ??= MapManager.Instance.Player.transform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        var pos = PlayerTransform.position;
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
