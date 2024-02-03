namespace GbaMonoGame.Rayman3;

// Right now this class is a skeleton re-implementation of the class in the GBA code. In the future 
// we might be able to implement this to connect to Dolphin through TPC.
public class JoyBus
{
    public bool IsConnected { get; set; }
    public bool HasReceivedData { get; set; }
    public int ReceivedData { get; set; }
    public int Checksum { get; set; }
    public byte ErrorState { get; set; }
    public int Size { get; set; }
    public int RemainingSize { get; set; }

    public void SetRegion(uint gbaRegion, uint gcnRegion)
    {

    }

    public void Disconnect()
    {

    }

    public void Connect()
    {

    }

    public void NewTransfer(int size)
    {

    }

    public void SendValue(int value)
    {

    }

    public bool CheckForLostConnection()
    {
        return false;
    }
}