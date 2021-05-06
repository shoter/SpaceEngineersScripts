
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

// Change this namespace for each script you create.
namespace SpaceEngineers.UWBlockPrograms.Miner
{
    public sealed class Program : MyGridProgram
    {
        // Your code goes between the next #endregion and #region

        enum State
        {
            ZDown,
            ZUp,
            Inc,
            PositionChange,
            Idle
        }

        static State state = State.ZDown;
        static float x = 0;
        static float y = 0;
        static float step = 3;
        public float zSpeed = 0.2f;
        public float speed = 2f;


        public Program()
        {
        }

        public int GetLength(List<IMyPistonBase> pistons) => pistons.Count * 10;

        public float GetCurrentLength(List<IMyPistonBase> pistons)
        {
            float current = 0;
            for (int i = 0; i < pistons.Count; ++i)
            {
                current += pistons[i].CurrentPosition;
            }

            return current;
        }

        public void SetLength(List<IMyPistonBase> pistons, float length, float speed)
        {
            if(Equals(GetCurrentLength(pistons), length))
            {
                return;
            }
            for (int i = 0; i < pistons.Count; ++i)
            {
                if (length > 0)
                {
                    float appliedLength = length > 10 ? 10 : length;
                    float velocity = length > pistons[i].CurrentPosition ? speed : -speed;

                    pistons[i].Velocity = velocity;
                    pistons[i].MaxLimit = appliedLength;

                    length -= appliedLength;
                }
                else
                {
                    pistons[i].Velocity = -speed;
                    pistons[i].MinLimit = 0;
                }
            }
        }

        public void SetMaxLength(List<IMyPistonBase> pistons, float speed)
        {
            for (int i = 0; i < pistons.Count; ++i)
            {
                pistons[i].Velocity = speed;
                pistons[i].MaxLimit = 10f;
            }
        }

        public bool IsMaxLengthReached(List<IMyPistonBase> pistons)
        {
            for (int i = 0; i < pistons.Count; ++i)
            {
                if (Math.Abs(pistons[i].CurrentPosition - 10) > 0.01f)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsMinLengthReached(List<IMyPistonBase> pistons)
        {
            for (int i = 0; i < pistons.Count; ++i)
            {
                if (Math.Abs(pistons[i].CurrentPosition - 0) > 0.01f)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsLengthReached(List<IMyPistonBase> pistons, float length)
        {
            float current = GetCurrentLength(pistons);

            if (Math.Abs(current - length) < 0.01f)
            {
                return true;
            }
            return false;
        }



        public void Reset(List<IMyPistonBase> pistons, float speed)
        {
            for (int i = 0; i < pistons.Count; ++i)
            {
                pistons[i].Velocity = -speed;
                pistons[i].MinLimit = 0f;
            }
        }

        public bool Equal(float a, float b)
        {
            return Math.Abs(a - b) < 0.01f;
        }

        public void SetUp(List<IMyPistonBase> pistons)
        {
            foreach(var p in pistons)
            {
                p.MaxLimit = 10f;
            }
        }



        public void Main(string args)
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            List<IMyPistonBase> xPistons = new List<IMyPistonBase>();
            List<IMyPistonBase> yPistons = new List<IMyPistonBase>();
            List<IMyPistonBase> zPistons = new List<IMyPistonBase>();
            List<IMyPistonBase> all = new List<IMyPistonBase>();

            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(xPistons, p => p.CustomName.Contains("X") && p.CustomName.StartsWith("Miner"));
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(yPistons, p => p.CustomName.Contains("Y") && p.CustomName.StartsWith("Miner"));
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(zPistons, p => p.CustomName.Contains("Z") && p.CustomName.StartsWith("Miner"));
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(all);

            Echo($"State = {state}");
            Echo($"{x}, {y}");
            Echo($"x = {GetCurrentLength(xPistons)}");
            Echo($"y = {GetCurrentLength(yPistons)}");
            Echo($"z = {GetCurrentLength(zPistons)}");

            SetLength(xPistons, x, speed);
            SetLength(yPistons, y, speed);



            switch (state)
            {
                case State.ZDown:
                    {
                        SetMaxLength(zPistons, zSpeed);
                        if (IsMaxLengthReached(zPistons))
                        {
                            state = State.ZUp;
                        }
                        break;
                    }
                case State.ZUp:
                    {
                        Reset(zPistons, zSpeed * 2);
                        if (IsMinLengthReached(zPistons))
                        {
                            state = State.Inc;
                        }

                        break;
                    }

                case State.Inc:
                    {
                        x += step;
                        if(x >= 30f)
                        {
                            x = 0f;
                            y += step;
                        }

                        if(y > 30f)
                        {
                            state = State.Idle;
                        };

                        state = State.PositionChange;
                        SetLength(xPistons, x, speed);
                        SetLength(yPistons, y, speed);

                        break;
                    }
                case State.PositionChange:
                    {
                        if(IsLengthReached(xPistons, x) && IsLengthReached(yPistons, y))
                        {
                            state = State.ZDown;
                        }

                        break;
                    }

            }
        }
    }
}
