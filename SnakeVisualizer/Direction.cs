using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JsonConstructorAttribute = Newtonsoft.Json.JsonConstructorAttribute;

namespace SnakeVisualizer

{
    public class Direction
    {
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);
        [JsonProperty("RowOffset")]
        public int RowOffset { get; }
        [JsonProperty("ColumnOffset")]
        public int ColumnOffset { get; }
        [JsonConstructor]
        private Direction(int rowOffset, int columnOffset)
        {
            this.ColumnOffset = columnOffset;
            this.RowOffset = rowOffset;
        }
        public Direction Opposite()
        {
            return new Direction(-this.RowOffset, -this.ColumnOffset);
        }

        public override bool Equals(object? obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColumnOffset == direction.ColumnOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColumnOffset);
        }

        public static bool operator ==(Direction? left, Direction? right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction? left, Direction? right)
        {
            return !(left == right);
        }
    }
}
