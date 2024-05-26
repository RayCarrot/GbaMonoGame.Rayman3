﻿using System;
using BinarySerializer.Nintendo.GBA;

namespace GbaMonoGame.Engine2d;

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

    public float GetAngleSolidHeight(float xPos)
    {
        float subTileX = MathHelpers.Mod(xPos, Constants.TileSize);

        // In the game this is done using pre-calculated arrays and rounded down to nearest integer
        return Value switch
        {
            PhysicalTypeValue.SolidAngle90Left => subTileX,
            PhysicalTypeValue.SolidAngle90Right => Constants.TileSize - subTileX - 1,

            PhysicalTypeValue.SolidAngle30Left1 => subTileX / 2,
            PhysicalTypeValue.SlideAngle30Left1 => subTileX / 2,
            PhysicalTypeValue.SolidAngle30Right1 => Constants.TileSize - subTileX / 2 - 0.5f,
            PhysicalTypeValue.SlideAngle30Right1 => Constants.TileSize - subTileX / 2 - 0.5f,
            
            PhysicalTypeValue.SolidAngle30Left2 => subTileX / 2 + Constants.TileSize / 2f,
            PhysicalTypeValue.SlideAngle30Left2 => subTileX / 2 + Constants.TileSize / 2f,
            PhysicalTypeValue.SolidAngle30Right2 => Constants.TileSize - (subTileX / 2 + Constants.TileSize / 2f) - 0.5f,
            PhysicalTypeValue.SlideAngle30Right2 => Constants.TileSize - (subTileX / 2 + Constants.TileSize / 2f) - 0.5f,
            _ => throw new Exception($"The physical value {this} is not angled")
        };
    }

    public bool IsAnglePointSolid(Vector2 position)
    {
        float subTileY = MathHelpers.Mod(position.Y, Constants.TileSize);
        float solidHeight = GetAngleSolidHeight(position.X);

        // In the game this is done using a pre-calculated table, but since we want float-precision we calculate it dynamically
        return subTileY >= Constants.TileSize - solidHeight;
    }

    public static implicit operator byte(PhysicalType type) => type.ValueByte;
    public static implicit operator PhysicalTypeValue(PhysicalType type) => type.Value;
    public static implicit operator PhysicalType(PhysicalTypeValue type) => new(type);

    public override string ToString() => Value.ToString();
}