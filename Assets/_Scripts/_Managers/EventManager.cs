using System;
using System.Collections.Generic;

public class EventManager<T>
{
    private readonly List<Action<T>> callbacks = new List<Action<T>>();
    private readonly List<MethodContainer> events = new List<MethodContainer>();

    public delegate void MethodContainer();

    private event MethodContainer Event_;

    public void Subscribe(Action<T> callback)
    {
        callbacks.Add(callback);
    }
    public void Subscribe(MethodContainer callback)
    {
        Event_ += callback;
        events.Add(callback);
    }

    public void Unsubscribe(Action<T> callback)
    {
        if (callbacks.Contains(callback)) callbacks.Remove(callback);
        else throw new NullReferenceException("No match callback: " + callback);
    }
    public void Unsubscribe(MethodContainer callback)
    {
        if (Event_.Method == callback.Method) Event_ -= callback;
        else throw new NullReferenceException("No match callback: " + callback);
    }

    public void Publish(T action)
    {
        for (int i = 0; i < callbacks.Count; i++) callbacks[i](action);
    }
    public void Publish()
    {
        Event_();
    }
    public void Publish(int index)
    {
        events[index]();
    }
}

public class EventAggregator
{
    public static EventManager<Coins> coinEvents;
    public static EventManager<Emeralds> emeraldEvents;
    public static EventManager<_Player> loseEvents;
    public static EventManager<Finish> finishEvents;
    public static EventManager<LoadLevel> loadEvents;
    public static EventManager<HideLevel> hideEvents;
    public static EventManager<JumpBoost> jumpEvents;
    public static EventManager<UnityEngine.Animation> animationEvents;
}
