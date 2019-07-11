﻿using Refit;
using System.Threading.Tasks;
using System.Threading;
using PineGroveWebForms.Models;

namespace PineGroveWebForms.Services
{
    public interface IRestable
    {
        [Get("/api/users")]
        Task<User[]> GetUsers();

        [Get("/api/users/{username}")]
        Task<User> GetUser(string username, CancellationToken token);

        [Get("/api/users/GetNames?firstName={firstName}&lastName={lastName}")]
        Task<User[]> GetUsersByName(string firstName, string lastName, CancellationToken token);

        [Post("/api/announcementrequests")]
        Task<AnnouncementRequest> CreateAnnouncement([Body] AnnouncementRequest announcement, CancellationToken token);

        [Get("/api/Events")]
        Task<Event[]> GetEvents();

        [Post("/api/eventregistrations")]
        Task<EventRegistration> CreateRegistration([Body] EventRegistration registration, CancellationToken token);

        [Put("/api/Events/{eventId}")]
        Task<Event> UpdateEvent(int eventId, [Body] Event @event, CancellationToken token);

        [Post("/api/users")]
        Task<User> CreateUser([Body] User user, CancellationToken token);

        [Put("/api/users/{userId}")]
        Task<User> UpdateUser(int userId, [Body] User user, CancellationToken token);

        [Post("/api/prayerrequests")]
        Task<PrayerRequest> CreatePrayerRequest([Body] PrayerRequest prayerRequest, CancellationToken token);

        [Post("/api/visitrequests")]
        Task<VisitRequest> CreateVisitRequest([Body] VisitRequest visitRequest, CancellationToken token);
    }
}
