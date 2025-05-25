using System;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class SearchingHistoryItemUI : VisualElement
    {
        public Action<string> OnSelected;

        private const string _rootClass = "searching-history-item";
        private const string _iconClass = _rootClass + "__icon";
        private const string _textClass = _rootClass + "__text";
        private const string _removeButtonClass = _rootClass + "__remove-button";
        private const string _buildingClass = "building";

        private VisualElement _icon;
        private Label _text;
        private VisualElement _removeButton;

        private string _value;

        public SearchingHistoryItemUI()
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["SearchingHistoryItemUI"]);
            this.AddClass(_rootClass);

            _icon = new VisualElement().AddClass(_iconClass);
            this.AddElements(_icon);

            _text = new Label().AddClass(_textClass);
            this.AddElements(_text);

            _removeButton = new VisualElement().AddClass(_removeButtonClass);
            this.AddElements(_removeButton);

            this.RegisterCallback<PointerUpEvent>(OnSelected_HistoryItem);
        }

        public void Apply(string value, bool isBuilding = false)
        {
            _value = value;

            _text.SetText(value);
            _icon.EnableClass(isBuilding, _buildingClass);
        }

        private void OnSelected_HistoryItem(PointerUpEvent evt)
        {
            OnSelected?.Invoke(_value);
        }
    }
}