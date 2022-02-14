
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
    public sealed class Hinge : MyGridProgram
    {
        // Your code goes between the next #endregion and #region

        public enum State
        {
            Increase,
            Decrease,
            MoveForward
        };

        public static State state = State.Increase;
        public static float speed = 1f;
        public static string hingeName = "Miner.Hinge";
        public static string motorPrefix = "Miner";
        public static float length = 0f;


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
            if (Equals(GetCurrentLength(pistons), length))
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

        public float GetMaxLength(List<IMyPistonBase> pistons)
        {
            return pistons.Count * 10f;
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
            foreach (var p in pistons)
            {
                p.MaxLimit = 10f;
            }
        }

        public static bool Equal(float a, float b, float diff)
        {
            return Math.Abs(a - b) <= diff;
        }


        public void Main(string args)
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            var hinge = GridTerminalSystem.GetBlockWithName(hingeName) as IMyMotorStator;
            List<IMyPistonBase> pistons = new List<IMyPistonBase>();

            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistons, p => p.CustomName.StartsWith(motorPrefix));

            if (hinge == null)
            {
                Echo("Brak Hinge. Ustaw dobrze nazwe buraku!");

                return;
            }

            Echo($"State = {state} {hinge.LowerLimitRad} | {hinge.Angle} | {hinge.UpperLimitRad}");
            Echo($"Length = {length}");
            Echo($"Real Length = {GetCurrentLength(pistons)} ( {pistons.Count} )");
            switch (state)
            {
                case State.Increase:
                    {
                        hinge.TargetVelocityRPM = speed * 0.25f;
                        if (Equal(hinge.UpperLimitRad, hinge.Angle, 0.01f))
                        {
                            state = State.Decrease;
                        }
                        break;
                    }
                case State.Decrease:
                    {
                        hinge.TargetVelocityRPM = speed * -0.25f;
                        if (Equal(hinge.LowerLimitRad, hinge.Angle, 0.01f))
                        {
                            state = State.MoveForward;
                        }

                        break;
                    }
                case State.MoveForward:
                    {
                        length += 1f;
                        SetLength(pistons, length, 0.1f);
                        state = State.Increase;
                        break;
                    }
            }

        }
        // end
    }
}

