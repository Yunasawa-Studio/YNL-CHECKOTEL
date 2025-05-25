using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class SigningViewSingingUpPageUI : ViewPageUI
    {
        private TextField _accountInputField;
        private Label _accountMessage;
        private TextField _passwordInputField;
        private Label _8charactersMessage;
        private Label _oneNumberMessage;
        private Label _oneSpecialCharacterMessage;
        private Label _passwordMessage;
        private TextField _confirmInputField;
        private Label _confirmMessage;
        private VisualElement _switchLabel;

        private Button _signingButton;

        private VisualElement _signInWithFacebookButton;
        private VisualElement _signInWithGoogleButton;

        private bool _validEmailInput;
        private string _accountInput;
        private string _passwordInput;
        private bool _validAccountInput;
        private bool _validPasswordInput;
        private bool _validConfirmInput;

        protected override void Collect()
        {
            var signingInputField = Root.Q("SigningInputField");

            _accountInputField = signingInputField.Q("AccountField").Q("TextField") as TextField;
            _accountInputField.RegisterValueChangedCallback(OnValueChanged_AccountInputField);

            _accountMessage = signingInputField.Q("AccountField").Q("Message") as Label;

            _passwordInputField = signingInputField.Q("PasswordField").Q("TextField") as TextField;
            _passwordInputField.RegisterValueChangedCallback(OnValueChanged_PasswordInputField);

            _8charactersMessage = signingInputField.Q("PasswordField").Q("AtLest8Characters") as Label;
            _oneNumberMessage = signingInputField.Q("PasswordField").Q("AtLeastOneNumber") as Label;
            _oneSpecialCharacterMessage = signingInputField.Q("PasswordField").Q("AtLeastOneSpecialCharacter") as Label;
            _passwordMessage = signingInputField.Q("PasswordField").Q("Message") as Label;

            _confirmInputField = signingInputField.Q("ConfirmField").Q("TextField") as TextField;
            _confirmInputField.RegisterValueChangedCallback(OnValueChanged_ConfirmInputField);

            _confirmMessage = signingInputField.Q("ConfirmField").Q("Message") as Label;

            _signInWithFacebookButton = Root.Q("SigningMethod").Q("FacebookSigning");
            _signInWithFacebookButton.RegisterCallback<PointerUpEvent>(SigningWithFacebook);

            _signInWithGoogleButton = Root.Q("SigningMethod").Q("GoogleSigning");
            _signInWithGoogleButton.RegisterCallback<PointerUpEvent>(SignInWithGoogle);

            _signingButton = signingInputField.Q("SigningButton").Q("Button") as Button;
            _signingButton.clicked += SigningAccount;

            _switchLabel = Root.Q("SwitchLabel");
            _switchLabel.RegisterCallback<PointerUpEvent>(OnClicked_SwitchLabel);
        }

        protected override void Initialize()
        {
            _accountMessage.SetText(string.Empty);
            _passwordMessage.SetText(string.Empty);
            _confirmMessage.SetText(string.Empty);
        }

        private void OnValueChanged_AccountInputField(ChangeEvent<string> evt)
        {
            _accountInput = evt.newValue;

            if (_accountInput == string.Empty) return;

            _validEmailInput = Extension.Validator.ValidateEmail(_accountInput);
            var validPhoneInput = Extension.Validator.ValidatePhoneNumber(_accountInput);

            _validAccountInput = false;

            if (!_validEmailInput && !validPhoneInput)
            {
                _accountMessage.SetText("Email or Phone number is not valid!");
                return;
            }

            var account = Main.Database.Accounts.Values.First(i => i.Email == _accountInput || i.PhoneNumber == _accountInput);

            if (account != null)
            {
                _accountMessage.SetText("This email or phone number is registered by another account!");
                return;
            }

            _accountMessage.SetText(string.Empty);
            _validAccountInput = true;
        }

        private void OnValueChanged_PasswordInputField(ChangeEvent<string> evt)
        {
            _passwordInput = evt.newValue;

            if (_passwordInput == string.Empty) return;

            var valid8character = _passwordInput.Length >= 8;
            var valid1number = Regex.IsMatch(_passwordInput, @"\d");
            var valid1special = Regex.IsMatch(_passwordInput, @"[!@#$%^&*(),.?\:{ }|<>]");

            _8charactersMessage.SetColor(valid8character ? "#5FFF9F" : "#FF5F5F");
            _oneNumberMessage.SetColor(valid1number ? "#5FFF9F" : "#FF5F5F");
            _oneSpecialCharacterMessage.SetColor(valid1special ? "#5FFF9F" : "#FF5F5F");

            _validPasswordInput = false;

            if (!valid8character && !valid1number && !valid1special)
            {
                _passwordMessage.SetText("Password must meet all the requirements above.");
                return;
            }

            string[] accountParts = Regex.Split(_accountInput, @"[@.]+");
            foreach (string part in accountParts)
            {
                if (!string.IsNullOrWhiteSpace(part) && _passwordInput.Contains(part, StringComparison.OrdinalIgnoreCase))
                {
                    _passwordMessage.SetText("Your password cannot be the same as your username or email.");
                    _validPasswordInput = false;
                    return;
                }
            }

            _passwordMessage.SetText(string.Empty);
            _validPasswordInput = true;
        }

        private void OnValueChanged_ConfirmInputField(ChangeEvent<string> evt)
        {
            var input = evt.newValue;

            if (input == string.Empty) return;

            var isMatchWithPassword = input == _passwordInput;

            _validConfirmInput = false;

            if (!isMatchWithPassword)
            {
                _confirmMessage.SetText("Passwords do not match");
                return;
            }

            _confirmMessage.SetText(string.Empty);
            _validConfirmInput = true;
        }

        private void OnClicked_SwitchLabel(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.SigningViewSignInPage, true, false);
        }

        private void SigningWithFacebook(PointerUpEvent evt)
        {
            
        }

        private void SignInWithGoogle(PointerUpEvent evt)
        {

        }

        private void SigningAccount()
        {
            var account = new Account();
            if (_validEmailInput) account.Email = _accountInput;
            else account.PhoneNumber = _accountInput;
            account.Password = _passwordInput;
            Main.Database.Accounts.Add(account.ID, account);

            Marker.OnViewPageSwitched?.Invoke(ViewType.MainViewHomePage, true, true);
        }

        private void RecoveryAccount(PointerUpEvent evt)
        {

        }
    }
}
