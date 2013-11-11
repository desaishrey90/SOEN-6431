using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using LinqCommerce;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;

/// <summary>
/// Summary description for ValidatePayment
/// </summary>
public class ValidatePayment
{
    #region Properties

    private static string AuthNetVersion = "3.1"; // Contains CCV support
    public static Boolean isPayPal;
    #endregion

    public ValidatePayment()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static bool DoAuthorizePayment(out string message, string FirstName, string LastName, string Address, string Address2,
    string City, string State, string ZIP, string Country, double Amount
        , string Company, bool IsTest, string CCNumber, string ExpireDate, string CCV, string Phone)
    {
        if (isPayPal)
        {
            message = null;
            String strRequestID = PayflowUtility.RequestId;
            SectionConfigurationGroup config = (SectionConfigurationGroup)WebConfigurationManager.GetSection("LinqCommerce/Payment");

            UserInfo User = new UserInfo(config.PayPal.UserID, config.PayPal.VendorID, config.PayPal.Partner, config.PayPal.PayFlowPassword);
            PayflowConnectionData Connection = new PayflowConnectionData("pilot-payflowpro.paypal.com", 443, 45, "", 0, "", "");
            Invoice Inv = new Invoice();
            Currency Amt = new Currency(new decimal(Amount), "USD");
            Inv.Amt = Amt;
            // Create the BillTo object.
            BillTo Bill = new BillTo();

            // Set the customer name.
            Bill.FirstName = FirstName;
            // Bill.MiddleName = "M";
            Bill.LastName = LastName;
            Bill.CompanyName = Company;

            // It is highly suggested that you pass at minimum Street and Zip for AVS response.
            // However, AVS is only supported by US banks and some foreign banks.  See the Payflow 
            // Developer's Guide for more information.  Sending these fields could help in obtaining
            // a lower discount rate from your Internet merchant Bank.  Consult your bank for more information.

            Bill.Street = Address;
            // Secondary street address.
            Bill.BillToStreet2 = Address2;
            Bill.City = City;
            Bill.State = State;
            Bill.Zip = ZIP;

            Bill.PhoneNum = Phone;
            Inv.BillTo = Bill;
            CreditCard CC = new CreditCard(CCNumber, ExpireDate);
            // *** Card Security Code ***
            // This is the 3 or 4 digit code on either side of the Credit Card.
            // It is highly suggested that you obtain and pass this information to help prevent fraud.
            // Sending this fields could help in obtaining a lower discount rate from your Internet merchant Bank.
            // CVV2 is not required when performing a Swipe transaction as the card is present.
            CC.Cvv2 = CCV;
            // Name on Credit Card is optional.
            //CC.Name = "Joe M Smith";

            // *** Create a new Tender - Card Tender data object. ***
            CardTender Card = new CardTender(CC);  // credit card
            //CardTender Card = new CardTender(Swipe); // swipe card

            // *** Create a new Sale Transaction. ***
            // The Request Id is the unique id necessary for each transaction.  If you are performing an authorization
            // - delayed capture trnsaction, make sure that you pass two different unique request ids for each of the
            // transaction.
            // If you pass a non-unique request id, you will receive the transaction details from the original request.
            // The only difference is you will also receive a parameter DUPLICATE indicating this request id has been used
            // before.
            // The Request Id can be any unique number such lc_Order id, invoice number from your implementation or a random
            // id can be generated using the PayflowUtility.RequestId.
            SaleTransaction Trans = new SaleTransaction(User, Connection, Inv, Card, strRequestID);

            // Transaction results (especially values for declines and error conditions) returned by each PayPal-supported
            // processor vary in detail level and in format. The Payflow Verbosity parameter enables you to control the kind
            // and level of information you want returned. 

            // By default, Verbosity is set to LOW. A LOW setting causes PayPal to normalize the transaction result values. 
            // Normalizing the values limits them to a standardized set of values and simplifies the process of integrating 
            // the Payflow SDK.

            // By setting Verbosity to MEDIUM, you can view the processor’s raw response values. This setting is more “verbose”
            // than the LOW setting in that it returns more detailed, processor-specific information. 

            //  Review the chapter in the Payflow Pro Developer's Guide regarding VERBOSITY and the INQUIRY function for more details.

            // Set the transaction verbosity to MEDIUM.
            Trans.Verbosity = "MEDIUM";

            // Submit the Transaction
            Response Resp = Trans.SubmitTransaction();
            // Display the transaction response parameters.
            if (Resp != null)
            {
                // Get the Transaction Response parameters.
                TransactionResponse TrxnResponse = Resp.TransactionResponse;

                #region Docs
                // Refer to the Payflow Pro .NET API Reference Guide and the Payflow Pro Developer's Guide
                // for explanation of the items returned and for additional information and parameters available.
                //if (TrxnResponse != null)
                //{
                //    Console.WriteLine("Transaction Response:");
                //        Console.WriteLine("Result Code (RESULT) = " + TrxnResponse.Result);
                //        Console.WriteLine("Transaction ID (PNREF) = " + TrxnResponse.Pnref);
                //        Console.WriteLine("Response Message (RESPMSG) = " + TrxnResponse.RespMsg);
                //        Console.WriteLine("Authorization (AUTHCODE) = " + TrxnResponse.AuthCode);
                //        Console.WriteLine("Street Address Match (AVSADDR) = " + TrxnResponse.AVSAddr);
                //        Console.WriteLine("Streep Zip Match (AVSZIP) = " + TrxnResponse.AVSZip);
                //        Console.WriteLine("International Card (IAVS) = " + TrxnResponse.IAVS);
                //        Console.WriteLine("CVV2 Match (CVV2MATCH) = " + TrxnResponse.CVV2Match);
                //        Console.WriteLine("------------------------------------------------------");
                //        Console.WriteLine("Verbosity Response:");
                //        Console.WriteLine("Processor AVS (PROCAVS) = " + TrxnResponse.ProcAVS);
                //        Console.WriteLine("Processor CSC (PROCCVV2) = " + TrxnResponse.ProcCVV2);
                //}

                // Get the Fraud Response parameters.
                // All trial accounts come with basic Fraud Protection Services enabled.
                // Review the PayPal Manager guide to set up your Fraud Filters prior to 
                // running this sample code.
                // If Fraud Filters are not set, you will receive a RESULT code 126.
                //FraudResponse FraudResp =  Resp.FraudResponse;
                //if (FraudResp != null)
                //{
                //    Console.WriteLine("Fraud Response:");
                //    Console.WriteLine("Pre-Filters (PREFPSMSG) = " + FraudResp.PreFpsMsg);
                //    Console.WriteLine("Post-Filters (POSTFPSMSG) = " + FraudResp.PostFpsMsg);
                //}

                // Was this a duplicate transaction?
                // If this value is true, then the orignal response of the orginal transaction will
                // be returned.  The transction type listed in Manager will be "N" and the Original
                // Transaction ID will be the PNREF of the original transaction.  The value would be
                // true if the Request ID of the Transaction Object has not been changed.
                //Console.WriteLine("------------------------------------------------------");
                //Console.WriteLine("Duplicate Response:");
                //string DupMsg; 
                //if (TrxnResponse.Duplicate == "1") 
                //{ 
                //    DupMsg = "Duplicate Transaction"; 
                //} 
                //else 
                //{ 
                //    DupMsg = "Not a Duplicate Transaction"; 
                //} 
                //Console.WriteLine(("Duplicate Transaction (DUPLICATE) = " + DupMsg));

                // Example of adding in business rules.  This is not an exhaustive list of failures or issues
                // that could arise.  Review the list of Result Code's in the Developer's Guide and add logic as 
                // you deem necessary.
                // 
                // These responses are just an example of what you can do and how you handle the response received
                // from the bank is dependent on your business rules and needs.
                #endregion
                //string RespMsg; 
                // Evaluate Result Code
                if (TrxnResponse.Result == 0)
                {
                    message = "Your transaction was approved. Will ship in 24 hours.";
                    return true;
                }
                else
                {
                    if (TrxnResponse.Result < 0)
                    {
                        // Transaction failed.
                        message = "There was an error processing your transaction. Please contact Customer Service." + Environment.NewLine + "Error: " + TrxnResponse.Result.ToString();
                    }
                    else if (TrxnResponse.Result == 1 || TrxnResponse.Result == 26)
                    {
                        // This is just checking for invalid login information.  You would not want to display this to your customers.
                        message = "Account configuration issue.  Please verify your login credentials.";
                    }
                    else if (TrxnResponse.Result == 12)
                    {
                        // Hard decline from bank.
                        message = "Your transaction was declined.";
                    }
                    else if (TrxnResponse.Result == 13)
                    {
                        // Voice authorization required.
                        message = "Your Transaction is pending. Contact Customer Service to complete your lc_Order.";
                    }
                    else if (TrxnResponse.Result == 23 || TrxnResponse.Result == 24)
                    {
                        // Issue with credit card number or expiration date.
                        message = "Invalid credit card information. Please re-enter.";
                    }
                    else if (TrxnResponse.Result == 125)
                    {
                        // 125, 126 and 127 are Fraud Responses.
                        // Refer to the Payflow Pro Fraud Protection Services User's Guide or Website Payments Pro Payflow Edition - Fraud Protection Services User's Guide.
                        message = "Your Transactions has been declined. Contact Customer Service.";
                    }
                    else if (TrxnResponse.Result == 126)
                    {
                        // Decline transaction if AVS fails.
                        if ((TrxnResponse.AVSAddr != "Y" | TrxnResponse.AVSZip != "Y"))
                        {
                            // Display message that transaction was not accepted.  At this time, you
                            // could display message that information is incorrect and redirect user 
                            // to re-enter STREET and ZIP information.  However, there should be some sort of
                            // 3 strikes your out check.
                            message = "Your billing information does not match. Please re-enter.";
                        }
                        else
                        {
                            message = "Your Transaction is Under Review. We will notify you via e-mail if accepted.";
                        }
                    }
                    else if (TrxnResponse.Result == 127)
                    {
                        message = "Your Transaction is Under Review. We will notify you via e-mail if accepted.";
                    }
                    else
                    {
                        // Error occurred, display normalized message returned.
                        message = TrxnResponse.RespMsg;
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            
        

        SectionConfigurationGroup config = (SectionConfigurationGroup)WebConfigurationManager.GetSection("LinqCommerce/AuthorizeNETSettings");
        message = null;
        WebClient objRequest = new WebClient();
        System.Collections.Specialized.NameValueCollection objInf =
          new System.Collections.Specialized.NameValueCollection(30);
        System.Collections.Specialized.NameValueCollection objRetInf =
          new System.Collections.Specialized.NameValueCollection(30);
        byte[] objRetBytes;
        string[] objRetVals;
        string strError;

        objInf.Add("x_version", AuthNetVersion);
        objInf.Add("x_delim_data", "True");
        objInf.Add("x_login", config.AuthorizeNET.APILogin);
        //objInf.Add("x_password", AuthNetPassword);
        objInf.Add("x_tran_key", config.AuthorizeNET.TransactionKey);
        objInf.Add("x_relay_response", "False");

        // Switch this to False once you go live
        objInf.Add("x_test_request", "True");

        objInf.Add("x_delim_char", ",");
        objInf.Add("x_encap_char", "|");

        // Billing Address
        objInf.Add("x_first_name", FirstName);
        objInf.Add("x_last_name", LastName);
        objInf.Add("x_address", Address);
        objInf.Add("x_city", City);
        objInf.Add("x_state", State);
        objInf.Add("x_zip", ZIP);
        objInf.Add("x_country", Country);

        objInf.Add("x_description", "lc_Order");

        // Card Details
        objInf.Add("x_card_num", CCNumber);
        //objInf.Add("x_exp_date", "01/06");
        objInf.Add("x_exp_date", ExpireDate);
        // Authorisation code of the card (CCV)
        objInf.Add("x_card_code", CCV);

        objInf.Add("x_method", "CC");
        objInf.Add("x_type", "AUTH_CAPTURE");
        objInf.Add("x_amount", Amount.ToString());

        // Currency setting. Check the guide for other supported currencies
        objInf.Add("x_currency_code", "USD");

        try
        {
            // Pure Test Server
            if (IsTest)
            {
                objRequest.BaseAddress =
                  "https://test.authorize.net/gateway/transact.dll";
            }

            // Actual Server
            else
            {
                objRequest.BaseAddress =
                  "https://secure.authorize.net/gateway/transact.dll";
            }

            objRetBytes =
              objRequest.UploadValues(objRequest.BaseAddress, "POST", objInf);
            objRetVals =
              System.Text.Encoding.ASCII.GetString(objRetBytes).Split(",".ToCharArray());

            if (objRetVals[0].Trim(char.Parse("|")) == "1")
            {
                // Returned Authorisation Code
                //this.lblAuthNetCode.Text = objRetVals[4].Trim(char.Parse("|"));
                // Returned Transaction ID
                //this.lblAuthNetTransID.Text = objRetVals[6].Trim(char.Parse("|"));
                //message = objRetVals[4].Trim(char.Parse("|"));
                message = "Success! Your lc_Order shall be shipped soon.";
                return true;
            }
            else
            {
                // Error!
                strError = objRetVals[3].Trim(char.Parse("|")) + " (" +
                  objRetVals[2].Trim(char.Parse("|")) + ")";

                if (objRetVals[2].Trim(char.Parse("|")) == "44")
                {
                    // CCV transaction decline
                    strError += "Our Card Code Verification (CCV) returned " +
                      "the following error: ";

                    switch (objRetVals[38].Trim(char.Parse("|")))
                    {
                        case "N":
                            strError += "Card Code does not match.";
                            break;
                        case "P":
                            strError += "Card Code was not processed.";
                            break;
                        case "S":
                            strError += "Card Code should be on card but was not indicated.";
                            break;
                        case "U":
                            strError += "Issuer was not certified for Card Code.";
                            break;
                    }
                }

                if (objRetVals[2].Trim(char.Parse("|")) == "45")
                {
                    if (strError.Length > 1)
                        strError += "<br />n";

                    // AVS transaction decline
                    strError += "Our Address Verification System (AVS) " +
                      "returned the following error: ";

                    switch (objRetVals[5].Trim(char.Parse("|")))
                    {
                        case "A":
                            strError += " the zip code entered does not match " +
                              "the billing address.";
                            break;
                        case "B":
                            strError += " no information was provided for the AVS check.";
                            break;
                        case "E":
                            strError += " a general error occurred in the AVS system.";
                            break;
                        case "G":
                            strError += " the credit card was issued by a non-US bank.";
                            break;
                        case "N":
                            strError += " neither the entered street address nor zip " +
                              "code matches the billing address.";
                            break;
                        case "P":
                            strError += " AVS is not applicable for this transaction.";
                            break;
                        case "R":
                            strError += " please retry the transaction; the AVS system " +
                              "was unavailable or timed out.";
                            break;
                        case "S":
                            strError += " the AVS service is not supported by your " +
                              "credit card issuer.";
                            break;
                        case "U":
                            strError += " address information is unavailable for the " +
                              "credit card.";
                            break;
                        case "W":
                            strError += " the 9 digit zip code matches, but the " +
                              "street address does not.";
                            break;
                        case "Z":
                            strError += " the zip code matches, but the address does not.";
                            break;
                    }
                }

                // strError contains the actual error
                //lblMsg1.Text = strError;
                message = strError;
                return false;
            }
        }
        catch (Exception ex)
        {
            //lblMsg1.Text = ex.Message;
            message = ex.Message.ToString();
            return false;
        }
    }
    }

}