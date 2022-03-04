
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
    public sealed class Rocket : MyGridProgram
    {
        // Your code goes between the next #endregion and #region


        public void Write(string txt)
        {
            foreach(var s in surfaces)
            {
                s.WriteText($"{txt}\n", append: true);
            }
        }

        private static IList<IMyTextSurface> surfaces;

        public void Main(string args)
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            List<IMyCockpit> cockpits = new List<IMyCockpit>();
            GridTerminalSystem.GetBlocksOfType(cockpits, x => x.CubeGrid == Me.CubeGrid);
            var cockpit = cockpits.FirstOrDefault();
            surfaces = new List<IMyTextSurface>();

            for(int i = 0; i < cockpit.SurfaceCount; ++i)
            {
                surfaces.Add(cockpit.GetSurface(i));
                surfaces[i].WriteText("");
            }



            var rot = cockpit.pi

            Write($"{rot.X:#.#}");
            Write($"{rot.Y:#.#}");




        }
        // end
    }
}

