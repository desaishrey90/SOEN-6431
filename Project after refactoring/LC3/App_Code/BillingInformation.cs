using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BillingInformation
/// </summary>
public class BillingInformation
{
    private string _FirstName, _LastName, _MiddleName, _NickName, _Phone, _Prefix, _State, _Zip;
    private int _OrderID;
    public BillingInformation(string FirstName, string LastName, string MiddleName, string NickName, int OrderID, string Phone, string Prefix, string State, string Zip)
	{
        _FirstName = FirstName;
        _LastName = LastName;
        _MiddleName = MiddleName;
        _NickName = NickName;
        _OrderID = OrderID;
        _Phone = Phone;
        _Prefix = Prefix;
        _State = State;
        _Zip = Zip;
	}

    public String getFirstName()
    {
        return _FirstName;
    }

    public String getLastName()
    {
        return _LastName;
    }

    public String getMiddleName()
    {
        return _MiddleName;
    }

    public String getNickName()
    {
        return _NickName;
    }

    public int getOrderID()
    {
        return _OrderID;
    }

    public String getPhone()
    {
        return _Phone;
    }

    public String getPrefix()
    {
        return _Prefix;
    }

    public String getState()
    {
        return _State;
    }

    public String getZip()
    {
        return _Zip;
    }
}