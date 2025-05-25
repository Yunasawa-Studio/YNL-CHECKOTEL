using System;

namespace YNL.Checkotel.Exceptions
{
    public class UninitializedSystemException : Exception
    {
        public static event Action OnUninitializedSystem;

        public UninitializedSystemException(string message) : base(message)
        {
            OnUninitializedSystem?.Invoke();
        }
    }
}
