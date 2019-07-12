using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using PineGroveWebForms.Services;
using System.Data;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace PineGroveWebForms
{
    // I learned a lot doing this project, for sure. Part of it is shown here.
    public partial class EventsWebForm : Page
    {
        // Class variables.
        private EventsSingleton eventsSingleton;
        private string username;
        private int guests;

        /// <summary>
        /// This is called every time the page is loaded, including post-backs.
        /// </summary>
        /// <param name="sender">The object that caused this event.</param>
        /// <param name="e">The event arguments.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            eventsSingleton = EventsSingleton.Instance; // If there is no instance, one is created. If there is one, it sets it to the one that has already been created.
            LoadData(); // This get's the data from the events singleton.
        }

        /// <summary>
        /// This is called upon the pager changing the index of the page.
        /// </summary>
        /// <param name="sender">The object that caused this event.</param>
        /// <param name="e">The event arguments.</param>
        protected void EventView_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            eventView.PageIndex = e.NewPageIndex;   // Let's move the page to the new page.
            LoadData();                             // And reload the data to reflect the change.
        }

        /// <summary>
        /// This is called upon the register button being pressed.
        /// </summary>
        /// <param name="sender">The object that caused this event.</param>
        /// <param name="e">The event arguments.</param>
        protected async void BtnRegister_Click(object sender, EventArgs e)
        {
            // This event is only ever called after the client-side JavaScript returns true. So that means that the information is, up to this point, valid.
            // First, let's see if the user passed the Google reCAPTCHA test.
            if (CaptchaPass())
            {
                // If they passed, we get to try and register them for an event!
                try
                {
                    // All of this is already on the EventRegistrationPage of the mobile application, but I'll explain it here for fun.
                    RestClient client = new RestClient();   // Object that calls the API.
                    Models.User user = await client.GetUser(username);  // Getting the user from the username field.
                    // Making a new event registration model.
                    Models.EventRegistration eventRegistration = new Models.EventRegistration()
                    {
                        EventId = (int)(eventView.DataSource as DataTable).Rows[eventView.PageIndex]["eventId"],
                        Guests = guests,
                        UserId = user.UserId
                    };
                    // Getting the event from the current page.
                    Models.Event @event = (eventView.DataSource as DataTable).Rows[eventView.PageIndex]["event"] as Models.Event;
                    // Re-check to ensure that we can sign-up for this event still.
                    if (@event.CurrentAttendees >= @event.MaxAttendees)
                        throw new InvalidOperationException("Maximum reached!");
                    // Actually create the registration record in the database.
                    await client.CreateRegistration(eventRegistration);
                    // If they are volunteering they do not count against the event's maximum attendees. Otherwise, they will.
                    if (!(eventView.FindControl("chkVolunteering") as CheckBox).Checked)
                    {
                        @event.CurrentAttendees += eventRegistration.Guests + 1;    // Add the user and their guests to the event.
                        await client.UpdateEvent(@event.EventId, @event);           // Update the event record in the database.
                    }
                    // Show the user an alert signifying that they succeeded in the registration.
                    eventView.Page.ClientScript.RegisterStartupScript(GetType(), "Success", "alert('Registration successful!');", true);
                    // Re-call the API to populate the event view here.
                    eventsSingleton.Restart();
                    eventsSingleton = EventsSingleton.Instance;
                    // Re-bind the event data.
                    LoadData();
                }
                catch (Refit.ApiException)  // No username found.
                {
                    eventView.Page.ClientScript.RegisterStartupScript(GetType(), "NoUsernameFound", "alert('Error - No user exists with that username. Try again or download the Pine Grove App to become a member!');", true);
                }
                catch (InvalidOperationException)   // The event was already filled.
                {
                    eventView.Page.ClientScript.RegisterStartupScript(GetType(), "EventFull", "alert('Error - The event became full prior to form submission!');", true);
                }
            }
            // The user failed the reCAPTCHA.
            else
                eventView.Page.ClientScript.RegisterStartupScript(GetType(), "Failure", "alert('Error - Failed reCAPTCHA Response.');", true);
        }

        /// <summary>
        /// This checks to see whether or not the user passed the Google reCAPTCHA.
        /// </summary>
        /// <returns>A boolean indicating whether or not the user passed the Google reCAPTCHA.</returns>
        private bool CaptchaPass()
        {
            // First lets build a response message that requests from Google.
            string responseMessage = Request["g-recaptcha-response"];
            bool validResponse = false;
            // Now we need to actually request the information from google by passing it the message that was generated client-side. The secret=xxxxx part is a secret key generated by Google upon asking for a reCAPTCHA test.
            HttpWebRequest googleRequest = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdBE60UAAAAAA3sTZ2Rt02vhd9bcxTGffoc-4gq&response=" + responseMessage);
            try
            {
                // Now we have to get Google's response from the request we just sent. The response is actually a JSON object and we need to get the string from the key-value pair of "success":"xxxx".
                using (WebResponse googleResponse = googleRequest.GetResponse())
                using (StreamReader responseReader = new StreamReader(googleResponse.GetResponseStream()))
                {
                    string jsonResponse = responseReader.ReadToEnd();   // Read the response.
                    JavaScriptSerializer deserializer = new JavaScriptSerializer(); // Create a serializer so we can deserialize the data.
                    GoogleObject data = deserializer.Deserialize<GoogleObject>(jsonResponse);   // Deserialize the only part we care about into this object.
                    validResponse = Convert.ToBoolean(data.success);    // Convert the "true" or "false" string to a boolean to return.
                }
            }
            catch (WebException)
            {
                return false;   // Something problematic arose? Return false, then.
            }
            return validResponse;   // Otherwise, return the value that Google gave us back.
        }

        /// <summary>
        /// This will load the data from the DataTable object stored in the EventsSingleton instance.
        /// </summary>
        private async void LoadData()
        {
            eventView.Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;  // Originally I had ASP.NET Validators in place for this page but I ended up not using them in favor of JavaScript instead.

            // These next few lines are doozies... But bear with me. The post-back of the form actually calls Page_Load and resets all textboxes without sending any of the information due to this page being a
            // form view with an item template. This poses several issues. One of which is that I cannot access the data from any individual page... another is that I cannot directly access any control from
            // any individual page. Instead, I can search for the control on the current event view form. HOWEVER, the first time this is called (the initial Page_Load event), the controls do not exist as they
            // have not been bound to any data. So I actually have to check first if the controls even exist. Then I have to find out if there is anything in the text field (or if the drop-down list has a selected
            // value) that is not null (or not 0), and then store it locally on this page.

            // I am sure that there is an eaiser way of doing this, but this works and I couldn't find anywhere on the internet that was having similar issues as I was. So allow me to explain it here.
                // 1) Find the control by name and implicitly cast it to it's type.
                // 2) If it doesn't exist, create a new empty version to avoid errors.
                // 3) If it does exist, find it's value.
                // 4) If the value is null, keep it null.
                // 5) If the value isn't null, store it.
            username = (eventView.FindControl("txtUsername") as TextBox ?? new TextBox()).Text.Equals(string.Empty) ? null : (eventView.FindControl("txtUsername") as TextBox).Text;
            guests = (eventView.FindControl("lstGuests") as DropDownList ?? new DropDownList()).SelectedValue.Equals(string.Empty) ? 0 : int.Parse((eventView.FindControl("lstGuests") as DropDownList).SelectedValue);
            eventView.DataSource = await eventsSingleton.Events;
            // If I were to bind the data first before getting the previous information, the value of that textbox would be null.
            eventView.DataBind();
            CheckAvailability();    // Changes need to occur to the data in certain circumstances.
        }

        /// <summary>
        /// This checks the event's available seats and adjusts controls accordingly.
        /// </summary>
        private void CheckAvailability()
        {
            // If the event is full, the user cannot register.
            if ((eventView.DataSource as DataTable).Rows[eventView.PageIndex]["txtMaximum"].ToString().Equals("Event full!"))
            {
                (eventView.FindControl("btnRegister") as Button).Enabled = false;
                (eventView.FindControl("btnRegister") as Button).CssClass = "registerDisabled";
                (eventView.FindControl("btnRegister") as Button).Text = "Event full!";
            }
            // If the event only has one spot, the user cannot bring guests, sadly.
            else if ((eventView.DataSource as DataTable).Rows[eventView.PageIndex]["txtMaximum"].ToString().Equals("Only one spot left!"))
            {
                (eventView.FindControl("lblGuestPrompt") as Label).Visible = false;
                (eventView.FindControl("lstGuests") as DropDownList).Visible = false;
            }
            // Otherwise, let's populate the drop-down list with some numbers!
            else
                foreach (string number in (eventView.DataSource as DataTable).Rows[eventView.PageIndex]["lstGuests"] as List<string>)
                    (eventView.FindControl("lstGuests") as DropDownList).Items.Add(number);
        }

        /// <summary>
        /// This is an object that will contain the string indicating success or failure from Google regarding the reCAPTCHA test.
        /// </summary>
        private sealed class GoogleObject
        {
            // This name must be lower-case to reflect the JSON response.
            public string success { get; set; }
        }
    }
}