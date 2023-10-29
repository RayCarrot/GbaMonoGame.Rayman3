using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.Engine2d;

public class Captor : GameObject
{
    public Captor(int id, CaptorResource captorResource) : base(id, captorResource)
    {
        CaptorBox = new Rectangle(
            x: captorResource.BoxMinX, 
            y: captorResource.BoxMinY,
            width: captorResource.BoxMaxX - captorResource.BoxMinX, 
            height: captorResource.BoxMaxY - captorResource.BoxMinY);
    }

    public Rectangle CaptorBox { get; set; }
}