using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace DeSmuMe_Movie_Editor
{
	public class MovieEditor : IDisposable
	{
		private bool is_x64;

		public MovieEditor()
		{
			FrameEdited += FrameEditM;
			DesyncDetected += DesyncM;
		}

		MemoryHacker Mem;
		public int GetMovie(DsmVersionInfo ver, int inst)
		{
			// Get the process
            Process[] emus = null;
            emus = Process.GetProcessesByName(ver.processName);
            if (emus.Length == 0)
            {
                MessageBox.Show(string.Format("Error: Could not find {0}. Please open the program, start a movie, and try again.", ver.processName));
                return 1;
            }
			else if (inst >= emus.Length)
			{
				MessageBox.Show("You selected instance #" + (inst + 1) + ". There are only " + emus.Length + " instances.");
				Mem = null;
				return 2;
			}

			// Setup by version
			is_x64 = ver.is_x64;
			_cfPtr = ver.currentFramePtr;
			_moviePtr = ver.movieRecordsPtr;

            // Set up the memory hacker
			Mem = new MemoryHacker(emus[inst]);
            if (ver.name == "p")
            {
				// I made a custom build that recorded and printed out some addresses for me.
				// So this code is different from how other versions are handled.
				// I don't understand why I made these decisions.

				_cfPtr = ReadLong(ver.currentFramePtr);
				// this pointer actually points to a MovieData struct, hence the +0x70
				_moviePtr = ReadLong(ver.movieRecordsPtr) + 0x70;
            }
			
			// If no movie is playing, complain
            if (recordsStart == 0)
            { 
                MessageBox.Show("The selected instance is not playing a movie.");
                return 3;
            }

			// Set up frame watcher, for re-record count & desync warnings
			System.Threading.Thread t = new System.Threading.Thread(FrameWatch);
			t.Start();

			return 0;
		}
		#region "FrameWatch"
		private int lastFrame;
		public int reRecords = 0;
		private int closestEdit = int.MaxValue;
		private bool rrCounted = false;
		private int desyncFrame = int.MaxValue;
		private void FrameWatch()
		{
			do
			{
				// Working on re-record counter
				if (closestEdit < CurrentFrame && !rrCounted)
				{
					reRecords++;
					if (RerecordIncremented != null)
						RerecordIncremented();
					rrCounted = true;
				}
				if (lastFrame > CurrentFrame)
				{ // Go backwards [e.g. load a save state]
					if (rrCounted)
						closestEdit = int.MaxValue;
					rrCounted = false;
					if (CurrentFrame <= desyncFrame)
						DesyncDetected(-1);
				}

				lastFrame = CurrentFrame;
				System.Threading.Thread.Sleep(2);
			} while (!disposedValue);
		}
		#endregion
		#region "Events"
		public delegate void EvDel(int f);
		public delegate void EvDel2(int f, int c = 1);
		public delegate void EvDel3();
		public event EvDel2 FrameEdited;
		public event EvDel DesyncDetected;
		public event EvDel3 RerecordIncremented;
		private void FrameEditM(int frame, int count = 1)
		{
			if (frame >= CurrentFrame)
			{
				if (frame < closestEdit)
					closestEdit = frame;
			}
			else if (frame + count - 1 >= CurrentFrame && frame + count - 1 < closestEdit)
			{
				closestEdit = CurrentFrame;
				DesyncDetected(frame);
			}
			else
				DesyncDetected(frame);
		}
		private void DesyncM(int frame)
		{
			if (frame < desyncFrame)
				desyncFrame = frame;
			if (frame == -1)
				desyncFrame = int.MaxValue;
		}
		#endregion

		public FrameInput getInput(int index)
		{
			FrameInput f = new FrameInput();
			f.Bytes = Mem.ReadMemory((IntPtr)recordsStart + index * 12, 12);
			return f;
		}
		public void setInput(int index, FrameInput f)
		{
			Mem.WriteMemory((IntPtr)recordsStart + index * 12, f.Bytes);
			FrameEdited(index);
		}
		public void setInput(int index, byte[] f)
		{
			Mem.WriteMemory((IntPtr)recordsStart + index * 12, f);
			FrameEdited(index);
		}
		public void moveInput(int source, int dest, int length = 1)
		{
			IntPtr sPtr = (IntPtr)recordsStart + (source * 12);
			IntPtr dPtr = (IntPtr)recordsStart + (dest * 12);
			length *= 12;
			Mem.WriteMemory(dPtr, Mem.ReadMemory(sPtr, length));
			FrameEdited(dest, length);
		}

		public void deleteFrames(int index, int num)
		{
            if (index + num >= MovieLength)
                return;

			// Copy later values to an earlier place
			IntPtr cPtr = (IntPtr)recordsStart + (index + num) * 12;
			int cLen = (int)(recordsEnd - (long)cPtr);
			byte[] copypaste = Mem.ReadMemory(cPtr, cLen);
			Mem.WriteMemory(cPtr - num * 12, copypaste);
			MovieLength -= num;

			// Clear old values
			FrameInput setTo = new FrameInput();
			for (int i = 0; i < num; i++)
				setInput(i + MovieLength, setTo);
			FrameEdited(index, num);
		}
		public void insertFrames(int index, int num, bool copy)
		{
			if (MovieLength + num >= MaxMovieLength)
			{
				MessageBox.Show("DeSmuMe has not allocated enough memory for this operation!\nMaximum movie length: " + MaxMovieLength +
					"\n\nPlease save the movie and re-load it in DeSmuMe.");
				return;
			}

			// Copy values to a later place
			IntPtr cPtr = (IntPtr)recordsStart + index * 12;
			int cLen = (int)(recordsEnd - (long)cPtr);
			byte[] copypaste = Mem.ReadMemory(cPtr, cLen);
			Mem.WriteMemory(cPtr + num * 12, copypaste);
			MovieLength += num;

			// Clear or copy old values
			FrameInput setTo = new FrameInput();
			if (copy)
				setTo = getInput(index);
			for (int i = 0; i < num; i++)
			{
				int id = i + index;
				setInput(id, setTo);
			}
			FrameEdited(index, num);
		}

		long _cfPtr;
		public int CurrentFrame
		{
			get { return ReadInteger(_cfPtr); }
			set { WriteInteger(_cfPtr, value); }
		}

        long _moviePtr = 0;
		public int MovieLength
		{
			get { return (int)(recordsEnd - recordsStart) / 12; }
			set { recordsEnd = recordsStart + value * 12; }
		}
		public int MaxMovieLength
		{
            get {
				// _moviePtr: beginPtr, endPtr, capacityEndPtr
				long capacityEndPtr;
				if (is_x64) capacityEndPtr = ReadLong(_moviePtr + 0x10);
				else        capacityEndPtr = ReadInteger(_moviePtr + 0x8);

				if (capacityEndPtr == 0)
                    return 0;
                else
                    return (int)(capacityEndPtr - recordsStart) / 12;
            }
        }

		// _moviePtr: beginPtr, endPtr, capacityEndPtr
		long recordsStart
		{
			get
			{
				if (is_x64) return ReadLong(_moviePtr);
				else        return ReadInteger(_moviePtr);
			}
        }
		long recordsEnd
		{
			get
			{
				if (is_x64) return ReadLong(_moviePtr + 0x8);
				else        return ReadInteger(_moviePtr + 0x4);
			}
			set
			{
				if (is_x64) WriteLong(_moviePtr + 0x8, value);
				else        WriteInteger(_moviePtr + 0x4, (int)value);
			}
		}

		// Copy and paste
		int copyStart;
		byte[] copyBy;
		public void setCopyStart(int start)
		{ copyStart = start; }
		public int setCopyEnd(int end)
		{
			IntPtr sPtr = (IntPtr)recordsStart + (copyStart * 12);
			int length = (end - copyStart) * 12;
			copyBy = Mem.ReadMemory(sPtr, length);

			return end - copyStart;
		}
		public void Paste(int frame)
		{
			IntPtr ptr = (IntPtr)recordsStart + (frame * 12);
			Mem.WriteMemory(ptr, copyBy);
		}

		private int ReadInteger(long address)
		{
			byte[] bytes = Mem.ReadMemory((IntPtr)address, 4);

			return BitConverter.ToInt32(bytes, 0); ;
		}
		private void WriteInteger(long address, int value)
		{
			byte[] b = BitConverter.GetBytes(value);
			Mem.WriteMemory((IntPtr)address, b);
		}

        private long ReadLong(long address)
        {
            byte[] bytes = Mem.ReadMemory((IntPtr)address, 8);

            return BitConverter.ToInt64(bytes, 0);
        }
        private void WriteLong(long address, long value)
        {
            byte[] b = BitConverter.GetBytes(value);
            Mem.WriteMemory((IntPtr)address, b);
        }


        public string GenerateSaveString()
		{
			string[] strs = new string[MovieLength];
			for (int i = 0; i < MovieLength; i++)
				strs[i] = getInput(i).GetLine();

			return String.Join("\n", strs);
		}

		public byte[] GetPSave(int start, int end)
		{
			if (start < 0 || end > MovieLength || start >= end)
			{ MessageBox.Show("Start and end must be within the movie, with start being before end."); return null; }

			byte[] sBytes = new byte[(end - start) * 13 + 4];
			// Write length
			BitConverter.GetBytes(end - start).CopyTo(sBytes, 0);
			// Get first frame
			Mem.ReadMemory((IntPtr)recordsStart + start * 12, 12).CopyTo(sBytes, 5);
			int lastFrameAt = 5;
			// Loop through remaining frames
			for (int i = start + 1; i < end; i++)
			{
				byte[] cFrame = Mem.ReadMemory((IntPtr)recordsStart + i * 12, 12);
				bool newFrame = true;
				if (BitConverter.ToInt64(cFrame, 0) == BitConverter.ToInt64(sBytes, lastFrameAt) && BitConverter.ToInt32(cFrame, 8) == BitConverter.ToInt32(sBytes, lastFrameAt + 8))
				{
					if (sBytes[lastFrameAt - 1] < 255)
					{ sBytes[lastFrameAt - 1] += 1; newFrame = false; }
				}
				if (newFrame)
				{
					lastFrameAt += 13;
					cFrame.CopyTo(sBytes, lastFrameAt);
				}
			}
			lastFrameAt += 12;
			Array.Resize(ref sBytes, lastFrameAt);

			return sBytes;
		}
		public int UsePSave(byte[] sBytes, int start, bool replace = false)
		{
            string notEnoughMemoryAllocated = "DeSmuMe has not allocated enough memory for this operation!\nMaximum movie length: " + MaxMovieLength +
                        "\n\nPlease save the movie (and possibly manually add frames), then re-load it in DeSmuMe.";

			// Load data into a new byte array
			int cFrame = 0;
			byte[] lBytes = new byte[12 * BitConverter.ToInt32(sBytes, 0)];
			IntPtr fPtr;
			unsafe { fixed (byte* p = lBytes) fPtr = (IntPtr)p; }
			for (int i = 4; i < sBytes.Length; i += 13)
			{
				for (int iF = -1; iF < sBytes[i]; iF++)
				{
					// Copy data to new array
					System.Runtime.InteropServices.Marshal.Copy(sBytes, i + 1, fPtr, 12);
					fPtr += 12;
					cFrame += 1;
				}
			}

			if (!replace)
			{
				if (MovieLength + cFrame >= MaxMovieLength)
				{
					MessageBox.Show(notEnoughMemoryAllocated);
					return 1;
				}
				// Increase length of movie
				MovieLength += cFrame;
				// Move what was there to later spot
				moveInput(start, start + cFrame, MovieLength - start - cFrame);
			}
			else if (start + cFrame > MaxMovieLength)
			{
				MessageBox.Show(notEnoughMemoryAllocated);
				return 1;
			}
			// Copy the new byte array to the movie
			IntPtr dPtr = (IntPtr)recordsStart + (start * 12);
			Mem.WriteMemory(dPtr, lBytes);
			// Did this go past previous end of movie?
			if (start + cFrame > MovieLength)
				MovieLength = start + cFrame;

			return 0;
		}

		// Modifying stuff, like mirror
		public void MirrorFrame(int frame)
		{
			FrameInput f = getInput(frame);
			bool left = f.leftButton;
			f.leftButton = f.rightButton;
			f.rightButton = left;

			setInput(frame, f);
		}

		private bool disposedValue = false;
		public void Dispose()
		{
			disposedValue = true;
		}
	}
}
