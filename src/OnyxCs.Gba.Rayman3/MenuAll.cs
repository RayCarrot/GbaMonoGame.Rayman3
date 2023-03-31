using System;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.Rayman3;

public class MenuAll : Frame
{
    public MenuAll(Page startPage)
    {
        CurrentPage = startPage;
    }

    public Page CurrentPage { get; set; }

    public enum Page
    {

    }

    public override void Init(FrameMngr frameMngr)
    {
        throw new NotImplementedException();
    }

    public override void UnInit()
    {
        throw new NotImplementedException();
    }

    public override void Step()
    {
        throw new NotImplementedException();
    }
}