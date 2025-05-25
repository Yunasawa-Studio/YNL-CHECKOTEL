namespace YNL.Checkotel
{
    public enum AccountType : byte
    { 
        Customer, Owner, Manager
    }

    [System.Serializable]
    public class Account
    {
        public UID ID;
        public string Name = string.Empty;
        public string Email = string.Empty;
        public string PhoneNumber = string.Empty;
        public string Password = string.Empty;
        public AccountType Type = AccountType.Customer;

        public Account()
        {
            ID = UID.Create(UIDType.Account);
            Name = $"User{ID}";
        }
    }
}
