using System;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DeSmuMe_Movie_Editor
{


    public class MemoryHacker
    {
        //implements IDisposable

        #region "API Definitions"

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory", SetLastError = true)]
        public static extern bool ReadProcessMemory([InAttribute()] IntPtr hProcess, [InAttribute()] IntPtr lpBaseAddress, [Out()] byte[] lpBuffer, uint nSize, [OutAttribute()] uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        // I put this in here. :)
        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, ref IntPtr lpNumberOfBytesWritten);

        #endregion


        #region "Private Fields"

        Process _TargetProcess = null;
        IntPtr _TargetProcessHandle = IntPtr.Zero;
        const uint PROCESS_VM_READ = 16;
        const uint PROCESS_QUERY_INFORMATION = 1024;
        // These two are mine. :)
        const uint PROCESS_VM_WRITE = 32;
        const uint PROCESS_VM_OPERATION = 8;

        #endregion


        #region "public Properties"

        /// <summary>
        /// The process that memory will be read from when ReadMemory is called
        /// </summary>
        public Process TargetProcess
        {
            get { return _TargetProcess; }
        }

        /// <summary>
        /// The handle to the process that was retrieved during the constructor or the last
        /// successful call to the Open method
        /// </summary>
        public IntPtr TargetProcessHandle
        {
            get { return _TargetProcessHandle; }
        }

        #endregion


        #region "Public Methods"

        /// <summary>
        /// Reads the specified number of bytes from an address in the process//s memory.
        /// All memory in the specified range must be available or the method will fail.
        /// returns Nothing if the method fails for any reason
        /// </summary>
        /// <param name="MemoryAddress">The address in the process//s virtual memory to start reading from</param>
        /// <param name="Count">The number of bytes to read</param>
        public byte[] ReadMemory(IntPtr MemoryAddress, int Count)
        {
            byte[] Bytes = new byte[Count];
            bool Result = ReadProcessMemory(_TargetProcessHandle, MemoryAddress, Bytes, (uint)Count, 0);
            if (Result)
                return Bytes;
            else
                return null;
        }

        // I put this here. :)
        public bool WriteMemory(IntPtr MemoryAddress, byte[] val)
        {
            IntPtr zer = IntPtr.Zero;
            bool Result = WriteProcessMemory(_TargetProcessHandle, MemoryAddress, val, (uint)val.Length, ref zer);
            if (Result)
                return true;
            else
            {
                System.Windows.Forms.MessageBox.Show(Marshal.GetLastWin32Error().ToString());
                return false;
            }
        }

        /// <summary>
        /// get {s a handle to the process specified in the TargetProcess property.
        /// A handle is automatically obtained by the constructor of this class but if the Close
        /// method has been called to close a previously obtained handle then another handle can
        /// be obtained by calling this method. If a handle has previously been obtained and Close has
        /// not been called yet then an exception will be thrown.
        /// </summary>
        public void Open()
        {
            if (_TargetProcess == null)
                throw new ApplicationException("Process not found");
            if (_TargetProcessHandle == IntPtr.Zero)
            {
                _TargetProcessHandle = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, true, (uint)_TargetProcess.Id);
                if (_TargetProcessHandle == IntPtr.Zero)
                    throw new ApplicationException("Unable to open process for memory reading. The last error reported was: " + new System.ComponentModel.Win32Exception().Message);
            }
            else
                throw new ApplicationException("A handle to the process has already been obtained, " +
                    "close the existing handle by calling the Close method before calling Open again");
        }

        /// <summary>
        /// Closes a handle that was previously obtained by the constructor or a call to the Open method
        /// </summary>
        public void Close()
        {
            if (_TargetProcessHandle != IntPtr.Zero)
            {
                bool Result = CloseHandle(_TargetProcessHandle);
                if (!Result)
                    throw new ApplicationException("Unable to close process handle. The last error reported was: " +
                                                   new System.ComponentModel.Win32Exception().Message);
                _TargetProcessHandle = IntPtr.Zero;
            }
        }

        #endregion


        #region "constructors"

        /// <summary>
        /// Creates a new instance of the NativeMemoryReader class and attempts to get { a handle to the
        /// process that is to be read by calls to the ReadMemory method.
        /// If a handle cannot be obtained then an exception is thrown
        /// </summary>
        /// <param name="ProcessToRead">The process that memory will be read from</param>
        public MemoryHacker(Process ProcessToRead)
        {
            if (ProcessToRead == null)
                throw new ArgumentNullException("ProcessToRead");
            _TargetProcess = ProcessToRead;
            this.Open();
        }

        #endregion


        //#region "IDisposable Support"

        //    bool disposedValue;

        //    protected override void Dispose(bool disposing)
        //{
        //        if (!this.disposedValue) 
        //        {
        //            if (_TargetProcessHandle != IntPtr.Zero)
        //            {
        //                try                {                    CloseHandle(_TargetProcessHandle); }
        //                catch Exception ex {                    Debug.WriteLine("Error closing handle - " + ex.Message); }

        //            }
        //        }
        //        this.disposedValue = true;
        //        }

        //    Protected Overrides Sub Finalize()
        //        Dispose(False)
        //        MyBase.Finalize()
        //    End Sub

        //    /// <summary>
        //    /// Releases resources and closes any process handles that are still open
        //    /// </summary>
        //    public Sub Dispose() Implements IDisposable.Dispose
        //        Dispose(True)
        //        GC.SuppressFinalize(Me)
        //    End Sub

        //#endregion

    }
}
