using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OnyxCs.Gba;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the first attribute of specified type for the enum value
    /// </summary>
    /// <typeparam name="T">The type of attribute to retrieve</typeparam>
    /// <param name="value">The enum value to get the attribute for</param>
    /// <returns>The attribute instance</returns>
    public static IEnumerable<T> GetAttributes<T>(this Enum value)
        where T : Attribute
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Get the member info for the value
        MemberInfo[] memberInfo = value.GetType().GetMember(value.ToString());

        // Return the attribute
        return memberInfo.First().GetCustomAttributes<T>(false);
    }
}