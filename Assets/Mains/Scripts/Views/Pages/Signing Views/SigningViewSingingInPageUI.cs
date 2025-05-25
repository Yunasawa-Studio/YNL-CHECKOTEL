using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.Extensions;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public class SigningViewSingingInPageUI : ViewPageUI
    {
        private TextField _accountInputField;
        private Label _accountMessage;
        private TextField _passwordInputField;
        private Label _passwordMessage;

        private Button _signingButton;
        private VisualElement _recoveryButton;

        private VisualElement _signInWithFacebookButton;
        private VisualElement _signInWithGoogleButton;

        private VisualElement _switchLabel;

        private string _accountInput;
        private string _passwordInput;
        private bool _validAccountInput;
        private bool _validPasswordInput;

        protected override void Collect()
        {
            var signingInputField = Root.Q("SigningInputField");

            _accountInputField = signingInputField.Q("AccountField").Q("TextField") as TextField;
            _accountInputField.RegisterValueChangedCallback(OnValueChanged_AccountInputField);

            _accountMessage = signingInputField.Q("AccountField").Q("Message") as Label;

            _passwordInputField = signingInputField.Q("PasswordField").Q("TextField") as TextField;
            _passwordInputField.RegisterValueChangedCallback(OnValueChanged_PasswordInputField);

            _passwordMessage = signingInputField.Q("PasswordField").Q("Message") as Label;

            _signInWithFacebookButton = Root.Q("SigningMethod").Q("FacebookSigning");
            _signInWithFacebookButton.RegisterCallback<PointerUpEvent>(SigningWithFacebook);

            _signInWithGoogleButton = Root.Q("SigningMethod").Q("GoogleSigning");
            _signInWithGoogleButton.RegisterCallback<PointerUpEvent>(SignInWithGoogle);

            _signingButton = signingInputField.Q("SigningButton").Q("Button") as Button;
            _signingButton.clicked += SigningAccount;

            _recoveryButton = signingInputField.Q("RecoveryButton");
            _recoveryButton.RegisterCallback<PointerUpEvent>(RecoveryAccount);

            _switchLabel = Root.Q("SwitchLabel");
            _switchLabel.RegisterCallback<PointerUpEvent>(OnClicked_SwitchLabel);
        }

        protected override void Initialize()
        {
            _accountMessage.SetText(string.Empty);
            _passwordMessage.SetText(string.Empty);
        }

        private void OnValueChanged_AccountInputField(ChangeEvent<string> evt)
        {
            _accountInput = evt.newValue;

            _accountMessage.SetText(string.Empty);

            if (_accountInput == string.Empty) return;
        }

        private void OnValueChanged_PasswordInputField(ChangeEvent<string> evt)
        {
            _passwordInput = evt.newValue;

            _passwordMessage.SetText(string.Empty);

            if (_passwordInput == string.Empty) return;
        }

        private void OnClicked_SwitchLabel(PointerUpEvent evt)
        {
            Marker.OnViewPageSwitched?.Invoke(ViewType.SigningViewSignUpPage, true, false);
        }

        private void SigningWithFacebook(PointerUpEvent evt)
        {

        }

        private void SignInWithGoogle(PointerUpEvent evt)
        {

        }

        private void SigningAccount()
        {
            var existedEmailAccount = Main.Database.Accounts.Values.Any(i => i.Email == _accountInput);
            var existedPhoneAccount = Main.Database.Accounts.Values.Any(i => i.PhoneNumber == _accountInput);

            if (!existedEmailAccount && !existedPhoneAccount)
            {
                _accountMessage.SetText("This email or phone number is not registered yet");
                return;
            }

            var account = Main.Database.Accounts.Values.FirstOrDefault(i => i.Email == _accountInput || i.PhoneNumber == _accountInput);

            if (_passwordInput != account.Password)
            {
                _passwordMessage.SetText("Incorrect password! Please check again.");
                return;
            }

            Marker.OnViewPageSwitched?.Invoke(ViewType.MainViewHomePage, true, true);
        }

        private void RecoveryAccount(PointerUpEvent evt)
        {

        }
    }
}
