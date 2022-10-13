using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DOTweenMethods : MonoBehaviour
{
    [SerializeField] private ExampleType _example;
    [SerializeField] private Transform _objectToMove;

    private Dictionary<ExampleType,Action> _examplesDictionary;

    private async void Start()
    {
        _examplesDictionary = new Dictionary<ExampleType,Action>
        {
            {ExampleType.FirstExample, FirstMethod},
            {ExampleType.SecondExample, SecondMethod}
        };

        await Task.Delay(1000);
        _examplesDictionary[_example].Invoke();
    }

    //ease-https://easings.net/
    //ease.flash - мерцание
    //все DO разобрать
    private void FirstMethod()
    {
        _objectToMove.DOLocalMove(Vector3.zero, 1).SetEase(Ease.Linear).SetLoops(2,LoopType.Yoyo);
        //_objectToMove.do
    }

    private void SecondMethod()
    {
        Debug.Log("xey");
    }

    private enum ExampleType
    {
        FirstExample,
        SecondExample
    }
}
