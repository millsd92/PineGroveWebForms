using System.Threading;
using System.Threading.Tasks;
using Refit;
using PineGroveWebForms.Models;

namespace PineGroveWebForms.Services
{
    public class RestClient
    {
        private readonly IRestable _client;

        CancellationToken token;

        public RestClient()
        {
            RefitSettings settings = new RefitSettings() { ContentSerializer = new JsonContentSerializer() };
            _client = RestService.For<IRestable>("https://36vhnw37q6.execute-api.us-east-2.amazonaws.com/Prod", settings);
            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(15000);
            token = source.Token;
        }

        public async Task<User[]> GetUsers()
        {
            return await _client.GetUsers();
        }

        public async Task<User> GetUser(string username)
        {
            return await _client.GetUser(username, token);
        }

        public async Task<User[]> GetUsersByName(string firstName, string lastName)
        {
            return await _client.GetUsersByName(firstName, lastName, token);
        }

        public async Task<AnnouncementRequest> CreateAnnouncement([Body] AnnouncementRequest announcement)
        {
            return await _client.CreateAnnouncement(announcement, token);
        }

        public async Task<Event[]> GetEvents()
        {
            return await _client.GetEvents();
        }

        public async Task<EventRegistration> CreateRegistration([Body] EventRegistration registration)
        {
            return await _client.CreateRegistration(registration, token);
        }

        public async Task<Event> UpdateEvent(int eventId, [Body] Event @event)
        {
            return await _client.UpdateEvent(eventId, @event, token);
        }

        public async Task<User> CreateUser([Body] User user)
        {
            return await _client.CreateUser(user, token);
        }

        public async Task<User> UpdateUser(int userId, [Body] User user)
        {
            return await _client.UpdateUser(userId, user, token);
        }

        public async Task<PrayerRequest> CreatePrayerRequest([Body] PrayerRequest prayerRequest)
        {
            return await _client.CreatePrayerRequest(prayerRequest, token);
        }

        public async Task<VisitRequest> CreateVisitRequest([Body] VisitRequest visitRequest)
        {
            return await _client.CreateVisitRequest(visitRequest, token);
        }
    }
}