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
    public partial class EventsWebForm : Page
    {
        private EventsSingleton eventsSingleton;
        private string username;
        private int guests;
        protected void Page_Load(object sender, EventArgs e)
        {
            eventsSingleton = EventsSingleton.Instance;
            LoadData();
        }

        protected void EventView_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            eventView.PageIndex = e.NewPageIndex;
            LoadData();
        }

        protected async void BtnRegister_Click(object sender, EventArgs e)
        {
            if (CaptchaPass())
            {
                try
                {
                    RestClient client = new RestClient();
                    Models.User user = await client.GetUser(username);
                    Models.EventRegistration eventRegistration = new Models.EventRegistration()
                    {
                        EventId = (int)(eventView.DataSource as DataTable).Rows[eventView.PageIndex]["eventId"],
                        Guests = guests,
                        UserId = user.UserId
                    };
                    Models.Event @event = (eventView.DataSource as DataTable).Rows[eventView.PageIndex]["event"] as Models.Event;
                    if (@event.CurrentAttendees >= @event.MaxAttendees)
                        throw new InvalidOperationException("Maximum reached!");
                    await client.CreateRegistration(eventRegistration);
                    if (!(eventView.FindControl("chkVolunteering") as CheckBox).Checked)
                    {
                        @event.CurrentAttendees += eventRegistration.Guests + 1;
                        await client.UpdateEvent(@event.EventId, @event);
                    }
                    eventView.Page.ClientScript.RegisterStartupScript(GetType(), "Success", "alert('Registration successful!');", true);
                    eventsSingleton.Restart();
                    eventsSingleton = EventsSingleton.Instance;
                    LoadData();
                }
                catch (Refit.ApiException)
                {
                    eventView.Page.ClientScript.RegisterStartupScript(GetType(), "NoUsernameFound", "alert('Error - No user exists with that username. Try again or download the Pine Grove App to become a member!');", true);
                }
                catch (InvalidOperationException)
                {
                    eventView.Page.ClientScript.RegisterStartupScript(GetType(), "EventFull", "alert('Error - The event became full prior to form submission!');", true);
                }
            }
            else
                eventView.Page.ClientScript.RegisterStartupScript(GetType(), "Failure", "alert('Error - Failed reCAPTCHA Response.');", true);
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

        private async void LoadData()
        {
            eventView.Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            username = (eventView.FindControl("txtUsername") as TextBox ?? new TextBox()).Text.Equals(string.Empty) ? null : (eventView.FindControl("txtUsername") as TextBox).Text;
            guests = (eventView.FindControl("lstGuests") as DropDownList ?? new DropDownList()).SelectedValue.Equals(string.Empty) ? 0 : int.Parse((eventView.FindControl("lstGuests") as DropDownList).SelectedValue);
            eventView.DataSource = await eventsSingleton.Events;
            eventView.DataBind();
            CheckAvailability();
        }

        private void CheckAvailability()
        {
            if ((eventView.DataSource as DataTable).Rows[eventView.PageIndex]["txtMaximum"].ToString().Equals("Event full!"))
            {
                (eventView.FindControl("btnRegister") as Button).Enabled = false;
                (eventView.FindControl("btnRegister") as Button).CssClass = "registerDisabled";
                (eventView.FindControl("btnRegister") as Button).Text = "Event full!";
            }
            else if ((eventView.DataSource as DataTable).Rows[eventView.PageIndex]["txtMaximum"].ToString().Equals("Only one spot left!"))
            {
                (eventView.FindControl("lblGuestPrompt") as Label).Visible = false;
                (eventView.FindControl("lstGuests") as DropDownList).Visible = false;
            }
            else
                foreach (string number in (eventView.DataSource as DataTable).Rows[eventView.PageIndex]["lstGuests"] as List<string>)
                    (eventView.FindControl("lstGuests") as DropDownList).Items.Add(number);
        }
        private sealed class GoogleObject
        {
            public string success { get; set; }
        }
    }
}