using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kagekuri
{
    public enum UnitType
    {
        Player, 
    }

    public abstract class Unit
    {
        public const int ParameterCount = 0;

        public Square Square { get; protected set; }
        public Point Location { get { return Square.Location; } }
        public Point Direction { get; protected set; }

        public bool IsAttackable { get; protected set; }

        public Unit(UnitData data, Field field)
        {
            SetSquare(field[data.Location.X, data.Location.Y]);
            Direction = data.Direction;
        }

        public virtual void Attacked(Skill skill) { }

        public virtual void SetSquare(Square square)
        {
            Square = square;
            if (Square.Unit != this)
                Square.SetUnit(this);
        }
    }

    /// <summary>
    /// ActivePoint(行動力)を持っていて、ターンが回ってきて何らかのアクションを起こすユニット
    /// </summary>
    public abstract class ActiveUnit : Unit
    {
        /// <summary>
        /// アクティブポイントの上限
        /// アクティブポイントがこの値になると行動できる
        /// 待機したときはこれの1.5倍まで溜めることができる
        /// </summary>
        public readonly double MaxAP;

        /// <summary>素の素早さ</summary>
        public readonly int RawSpeed;

        public readonly double ChargingAPExpantionRate = 1.5;
        public readonly double ChargingSpeedRate = 0.5;

        /// <summary>
        /// 現在のアクティブポイント
        /// この値がMaxActivePointに達すると行動できるようになる
        /// 待機時はMaxActivePointを超過して溜めることができる
        /// </summary>
        public double AP { get; protected set; }

        /// <summary>
        /// キャラクターの実効素早さ
        /// この値に応じてActivePointが溜まっていく
        /// </summary>
        public int Speed { get; protected set; }

        /// <summary>待機中かどうか</summary>
        public bool IsCharging { get; protected set; }

        public ActiveUnit(ActiveUnitData data, Field field)
            : base(data, field)
        {
            MaxAP = data.RawMaxAP;
            RawSpeed = data.RawSpeed;
            ChargingAPExpantionRate = data.WaitingAPExpantionRate;
            ChargingSpeedRate = data.WaitingSpeedRate;

            AP = 0;
            Speed = RawSpeed;
            IsCharging = false;
        }

        /// <summary>
        /// 行動する
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Act() { yield return true; }

        public bool Wait(double time)
        {
            if (!IsCharging)
            {
                AP += Speed * time;
                if (MaxAP < AP)
                    return true;
            }
            else
            {
                AP += ChargingSpeedRate * Speed * time;
                if (ChargingAPExpantionRate * MaxAP < AP)
                {
                    AP = ChargingAPExpantionRate * MaxAP;
                    return true;
                }
            }

            return false;
        }

        public override void Attacked(Skill skill)
        {
            base.Attacked(skill);
        }

        public double GetWaitTime()
        {
            if (!IsCharging)
                return (MaxAP - AP) / Speed;
            else
            {
                return (ChargingAPExpantionRate * MaxAP - AP) / (ChargingSpeedRate * Speed);
            } 
        }
    }

    /// <summary>
    /// キャラクターユニット
    /// いくつかのパラメータを持っていて、戦闘行動をするユニット
    /// </summary>
    public abstract class CharacterUnit : ActiveUnit
    {
        #region Status
        public int Level { get; protected set; }
        public int ExP { get; protected set; }

        /// <summary>素のHP上限</summary>
        public int RawMaxHP { get; protected set; }
        /// <summary>素のMP上限</summary>
        public int RawMaxMP { get; protected set; }
        /// <summary>素の攻撃力</summary>
        public int RawAttack { get; protected set; }
        /// <summary>素の防御力</summary>
        public int RawDefence { get; protected set; }

        /// <summary>HP上限</summary>
        public int MaxHP { get; protected set; }
        /// <summary>現在のHP</summary>
        public int HP { get; protected set; }
        /// <summary>MP上限</summary>
        public int MaxMP { get; protected set; }
        /// <summary>現在のMP</summary>
        public int MP { get; protected set; }
        /// <summary>現在の攻撃力</summary>
        public int Attack { get; protected set; }
        /// <summary>現在の防御力</summary>
        public int Defence { get; protected set; }
        #endregion

        /// <summary>死んでたらtrue</summary>
        public bool IsDead { get; protected set; }

        /// <summary>技リスト</summary>
        public Move Move { get; protected set; }
        public UseItem UseItem { get; protected set; }
        public Charge Charge { get; protected set; }
        public UseSkill UseSkill { get; protected set; }

        public List<Action> Actions { get; protected set; }

        /// <summary>状態異常リスト</summary>
        public List<Condition> Conditions { get; protected set; }

        public CharacterUnit(CharacterUnitData data, Field field)
            : base(data, field)
        {
            #region Status初期化
            Level = data.Level;
            ExP = data.ExP;

            RawMaxHP = data.RawMaxHP;
            RawMaxMP = data.RawMaxMP;
            RawAttack = data.RawAttack;
            RawDefence = data.RawDefence;

            MaxHP = RawMaxHP;
            HP = MaxHP;
            MaxMP = RawMaxMP;
            MP = MaxMP;
            Attack = RawAttack;
            Defence = RawDefence;
            #endregion

            IsDead = false;

            Move = new Move(this);
            UseItem = new UseItem(this);
            Charge = new Charge(this);
            UseSkill = new UseSkill(this, data.SkillDatas);

            Actions.Add(Move);
            Actions.Add(UseItem);
            Actions.Add(Charge);
            Actions.Add(UseSkill);
        }

        protected virtual void Die()
        {
            IsDead = true;
        }

        public override IEnumerator Act()
        {
            return base.Act();
        }

        public override void Attacked(Skill skill)
        {
            base.Attacked(skill);
        }
    }

    /// <summary>
    /// 操作するユニット
    /// WaitTimeが0になったら何をするかプレイヤーが決める
    /// </summary>
    public abstract class OperableUnit : CharacterUnit
    {
        public OperableUnit(CharacterUnitData data, Field field)
            : base(data, field)
        {

        }

        /// <summary>
        /// 行動する
        /// Bボタンを押すことで行動決定モードとフィールド確認モードとを切り替えることができる
        /// キャンセルとか無い
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Act()
        {
            bool isViewing = false;
            IEnumerator routine = SelectAction();
            while (true)
            {
                //行動選択する
                //選択途中にBボタンで選択モードと眺めるモードが切り替わる
                while (routine.MoveNext())
                {
                    if (InputController.GetButtonDown(InputController.Button.B))
                    {
                        isViewing = !isViewing;
                        if (isViewing)
                            routine = View();
                        else
                            routine = SelectAction();
                    }

                    yield return null;
                }

                //行動が選ばれた（選ばれた行動が使用不可の場合やり直し）
                Action action = (Action)routine.Current;
                if (!action.IsAvailable())
                    continue;

                //行動を実行
                routine = action.Do();
                yield return new WaitWhile(() => routine.MoveNext());

                //行動が完了した
                if ((bool)routine.Current) { }
                //完了しなかった（キャンセルされた）
                else { }

                //APが尽きるか待機が選択されるまで続ける
                if (AP < 1 || action == Charge)
                    break;
            }
            yield break;
        }

        /// <summary>
        /// フィールドの確認をする
        /// </summary>
        /// <returns></returns>
        protected IEnumerator View()
        {
            while(true)
            {
                IEnumerator routine = Square.Field.SelectSquare(Square.Field.GetEntireRange(), Point.Zero, Point.DefaultDirection, Point.Zero);

                while(routine.MoveNext())
                {
                    yield return null;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 行動を選択する
        /// </summary>
        /// <returns></returns>
        protected IEnumerator SelectAction()
        {
            int index = 0;

            while (true)
            {
                //決定ボタンが押されるまで
                while (!InputController.GetButtonDown(InputController.Button.A))
                {
                    int y = InputController.GetAxisDown(InputController.Axis.Cross_Vertical);
                    index -= y;
                    index %= Actions.Count;

                    yield return null;
                }

                //決定ボタンが押されたら
                //行動が使用可能か判定する
                if (Actions[index].IsAvailable())
                {
                    //使用可能ならその行動を返す
                    yield return Actions[index];
                    yield break;
                }
                //使用不可なら
                else
                {
                    //続ける
                    yield return null;
                }
            }
        }

        protected IEnumerator SelectSkill()
        {
            yield break;
        }
    }

    /// <summary>
    /// キャラクターでないユニット（つまりActiveUnitまで）の初期化に使うデータ
    /// </summary>
    public class UnitData
    {
        public string ClassName;
        public Point Location;
        public Point Direction;

        public List<double> Parameters;
    }

    public class ActiveUnitData : UnitData
    {
        public int RawMaxAP;
        public int RawSpeed;
        public double WaitingAPExpantionRate;
        public double WaitingSpeedRate;
    }

    public class CharacterUnitData : ActiveUnitData
    {
        public int Level;
        public int ExP;

        public int RawMaxHP;
        public int RawMaxMP;
        public int RawAttack;
        public int RawDefence;

        public List<SkillData> SkillDatas;
    }
}