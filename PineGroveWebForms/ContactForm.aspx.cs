using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PineGroveWebForms
{
    public partial class ContactForm : Page
    {
        /// <summary>
        /// This is called upon the user clicking the submit button.
        /// </summary>
        /// <param name="sender">The object that caused this event.</param>
        /// <param name="e">The event arguments.</param>
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Note, the client-side JavaScript prevents the page being posted until the information is valid on the form.

            // Check to see if the user passed the reCAPTCHA.
            if (CaptchaPass())
            {
                // If they did pass, we need to send the church an email!
                // Note - SmtpClient is depricated and will likely need to be replaced.
                // Also, upon deployment this will be changed to the church's own SMTP server.
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("PineGroveWebsite@donotreply.com");  // Don't reply to this email.
                    message.To.Add(new MailAddress("PineGroveUnitedChurch@gmail.com")); // To the church's account.
                    message.Subject = txtFirstName.Text + " " + txtLastName.Text + " - Message from Pine Grove Website";    // Let them know who is submitting information.
                    message.Body = txtComments.Text;    // Add their message to the body of the email.
                    // This currently uses a 'personal' email of mine and the free Google SMTP server (limited to 100 emails a day), which would be suitable to the church
                    // upon being deployed.
                    SmtpClient client = new SmtpClient
                    {
                        UseDefaultCredentials = false,  // We will provide the credentials.
                        Host = "smtp.gmail.com",        // Google's smtp server.
                        EnableSsl = true,               // This is required for Google.
                        Port = 587,                     // This is the TLS port.
                        Credentials = new NetworkCredential("PineGroveUnitedChurch@gmail.com", "sqymbuvhghjnocrg")  // This is the account we are using to send the email and it's secret app password.
                    };
                    // Because all of this would be on the server-side, it is safer to do it this way than client-side where any user can hit F12 and see the email and password.
                    client.Send(message);   // Send the message.
                }
                // Tell them that they were successful in emailing the church.
                ClientScript.RegisterStartupScript(GetType(), "Success!", "alert('Successful send!');", true);
                ClearForm();    // Clear the form.
            }
            else
                ClientScript.RegisterStartupScript(GetType(), "Failure", "alert('Error - Failed reCAPTCHA Response.');", true); // The user failed the reCAPTCHA.
        }

        /// <summary>
        /// If you are interested in how this works, read the comments on the EventsWebForm.aspx.cs page.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// This clears all of the controls on the screen that are used for input.
        /// </summary>
        private void ClearForm()
        {
            foreach (Control control in Page.Form.Controls)
                if (control is TextBox)
                    (control as TextBox).Text = string.Empty;
                else if (control is CheckBox)
                    (control as CheckBox).Checked = false;
        }

        /// <summary>
        /// This is the object that is deserialized into that will tell us if the user passed the reCAPTCHA or not.
        /// </summary>
        private sealed class GoogleObject
        {
            public string success { get; set; }
        }
    }
}