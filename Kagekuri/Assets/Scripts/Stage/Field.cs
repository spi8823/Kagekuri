using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kagekuri
{
    public class Field : MonoBehaviour
    {
        private static GameObject _FieldPrefab = null;
        public Stage Stage { get; private set; }

        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Discription { get; private set; }
        public int Width { get; private set;}
        public int Depth { get; private set; }
        private List<GameObject> _PanelList = new List<GameObject>();

        private Square[,] _Squares { get; set; }

        public IEnumerator<Square> SelectSquare(Point startPosition, Range range, System.Func<Square, bool> selector = null)
        {
            var selectedPosition = startPosition;
            var prefab = Resources.Load<GameObject>("Prefab/Other/MoveDestinationCandidatePanel");
            var list = new List<GameObject>();
            GameObject panel;
            foreach(var item in range.Points)
            {
                panel = Instantiate(prefab);
                var position = item.Key;
                var square = this[position];
                if (square == null)
                    continue;
                position.Z = square.Height - 0.01;
                panel.transform.position = position.ToUnityPosition();
                list.Add(panel);
            }
            _PanelList.AddRange(list);
            prefab = Resources.Load<GameObject>("Prefab/Other/SelectingSquarePanel");
            panel = Instantiate(prefab);
            _PanelList.Add(panel);
            while(!InputController.GetButtonDown(InputController.Button.B))
            {
                if (InputController.GetButtonDown(InputController.Button.A))
                {
                    foreach(var item in range.Points)
                    {
                        if(item.Key.EqualsRoughly(selectedPosition))
                        {
                            var s = this[selectedPosition];
                            if (selector == null || selector(s))
                            {
                                yield return s;
                                yield break;
                            }
                            else
                                break;
                        }
                    }
                }

                selectedPosition += new Point(InputController.GetAxisPeriodicly(InputController.Axis.Cross_Horizontal), InputController.GetAxisPeriodicly(InputController.Axis.Cross_Vertical));
                selectedPosition.X = selectedPosition.X < 0 ? 0 : (Width <= selectedPosition.X ? Width - 1 : selectedPosition.X);
                selectedPosition.Y = selectedPosition.Y < 0 ? 0 : (Depth <= selectedPosition.Y ? Depth - 1 : selectedPosition.Y);
                selectedPosition.Z = this[selectedPosition].Height - 0.005;
                panel.transform.position = selectedPosition.ToUnityPosition();
                var square = this[selectedPosition];
                if (square.Unit is ActiveUnit)
                    StatusPanel.Instance.ShowSub(square.Unit as ActiveUnit);
                else
                    StatusPanel.Instance.HideSub();
                yield return null;
            }

            yield return null;
        }

        public void DeletePanels()
        {
            foreach(var panel in _PanelList)
            {
                Destroy(panel);
            }
        }

        public IEnumerator ViewField(Point startPosition)
        {
            StatusPanel.Instance.HideAll();
            var selectedPosition = startPosition;
            var panel = Instantiate(Resources.Load<GameObject>("Prefab/Other/SelectingSquarePanel"));
            while(!InputController.GetButtonDown(InputController.Button.B))
            {
                if(InputController.GetButtonDown(InputController.Button.A))
                {

                }

                selectedPosition += new Point(InputController.GetAxisPeriodicly(InputController.Axis.Cross_Horizontal), InputController.GetAxisPeriodicly(InputController.Axis.Cross_Vertical));
                selectedPosition.X = selectedPosition.X < 0 ? 0 : (Width - 1 < selectedPosition.X ? Width - 1 : selectedPosition.X);
                selectedPosition.Y = selectedPosition.Y < 0 ? 0 : (Depth - 1 < selectedPosition.Y ? Depth - 1 : selectedPosition.Y);
                selectedPosition.Z = this[selectedPosition].Height - 0.005;
                panel.transform.position = selectedPosition.ToUnityPosition();
                var square = this[selectedPosition];
                if (square.Unit is ActiveUnit)
                    StatusPanel.Instance.ShowSub(square.Unit as ActiveUnit);
                else
                    StatusPanel.Instance.HideSub();
                yield return null;
            }
            Destroy(panel);
            Debug.Log("実装してない");
            yield return null;
        }

        public static Field Create(string filename, Stage stage)
        {
            if (_FieldPrefab == null)
                _FieldPrefab = Resources.Load<GameObject>("Prefab/Stage/FieldPrefab");

            var field = Instantiate(_FieldPrefab).GetComponent<Field>();
            field.Stage = stage;

            var data = JsonConvert.DeserializeObject<FieldData>(Resources.Load<TextAsset>("Stage/FieldData/" + filename).text);
            field.ID = data.ID;
            field.Name = data.Name;
            field.Discription = data.Discription;
            field.Width = data.Width;
            field.Depth = data.Depth;

            field._Squares = new Square[field.Width, field.Depth];
            foreach(var row in data.Squares)
            {
                foreach(var sd in row)
                {
                    var square = Square.Create(sd, field);
                    field._Squares[square.Position.RoundX, square.Position.RoundY] = square;
                    square.transform.SetParent(field.transform);
                }
            }
            field.SetParameters(data.Parameters);
            foreach(var item in data.MapObjects)
            {
                var m = MapObject.Create(item);
                if (m != null)
                    field.Stage.Units.Add(m);
            }

            return field;
        }

        public void SetParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null)
                return;
            foreach(var item in parameters)
            {
                switch(item.Key)
                {
                }
            }
        }

        public bool Contains(Point position)
        {
            if (position.RoundX < 0 || Width - 1 < position.RoundX)
                return false;
            if (position.RoundY < 0 || Depth - 1 < position.RoundY)
                return false;

            return true;
        }

        #region Square
        public Square this[Point location]
        {
            get
            {
                return this[location.RoundX, location.RoundY];
            }
            private set
            {
                this[location.RoundX, location.RoundY] = value;
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

    }

    public class FieldData
    {
        public int ID;
        public string Name;
        public string Discription;
        public Dictionary<string, string> Parameters;
        public int Width;
        public int Depth;
        public Square.SquareData[][] Squares;
        public List<MapObjectData> MapObjects;
    }
}