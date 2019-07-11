using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PineGroveWebForms
{
    public partial class ContactForm : Page
    {
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (CaptchaPass())
            {
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("PineGroveWebsite@donotreply.com");
                    message.To.Add(new MailAddress("PineGroveUnitedChurch@gmail.com"));
                    message.Subject = txtFirstName.Text + " " + txtLastName.Text + " - Message from Pine Grove Website";
                    message.Body = txtComments.Text;
                    SmtpClient client = new SmtpClient
                    {
                        UseDefaultCredentials = false,
                        Host = "smtp.gmail.com",
                        EnableSsl = true,
                        Port = 587,
                        Credentials = new NetworkCredential("PineGroveUnitedChurch@gmail.com", "sqymbuvhghjnocrg")
                    };
                    //client.Send(message);
                }
                ClientScript.RegisterStartupScript(GetType(), "Success!", "alert('Successful send!');", true);
                ClearForm();
            }
            else
                ClientScript.RegisterStartupScript(GetType(), "Failure", "alert('Error - Failed reCAPTCHA Response.');", true);
        }

        private bool CaptchaPass()
        {
            string responseMessage = Request["g-recaptcha-response"];
            bool validResponse = false;
            HttpWebRequest googleRequest = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdBE60UAAAAAA3sTZ2Rt02vhd9bcxTGffoc-4gq&response=" + responseMessage);
            try
            {
                using (WebResponse googleResponse = googleRequest.GetResponse())
                using (StreamReader responseReader = new StreamReader(googleResponse.GetResponseStream()))
                {
                    string jsonResponse = responseReader.ReadToEnd();
                    JavaScriptSerializer deserializer = new JavaScriptSerializer();
                    GoogleObject data = deserializer.Deserialize<GoogleObject>(jsonResponse);
                    validResponse = Convert.ToBoolean(data.success);
                }
            }
            catch (WebException)
            {
                return false;
            }
            return validResponse;
        }

        private void ClearForm()
        {
            foreach (Control control in Page.Form.Controls)
                if (control is TextBox)
                    (control as TextBox).Text = string.Empty;
                else if (control is CheckBox)
                    (control as CheckBox).Checked = false;
        }

        private sealed class GoogleObject
        {
            public string success { get; set; }
        }
    }
}