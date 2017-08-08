using System;
using System.Text;
using System.Threading.Tasks;

namespace DeSmuMe_Movie_Editor
{
	public class FrameInput
	{

		static readonly byte[] Bits = { 1, 2, 4, 8, 16, 32, 64, 128 };
		// 12 bytes per (0th-11th)
		public byte[] Bytes = new byte[12];
		private void SetBit(Byte ByteID, Byte BitID, bool OnOff)
		{
			byte bOnOff = 0;
			if (OnOff) { bOnOff = 255; }
			Bytes[ByteID] = (byte)((Bits[BitID] & bOnOff) | (Bytes[ByteID] & ~Bits[BitID]));
		}


		// 0th-1st bytes
		#region "Button Properties"
		// 0th bit of 0th byte
		public bool gButton
		{
			get { return (Bytes[0] | Bits[0]) == Bytes[0]; }
			set { SetBit(0, 0, value); }
		}
		// 1st bit of 0th byte
		public bool RButton
		{
			get { return (Bytes[0] | Bits[1]) == Bytes[0]; }
			set { SetBit(0, 1, value); }
		}
		// 2nd bit of 0th byte
		public bool LButton
		{
			get { return (Bytes[0] | Bits[2]) == Bytes[0]; }
			set { SetBit(0, 2, value); }
		}
		// 3rd bit of 0th byte
		public bool XButton
		{
			get { return (Bytes[0] | Bits[3]) == Bytes[0]; }
			set { SetBit(0, 3, value); }
		}
		// 4th bit of 0th byte
		public bool YButton
		{
			get { return (Bytes[0] | Bits[4]) == Bytes[0]; }
			set { SetBit(0, 4, value); }
		}
		// 5th bit of 0th byte
		public bool AButton
		{
			get { return (Bytes[0] | Bits[5]) == Bytes[0]; }
			set { SetBit(0, 5, value); }
		}
		// 6th bit of 0th byte
		public bool BButton
		{
			get { return (Bytes[0] | Bits[6]) == Bytes[0]; }
			set { SetBit(0, 6, value); }
		}
		// 7th bit of 0th byte
		public bool StartButton
		{
			get { return (Bytes[0] | Bits[7]) == Bytes[0]; }
			set { SetBit(0, 7, value); }
		}
		// 0th bit of 1st byte
		public bool selectButton
		{
			get { return (Bytes[1] | Bits[0]) == Bytes[1]; }
			set { SetBit(1, 0, value); }
		}
		// 1st bit of 1st byte
		public bool upButton
		{
			get { return (Bytes[1] | Bits[1]) == Bytes[1]; }
			set { SetBit(1, 1, value); }
		}
		// 2nd bit of 1st byte
		public bool downButton
		{
			get { return (Bytes[1] | Bits[2]) == Bytes[1]; }
			set { SetBit(1, 2, value); }
		}
		// 3rd bit of 1st byte
		public bool leftButton
		{
			get { return (Bytes[1] | Bits[3]) == Bytes[1]; }
			set { SetBit(1, 3, value); }
		}
		// 4th bit of 1st byte
		public bool rightButton
		{
			get { return (Bytes[1] | Bits[4]) == Bytes[1]; }
			set { SetBit(1, 4, value); }
		}
		// 5th-7th bits of 1st byte are nothing
		#endregion

		#region "Buttons"
		public const int _gButton = 1;
		public const int _RButton = 2;
		public const int _LButton = 4;
		public const int _XButton = 8;
		public const int _YButton = 0x10;
		public const int _AButton = 0x20;
		public const int _BButton = 0x40;
		public const int _StartButton = 0x80;
		public const int _selectButton = 0x100;
		public const int _upButton = 0x200;
		public const int _downButton = 0x400;
		public const int _leftButton = 0x800;
		public const int _rightButton = 0x1000;
		/// <summary>
		/// These are in the order displayed in the text file, which is backwards or bit order. 13
		/// </summary>
		// Movie file order?
		public static string[] buttonNames = { "R", "L", "D", "U", "T", "S", "B", "A", "Y", "X", "W", "E", "g" };
		public bool ButtonDown(int ButtonID)
		{
			// byteID
			int bID = -1;
			ButtonID += 8;
			// is 1 if getting 8th or later button
			do
			{
				ButtonID -= 8;
				bID += 1;
			} while (ButtonID > 7);
			return (Bytes[bID] | Bits[ButtonID]) == Bytes[bID];
		}
		public void setButton(int ButtonID, bool OnOff)
		{
			// byteID
			int bID = -1;
			ButtonID += 8;
			// is 1 if getting 8th or later button
			do
			{
				ButtonID -= 8;
				bID += 1;
			} while (ButtonID > 7);
			SetBit((byte)bID, (byte)ButtonID, OnOff);
		}
		#endregion

		// 4th byte
		public byte touchX
		{
			get { return Bytes[4]; }
			set { Bytes[4] = value; }
		}
		// 5th byte
		public byte touchY
		{
			get { return Bytes[5]; }
			set { Bytes[5] = value; }
		}
		// 6-9th bytes?
		public int touchP
		{
			get { return BitConverter.ToInt32(Bytes, 6); }
			set { BitConverter.GetBytes(value).CopyTo(Bytes, 6); }
		}

		// 10th byte (+11th?)
		public byte Mic
		{
			get { return Bytes[10]; }
			set { Bytes[10] = value; }
		}

		string[] b = { "R", "L", "D", "U", "T", "S", "B", "A", "Y", "X", "W", "E", "g" };
		public string GetLine()
		{
			string str = "|" + Mic + "|";

			for (int i = 0; i < b.Length; i++)
			{
				string a = ".";
				if (ButtonDown(12 - i))
				{
					a = b[i];
				}
				str += a;
			}

			str = str + (touchX + " " + touchY + " " + touchP + "|");

			return str;
		}
	}
}
