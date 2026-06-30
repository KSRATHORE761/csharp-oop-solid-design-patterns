using System;

public class Program
{
    public static void Main(string[] args)
    {
        BankAccount bankAccount1= new BankAccount("123456789012","Kuldeep Singh Rathore","Saving",100000);
        BankAccount bankAccount2= new BankAccount("123456789013","Sushant Das","Current",10000);
        
        // Display account details;
        bankAccount1.DisplayAccountDetails();
        bankAccount2.DisplayAccountDetails();

        //Depost and check details;
        bankAccount1.Deposit(200000);
        bankAccount2.Deposit(1300);

        //deposit and check details;
        bankAccount1.Withdraw(20000);
        bankAccount2.Withdraw(10000);

    }
}