﻿using System;
using System.Web.UI;
using LinqCommerce;

public partial class LINQControls_ControlTemplates_PaymentForm : System.Web.UI.UserControl
{
    #region Properties

    public LinqOrderAccess lo
    {
        get
        {
            return new LinqOrderAccess();
        }
    }

    public double Tax
    {
        get
        {
            return Convert.ToDouble(Session["Tax"]);
        }
    }

    public int lc_OrderID
    {
        get
        {
            return Convert.ToInt16(Session["lc_OrderID"]);
        }
    }

    public string BillingState
    {
        get
        {
            return BillStateDropDown.Text;
        }
    }


    public string CreditCardNumber
    {
        get
        {
            return CreditCardNumberTextBox.Text;
        }
    }

    public string CVC
    {
        get
        {
            return CVCTextBox.Text;
        }
    }

    public string TypeofCard
    {
        get
        {
            return TypeCardDropDown.Text;
        }
    }

    public string Month
    {
        get
        {
            return MonthDropDown.Text;
        }
    }

    public string Year
    {
        get
        {
            return YearDropDown.SelectedValue;
        }
    }

    public double Amount
    {
        get
        {
            return Convert.ToDouble(Session["Amount"]);
        }
    }

    public string IPAddress1
    {
        get
        {
            return Request.UserHostAddress;
        }
    }

