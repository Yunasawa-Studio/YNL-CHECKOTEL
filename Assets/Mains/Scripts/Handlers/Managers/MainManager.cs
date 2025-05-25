using UnityEngine;
using YNL.Utilities.Patterns;

namespace YNL.Checkotel
{
    public static class Main
    {
        public static bool IsSystemStarted { get; set; }

        public static MainManager Manager => MainManager.Instance;
        public static ResourceContainerSO Resources => Manager.Resources;
        public static DatabaseContainerSO Database => Manager.Database;
        public static RuntimePropertiesSO Runtime => Manager.Runtime;
        public static ViewManager View => Manager.ViewManager;
	}

    public class MainManager : Singleton<MainManager>
    {
        public ResourceContainerSO Resources;
        public DatabaseContainerSO Database;
        public RuntimePropertiesSO Runtime;

        public ViewManager ViewManager;

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 120;
        }
    }
}