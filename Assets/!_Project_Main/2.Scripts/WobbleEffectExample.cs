using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class WobbleEffectExample : MonoBehaviour
{
    private Vector3 _lastPos;
    private Vector3 _lastRot;  
    private Vector3 _currentVel;
    private Vector3 _angularVel;
    private float _lastWobbleAmountX;
    private float _lastWobbleAmountZ;
    
    private float MaxWobble = 0.03f;
    private float WobbleSpeed = 1f;
    private float Recovery = 1f;

    private void Update()
    {
        WobbleStart();
    }

    private void WobbleStart()
    {
        float time = 0.2f;
        time += Time.deltaTime;
        
        _lastWobbleAmountX = Mathf.Lerp(_lastWobbleAmountX, 0, Time.deltaTime * (Recovery));
        _lastWobbleAmountZ = Mathf.Lerp(_lastWobbleAmountZ, 0, Time.deltaTime * (Recovery));
 
        float pulseAmount = 2 * Mathf.PI * WobbleSpeed;
        float wobbleAmountX = _lastWobbleAmountX * Mathf.Sin(pulseAmount * time);
        float wobbleAmountZ = _lastWobbleAmountZ * Mathf.Sin(pulseAmount * time);

        
        Shader.SetGlobalFloat("_WobbleX", wobbleAmountX);
        Shader.SetGlobalFloat("_WobbleZ", wobbleAmountZ);
 
        _currentVel = (_lastPos - transform.position) / Time.deltaTime;
        _angularVel = transform.rotation.eulerAngles - _lastRot;
 
        _lastWobbleAmountX += Mathf.Clamp((_currentVel.x + (_angularVel.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        _lastWobbleAmountZ += Mathf.Clamp((_currentVel.z + (_angularVel.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        
        _lastPos = transform.position;
        _lastRot = transform.rotation.eulerAngles;
    }
 
}