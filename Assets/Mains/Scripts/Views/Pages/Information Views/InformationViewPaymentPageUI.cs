using System;
using UnityEngine.UIElements;
using YNL.Utilities.Addons;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public enum PaymentMethod : byte { CreditCard, VISA, Paypal, PayAtPlace}

    public class InformationViewPaymentPageUI : ViewPageUI
    {
        private VisualElement _labelField;
        private VisualElement _previewImage;
        private Label _hotelName;
        private Label _roomName;
        private Label _addressText;
        private Label _durationText;
        private Label _checkInTime;
        private Label _checkOutTime;
        private Label _phoneNumber;
        private Label _userName;
        private Label _priceText;
        private VisualElement _bookButton;

        private SerializableDictionary<PaymentMethod, (VisualElement field, VisualElement check)> _methodButtons = new();
        private UID _hotelID;
        private UID _roomID;
        private PaymentMethod _selectedPayment = PaymentMethod.PayAtPlace;

        protected override void VirtualAwake()
        {
            Marker.OnPaymentRequested += OnPaymentRequested;
        }

        private void OnDestroy()
        {
            Marker.OnPaymentRequested -= OnPaymentRequested;
        }

        protected override void Collect()
        {
            _labelField = Root.Q("HeaderField").Q("LabelField");
            _labelField.RegisterCallback<PointerUpEvent>(OnClicked_LabelField);

            var container = Root.Q("ScrollView").Q("unity-content-container");

            var roomField = container.Q("RoomField");
            _previewImage = roomField.Q("PreviewField").Q("PreviewImage");
            _roomName = roomField.Q("InformationField").Q<Label>("RoomName");
            _hotelName = roomField.Q("InformationField").Q("HotelNameField").Q<Label>("HotelName");
            _addressText = roomField.Q("InformationField").Q("AddressField").Q<Label>("Name");

            var timeField = container.Q("TimeField");
            _durationText = timeField.Q("TimeBox").Q<Label>("DurationText");
            _checkInTime = timeField.Q("TimeRange").Q("CheckInTime").Q<Label>("TimeField");
            _checkOutTime = timeField.Q("TimeRange").Q("CheckOutTime").Q<Label>("TimeField");

            var accountField = container.Q("AccountField");
            _phoneNumber = accountField.Q("InformationArea").Q("PhoneNumberField").Q<Label>("Value");
            _userName = accountField.Q("InformationArea").Q("FullNameField").Q<Label>("Value");

            _priceText = Root.Q("Footer").Q<Label>("LastPrice");
            _bookButton = Root.Q("Footer").Q("BookButton");
            _bookButton.RegisterCallback<PointerUpEvent>(OnClicked_BookButton);

            var paymentArea = Root.Q("PaymentMethodField").Q("PaymentArea");
            _methodButtons.Clear();
            foreach (PaymentMethod type in Enum.GetValues(typeof(PaymentMethod)))
            {
                var field = paymentArea.Q(type.ToString());
                field.RegisterCallback<PointerUpEvent>(evt => OnSelected_PaymentMethod(type));

                var check = field.Q("Check");

                _methodButtons.Add(type, (field, check));
            }
        }

        private void OnClicked_LabelField(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.InformationViewMainPage, true, false);
        }

        private void OnSelected_PaymentMethod(PaymentMethod method)
        {
            _selectedPayment = method;

            foreach (var button in _methodButtons)
            {
                if (button.Key == method)
                {
                    button.Value.check.SetBackgroundImage(Main.Resources.Icons["Check"]);
                    button.Value.check.SetBackgroundImageTintColor("#FED1A7");
                }
                else
                {
                    button.Value.check.SetBackgroundImage(Main.Resources.Icons["Uncheck"]);
                    button.Value.check.SetBackgroundImageTintColor("#FFFFFF");
                }
            }
        }

        private void OnClicked_BookButton(PointerUpEvent evt)
        {
            if (Main.Runtime.Data.BookedRooms.TryGetValue(_hotelID, out var rooms))
            {
                rooms.Rooms.Add(_roomID);
            }
            else
            {
                Main.Runtime.Data.BookedRooms.Add(_hotelID, new() { Rooms = new() { _roomID } });

				Marker.OnRuntimeSavingRequested?.Invoke();
			}

            Marker.OnViewPageSwitched?.Invoke(ViewType.MainViewHomePage, true, false);
        }

        private void OnPaymentRequested(UID hotelID, UID roomID)
        {
            _hotelID = hotelID;
            _roomID = roomID;

            var unit = Main.Database.Hotels[hotelID];
            var account = Main.Database.Accounts[Main.Runtime.Data.AccountID];

            _hotelName.SetText(unit.Description.Name);
            _roomName.SetText(Main.Database.Rooms[roomID].Name);
            _addressText.SetText(unit.Description.Address);

            var timeRangeText = Main.Runtime.Data.StayType.GetTimeRangeText(Main.Runtime.Data.CheckInTime, Main.Runtime.Data.Duration, "HH:mm • dd/MM/yyyy");

            _durationText.SetText(timeRangeText.Duration);
            _checkInTime.SetText(timeRangeText.In);
            _checkOutTime.SetText(timeRangeText.Out);

            _phoneNumber.SetText(account.PhoneNumber);
            _userName.SetText(account.Name);

            _priceText.SetText($"<size=35>Total payment</size>\r\n<b><color=#FED1A7>${Main.Database.Rooms[roomID].Price.FinalPrice:0.00}</color></b>");
        }
    }
}