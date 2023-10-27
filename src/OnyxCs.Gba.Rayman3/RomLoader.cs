using System.IO;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;

namespace OnyxCs.Gba.Rayman3;

public class RomLoader
{
    public RomLoader(Context context, string romFilePath)
    {
        Context = context;
        RomFilePath = romFilePath;
    }

    private Context Context { get; }
    private string RomFilePath { get; }

    public OffsetTable OffsetTable { get; private set; }
    public LevelInfo[] LevelInfo { get; private set; }

    private byte[] ReadBuffer(Stream stream, long offset, long length)
    {
        stream.Position = offset;
        byte[] buffer = new byte[length];
        int read = stream.Read(buffer, 0, buffer.Length);

        if (read != buffer.Length)
            throw new EndOfStreamException();

        return buffer;
    }

    public void Load()
    {
        // TODO: Don't hard-code this
        const int gameDataOffset = 0x29beec;
        const int gameDataLength = 0x55FFB4;
        const int levelInfoPointer = 0x080d4080;

        string romFilePath = Path.GetFullPath(RomFilePath);
        string romFileName = Path.GetFileName(romFilePath);

        // Load the game data into a virtual memory stream file
        const string gameDataName = "GameData";
        using (FileStream file = File.OpenRead(romFilePath))
        {
            byte[] gameData = ReadBuffer(file, gameDataOffset, gameDataLength);
            Context.AddFile(new StreamFile(Context, gameDataName, new MemoryStream(gameData), mode: VirtualFileMode.Maintain));
        }

        // Add the ROM to load other data from it
        Context.AddFile(new MemoryMappedFile(Context, romFileName, baseAddress: Constants.Address_ROM));

        using (Context)
        {
            // Load the game data offset table
            OffsetTable = FileFactory.Read<OffsetTable>(Context, gameDataName);
            Context.AddSettings(new OnyxGbaSettings()
            {
                RootTable = OffsetTable
            });

            // Load level info
            LevelInfo = FileFactory.Read<ObjectArray<LevelInfo>>(
                Context,
                new Pointer(levelInfoPointer, Context.GetRequiredFile(romFileName)), 
                (_, obj) => obj.Pre_Length = 65);
        }
    }
}