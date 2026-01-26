using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExecutionConsumptionLogger
{
    void Mark(string name);
    void Log();
}

public class ExecutionConsumptionLogger : IExecutionConsumptionLogger
{
    public class Item
    {
        private string _name;
        private double _interval;

        public Item(string name, double interval)
        {
            _name = name;
            _interval = interval;
        }

        public override string ToString() => string.Format("[{0} - {1}ms]", _name, _interval);
    }

    const string TAG = "ExecutionConsumptionLogger";

    private string _name;
    private List<Item> _items = new List<Item>();

    private DateTime _stepStartTime;

    public ExecutionConsumptionLogger(string name)
    {
        _name = name;
        _stepStartTime = DateTime.Now;
    }

    public void Mark(string stepName)
    {
        double interval = (DateTime.Now - _stepStartTime).TotalMilliseconds;
        _items.Add(new Item(stepName, interval));

        Debug.LogFormat("{0}->{1}->{2} - {3}ms", TAG, _name, stepName, interval);
    }

    public void Log() => Debug.Log(ToString());

    public override string ToString()
    {
        string headerStr = string.Format("{0} - {1}\n", TAG, _name);
        string str = headerStr;
        foreach (Item i in _items)
        {
            str += i.ToString() + "\n";
        }
        return str;
    }
}
