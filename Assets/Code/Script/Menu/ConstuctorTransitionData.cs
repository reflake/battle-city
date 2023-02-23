using LevelDesigner;

namespace Menu
{
	public class ConstuctorTransitionData
	{
		public LevelData CustomLevelData { get; private set; }

		public ConstuctorTransitionData(LevelData customLevelData)
		{
			CustomLevelData = customLevelData;
		}
	}
}