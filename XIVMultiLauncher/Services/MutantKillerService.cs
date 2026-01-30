using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XIVMultiLauncher.Services;

/// <summary>
/// Kills FFXIV mutant handles to allow more than 2 game instances.
/// Based on: https://scorpiosoftware.net/2020/03/15/how-can-i-close-a-handle-in-another-process/
/// </summary>
public class MutantKillerService
{
    private const string FFXIV_PROCESS_NAME = "ffxiv_dx11";
    private const string FFXIV_MUTANT_PREFIX = "ffxiv_game";

    #region Native API Declarations

    [DllImport("ntdll.dll")]
    private static extern NtStatus NtQuerySystemInformation(
        SystemInformationClass systemInformationClass,
        IntPtr systemInformation,
        uint systemInformationLength,
        out uint returnLength);

    [DllImport("ntdll.dll")]
    private static extern NtStatus NtQueryObject(
        IntPtr handle,
        ObjectInformationClass objectInformationClass,
        IntPtr objectInformation,
        uint objectInformationLength,
        out uint returnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
        ProcessAccessFlags processAccess,
        bool inheritHandle,
        int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DuplicateHandle(
        IntPtr sourceProcessHandle,
        IntPtr sourceHandle,
        IntPtr targetProcessHandle,
        out IntPtr targetHandle,
        uint desiredAccess,
        bool inheritHandle,
        DuplicateOptions options);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    private enum NtStatus : uint
    {
        Success = 0x00000000,
        InfoLengthMismatch = 0xC0000004
    }

    private enum SystemInformationClass
    {
        SystemHandleInformation = 16,
        SystemExtendedHandleInformation = 64
    }

    private enum ObjectInformationClass
    {
        ObjectBasicInformation = 0,
        ObjectNameInformation = 1,
        ObjectTypeInformation = 2
    }

    [Flags]
    private enum ProcessAccessFlags : uint
    {
        DupHandle = 0x0040,
        QueryInformation = 0x0400
    }

    [Flags]
    private enum DuplicateOptions : uint
    {
        CloseSource = 0x00000001,
        SameAccess = 0x00000002
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SystemHandleTableEntryInfoEx
    {
        public IntPtr Object;
        public UIntPtr UniqueProcessId;
        public UIntPtr HandleValue;
        public uint GrantedAccess;
        public ushort CreatorBackTraceIndex;
        public ushort ObjectTypeIndex;
        public uint HandleAttributes;
        public uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UnicodeString
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ObjectNameInformation
    {
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ObjectTypeInformation
    {
        public UnicodeString TypeName;
        // Additional fields omitted - we only need the type name
    }

    #endregion

    /// <summary>
    /// Finds and closes FFXIV mutant handles to allow launching more than 2 instances.
    /// </summary>
    /// <returns>Number of mutants killed</returns>
    public int KillFfxivMutants()
    {
        var ffxivProcesses = Process.GetProcessesByName(FFXIV_PROCESS_NAME);
        if (ffxivProcesses.Length == 0)
            return 0;

        var ffxivPids = new HashSet<int>(ffxivProcesses.Select(p => p.Id));
        var killedCount = 0;

        // Get all system handles
        var handles = GetSystemHandles();
        if (handles == null)
            return 0;

        // Group handles by process for efficiency
        var handlesByProcess = handles
            .Where(h => ffxivPids.Contains((int)(ulong)h.UniqueProcessId))
            .GroupBy(h => (int)(ulong)h.UniqueProcessId);

        foreach (var processGroup in handlesByProcess)
        {
            var pid = processGroup.Key;
            var processHandle = OpenProcess(
                ProcessAccessFlags.DupHandle | ProcessAccessFlags.QueryInformation,
                false,
                pid);

            if (processHandle == IntPtr.Zero)
                continue;

            try
            {
                foreach (var handleInfo in processGroup)
                {
                    if (TryKillMutantHandle(processHandle, handleInfo))
                        killedCount++;
                }
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        return killedCount;
    }

    private bool TryKillMutantHandle(IntPtr processHandle, SystemHandleTableEntryInfoEx handleInfo)
    {
        var sourceHandle = (IntPtr)(ulong)handleInfo.HandleValue;

        // Duplicate handle to our process to query it
        if (!DuplicateHandle(
            processHandle,
            sourceHandle,
            GetCurrentProcess(),
            out var duplicatedHandle,
            0,
            false,
            DuplicateOptions.SameAccess))
        {
            return false;
        }

        try
        {
            // Check if it's a Mutant type
            var typeName = GetObjectTypeName(duplicatedHandle);
            if (typeName != "Mutant")
                return false;

            // Check if it's an FFXIV mutant
            var objectName = GetObjectName(duplicatedHandle);
            if (string.IsNullOrEmpty(objectName) || !objectName.Contains(FFXIV_MUTANT_PREFIX))
                return false;

            // Close our duplicated handle first
            CloseHandle(duplicatedHandle);
            duplicatedHandle = IntPtr.Zero;

            // Now close the source handle using DuplicateHandle with DUPLICATE_CLOSE_SOURCE
            DuplicateHandle(
                processHandle,
                sourceHandle,
                IntPtr.Zero,
                out _,
                0,
                false,
                DuplicateOptions.CloseSource);

            return true;
        }
        finally
        {
            if (duplicatedHandle != IntPtr.Zero)
                CloseHandle(duplicatedHandle);
        }
    }

    private string? GetObjectTypeName(IntPtr handle)
    {
        const int bufferSize = 1024;
        var buffer = Marshal.AllocHGlobal(bufferSize);

        try
        {
            var status = NtQueryObject(
                handle,
                ObjectInformationClass.ObjectTypeInformation,
                buffer,
                bufferSize,
                out _);

            if (status != NtStatus.Success)
                return null;

            var typeInfo = Marshal.PtrToStructure<ObjectTypeInformation>(buffer);
            if (typeInfo.TypeName.Buffer == IntPtr.Zero || typeInfo.TypeName.Length == 0)
                return null;

            return Marshal.PtrToStringUni(typeInfo.TypeName.Buffer, typeInfo.TypeName.Length / 2);
        }
        catch
        {
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private string? GetObjectName(IntPtr handle)
    {
        const int bufferSize = 2048;
        var buffer = Marshal.AllocHGlobal(bufferSize);

        try
        {
            var status = NtQueryObject(
                handle,
                ObjectInformationClass.ObjectNameInformation,
                buffer,
                bufferSize,
                out _);

            if (status != NtStatus.Success)
                return null;

            var nameInfo = Marshal.PtrToStructure<ObjectNameInformation>(buffer);
            if (nameInfo.Name.Buffer == IntPtr.Zero || nameInfo.Name.Length == 0)
                return null;

            return Marshal.PtrToStringUni(nameInfo.Name.Buffer, nameInfo.Name.Length / 2);
        }
        catch
        {
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private List<SystemHandleTableEntryInfoEx>? GetSystemHandles()
    {
        uint bufferSize = 1024 * 1024; // Start with 1MB
        var buffer = IntPtr.Zero;

        try
        {
            // Keep increasing buffer size until we get all handles
            for (var i = 0; i < 10; i++)
            {
                buffer = Marshal.AllocHGlobal((int)bufferSize);

                var status = NtQuerySystemInformation(
                    SystemInformationClass.SystemExtendedHandleInformation,
                    buffer,
                    bufferSize,
                    out var returnLength);

                if (status == NtStatus.Success)
                {
                    return ParseHandleInformation(buffer);
                }

                if (status == NtStatus.InfoLengthMismatch)
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = IntPtr.Zero;
                    bufferSize = returnLength + 1024 * 1024; // Add extra buffer
                    continue;
                }

                // Other error
                return null;
            }

            return null;
        }
        finally
        {
            if (buffer != IntPtr.Zero)
                Marshal.FreeHGlobal(buffer);
        }
    }

    private List<SystemHandleTableEntryInfoEx> ParseHandleInformation(IntPtr buffer)
    {
        var handles = new List<SystemHandleTableEntryInfoEx>();

        // First field is the count (IntPtr-sized)
        var handleCount = (long)Marshal.ReadIntPtr(buffer);

        // Skip count and reserved field (both IntPtr-sized)
        var entryOffset = IntPtr.Size * 2;
        var entrySize = Marshal.SizeOf<SystemHandleTableEntryInfoEx>();

        for (long i = 0; i < handleCount; i++)
        {
            var entryPtr = IntPtr.Add(buffer, entryOffset + (int)(i * entrySize));
            var entry = Marshal.PtrToStructure<SystemHandleTableEntryInfoEx>(entryPtr);
            handles.Add(entry);
        }

        return handles;
    }
}
