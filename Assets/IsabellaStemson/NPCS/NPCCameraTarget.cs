using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Controller;
using UnityEngine;

public class NPCCameraTarget : PlayerCamera
{
    private CinemachineVirtualCamera firstPersonCam;
    private Transform npcTransform;

    protected override void Awake()
    {
        base.Awake();
        npcTransform = transform;
        if (m_Player == null)
        {
            m_Player = transform;
        }
        if (m_Target == null)
        {
            m_Target = new GameObject("CameraTarget").transform;
            m_Target.SetParent(transform);
        }
    }

    private void Start()
    {
        var charSwitch = FindObjectOfType<CharacterSwitch>();
        if (charSwitch != null)
        {
            firstPersonCam = charSwitch.firstPersonCam;
        }
    }

    private void Update()
    {
        if (m_Target != null && m_Player != null)
        {
            if (firstPersonCam != null && firstPersonCam.Priority > 10)
            {
                Vector3 cameraForward = firstPersonCam.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                m_Target.position = m_Player.position + cameraForward * TargetDistance;
            }
            else
            {
                m_Target.position = m_Player.position + m_Player.forward * TargetDistance;
            }
        }
    }

    public override void SetInput(in Vector2 delta, float scroll)
    {

    }

}
