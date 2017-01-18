namespace PlayGen.ITAlert.Unity.Settings
{
	public class SettingType
	{
		// todo should probably be properties
		public string Title;
		public SettingObjectType ObjectType;
		public bool ShowOnMobile;

		public SettingType(string title, SettingObjectType type, bool mobile)
		{
			Title = title;
			ObjectType = type;
			ShowOnMobile = mobile;
		}
	}
}