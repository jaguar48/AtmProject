# ATM OPERATIONS

This is a C# project that contains the implementation of the IBankOperation interface, which provides the following functionalities:

   * Create a new customer

   * Withdraw an amount from a customer's account

   * Check the balance of a customer's account
   Transfer an amount from a customer's account to another customer's account

The project uses a CustomerViewModel class to represent customer data and a TransactionViewModel class to represent transaction data. 

The Atmservice class is used to manage the database connection.
Installation

To use this project, you need to have the following installed on your machine:

    .NET 5 SDK
    Microsoft.Data.SqlClient

To install the Microsoft.Data.SqlClient package, run the following command in the Package Manager Console:

```Install-Package Microsoft.Data.SqlClient```

## Usage

To use this project, you can create an instance of the BankOperations class, passing an instance of the Atmservice class as a parameter. Then, you can call the methods of the IBankOperation interface to perform the desired operations.

## Creating a new customer

To create a new customer, call the CreateCustomer method, passing a CustomerViewModel object with the customer's data as a parameter. The method returns a Task<string> that represents the result of the operation.

```csharp

var customer = new CustomerViewModel {
    Name = "John Doe",
    AccountNumber = 123456789,
    Pin = 1234,
    Balance = 1000
};

var result = await bankOperations.CreateCustomer(customer);

Console.WriteLine(result);
```

## Withdrawing an amount from a customer's account

To withdraw an amount from a customer's account, call the Withdraw method, passing the account number as a parameter. The method prompts the user to enter the amount to withdraw and returns a Task<CustomerViewModel> that represents the updated customer data.

```csharp

var customer = await bankOperations.Withdraw(123456789);

Console.WriteLine($"Customer balance: {customer.Balance}");
```

Checking the balance of a customer's account

To check the balance of a customer's account, call the GetBalance method, passing the account number as a parameter. The method returns a Task<decimal> that represents the customer's balance.

```csharp

var balance = await bankOperations.GetBalance(123456789);


Console.WriteLine($"Customer balance: {balance}");
```

## Transferring an amount from a customer's account to another customer's account

To transfer an amount from a customer's account to another customer's account, call the Transfer method, passing the source account number and the destination account number as parameters. The method prompts the user to enter the amount to transfer.

```csharp

await bankOperations.Transfer(123456789, 987654321);
```

License

This project is licensed under the MIT License.# Foobar
