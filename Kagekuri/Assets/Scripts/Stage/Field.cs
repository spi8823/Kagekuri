using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public class Field
    {
        public Stage Stage { get; private set; }

        public readonly int Width;
        public readonly int Height;
        public readonly Point Size;

        private Square[,] _Squares;

        public Field(FieldData data, Stage stage)
        {
            Stage = stage;
            Size = data.Size;

            _Squares = new Square[Size.X, Size.Y];

            foreach (var i in data.SquareDatas)
                foreach (var j in i)
                {
                    var square = new Square(j, this);
                    this[square.Location.X, square.Location.Y] = square;
                }
        }

        #region マス目選択
        /// <summary>
        /// マス目の選択を始める
        /// </summary>
        /// <param name="range"></param>
        /// <param name="center"></param>
        public void StartSelectingSquare(Range range, Point center, Point direction)
        {
            Debug.Log("実装されてないよ！！");
        }

        /// <summary>
        /// マス目の選択を終わる
        /// </summary>
        public void EndSelectingSuqare()
        {
            Debug.Log("実装されてないよ！！");
        }

        /// <summary>
        /// 今カーソルされているマス目を返す
        /// </summary>
        /// <returns></returns>
        public IEnumerator SelectSquare(Range range, Point center, Point direction, Point initialPos)
        {
            StartSelectingSquare(range, center, direction);

            Point now = initialPos;

            while(true)
            {
                if (InputController.GetButtonDown(InputController.Button.A))
                {
                    if (range.IsContains(now - center, direction))
                    {
                        EndSelectingSuqare();
                        yield break;
                    }
                }

                int x = InputController.GetAxisDown(InputController.Axis.Cross_Horizontal);
                int y = InputController.GetAxisDown(InputController.Axis.Cross_Vertical);

                Point next = now + new Point(x, y);
                if (this[next] != null)
                    now = next;

                yield return now;
            }
        }
        #endregion

        #region Square
        public Square this[Point location]
        {
            get
            {
                return this[location.X, location.Y];
            }
            private set
            {
                this[location.X, location.Y] = value;
            }
        }

        public Square this[int i, int j]
        {
            get
            {
                if(i < 0 || _Squares.GetLength(0) - 1 < i || j < 0 || _Squares.GetLength(1) - 1 < j)
                {
                    Debug.Log("そんなSquareは無いよ(i = " + i + ", j = " + j + ")");
                    return null;
                }
                return _Squares[i, j];
            }
            private set
            {
                if (i < 0 || _Squares.GetLength(0) - 1 < i || j < 0 || _Squares.GetLength(1) - 1 < j)
                {
                    Debug.Log("そんなSquareは無いよ(i = " + i + ", j = " + j + ")");
                    return;
                }
                _Squares[i, j] = value;
            }
        }
        #endregion

        public Range GetEntireRange()
        {
            Range range = new Range();
            
            for(var i = 0;i < Width;i++)
                for(var j = 0;j < Height;j++)
                {
                    if (this[i, j] != null)
                        range.Add(new Point(i, j), 1);
                }

            return range;
        }
    }
    
    public class FieldData
    {
        public Point Size { get; set; }

        public SquareData[][] SquareDatas { get; set; }

        public FieldData() { }
    }
}