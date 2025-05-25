using System.Collections.Generic;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class MainViewFavoritePageUI : ViewPageUI
    {
        private List<UID> _favorites => Main.Runtime.Data.FavoriteHotels;

        public ListView _itemList;

        protected override void Collect()
        {
            _itemList = Root.Q("ItemList") as ListView;
            _itemList.Q("unity-content-container").SetFlexGrow(1);
            _itemList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _itemList.fixedItemHeight = 700;
            _itemList.itemsSource = _favorites;
            _itemList.makeItem = () => new SearchingResultItemUI();
            _itemList.bindItem = (element, index) =>
            {
                var item = element as SearchingResultItemUI;
                if (index < _favorites.Count)
                {
                    item.Apply(_favorites[index], Room.StayType.Hourly);
                }
            };
        }

        protected override void Refresh()
        {
            _itemList.itemsSource = null;
            _itemList.itemsSource = _favorites;
            _itemList.Rebuild();
        }
    }
}