using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PineGroveWebForms.Services
{
    /// <summary>
    /// This is a singleton, also known as an object that only allows one single instance of itself to exist, and contains all the event information.
    /// </summary>
    public sealed class EventsSingleton
    {
        // Class variables.
        private static EventsSingleton instance = null;
        private static RestClient client;
        private static DataTable events;
        private static List<Models.Event> currentEvents;

        /// <summary>
        /// This singleton provides access to all of the events and limits the number of calls to the API.
        /// </summary>
        public EventsSingleton()
        { Events = SetUpEvents(); }

        /// <summary>
        /// If there is no instance of this object, create it. There is no set method for the instance.
        /// </summary>
        public static EventsSingleton Instance
        {
            get
            {
                if (instance is null)
                    instance = new EventsSingleton();
                return instance;
            }
        }

        /// <summary>
        /// This is the data table of all of the events.
        /// </summary>
        public Task<DataTable> Events { get; private set; } = null;

        /// <summary>
        /// This is called to set up all of the events by calling the API.
        /// </summary>
        /// <returns>An awaitable task who's result is a data table containing all of the event data necessary to populate the events web form.</returns>
        private async Task<DataTable> SetUpEvents()
        {
            client = new RestClient();
            events = new DataTable("Events");
            events.Columns.Add("imgPicture", typeof(string));
            events.Columns.Add("txtTitle", typeof(string));
            events.Columns.Add("txtDescription", typeof(string));
            events.Columns.Add("txtStartTime", typeof(string));
            events.Columns.Add("txtEndTime", typeof(string));
            events.Columns.Add("txtAddress", typeof(string));
            events.Columns.Add("txtMaximum", typeof(string));
            events.Columns.Add("lstGuests", typeof(List<string>));
            events.Columns.Add("eventId", typeof(int));
            events.Columns.Add("event", typeof(Models.Event));

            DataRow row;
            currentEvents = (await client.GetEvents()).ToList();
            currentEvents.RemoveAll(e => e.EndTime < DateTime.Now);
            foreach (Models.Event @event in currentEvents)
            {
                row = events.NewRow();
                if (@event.Picture is null)
                    row["imgPicture"] = "~/Images/icon.png";
                else
                    row["imgPicture"] = "data:Image/jpeg;base64," + Convert.ToBase64String(@event.Picture);
                row["txtTitle"] = @event.EventTitle;
                row["txtDescription"] = @event.EventDescription;
                row["txtStartTime"] = string.Format("{0:g}", @event.StartTime);
                if (@event.EndTime is null)
                    row["txtEndTime"] = "Whenever it Ends!";
                else
                    row["txtEndTime"] = string.Format("{0:g}", @event.EndTime);
                row["txtAddress"] = @event.Address.Replace("\n", "<br /><br />");
                if ((@event.MaxAttendees ?? int.MaxValue) - @event.CurrentAttendees < 11 && (@event.MaxAttendees ?? int.MaxValue) - @event.CurrentAttendees > 1)
                {
                    row["txtMaximum"] = "Maximum allowed guests: " + (@event.MaxAttendees - @event.CurrentAttendees - 1);
                    row["lstGuests"] = new List<string>();
                    for (int i = 0; i < (@event.MaxAttendees - @event.CurrentAttendees); i++)
                        (row["lstGuests"] as List<string>).Add(i.ToString());
                }
                else if (@event.MaxAttendees - @event.CurrentAttendees == 1)
                {
                    row["txtMaximum"] = "Only one spot left!";
                    row["lstGuests"] = null;
                }
                else if ((@event.MaxAttendees ?? int.MaxValue) - @event.CurrentAttendees >= 11)
                {
                    row["txtMaximum"] = "Maximum allowed guests: 10";
                    row["lstGuests"] = new List<string>();
                    for (int i = 0; i <= 10; i++)
                        (row["lstGuests"] as List<string>).Add(i.ToString());
                }
                else
                {
                    row["txtMaximum"] = "Event full!";
                    row["lstGuests"] = null;
                }
                row["eventId"] = @event.EventId;
                row["event"] = @event;
                events.Rows.Add(row);
            }
            return events;
        }

        /// <summary>
        /// This is called to re-call the API after altering the database.
        /// </summary>
        public void Restart()
        {
            instance = null;
        }
    }
}