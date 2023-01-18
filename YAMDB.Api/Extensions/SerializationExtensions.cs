using System.Text;
using System.Text.Json;

#pragma warning disable CS8603

namespace YAMDB.Api.Extensions;

/// <summary>
///     Extensions for serialization
/// </summary>
public static class SerializationExtensions
{
    /// <summary>
    ///     Convert byte array to object
    /// </summary>
    /// <param name="arrayToDeserialize">The array to deserialize</param>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <returns></returns>
    public static T FromByteArray<T>(this byte[] arrayToDeserialize) where T : class
    {
        if (arrayToDeserialize is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(Encoding.Default.GetString(arrayToDeserialize));
    }

    /// <summary>
    ///     Convert object to byte array
    /// </summary>
    /// <param name="objectToSerialize">The object to serialize</param>
    /// <returns></returns>
    public static byte[]? ToByteArray(this object? objectToSerialize)
    {
        if (objectToSerialize == null)
        {
            return null;
        }

        return Encoding.Default.GetBytes(JsonSerializer.Serialize(objectToSerialize));
    }
}