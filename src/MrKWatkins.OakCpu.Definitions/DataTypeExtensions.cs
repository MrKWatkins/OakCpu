namespace MrKWatkins.OakCpu.Definitions;

/// <summary>
/// Helpers for <see cref="DataType" />.
/// </summary>
public static class DataTypeExtensions
{
    extension(DataType type)
    {
        /// <summary>
        /// Gets the size in bytes for the data type.
        /// </summary>
        [Pure]
        public int Size(bool isArray = false)
        {
            // Arrays are references, so they are 64 bits.
            if (isArray)
            {
                return 8;
            }

            return type switch
            {
                DataType.Void => 0,
                DataType.U8 => 1,
                DataType.I8 => 1,
                DataType.U16 => 2,
                DataType.I32 => 4,
                DataType.I32Bool => 4,
                DataType.Bool => 1,
                _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not supported.")
            };
        }

        /// <summary>
        /// Gets whether the data type is signed.
        /// </summary>
        [Pure]
        public bool IsSigned => type switch
        {
            DataType.U8 => false,
            DataType.I8 => true,
            DataType.U16 => false,
            DataType.I32 => true,
            DataType.I32Bool => true,
            _ => throw new NotSupportedException($"The {nameof(DataType)} {type} is not numeric.")
        };
    }
}