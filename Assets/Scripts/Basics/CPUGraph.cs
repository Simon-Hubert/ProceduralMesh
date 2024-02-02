using System;
using Unity.VisualScripting;
using UnityEngine;

public class CPUGraph : MonoBehaviour
{
    [SerializeField, Range(10,200)] private int _resolution = 10;
    [SerializeField] FunctionLibrary.FunctionName _function;
    [SerializeField, Min(0f)] float _functionDuration, _transitionDuration = 1f;

    public enum TransitionMode{ Cycle, Random}
    [SerializeField] TransitionMode _transitionMode;

    float _duration;
    bool _transitionning;
    FunctionLibrary.FunctionName _transitionFunction;

    ComputeBuffer positionsBuffer;
    [SerializeField] ComputeShader computeShader;
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;

    static readonly int 
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");

    private void OnEnable() {
        positionsBuffer = new ComputeBuffer(_resolution * _resolution, 3*4);
    }

    private void OnDisable() {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    private void Update() {
        _duration += Time.deltaTime;
        if(_transitionning){
            if(_duration >= _transitionDuration){
                _duration -= _transitionDuration;
                _transitionning = false;
            }
        }
        else{
            if (_duration >= _functionDuration){
                _duration -= _functionDuration;
                _transitionning = true;
                _transitionFunction = _function;
                PickNextFunction();
            }
        }
        UpdateFunctionOnGPU();
    }

    private void PickNextFunction()
    {
        _function = _transitionMode == TransitionMode.Cycle ? 
            FunctionLibrary.GetNextFunctionName(_function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(_function);
    }

    private void UpdateFunctionOnGPU() {
        float step = 2f / _resolution;
        computeShader.SetInt(resolutionId, _resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(_resolution/8f);
        computeShader.Dispatch(0,groups,groups,1);

        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2f+ 2f/_resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }
}
