using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zword = System.UInt16;
using zbyte = System.Byte;
using MainVM = IFEngine.ZMachineF.ZMachine1To5And7PlusIFEngine;

namespace Frotz.Generic
{

    internal static class FastMem
    {

        internal static MainVM main
        {
            get
            {
                return ZObject.main;
            }
        }

        internal static byte LO(zword v)
        {
            return (byte) (v & 0xff);
        }

        internal static byte HI(zword v)
        {
            return (byte) (v >> 8);
        }

        internal static void SET_WORD(long addr, zword v)
        {
            main.memory[addr] = HI(v);
            main.memory[addr + 1] = LO(v);

            //DebugState.Output("ZMP: {0} -> {1}", addr, v);
        }

        internal static void SET_BYTE(long addr, byte v)
        {
            main.memory[addr] = v;
            //DebugState.Output("ZMP: {0} -> {1}", addr, v);
        }


        internal static void LOW_WORD(long addr, out byte v)
        {
            v = (byte) ((main.memory[addr] << 8) | main.memory[addr + 1]);
        }

        internal static void LOW_WORD(long addr, out zword v)
        {
            v = (ushort) ((main.memory[addr] << 8) | main.memory[addr + 1]);
        }

        internal static void LOW_BYTE(long addr, out byte v)
        {
            v = main.memory[addr];
        }

    }
}
