using System;

namespace YNL.Checkotel
{
    public static partial class Marker
    { 
        public static Action<AccountVerificationResult> OnAccountVerificated { get; set; }
        public static Action<AccountDeletionResult> OnAccountDeleted { get; set; }
    }
}
