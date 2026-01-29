namespace FRASS.Interfaces
{
	public static class DatabaseIDs
	{
		public static class ReportTypes
		{
			public static int FullParcelReport { get { return 1; } }
		}

		public static class SiteSettings
		{
			public static int SitePassword { get { return 1; } }
			public static int AdminEmail { get { return 2; } }
			public static int ReportExpirationDays { get { return 3; } }
			public static int ErrorEmail { get { return 4; } }
		}

		public static class LogTypes
		{
			public static int Error { get { return 1; } }
			public static int Login { get { return 2; } }
			public static int Logout { get { return 3; } }
			public static int EmailSent { get { return 4; } }
			public static int ReportGeneration { get { return 5; } }
		}
	}

	public enum StandStats
	{
		ActChan = 1,
		Agriculture = 2,
		Commercial = 3,
		Operable = 4,
		Residential = 5,
		Riparian = 6,
		ROW = 7
	}
}
