
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



            Dictionary<ThrustDirection, double> thrust = new Dictionary<ThrustDirection, double>();

            foreach(ThrustDirection t in Enum.GetValues(typeof(ThrustDirection)).Cast<ThrustDirection>())
            {
                thrust[t] = 0;
            }

            foreach (var t in thrusters)
            {
                if (t.IsFunctional)
                {
                    ThrustDirection d = GetThrustDirection(t.GridThrustDirection);
                    thrust[d] += t.MaxThrust;
                }
            }

            Dictionary<ThrustDirection, double> maxSpeed = new Dictionary<ThrustDirection, double>();

            foreach (ThrustDirection t in Enum.GetValues(typeof(ThrustDirection)).Cast<ThrustDirection>())
            {
                maxSpeed[t] = 0;
            }

            string o = "";

            double g = cockpits[0].GetNaturalGravity().Length();

            double Fg = (cockpits[0].CalculateShipMass().TotalMass * cockpits[0].GetNaturalGravity().Length());

            foreach (ThrustDirection t in Enum.GetValues(typeof(ThrustDirection)).Cast<ThrustDirection>())
            {
                maxSpeed[t] = (thrust[t] - Fg) / cockpits[0].CalculateShipMass().TotalMass;
                o += $"{t} = {maxSpeed[t]}\n";
            }

            string thrustOutput = $"M = {cockpits[0].CalculateShipMass().TotalMass / 1000:0.##}T\n";


            thrustOutput += $"U = {maxSpeed[ThrustDirection.Down]:0.##} - {thrust[ThrustDirection.Down] / g / 1000:0.##}T \n";
            thrustOutput += $"F = {maxSpeed[ThrustDirection.Backward]:0.##} - {thrust[ThrustDirection.Backward] / g / 1000:0.##}T \n\n";
            thrustOutput += $"L = {maxSpeed[ThrustDirection.Right]:0.##} - {thrust[ThrustDirection.Right] / g / 1000:0.##}T \n";
            thrustOutput += $"R = {maxSpeed[ThrustDirection.Left]:0.##} - {thrust[ThrustDirection.Left] / g / 1000:0.##}T \n";
            thrustOutput += $"D = {maxSpeed[ThrustDirection.Up]:0.##} - {thrust[ThrustDirection.Up] / g / 1000:0.##}T \n";
            thrustOutput += $"B = {maxSpeed[ThrustDirection.Forward]:0.##} - {thrust[ThrustDirection.Forward] / g / 1000:0.##}T \n";


            var surface = cockpits[0].GetSurface(0);


            surface.WriteText(thrustOutput);
            Echo(thrustOutput);
        }


    }
}
