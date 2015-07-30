using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.PInvoke
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "WaitForDebugEvent", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WaitForDebugEvent(
            ref DEBUG_EVENT lpDebugEvent, 
            int dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine, 
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes, 
            bool bInheritHandles,
            ProcessCreationFlags dwCreationFlags, 
            IntPtr lpEnvironment, 
            string lpCurrentDirectory,
            STARTUPINFO lpStartupInfo,
            PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        public static extern bool ContinueDebugEvent(
            uint dwProcessId, 
            uint dwThreadId,
           ContinueStatus dwContinueStatus);

        [DllImport("kernel32.dll")]
        public static extern bool GetThreadContext(
            IntPtr hThread, 
            [In, Out] ref CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(
            ThreadAccess dwDesiredAccess, 
            bool bInheritHandle, 
            uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcessStop(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DebugActiveProcess(uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool DebugSetProcessKillOnExit(bool KillOnExit);

        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            uint dwLength);
    }
}
