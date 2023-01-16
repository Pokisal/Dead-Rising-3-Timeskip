using System;
using System.Diagnostics;
using System.Dynamic;
using System.Timers;
using ReadWriteMemory;

namespace DR_RTM
{

	public static class AllSkips
	{
		public static double TimerInterval = 16.666666666666668;

		public static Timer UpdateTimer = new Timer(TimerInterval);

		public static Process GameProcess;

		public static Form1 form;

		public static int skipMode = 0;

		private static ReadWriteMemory.ProcessMemory gameMemory;

		private static uint Days;
		private static uint Hours;
		private static uint Minutes;
		private static uint Seconds;

		private static string Chapter;
		private static string Objective;

		private static string LastSkip;

		private static string CurrentBoss;
		private static string OldCurrentBoss;

		private static uint BossHealth;

		private static IntPtr gameTimePtr;
		
		private static uint gameTime;

		private static dynamic old = new ExpandoObject();

		public static void Init()
		{
		}

		public static string StringTime(uint time)
		{
			uint num = Hours;
			string text = "AM";
			if (num >= 12)
			{
				text = "PM";
				num %= 12u;
			}
			if (num == 0)
            {
				num = 12u;
            }
			if (Chapter == "main_menu_")
			{
				return string.Format("<missing>");
			}
			else
			{
				return string.Format("Day: {0} \r\n{1}:{2}:{3} {4}", Days, num.ToString("D2"), Minutes.ToString("D2"), Seconds.ToString("D2"), text);
			}
		}

		public static void UpdateEvent(object source, ElapsedEventArgs e)
		{
			if (gameMemory != null && !gameMemory.CheckProcess())
			{
				gameMemory = null;
				UpdateTimer.Enabled = false;
				return;
			}
			if (gameMemory == null)
			{
				gameMemory = new ReadWriteMemory.ProcessMemory(GameProcess);
			}
			if (!gameMemory.IsProcessStarted())
			{
				gameMemory.StartProcess();
			}
			gameTimePtr = gameMemory.Pointer("deadrising3.exe", 27314776, 40, 160, 112, 80, 80);
			if (gameTimePtr == IntPtr.Zero)
			{
				if (!form.IsDisposed)
				{
					form.TimeDisplayUpdate("<missing>");
				}
				return;
			}
			old.Objective = Objective;
			old.BossHealth = BossHealth;
			OldCurrentBoss = CurrentBoss;
			Days = gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 2259268));
			Hours = gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 2259272));
			Minutes = gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 2259276));
			Seconds = gameMemory.ReadUInt(IntPtr.Add(gameTimePtr, 2259280));
			BossHealth = gameMemory.ReadUInt(IntPtr.Add(gameMemory.Pointer("deadrising3.exe", 27316024, 144, 40, 8, 176, 8), 16));
			CurrentBoss = gameMemory.ReadStringAscii(IntPtr.Add(gameMemory.Pointer("deadrising3.exe", 26900688, 288, 80), -4526569), 256);
			Chapter = gameMemory.ReadStringAscii(IntPtr.Add(gameMemory.Pointer("deadrising3.exe", 24203168, 3736), 0), 10);
			Objective = gameMemory.ReadStringAscii(IntPtr.Add(gameMemory.Pointer("deadrising3.exe", 24198456, 3280, 672, 1968, 304, 720), 1120), 110);
			form.TimeDisplayUpdate(StringTime(gameTime));
			if (Objective == "Eat Food To Restore Health")
            {
				LastSkip = " ";
            }
			if (skipMode == 0)
			{
				if (Objective == "Explore While Rhonda's Busy" && LastSkip != "Wait1")
				{
					LastSkip = "Wait1";
					gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 2259272), Hours + 6);
				}
				if (Objective == "Explore While Red Gets Fuel" && LastSkip != "Wait2")
				{
					LastSkip = "Wait2";
					gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 2259272), Hours + 6);
				}
				if (Objective == "Explore While Rhonda Researches" && LastSkip != "Wait3")
				{
					LastSkip = "Wait3";
					gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 2259272), Hours + 6);
				}
			}
			else if (skipMode == 1)
            {
				if (Objective == "Explore While Rhonda's Busy" && CurrentBoss == "Zhi" && BossHealth == 0)
				{
					gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 2259272), Hours + 1);
				}
				if (Objective == "Explore While Red Gets Fuel" && CurrentBoss == "Darlene" && BossHealth == 0)
				{
					gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 2259272), Hours + 1);
				}
				if (Objective == "Explore While Rhonda Researches" && OldCurrentBoss.Contains("Teddy") && !CurrentBoss.Contains("Teddy"))
				{
					gameMemory.WriteUInt(IntPtr.Add(gameTimePtr, 2259272), Hours + 1);
				}
			}
		}
	}
}