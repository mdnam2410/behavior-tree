using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Threading;

public class TestUniRx : MonoBehaviour
{
    IDisposable dispo2;
    ReactiveProperty<int> reactiveProperty = new ReactiveProperty<int>();
    ReactiveCommand reactiveCommand = new ReactiveCommand();
    ReactiveCommand<int> reactiveCommandInt = new ReactiveCommand<int>();
    ReactiveCollection<int> reactiveCollection = new ReactiveCollection<int>();

    // Start is called before the first frame update
    void Start()
    {
        var dispo1 = Observable.Timer(TimeSpan.FromSeconds(5), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
        {
            Debug.Log("Wait 5s");
        });

        dispo2 = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(_ =>
        {
            Debug.Log("Loop 5s");
        });

        Observable.IntervalFrame(10).Subscribe(_ =>
        {
        });

        reactiveProperty.Subscribe(_ =>
        {
            Debug.Log("reactiveProperty1: " + _);
        });
        reactiveProperty.Subscribe(_ =>
        {
            Debug.Log("reactiveProperty2: " + _);
        });

        reactiveCommand.Subscribe(_ =>
        {
            Debug.Log("reactiveCommand1");
        });
        reactiveCommand.Subscribe(_ =>
        {
            Debug.Log("reactiveCommand2");
        });
        reactiveCommandInt.Subscribe(_ =>
        {
            Debug.Log("reactiveCommandInt: " + _);
        });

        reactiveCollection.ObserveAdd().Subscribe(_ =>
        {
            Debug.Log("add value: " + _.Value + " at index: " + _.Index);
        });
        reactiveCollection.ObserveRemove().Subscribe(_ =>
        {
            Debug.Log("remove value: " + _.Value + " at index: " + _.Index);
        });

        MainThreadDispatcher.SendStartCoroutine(Wait(2));
        ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc));
    }

    private void OnDestroy()
    {
        
    }

    private IEnumerator Wait(int delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Finish StarCoroutine");
    }

    void ThreadProc(object stateInfo)
    {
        MainThreadDispatcher.Send(_ => {
            Debug.Log("position: " + transform.position);
        }, null);
        //Debug.Log("position: " + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if(dispo2 != null)
            {
                dispo2.Dispose();
                dispo2 = null;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            reactiveProperty.Value++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            reactiveCommand.Execute();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            reactiveCommandInt.Execute(100);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            reactiveCollection.Add(reactiveCollection.Count + 1);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            if(reactiveCollection.Count > 0)
            {
                reactiveCollection.RemoveAt(reactiveCollection.Count - 1);
            }
        }
    }
}
