using System;
using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity.Managers;
using UnityEngine;

public class DeviceChangeListener : MonoBehaviour
{
    UnityEngine.InputSystem.PlayerInput _controls;
    public static ControlDeviceType currentControlDevice;
    private bool deviceChanged = false;
    private Coroutine _coroutine;

    public Action evtDeviceChanged;

    public enum ControlDeviceType
    {
        KeyboardAndMouse,
        Gamepad,
    }

    void Start()
    {
        _controls = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _controls.onControlsChanged += OnControlsChanged;
    }

    private void Update()
    {
        if (deviceChanged)
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(ExecuteAfterControlsChanged());
        }
    }

    IEnumerator ExecuteAfterControlsChanged()
    {
        GameUIManager.Instance.SetDeviceChangePopup();
        deviceChanged = false;
        
        // ImageData current Image 변경 

        yield return new WaitForSeconds(2f);
        GameUIManager.Instance.CloseDeviceChangedPopup();
    }

    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput obj)
    {
        if (obj.currentControlScheme == "Gamepad")
        {
            if (currentControlDevice != ControlDeviceType.Gamepad)
            {
                currentControlDevice = ControlDeviceType.Gamepad;
                ManagerRoot.Input.LastUsedInputType = ControlDeviceType.Gamepad;
                Debug.Log("Changed Input Device to Gamepad");
                deviceChanged = true;

                evtDeviceChanged?.Invoke();
            }
        }
        else
        {
            if (currentControlDevice != ControlDeviceType.KeyboardAndMouse)
            {
                currentControlDevice = ControlDeviceType.KeyboardAndMouse;
                ManagerRoot.Input.LastUsedInputType = ControlDeviceType.KeyboardAndMouse;
                Debug.Log("Changed Input Device to KeyboardAndMouse");
                deviceChanged = true;

                evtDeviceChanged?.Invoke();
            }
        }
    }
}
