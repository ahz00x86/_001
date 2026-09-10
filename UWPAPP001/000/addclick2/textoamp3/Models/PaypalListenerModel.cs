using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace textoamp3.Models
{
    public class PayPalListenerModel //: BaseModel
    {
        public PayPalCheckoutInfo _PayPalCheckoutInfo { get; set; }

        public void GetStatus(byte[] parameters)
        {
            try
            {
                //verify the transaction             
                string status = Verify(false, parameters);
                if (status.ToUpper() == "VERIFIED")
                {
                    //check that the payment_status is Completed                 
                    if (_PayPalCheckoutInfo.payment_status.ToLower() == "completed")
                    {
                        if (_PayPalCheckoutInfo.receiver_email.ToLower() == "apzyx@yahoo.com")
                        {
                            using (addclickEntities entity = new addclickEntities())
                            {
                                Paypal pDB = entity.Paypals.Where(x => x.txn_id == _PayPalCheckoutInfo.txn_id).SingleOrDefault();
                                if (pDB == null)
                                {
                                    String mailUser = _PayPalCheckoutInfo.custom.ToLower();
                                    Users_Data2 uDB = entity.Users_Data2.Where(x => x.email == mailUser).SingleOrDefault();

                                    if (uDB != null)
                                    {

                                        uDB.prices += _PayPalCheckoutInfo.Total;

                                        entity.Paypals.Add(new Paypal()
                                        {
                                            txn_id = _PayPalCheckoutInfo.txn_id,
                                            mail = _PayPalCheckoutInfo.custom,
                                            prices = _PayPalCheckoutInfo.Total,
                                            paypalmail = _PayPalCheckoutInfo.payer_email,
                                            fecha = DateTime.Now
                                        });

                                        if (_PayPalCheckoutInfo.Total == new Decimal(5.99))
                                        {
                                            uDB.fecha = DateTime.Now.AddDays(1);
                                        }
                                        else if (_PayPalCheckoutInfo.Total == new decimal(50))
                                        {
                                            uDB.fecha = DateTime.Now.AddDays(31);
                                        }

                                        entity.SaveChanges();
                                    }
                                    else
                                    {
                                        // MARCAR PAGO ERROR
                                    }
                                }
                            }
                        }
                        ////////check that txn_id has not been previously processed to prevent duplicates                      

                        ////////check that receiver_email is your Primary PayPal email                                          

                        ////////check that payment_amount/payment_currency are correct                       

                        ////////process payment/refund/etc                     

                    }
                    else if (status == "INVALID")
                    {

                        //log for manual investigation             
                    }
                    else
                    {
                        //log response/ipn data for manual investigation             
                    }

                }
            }
            catch (Exception ex)
            {
                Utils.SendMail("apzyx@yahoo.com", "00X EXCEPTION", "(= ... =)" +  ex.ToString());
            }

        }

        private string Verify(bool isSandbox, byte[] parameters)
        {

            string response = "";
            try
            {

                string url = isSandbox ?
                  "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //must keep the original intact and pass back to PayPal with a _notify-validate command
                string data = Encoding.ASCII.GetString(parameters);
                data += "&cmd=_notify-validate";

                webRequest.ContentLength = data.Length;

                //Send the request to PayPal and get the response                 
                using (StreamWriter streamOut = new StreamWriter(webRequest.GetRequestStream(), System.Text.Encoding.ASCII))
                {
                    streamOut.Write(data);
                    streamOut.Close();
                }

                using (StreamReader streamIn = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                    streamIn.Close();
                }

            }
            catch { }

            return response;

        }
    }
}