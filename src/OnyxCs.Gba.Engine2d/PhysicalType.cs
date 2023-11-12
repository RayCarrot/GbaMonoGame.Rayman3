namespace OnyxCs.Gba.Engine2d;

public readonly struct PhysicalType
{
    public PhysicalType(byte value)
    {
        ValueByte = value;
        Value = (PhysicalTypeValue)value;
    }

    public byte ValueByte { get; }
    public PhysicalTypeValue Value { get; }

    public bool IsFullySolid => ValueByte < 16;
    public bool IsAngledSolid => ValueByte is >= 16 and < 32;
    public bool IsSolid => ValueByte < 32;

    public static implicit operator byte(PhysicalType type) => type.ValueByte;
    public static implicit operator PhysicalTypeValue(PhysicalType type) => type.Value;
}