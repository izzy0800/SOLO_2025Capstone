using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;

public class DummyPlayerCamera : PlayerCamera
{
    private Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();

        if (m_Player == null)
        {
            m_Player = transform;
        }

        mainCamera = Camera.main;

        if (m_Target != null)
        {
            m_Target.position = transform.position + transform.forward * 5f;
        }
    }
    private void Update()
    {
        if (m_Target != null && m_Player != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            m_Target.position = m_Player.position + m_Player.forward * TargetDistance;
        }
    }
    public override void SetInput(in Vector2 delta, float scroll)
    {
        
    }
}
