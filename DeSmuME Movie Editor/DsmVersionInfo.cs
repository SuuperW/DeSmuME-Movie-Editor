using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeSmuMe_Movie_Editor
{
	public class DsmVersionInfo
	{
		public bool is_x64;
		public string name;
		public string processName;

		public long currentFramePtr;
		public long movieRecordsPtr;
	}
}
