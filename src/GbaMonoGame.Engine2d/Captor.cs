using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.Engine2d;

public class Captor : GameObject
{
    public Captor(int instanceId, Scene2D scene, CaptorResource captorResource) : base(instanceId, scene, captorResource)
    {
        TriggerOnMainActorDetection = captorResource.TriggerOnMainActorDetection;
        IsTriggering = captorResource.IsTriggering;
        CaptorFlag_2 = captorResource.CaptorFlag_2;
        
        Events = captorResource.Events.Events;
        OriginalEventsToTrigger = captorResource.EventsCount;
        EventsToTrigger = captorResource.EventsCount;

        _captorBox = new Box(captorResource.BoxMinX, captorResource.BoxMinY, captorResource.BoxMaxX, captorResource.BoxMaxY);
    }

    private readonly Box _captorBox;

    // Flags
    public bool TriggerOnMainActorDetection { get; set; }
    public bool IsTriggering { get; set; }
    public bool CaptorFlag_2 { get; set; }

    public CaptorEvent[] Events { get; }
    public int OriginalEventsToTrigger { get; set; }
    public int EventsToTrigger { get; set; }
    public int TriggeredCount { get; set; }

    public Box GetCaptorBox() => _captorBox;

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        switch (message)
        {
            case Message.Captor_Trigger:
                IsTriggering = true;
                TriggerEvent();
                return true;
        }

        return base.ProcessMessageImpl(message, param);
    }

    public void TriggerEvent()
    {
        foreach (CaptorEvent evt in Events)
        {
            Message msg = (Message)evt.MessageId;

            switch (msg)
            {
                case Message.Captor_Trigger_Sound:
                    SoundEventsManager.ProcessEvent(evt.Param);
                    break;

                case Message.Captor_Trigger_None:
                    // Do nothing
                    break;
                
                case Message.Captor_Trigger_SendMessageWithParam:
                default:
                    Scene.KnotManager.GetGameObject(evt.Param & 0xFF).ProcessMessage(msg, evt.Param >> 8);
                    break;
                
                case Message.Captor_Trigger_SendMessageWithCaptorParam:
                    Scene.KnotManager.GetGameObject(evt.Param & 0xFF).ProcessMessage(msg, this);
                    break;
            }

            EventsToTrigger--;
        }

        TriggeredCount++;

        if (EventsToTrigger == 0)
        {
            ProcessMessage(Message.Destroy);
            TriggeredCount = 0;
            EventsToTrigger = OriginalEventsToTrigger;
            IsTriggering = false;
        }
    }
}