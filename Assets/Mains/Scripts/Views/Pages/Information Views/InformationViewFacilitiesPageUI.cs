using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class InformationViewFacilitiesPageUI : ViewPageUI
    {
        private VisualElement _labelField;
        private ListView _itemList;

        private HotelFacility[] _facilities = new HotelFacility[0];

        protected override void VirtualAwake()
        {
            Marker.OnHotelFacilitiesDisplayed += OnHotelFacilitiesDisplayed;
        }

        private void OnDestroy()
        {
            Marker.OnHotelFacilitiesDisplayed -= OnHotelFacilitiesDisplayed;
        }

        protected override void Collect()
        {
            _labelField = Root.Q("TopBar").Q("LabelField");
            _labelField.RegisterCallback<PointerUpEvent>(OnClicked_BackButton);

            var contentScroll = Root.Q("ContentScroll");
            Root.Remove(contentScroll);

            _itemList = Root.Q("ItemList") as ListView;
            _itemList.Q("unity-content-container").SetFlexGrow(1);
            _itemList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _itemList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _itemList.itemsSource = _facilities;
            _itemList.makeItem = () => new FacilityListItemUI();
            _itemList.bindItem = (element, index) =>
            {
                var item = element as FacilityListItemUI;
                if (index < _facilities.Length)
                {
                    item.Apply(_facilities[index]);
                }
            };
        }

        private void OnClicked_BackButton(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewMainPage, true, false);
        }

        private void RebuildHistoryList()
        {
            _itemList.itemsSource = null;
            _itemList.itemsSource = _facilities;
            _itemList.Rebuild();
        }

        private void OnHotelFacilitiesDisplayed(UID hotelID)
        {
            var unit = Main.Database.Hotels[hotelID];
            _facilities = unit.Description.Facilities.ToArray();

            RebuildHistoryList();
        }
    }
}