    public string ExpireDate
    {
        get
        {
            return string.Concat(Month, Year);
        }
    }


    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadProfile();
        }
    }

    //Load the user's billing and shipping profile on the page
    private void LoadProfile()
    {
        //Put all Billing Info into the form for viewing
        BillFNTB.Text = Profile.Billing.FirstName;
        BillLNTB.Text = Profile.Billing.LastName;
        BillMNTB.Text = Profile.Billing.MiddleName;
        BillNNTB.Text = Profile.Billing.NickName;
        BillPrefixCombo.SelectedValue = Profile.Billing.Prefix;
        BillPhoneTB.Text = Profile.Billing.Phone;
        BillFaxTB.Text = Profile.Billing.Fax;
        BillAdTB.Text = Profile.Billing.Address;
        BillAd2TB.Text = Profile.Billing.Address2;
        BillCityTB.Text = Profile.Billing.City;
        BillStateDropDown.SelectedValue = Profile.Billing.State;
        BillZipCodeTB.Text = Profile.Billing.Zip;
        BillStateDropDown.SelectedValue = Profile.Billing.State;
        BillCountryDropDown.SelectedValue = Profile.Billing.Country;

        //Put all Shipping Info into the form for shipping
        ShipFNTB.Text = Profile.Shipping.FirstName;
        ShipLNTB.Text = Profile.Shipping.LastName;
        ShipMNTB.Text = Profile.Shipping.MiddleName;
        ShipNNTB.Text = Profile.Shipping.NickName;
        ShipPrefixCombo.SelectedValue = Profile.Shipping.Prefix;
        ShipPhoneTB.Text = Profile.Shipping.Phone;
        ShipFaxTB.Text = Profile.Shipping.Fax;
        ShipADTB.Text = Profile.Shipping.Address;
        ShipAD2TB.Text = Profile.Shipping.Address2;
        ShipCityTB.Text = Profile.Shipping.City;
        ShipStateDropDown.SelectedValue = Profile.Shipping.State;
        ShipZipCodeTB.Text = Profile.Shipping.Zip;
        ShipStateDropDown.SelectedValue = Profile.Shipping.State;
        ShipCountryDropDown.SelectedValue = Profile.Shipping.Country;
    }

    //Submit the lc_Order for approval through Authorize.NET
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //shrey: duplicated code tech. done
        string message = null;
        bool t = ValidatePayment.DoAuthorizePayment(out message, BillFNTB.Text, BillLNTB.Text, BillAdTB.Text, BillAd2TB.Text, 
            BillCityTB.Text, BillStateDropDown.Text, BillZipCodeTB.Text, BillCountryDropDown.Text,
            Amount, BillCompany.Text, true, CreditCardNumber, Month + "/" + Year, CVC, BillPhoneTB.Text);
        //If lc_Order is successful, record it in the admin

        if (t)
        {
            
            //lo.InsertBilling(BillFNTB.Text, BillLNTB.Text, BillMNTB.Text, "", lc_OrderID, 
            //    BillPhoneTB.Text, BillPrefixCombo.Text, BillStateDropDown.Text, BillZipCodeTB.Text);
            lo.InsertBilling(new BillingInformation(BillFNTB.Text, BillLNTB.Text, BillMNTB.Text, "", lc_OrderID, BillPhoneTB.Text, BillPrefixCombo.Text, BillStateDropDown.Text, BillZipCodeTB.Text));
            

            //lo.InsertShipping(ShipFNTB.Text, ShipLNTB.Text, ShipMNTB.Text, "", lc_OrderID,
            //    ShipPhoneTB.Text, ShipPrefixCombo.Text, ShipStateDropDown.Text, ShipZipCodeTB.Text);
            lo.InsertShipping(new ShippingInformation(ShipFNTB.Text, ShipLNTB.Text, ShipMNTB.Text, "", lc_OrderID, ShipPhoneTB.Text, ShipPrefixCombo.Text, ShipStateDropDown.Text, ShipZipCodeTB.Text));

            
        }
        Redirect(message);
    }

    /// <summary>
    /// If the shipping and billing info are the same, then put the billing info in the shipping info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BillShipCheck_CheckedChanged(object sender, EventArgs e)
    {
        ProfileCommon _sp = new ProfileCommon();

        ProfileGroupBilling gb = new ProfileGroupBilling();
        gb.FirstName = _sp.FirstName;

        ShipFNTB.Text = BillFNTB.Text;
        ShipLNTB.Text = BillLNTB.Text;
        ShipNNTB.Text = BillNNTB.Text;
        ShipADTB.Text = BillAdTB.Text;
        ShipAD2TB.Text = BillAd2TB.Text;
        ShipCityTB.Text = BillCityTB.Text;
        ShipStateDropDown.SelectedValue = BillStateDropDown.SelectedValue;
        ShipCountryDropDown.SelectedValue = BillCountryDropDown.SelectedValue;
        ShipFaxTB.Text = BillFaxTB.Text;
        ShipMNTB.Text = BillMNTB.Text;
        ShipPhoneTB.Text = BillPhoneTB.Text;
        ShipPrefixCombo.SelectedValue = BillPrefixCombo.SelectedValue;
        ShipZipCodeTB.Text = BillZipCodeTB.Text;
    }

    /// <summary>
    /// Handles PayPal PayFlow Pro lc_Order
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void PayPalButton_Click(object sender, EventArgs e)
    {
        string RespMsg = null;
        //bool t = DOSaleCompleteCS.DoCompleteSale(out RespMsg, Amount, BillFNTB.Text, BillLNTB.Text, BillCompany.Text, BillAdTB.Text,
        //    BillAd2TB.Text, BillCityTB.Text, BillZipCodeTB.Text, BillStateDropDown.SelectedValue, CreditCardNumber, CVC,
        //    ExpireDate, BillPhoneTB.Text);
        //shrey: duplicated code tech. done
        ValidatePayment.isPayPal = true;
        bool t = ValidatePayment.DoAuthorizePayment(out RespMsg, BillFNTB.Text, BillLNTB.Text, BillAdTB.Text,
            BillAd2TB.Text, BillCityTB.Text, BillStateDropDown.SelectedValue, BillZipCodeTB.Text, BillCountryDropDown.Text, Amount, 
            BillCompany.Text, false, CreditCardNumber, ExpireDate, CVC, BillPhoneTB.Text);
        //If lc_Order is successful, record it in the admin
        if (t)
        {
            //lo.InsertBilling(BillFNTB.Text, BillLNTB.Text, BillMNTB.Text, "", lc_OrderID,
            //    BillPhoneTB.Text, BillPrefixCombo.Text, BillStateDropDown.Text, BillZipCodeTB.Text);

            lo.InsertBilling(new BillingInformation(BillFNTB.Text, BillLNTB.Text, BillMNTB.Text, "", lc_OrderID, BillPhoneTB.Text, BillPrefixCombo.Text, BillStateDropDown.Text, BillZipCodeTB.Text));

            //lo.InsertShipping(ShipFNTB.Text, ShipLNTB.Text, ShipMNTB.Text, "", lc_OrderID,
            //    ShipPhoneTB.Text, ShipPrefixCombo.Text, ShipStateDropDown.Text, ShipZipCodeTB.Text);
            lo.InsertShipping(new ShippingInformation(ShipFNTB.Text, ShipLNTB.Text, ShipMNTB.Text, "", lc_OrderID, ShipPhoneTB.Text, ShipPrefixCombo.Text, ShipStateDropDown.Text, ShipZipCodeTB.Text));
        }
        Redirect(RespMsg);
    }

    private void Redirect(string RespMsg)
    {
        Response.Redirect("~/ReceiptPage.aspx?Message=" + RespMsg);
    }
}