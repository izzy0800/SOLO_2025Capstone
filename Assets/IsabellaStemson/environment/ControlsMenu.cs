using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button closeButton;

    [Header("Optional")]
    [SerializeField] private bool pauseGameWhenOpen = false;

    private void Start()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (controlsButton != null)
            controlsButton.onClick.AddListener(OpenControls);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseControls);
    }

    public void OpenControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);

            if (pauseGameWhenOpen)
                Time.timeScale = 0f;
        }
    }

    public void CloseControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);

            if (pauseGameWhenOpen)
                Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (controlsPanel != null && controlsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseControls();
        }
    }
}
