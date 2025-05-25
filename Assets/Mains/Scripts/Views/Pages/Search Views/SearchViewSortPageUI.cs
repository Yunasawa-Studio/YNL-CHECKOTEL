using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public partial class SearchViewSortPageUI : ViewPageUI
    {
        public SortingSelectionType SortingType;

        private VisualElement _background;
        private VisualElement _page;
        private VisualElement _sortingPage;
        private VisualElement _closeButton;
        private VisualElement _applyButton;
        private VisualElement _sortSelectionArea;

        private List<SortingSelectionItemUI> _sortingItemUI = new();

        protected override void VirtualAwake()
        {
            SortingSelectionItemUI.OnSelected += OnSortingTypeSelected;
        }

        private void OnDestroy()
        {
            SortingSelectionItemUI.OnSelected -= OnSortingTypeSelected;
        }

        protected override void Collect()
        {
            _background = Root.Q("ScreenBackground");
            _background.RegisterCallback<PointerUpEvent>(OnClicked_CloseButton);
            _page = Root.Q("SortingPage");

            _sortingPage = Root.Q("SortingPage");

            _closeButton = _sortingPage.Q("LabelField");
            _closeButton.RegisterCallback<PointerUpEvent>(OnClicked_CloseButton);

            _applyButton = _sortingPage.Q("Toolbar").Q("ApplyButton");
            _applyButton.RegisterCallback<PointerUpEvent>(OnClicked_ApplyButton);

            _sortSelectionArea = _sortingPage.Q("SortSelectionArea");
            _sortSelectionArea.Clear();
        }

        protected override void Initialize()
        {
            var sortingTypes = Enum.GetValues(typeof(SortingSelectionType)) as SortingSelectionType[];

            for (byte i = 0; i < sortingTypes.Length; i++)
            {
                var item = new SortingSelectionItemUI(sortingTypes[i]);
                if (i == sortingTypes.Length - 1) item.SetAsLastItem();

                _sortingItemUI.Add(item);
                _sortSelectionArea.Add(item);
            }
        }

        protected override void Refresh()
        {
            return;

            _sortingItemUI[0].OnClicked__Toggle();
        }

        public override void OnPageOpened(bool isOpen, bool needRefresh = true)
        {
            if (isOpen)
            {
                _background.SetPickingMode(PickingMode.Position);
                _background.SetBackgroundColor(new Color(0.0865f, 0.0865f, 0.0865f, 0.725f));
                _page.SetTranslate(0, 0, true);
            }
            else
            {
                _background.SetBackgroundColor(Color.clear);
                _background.SetPickingMode(PickingMode.Ignore);
                _page.SetTranslate(0, 100, true);
            }

            if (isOpen && needRefresh) Refresh();
        }

        private void OnClicked_CloseButton(PointerUpEvent evt)
        {
            OnPageOpened(false);
        }

        private void OnClicked_ApplyButton(PointerUpEvent evt)
        {
            Marker.OnSearchResultSorted?.Invoke(SortingType);
            OnPageOpened(false);
        }

        private void OnSortingTypeSelected(SortingSelectionType type)
        {
            SortingType = type;
        }
    }
}