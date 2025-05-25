using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace YNL.Checkotel
{
    public class AccountGenerator : MonoBehaviour
    {
        [SerializeField] private DatabaseContainerSO Database;
        [SerializeField] private int _amount = 100;

        [Button]
        public void Generate()
        {
            Database.Accounts.Clear();
            UID.SUID = 0;
            GenerateRandomAccounts(_amount);
        }

        private System.Random random = new System.Random();
        private string[] firstNames = {
            "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles",
            "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Kenneth",
            "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Jacob"
        };

        private string[] lastNames = {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
        };

        public void GenerateRandomAccounts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Account account = new Account
                {
                    Name = GenerateRandomName(),
                    Email = GenerateRandomEmail(),
                    PhoneNumber = GenerateRandomPhoneNumber(),
                    Password = GenerateRandomPassword(),
                    Type = AccountType.Customer
                };

                Database.Accounts[account.ID] = account;
            }
        }

        private string GenerateRandomName()
        {
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];
            return $"{firstName} {lastName}";
        }

        private string GenerateRandomEmail()
        {
            string[] domains = { "@gmail.com", "@yahoo.com", "@outlook.com" };
            return $"user{random.Next(1000, 9999)}{domains[random.Next(domains.Length)]}";
        }

        private string GenerateRandomPhoneNumber()
        {
            return $"+{random.Next(1, 99)} {random.Next(100, 999)}-{random.Next(1000, 9999)}";
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
    }
}