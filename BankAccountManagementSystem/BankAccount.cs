using System;

public class BankAccount
{
    private string _accountNumber;
    private string _accountholderName;
    private string _accountType;
    private decimal _balance;

    public string AccountNumber
    {
        get
        {
            return _accountNumber;
        }

        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Invalid account number.");

            _accountNumber = value;
        }
    }
    public string AccountHolderName
    {
        get
        {
            return _accountholderName;
        }
        private set
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Account Holder name is not valid");
            }
            _accountholderName = value;
        }
    }
    public string AccountType
    {
        get
        {
            return _accountType;
        }
        private set
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(nameof(value), "Account type is not valid");
            }
            _accountType = value;
        }
    }
    public decimal Balance
    {
        get
        {
            return _balance;
        }
        private set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Balance cannot be in negative");
            }
            _balance = value;
        }
    }

    public BankAccount(string accountNumber, string accountholderName, string accountType, decimal balance)
    {
        AccountNumber = accountNumber;
        AccountHolderName = accountholderName;
        AccountType = accountType;
        Balance = balance;
    }


    public void DisplayAccountDetails()
    {
        Console.WriteLine("-----------------------Account Details-----------------");
        Console.WriteLine($"Account Holder Name: {AccountHolderName}");
        Console.WriteLine($"Account Number : {AccountNumber}");
        Console.WriteLine($"Account Type: {AccountType}");
        Console.WriteLine($"Balance: {Balance}");

    }

    public void Deposit(decimal amount)
    {        
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be greater than zero.");
        Balance += amount;
        DisplayAccountDetails();
    }

    public void Withdraw(decimal amount)
    {
        if(amount <= 0)
        throw new ArgumentOutOfRangeException(nameof(amount));
        if(amount>Balance)
        throw new ArgumentOutOfRangeException(nameof(amount),"Insufficient balance");

        Balance -= amount;
        DisplayAccountDetails();
    }

    public void Transfer(BankAccount receiver, decimal amount)
    {
        if(receiver == null)
        throw new ArgumentNullException(nameof(receiver));

        if(receiver == this)
        throw new InvalidOperationException("Cannot transfer to the same account.");

        Withdraw(amount);
        receiver.Deposit(amount);

    }

    public void UpdateAccountHolderName(string newName)
    {
        AccountHolderName = newName;
    }
}