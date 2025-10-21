using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Controller;
using UnityEngine;

public class NPCCameraTarget : PlayerCamera
{
    protected override void Awake()
    {
        base.Awake();

        if (m_Player == null)
        {
            m_Player = transform;
        }
    }

    private void Update()
    {
        if (m_Target != null && m_Player != null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Vector3 cameraForward = mainCam.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                m_Target.position = m_Player.position + cameraForward * TargetDistance;
            }
        }
    }

    public override void SetInput(in Vector2 delta, float scroll)
    {

    }
}
