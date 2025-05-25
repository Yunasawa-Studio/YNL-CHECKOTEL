using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class FacilityListItemUI : VisualElement
    {
        private const string _rootClass = "facility-list-item";
        private const string _labelClass = _rootClass + "__label";
        private const string _iconClass = _rootClass + "__icon";

        private VisualElement _icon;
        private Label _label;

        public FacilityListItemUI()
        {
            this.AddStyle(Main.Resources.Styles["StyleVariableUI"]);
            this.AddStyle(Main.Resources.Styles["FacilityListItemUI"]);
            this.AddClass(_rootClass);

            _icon = new VisualElement().AddClass(_iconClass);
            this.AddElements(_icon);

            _label = new Label().AddClass(_labelClass);
            this.AddElements(_label);
        }

        public void Apply(HotelFacility facility)
        {
            var field = facility.GetHotelFacilitiesField();

            _icon.SetBackgroundImage(field.Icon);
            _label.SetText(field.Name);
        }
    }
}