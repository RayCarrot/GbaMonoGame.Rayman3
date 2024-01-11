using System;
using BinarySerializer.Nintendo.GBA;

namespace OnyxCs.Gba.Engine2d;

public readonly struct PhysicalType
{
    public PhysicalType(PhysicalTypeValue type)
    {
        ValueByte = (byte)type;
        Value = type;
    }
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

    public float GetAngleHeight(float xPos)
    {
        float subTileX = MathHelpers.Mod(xPos, Constants.TileSize);

        // In the game this is done using pre-calculated arrays
        return Value switch
        {
            PhysicalTypeValue.SolidAngle90Right => subTileX,
            PhysicalTypeValue.SolidAngle90Left => Constants.TileSize - subTileX - 1,
            PhysicalTypeValue.SolidAngle30Left1 => subTileX / 2,
            PhysicalTypeValue.SlideAngle30Left1 => subTileX / 2,
            PhysicalTypeValue.SolidAngle30Left2 => subTileX / 2 + Constants.TileSize / 2,
            PhysicalTypeValue.SlideAngle30Left2 => subTileX / 2 + Constants.TileSize / 2,
            PhysicalTypeValue.SolidAngle30Right1 => Constants.TileSize - subTileX / 2,
            PhysicalTypeValue.SlideAngle30Right1 => Constants.TileSize - subTileX / 2,
            PhysicalTypeValue.SolidAngle30Right2 => Constants.TileSize - (subTileX / 2 + Constants.TileSize / 2),
            PhysicalTypeValue.SlideAngle30Right2 => Constants.TileSize - (subTileX / 2 + Constants.TileSize / 2),
            _ => throw new Exception($"The physical value {this} is not angled")
        };
    }

    public static implicit operator byte(PhysicalType type) => type.ValueByte;
    public static implicit operator PhysicalTypeValue(PhysicalType type) => type.Value;
    public static implicit operator PhysicalType(PhysicalTypeValue type) => new PhysicalType(type);

    public override string ToString() => Value.ToString();
}