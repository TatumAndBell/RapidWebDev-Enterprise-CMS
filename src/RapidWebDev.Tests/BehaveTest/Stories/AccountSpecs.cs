using System;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace NBehave.Examples
{
    [Theme("Account transfers and deposits")]
    public class AccountSpecs
    {
        [Story]
        public void Transfer_to_cash_account()
        {

            Account savings = null;
            Account cash = null;

            var transferStory = new Story("Transfer to cash account");

            transferStory
                .AsA("savings account holder")
                .IWant("to transfer money from my savings account")
                .SoThat("I can get cash easily from an ATM");

            transferStory
                .WithScenario("Savings account is in credit")

                    .Given("my savings account balance is $balance", 100, accountBalance => { savings = new Account(accountBalance); })
                        .And("my cash account balance is $balance", 10, accountBalance => { cash = new Account(accountBalance); })
                    .When("I transfer $amount to cash account", 20, transferAmount => savings.TransferTo(cash, transferAmount))
                    .Then("my savings account balance should be $balance", 80, expectedBalance => savings.Balance.ShouldEqual(expectedBalance))
                        .And("my cash account balance should be $balance", 30, expectedBalance => cash.Balance.ShouldEqual(expectedBalance))

                    .Given("my savings account balance is 400")
                        .And("my cash account balance is 100")
                    .When("I transfer 100 to cash account")
                    .Then("my savings account balance should be 300")
                        .And("my cash account balance should be 200")

                    .Given("my savings account balance is 500")
                        .And("my cash account balance is 20")
                    .When("I transfer 30 to cash account")
                    .Then("my savings account balance should be 470")
                        .And("my cash account balance should be 50");

            transferStory
                .WithScenario("Savings account is overdrawn")

                    .Given("my savings account balance is  -20")
                        .And("my cash account balance is 10")
                    .When("I transfer 20 to cash account")
                    .Then("my savings account balance should be -20")
                        .And("my cash account balance should be 10");


        }

        [Story]
        public void Withdraw_from_savings_account_pending()
        {

            var transferStory = new Story("Withdraw from savings account");

            transferStory
                .AsA("savings account holder")
                .IWant("to withdraw money from my savings account")
                .SoThat("I can pay for things with cash");

            transferStory
                .WithScenario("Savings account is in credit")
                    .Pending("ability to withdraw from accounts")

                    .Given("my savings account balance is 400")
                    .When("I withdraw $amount from my savings account", 100)
                    .Then("my savings account balance should be 300");

        }

        [Story]
        public void Deposit_not_implemented_properly()
        {

            var transferStory = new Story("Deposit to cash account");

            transferStory
                .AsA("savings account holder")
                .IWant("to deposit money into my cash account")
                .SoThat("I can have money for later");

            Account cash = null;

            transferStory
                .WithScenario("Savings account is in credit")
                .Given("my cash account balance is", 100,
                       accountBalance => { cash = new Account(accountBalance); })
                .When("I deposit into my cash account", 20, depositAmount => cash.Deposit(depositAmount))
                .Then("my cash account balance should be", 120,
                      expectedBalance => cash.Balance.ShouldEqual(expectedBalance));

        }
    }

    public class Account
    {
        private int _accountBalance;

        public Account(int accountBalance)
        {
            _accountBalance = accountBalance;
        }

        public int Balance
        {
            get { return _accountBalance; }
            set { _accountBalance = value; }
        }

        public void TransferTo(Account account, int amount)
        {
            if (_accountBalance > 0)
            {
                account.Balance = account.Balance + amount;
                Balance = Balance - amount;
            }
        }

        public void Withdraw(int amount)
        {
            if (amount > Balance)
                throw new ArgumentException("Amount exceeds balance", "amount");

            Balance -= amount;
        }

        public void Deposit(int amount)
        {
            // not implemented yet, do nothing for now
        }

        public override string ToString()
        {
            return string.Format("[Balance: {0}]", Balance);
        }
    }




}
