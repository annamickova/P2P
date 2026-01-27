namespace P2P.DataLayer;

using System;
using System.Collections.Generic;
using System.Data;

public class DbBankAccountDAO : IGenericDao<BankAccount>
{
    private readonly Func<IDbConnection> _connectionFactory;

    public DbBankAccountDAO(Func<IDbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Initialize()
    {
        using IDbConnection conn = _connectionFactory();
        conn.Open();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = "create table if not exists accounts (account_number int primary key auto_increment, balance bigint)";
        cmd.ExecuteNonQuery();
    }

    public List<BankAccount> GetAll()
    {
        var bankAccounts = new List<BankAccount>();
        using IDbConnection conn = _connectionFactory();
        conn.Open();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = "select * from accounts";
        
        using IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            bankAccounts.Add(new BankAccount
            {
                AccountNumber = Convert.ToInt32(reader["account_number"]),
                Balance = Convert.ToInt64(reader["balance"])
            });
        }
        return bankAccounts;
    }

    public BankAccount? GetById(int id)
    {
        using IDbConnection conn = _connectionFactory();
        conn.Open();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = "select * from accounts where account_number = @id";
        
        IDbDataParameter param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        using IDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new BankAccount
            {
                AccountNumber = Convert.ToInt32(reader["account_number"]),
                Balance = Convert.ToInt64(reader["balance"])
            };
        }
        return null;
    }

    public bool Save(BankAccount bankAccount)
    {
        try
        {
            using IDbConnection conn = _connectionFactory();
            conn.Open();
            using IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into accounts (account_number, balance) values (@id, @balance)";
            
            AddParam(cmd, "@id", bankAccount.AccountNumber);
            AddParam(cmd, "@balance", bankAccount.Balance);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }
    
    public bool Update(BankAccount bankAccount) {
        try
        {
            using IDbConnection conn = _connectionFactory();
            conn.Open();
            using IDbCommand cmd = conn.CreateCommand();
        
            cmd.CommandText = "update accounts set balance = @balance where account_number = @id";
        
            AddParam(cmd, "@id", bankAccount.AccountNumber);
            AddParam(cmd, "@balance", bankAccount.Balance);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch 
        { 
            return false; 
        }
        
    }
    public bool Delete(int id) { /* ... */ return false; }

    private void AddParam(IDbCommand cmd, string name, object value)
    {
        var parameter = cmd.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        cmd.Parameters.Add(parameter);
    }
}