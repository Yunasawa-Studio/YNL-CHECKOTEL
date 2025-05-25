using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public enum PreviewListFilterType : byte
    {
        NewHotels, MostPopular, HighRated, LuxuryStays, ExceptionalChoices
    }

    public class HotelPreviewListUI : VisualElement, IRefreshable
    {
        private const string _rootClass = "hotel-preview-list";
        private const string _labelFieldClass = _rootClass + "__label-field";
        private const string _labelClass = _rootClass + "__label";
        private const string _seeMoreButtonClass = _rootClass + "__see-more-button";
        private const string _previewListClass = _rootClass + "__preview-list";

        private VisualElement _labelField;
        private Label _label;
        private Label _seeMoreButton;
        private ScrollView _previewList;

        private List<HotelPreviewItemUI> _previewItems = new();

        public HotelPreviewListUI(PreviewListFilterType type, bool isMini = false)
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["HotelPreviewListUI"]);
            this.AddClass(_rootClass);

            _labelField = new VisualElement().AddClass(_labelFieldClass);
            this.AddElements(_labelField);

            _label = new Label(type.ToSentenceCase()).AddClass(_labelClass);
            _labelField.AddElements(_label);

            _seeMoreButton = new Label("See more").AddClass(_seeMoreButtonClass);
            _labelField.AddElements(_seeMoreButton);

            _previewList = new ScrollView().AddClass(_previewListClass);
            _previewList.mode = ScrollViewMode.Horizontal;
            _previewList.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            _previewList.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            this.AddElements(_previewList);

            Initialize(type, isMini);
        }

        private void Initialize(PreviewListFilterType type, bool isMini)
        {
            CreatePreviewItems(type, isMini).Forget();
        }

        public void Refresh()
        {
            foreach (var item in _previewItems) item.Refresh();
        }

        public HotelPreviewListUI SetAsLastItem()
        {
            this.SetMarginBottom(275);

            return this;
        }

        private UID[] GetPreviewItems(PreviewListFilterType type)
        {
            switch (type)
            {
                case PreviewListFilterType.NewHotels: return Extension.Query.GetNewHotelsList();
                case PreviewListFilterType.MostPopular: return Extension.Query.GetMostPopularList();
                case PreviewListFilterType.HighRated: return Extension.Query.GetHighRatedList();
                case PreviewListFilterType.LuxuryStays: return Extension.Query.GetLuxuryStaysList();
                case PreviewListFilterType.ExceptionalChoices: return Extension.Query.GetExceptionalChoicesList();
                default: return null;
            }
        }

        private async UniTaskVoid CreatePreviewItems(PreviewListFilterType type, bool isMini)
        {
            var previewItems = GetPreviewItems(type);

            for (int i = 0; i < previewItems.Length; i++)
            {
                await UniTask.Yield();

                var previewItem = new HotelPreviewItemUI(previewItems[i], isMini).SetAsLastItem(i == previewItems.Length - 1);
                _previewItems.Add(previewItem);
                _previewList.AddElements(previewItem);
            }
        }
    }
}