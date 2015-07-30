using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.PInvoke
{
    public class ManagedAllocation : IDisposable
    {
        public IntPtr Pointer { get; private set; }
        public int Size { get; private set; }

        private ManagedAllocation() { }

        public static ManagedAllocation Create(object structure)
        {
            var s = new ManagedAllocation();
            s.Size = Marshal.SizeOf(structure);
            s.Pointer = Marshal.AllocHGlobal(s.Size);
            Marshal.StructureToPtr(structure, s.Pointer, false);
            return s;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Marshal.FreeHGlobal(Pointer);
            Pointer = IntPtr.Zero;
        }

        #endregion
    }
}
