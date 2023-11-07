using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.Engine2d;

public class Captor : GameObject
{
    public Captor(int id, CaptorResource captorResource) : base(id, captorResource)
    {
        TriggerOnMainActorDetection = captorResource.TriggerOnMainActorDetection;
        IsTriggering = captorResource.IsTriggering;
        CaptorFlag_2 = captorResource.CaptorFlag_2;
        
        Events = captorResource.Events.Events;
        OriginalEventsToTrigger = captorResource.EventsCount;
        EventsToTrigger = captorResource.EventsCount;

        CaptorBox = new Rectangle(
            x: captorResource.BoxMinX, 
            y: captorResource.BoxMinY,
            width: captorResource.BoxMaxX - captorResource.BoxMinX, 
            height: captorResource.BoxMaxY - captorResource.BoxMinY);
    }

    // Flags
    public bool TriggerOnMainActorDetection { get; set; }
    public bool IsTriggering { get; set; }
    public bool CaptorFlag_2 { get; set; }

    public CaptorEvent[] Events { get; }
    public int OriginalEventsToTrigger { get; set; }
    public int EventsToTrigger { get; set; }
    public int TriggeredCount { get; set; }

    public Rectangle CaptorBox { get; set; }

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
                    SoundManager.Play(evt.Param, -1);
                    break;

                case Message.Captor_Trigger_None:
                    // Do nothing
                    break;
                
                case Message.Captor_Trigger_SendMessageWithParam:
                default:
                    Frame.GetComponent<Scene2D>().GameObjects.Objects[evt.Param & 0xFF].ProcessMessage(msg, evt.Param >> 8);
                    break;
                
                case Message.Captor_Trigger_SendMessageWithCaptorParam:
                    Frame.GetComponent<Scene2D>().GameObjects.Objects[evt.Param & 0xFF].ProcessMessage(msg, this);
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