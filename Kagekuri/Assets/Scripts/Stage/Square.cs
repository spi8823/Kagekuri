using UnityEngine;
using System.Collections;

namespace Kagekuri
{
    public class Square
    {
        public Field Field { get; private set; }
        public Unit Unit { get; private set; }

        public SquareType Type { get; private set; }
        public Point Location { get; set; }

        public Square(SquareData data, Field field)
        {
            Field = field;
            Unit = null;

            Type = data.Type;
            Location = new Point(data.Location);
        }

        public void SetUnit(Unit unit)
        {
            Unit = unit;
        }
    }

    public class SquareData
    {
        public SquareType Type { get; set; }

        public Point Location { get; set; }

        public SquareData() { }
    }

    public enum SquareType
    {
        Null, Normal, 
    }
}