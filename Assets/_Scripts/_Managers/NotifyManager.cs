using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class NotifyManager
{
    static bool showing = false;
    static Queue<Arguments> arguments = new Queue<Arguments>();

    public static void ShowNotify(Animation gameNotify, string text, NotifyType type, float showingTime = 3)
    {
        if (showing && EventAggregator.animationEvents != null)
        {
            Arguments args = new Arguments(gameNotify, text, type, showingTime);
            arguments.Enqueue(args);

            return;
        }

        EventAggregator.animationEvents = new EventManager<Animation>();
        EventAggregator.animationEvents.Subscribe(OnNotifyShowed);
        EventAggregator.animationEvents.Subscribe(OnNotifyHided);

        showing = true;

        gameNotify.gameObject.SetActive(true);

        Color color;
        switch (type)
        {
            case NotifyType.Error: color = new Color(0.9f, 0, 0.1f); break;
            case NotifyType.Neitral: color = new Color(0.7f, 0.7f, 0.7f); break;
            case NotifyType.Success: color = new Color(0.2f, 0.8f, 0.2f); break;

            default: color = Color.black; break;
        }
        gameNotify.GetComponent<Image>().color = color;
        gameNotify.transform.GetChild(0).GetComponent<Text>().text = text;
        gameNotify.Play("NotifyAnimShowing", PlayMode.StopAll);

        async void OnNotifyShowed()
        {
            await System.Threading.Tasks.Task.Delay((int)(showingTime * 1000));
            gameNotify.Play("NotifyAnimHiding");
        }

        void OnNotifyHided()
        {
            showing = false;
            EventAggregator.animationEvents.Unsubscribe(OnNotifyHided);
            EventAggregator.animationEvents.Unsubscribe(OnNotifyShowed);
            EventAggregator.animationEvents = null;
            
            if (arguments.Count > 0)
            {
                Arguments args = arguments.Dequeue();
                ShowNotify(args.gameNotify, args.text, args.type, args.showingTime);
            }
            else gameNotify.gameObject.SetActive(false);
        }
    }

    public static void ClearNotify()
    {
        showing = false;
        arguments?.Clear();
    }

    struct Arguments
    {
        public Arguments(Animation gameNotify, string text, NotifyType type, float showingTime)
        {
            this.gameNotify = gameNotify;
            this.text = text;
            this.type = type;
            this.showingTime = showingTime;
        }

        public Animation gameNotify;
        public string text;
        public NotifyType type;
        public float showingTime;
    }
}

public enum NotifyType
{
    Error,
    Neitral,
    Success
}
