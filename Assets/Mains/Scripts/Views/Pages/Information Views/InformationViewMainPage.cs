using System;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class InformationViewMainPage : ViewPageUI
    {
        [SerializeField] private InformationViewTimeRangePageUI _timeRangePageUI;

        private VisualElement _backButton;
        private VisualElement _favoriteButton;
        private VisualElement _shareButton;

        private PriceField _priceField;

        private ImageView _imageView;
        private NameView _nameView;
        private ReviewView _reviewView;
        private FacilityView _facilityView;
        private DescriptionField _descriptionField;
        private TimeField _timeField;
        private PolicyField _policyField;
        private CancellationField _cancellationPolicy;

        private UID _hotelID;

        protected override void VirtualAwake()
        {
            Marker.OnHotelInformationDisplayed += OnHotelInformationDisplayed;
        }

        private void OnDestroy()
        {
            Marker.OnHotelInformationDisplayed -= OnHotelInformationDisplayed;
        }

        protected override void Collect()
        { 
            _backButton = Root.Q("TopBar").Q("BackButton");
            _backButton.RegisterCallback<PointerUpEvent>(OnClicked_BackButton);

            _favoriteButton = Root.Q("TopBar").Q("FavoriteButton");
            _favoriteButton.RegisterCallback<PointerUpEvent>(OnClicked_FavoriteButton);

            _shareButton = Root.Q("TopBar").Q("ShareButton");

            _priceField = new(Root);
            _priceField.OnOpenTimeRangePage = OnOpenTimeRangePage;

            var contentContainer = Root.Q("ContentScroll").Q("unity-content-container");

            _imageView = new(contentContainer.Q("ImageView"));

            _nameView = new(contentContainer.Q("NameView"));

            _reviewView = new(contentContainer);

            _facilityView = new(contentContainer);

            _descriptionField = new(contentContainer);

            _timeField = new(contentContainer.Q("TimeField"));

            _policyField = new(contentContainer.Q("PolicyField"));

            _cancellationPolicy = new(contentContainer.Q("CancellationPolicy"));

            _timeRangePageUI.OnTimeRangeSubmitted = OnTimeRangeSubmitted;
        }

        private void OnClicked_BackButton(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.MainViewHomePage, true, false);
        }

        private void OnClicked_FavoriteButton(PointerUpEvent evt)
        {
            bool isFavorited = Main.Runtime.Data.FavoriteHotels.Contains(_hotelID);

            if (isFavorited)
            {
                Main.Runtime.Data.FavoriteHotels.Remove(_hotelID);
                _favoriteButton.SetBackgroundImage(Main.Resources.Icons["Heart"]);
            }
            else
            {
                Main.Runtime.Data.FavoriteHotels.Add(_hotelID);
                _favoriteButton.SetBackgroundImage(Main.Resources.Icons["Heart (Filled)"]);

                Marker.OnRuntimeSavingRequested?.Invoke();
			}
        }

        private void OnOpenTimeRangePage()
        {
            _timeRangePageUI.OnPageOpened(true, false);
        }

        private void OnTimeRangeSubmitted()
        {      
            _priceField.Apply(_hotelID);
        }

        private void OnHotelInformationDisplayed(UID id, bool isSearchResult)
        {
            _hotelID = id;

            var unit = Main.Database.Hotels[id];

            if (!isSearchResult)
            {
                var nearestTime = Main.Runtime.Data.CheckInTime.GetNextNearestTime();
                Main.Runtime.Data.CheckInTime = nearestTime;
                Main.Runtime.Data.Duration = 1;
            }

            _priceField.Apply(id);

            _nameView.Apply(unit.Description.Name, unit.Description.Address);
            _reviewView.Apply(id);
            _facilityView.Apply(unit);
            _descriptionField.Apply(unit.Description.Description);
            _imageView.Apply(unit).Forget();
            _timeField.Apply(unit);
            _policyField.Apply(unit);
            _cancellationPolicy.Apply(unit);

            bool isFavorited = Main.Runtime.Data.FavoriteHotels.Contains(id);

            _favoriteButton.SetBackgroundImage(Main.Resources.Icons[isFavorited ? "Heart (Filled)" : "Heart"]);
        }
    }
}