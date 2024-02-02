using System;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform _pointPrefab;
    [SerializeField, Range(10,100)] private int _resolution = 10;
    [SerializeField] FunctionLibrary.FunctionName _function;
    [SerializeField, Min(0f)] float _functionDuration, _transitionDuration = 1f;

    public enum TransitionMode{ Cycle, Random}
    [SerializeField] TransitionMode _transitionMode;

    Transform[] _points;
    float _duration;
    bool _transitionning;
    FunctionLibrary.FunctionName _transitionFunction;

    private void Awake() {
        _points = new Transform[_resolution*_resolution];

        float step = 2f/_resolution;
        Vector3 scale = Vector3.one*step;

        for(int i=0; i < _points.Length; i++){
            Transform point = _points[i] = Instantiate(_pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
        }
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

        if(_transitionning){
            UpdateFunctionTransition();
        }
        else {
            UpdateFunction();
        }
    }

    private void PickNextFunction()
    {
        _function = _transitionMode == TransitionMode.Cycle ? 
            FunctionLibrary.GetNextFunctionName(_function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(_function);
    }

    void UpdateFunction(){
        float time = Time.time;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(_function);
        float step = 2f / _resolution;
        float v = .5f * step - 1f;

        for (int i = 0, x=0, z=0; i < _points.Length; i++, x++)
        {
            if(x == _resolution){
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            _points[i].localPosition = f(u, v, time);
        }
    }

        void UpdateFunctionTransition(){
        FunctionLibrary.Function 
            from = FunctionLibrary.GetFunction(_transitionFunction),
            to = FunctionLibrary.GetFunction(_function);
        float progress = _duration / _transitionDuration;
        float time = Time.time;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(_function);
        float step = 2f / _resolution;
        float v = .5f * step - 1f;

        for (int i = 0, x=0, z=0; i < _points.Length; i++, x++)
        {
            if(x == _resolution){
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            _points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to , progress);
        }
    }
}
