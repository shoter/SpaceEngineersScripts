
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
namespace SpaceEngineers.UWBlockPrograms.BatteryMonitor
{
    public sealed class Program : MyGridProgram
    {
        // Your code goes between the next #endregion and #region

        public enum ThrustDirection
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        }

        public static Vector3D GetThrustVector(ThrustDirection dir)
        {
            switch(dir)
            {
                case ThrustDirection.Forward: return Vector3D.Forward;
                case ThrustDirection.Backward: return Vector3D.Backward;
                case ThrustDirection.Left: return Vector3D.Left;
                case ThrustDirection.Right: return Vector3D.Right;
                case ThrustDirection.Up: return Vector3D.Up;
                case ThrustDirection.Down: return Vector3D.Down;
            }

            throw new Exception();
        }

        public static ThrustDirection GetThrustDirection(Vector3D vector)
        {
            if(vector == Vector3D.Forward)
            {
                return ThrustDirection.Forward;
            }
            if (vector == Vector3D.Backward)
            {
                return ThrustDirection.Backward;
            }
            if (vector == Vector3D.Left)
            {
                return ThrustDirection.Left;
            }
            if (vector == Vector3D.Right)
            {
                return ThrustDirection.Right;
            }
            if (vector == Vector3D.Up)
            {
                return ThrustDirection.Up;
            }
            if (vector == Vector3D.Down)
            {
                return ThrustDirection.Down;
            }
            throw new Exception();
        }

        public Program()
        {
        }



        public void Main(string args)
        {
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
                List<IMyThrust> thrusters = new List<IMyThrust>();
                GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters);
                List<IMyCockpit> cockpits = new List<IMyCockpit>();
                GridTerminalSystem.GetBlocksOfType(cockpits);


                var cockpit = cockpits.FirstOrDefault();



                Dictionary<ThrustDirection, double> thrust = new Dictionary<ThrustDirection, double>();

                foreach (ThrustDirection t in Enum.GetValues(typeof(ThrustDirection)).Cast<ThrustDirection>())
                {
                    thrust[t] = 0;
                }


            foreach (var t in thrusters)
                {
                    if (t.IsFunctional)
                    {
                        ThrustDirection d = GetThrustDirection(t.GridThrustDirection);
                        thrust[d] += t.MaxEffectiveThrust;
                    }
                }

                Dictionary<ThrustDirection, double> maxSpeed = new Dictionary<ThrustDirection, double>();

                foreach (ThrustDirection t in Enum.GetValues(typeof(ThrustDirection)).Cast<ThrustDirection>())
                {
                    maxSpeed[t] = 0;
                }

                string o = "";

                double g = cockpits[0].GetNaturalGravity().Length();

                double m = cockpits[0].CalculateShipMass().TotalMass;

                double Fg = (m * cockpits[0].GetNaturalGravity().Length());

                foreach (ThrustDirection t in Enum.GetValues(typeof(ThrustDirection)).Cast<ThrustDirection>())
                {
                    maxSpeed[t] = (thrust[t] - Fg) / cockpits[0].CalculateShipMass().TotalMass;
                    o += $"{t} = {maxSpeed[t]}\n";
                }

                string thrustOutput = PrintThrust(thrust, maxSpeed, g, m, "U", ThrustDirection.Down);
                thrustOutput += PrintThrust(thrust, maxSpeed, g, m, "B", ThrustDirection.Forward);
                thrustOutput += "\n";
                thrustOutput += PrintThrust(thrust, maxSpeed, g, m, "F", ThrustDirection.Backward);
                thrustOutput += PrintThrust(thrust, maxSpeed, g, m, "L", ThrustDirection.Right);
                thrustOutput += PrintThrust(thrust, maxSpeed, g, m, "R", ThrustDirection.Left);
                thrustOutput += PrintThrust(thrust, maxSpeed, g, m, "D", ThrustDirection.Up);

                var surface = cockpits[0].GetSurface(0);


                surface.WriteText(thrustOutput);
                Echo(thrustOutput);
        }

        private static string PrintThrust(Dictionary<ThrustDirection, double> thrust, Dictionary<ThrustDirection, double> maxSpeed, double g, double m, string dir, ThrustDirection direction)
        {
            return $"{dir} = {maxSpeed[direction]:0.##} | {thrust[direction] / m:0.##} | {thrust[direction] / g / 1000:0.##}T \n";
        }

    }
}
