using UnityEngine.UIElements;

namespace YNL.Checkotel
{
    public class MainViewNavigationPageUI : ViewPageUI
    {
        private VisualElement _navigationBar;

        protected override void Collect()
        {
            _navigationBar = Root.Q("NavigationBar");
        }

        protected override void Initialize()
        {
            _navigationBar.Clear();
            _navigationBar.Add(new HomeNavigationButton(Main.Resources.Icons["Home"], "Home", true, ViewType.MainViewHomePage));
            _navigationBar.Add(new HomeNavigationButton(Main.Resources.Icons["Heart"], "Favorite", false, ViewType.MainViewFavoritePage));
            _navigationBar.Add(new HomeNavigationButton(Main.Resources.Icons["Building"], "Booking", false, ViewType.MainViewBookingPage));
            _navigationBar.Add(new HomeNavigationButton(Main.Resources.Icons["Account"], "Account", false, ViewType.MainViewAccountPage));
        }
    }
}