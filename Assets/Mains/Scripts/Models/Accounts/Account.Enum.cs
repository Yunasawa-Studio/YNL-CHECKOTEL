namespace YNL.Checkotel
{
    public enum AccountDeletionResult
    {
        AccountNotFound,
        AccountDeletionFailed,
        AccountDeletionSuccess
    }

    public enum AccountSignType
    {
        SignUp,
        SignIn
    }

    public enum AccountVerificationResult
    {
        SignUpPhoneNumberNotValid,  // Phone number not valid or not found
        SignUpPhoneNumberHasExisted,// Phone number has existed or has been taken
        SignUpEmailNotValid,        // Email not valid or not found
        SignUpEmailHasExisted,      // Email has existed or has been taken
        SignUpPasswordTooWeak,      // Password is too short or too weak
        SignUpPasswordNotMatch,     // Password not match
        SignUpSuccess,              // Sign up success

        SignInAccountDisabled,      // Account is disabled or banned
        SignInAccountNotFound,      // Account is disabled or banned
        SignInPhoneNumberNotValid,  // Phone number not valid or not found
        SignInEmailNotValid,        // Email not valid or not found
        SignInPasswordNotCorrect,   // Password not correct
        SignInSessionExpired,       // Authentication session has timed out
        SignInSuccess,              // Sign in success
    }

    public enum AccountVerificationType
    {
        Email,
        PhoneNumber
    }
}
