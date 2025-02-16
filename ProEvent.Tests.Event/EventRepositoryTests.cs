//using AutoMapper;
//using FluentValidation;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Memory;
//using ProEvent.Services.Core.DTOs;
//using ProEvent.Services.Core.Models;
//using ProEvent.Services.Infrastructure.Data;
//using ProEvent.Services.Infrastructure.Repository;
//using static System.Net.Mime.MediaTypeNames;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//public class EventRepositoryTests
//{
//    private IMapper _mapper;
//    private IMemoryCache _cache;
//    private IValidator<Event> _validator;

//    public EventRepositoryTests()
//    {
//        // Настройка AutoMapper
//        var config = new MapperConfiguration(cfg =>
//        {
//            cfg.CreateMap<Event, EventDTO>().ReverseMap();
//        });
//        _mapper = config.CreateMapper();

//        // Настройка кэша
//        _cache = new MemoryCache(new MemoryCacheOptions());
//    }

//    private ApplicationDbContext CreateContext(string dbName)
//    {
//        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//            .UseInMemoryDatabase(databaseName: dbName)
//            .Options;

//        return new ApplicationDbContext(options);
//    }


//    [Fact]
//    public async Task CreateUpdateEvent_ShouldUpdateEvent_WhenEventExists()
//    {
//        // Arrange
//        var existingEvent = new Event
//        {
//            Name = "New Event",
//            Category = "Some Category",
//            Description = "Event Description",
//            Location = "Event Location",
//            Date = DateTime.Parse("2025-02-09"),
//            MaxParticipants = 10
//        };

//        using (var context = CreateContext("CreateUpdateEvent_ShouldUpdateEvent_WhenEventExists"))
//        {
//            context.Events.Add(existingEvent);
//            await context.SaveChangesAsync();
//        }

//        var eventDTO = new EventDTO
//        {
//            Id = existingEvent.Id, // используем существующий Id
//            Name = "Updated Event",
//            Category = "Some Category",
//            Description = "Updated Event Description",
//            Location = "Updated Event Location",
//            Date = DateTime.Parse("2025-02-09"),
//            MaxParticipants = 10
//        };

//        using (var context = CreateContext("CreateUpdateEvent_ShouldUpdateEvent_WhenEventExists"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            // Act
//            var result = await repository.CreateUpdateEvent(eventDTO);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Updated Event", result.Name);
//            Assert.Equal(1, await context.Events.CountAsync()); // Ожидаем, что количество событий останется 1
//        }
//    }

//    [Fact]
//    public async Task DeleteEvent_ShouldReturnTrue_WhenEventExists()
//    {
//        // Arrange
//        var existingEvent = new Event
//        {
//            Name = "delet Event",
//            Category = "Some Category",
//            Description = "Updated Event Description",
//            Location = "Updated Event Location",
//            Date = DateTime.Parse("2025-02-09"),
//            MaxParticipants = 10
//        };
//        using (var context = CreateContext("DeleteEvent_ShouldReturnTrue_WhenEventExists"))
//        {
//            context.Events.Add(existingEvent);
//            await context.SaveChangesAsync();
//        }

//        using (var context = CreateContext("DeleteEvent_ShouldReturnTrue_WhenEventExists"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            // Act
//            var result = await repository.DeleteEvent(existingEvent.Id);

//            // Assert
//            Assert.True(result);
//            Assert.Equal(0, await context.Events.CountAsync());
//        }
//    }

//    [Fact]
//    public async Task DeleteEvent_ShouldReturnFalse_WhenEventDoesNotExist()
//    {
//        // Arrange
//        using (var context = CreateContext("DeleteEvent_ShouldReturnFalse_WhenEventDoesNotExist"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            // Act
//            var result = await repository.DeleteEvent(999); // ID, который не существует

//            // Assert
//            Assert.False(result);
//        }
//    }
//    [Fact]
//    public async Task GetEventById_ShouldReturnEvent_WhenEventExists()
//    {
//        // Arrange
//        var existingEvent = new Event
//        {
//            Id = 3,
//            Name = "Event to Get",
//            Category = "Some Category",
//            Description = "Event Description",
//            Location = "Event Location",
//            Date = DateTime.Parse("2025-02-09"),
//            MaxParticipants = 10
//        };
//        using (var context = CreateContext("GetEventById_ShouldReturnEvent_WhenEventExists"))
//        {
//            context.Events.Add(existingEvent);
//            await context.SaveChangesAsync();
//        }

//        using (var context = CreateContext("GetEventById_ShouldReturnEvent_WhenEventExists"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            // Act
//            var result = await repository.GetEventById(3);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Event to Get", result.Name);
//        }
//    }
//    [Fact]
//    public async Task GetFilteredEvents_ShouldReturnFilteredEvents_WhenFiltersAreApplied()
//    {
//        // Arrange
//        var event1 = new Event
//        {
//            Name = "Event 1",
//            Date = DateTime.Parse("2025-02-09"),
//            Location = "Location A",
//            Category = "Category A",
//            Description = "Event A",
//            MaxParticipants = 10
//        };

//        var event2 = new Event
//        {
//            Name = "Event 2",
//            Date = DateTime.Parse("2025-03-10"),
//            Location = "Location B",
//            Category = "Category B",
//            Description = "Event B",
//            MaxParticipants = 20
//        };

//        var event3 = new Event
//        {
//            Name = "Event 3",
//            Date = DateTime.Parse("2025-04-15"),
//            Location = "Location A",
//            Category = "Category A",
//            Description = "Event A",
//            MaxParticipants = 10
//        };


//        // Создаем контекст и добавляем события
//        using (var context = CreateContext("GetFilteredEvents_ShouldReturnFilteredEvents_WhenFiltersAreApplied"))
//        {
//            context.Events.AddRange(event1, event2, event3);
//            await context.SaveChangesAsync();
//        }

//        // Теперь вызываем метод GetFilteredEvents
//        using (var context = CreateContext("GetFilteredEvents_ShouldReturnFilteredEvents_WhenFiltersAreApplied"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            // Act
//            var result = await repository.GetEvents(1, 4, DateTime.Parse("2025-02-01"), DateTime.Parse("2025-03-31"), "Location A", "Category A");

//            // Assert
//            var eventList = result.Events.ToList();
//            Assert.Single(eventList); // Ожидаем одно событие
//            Assert.Equal("Event 1", eventList[0].Name); // Проверяем, что это именно Event 1
//        }
//    }
//    [Fact]
//    public async Task GetEvents_ShouldReturnPagedEvents_WhenCalled()
//    {
//        // Arrange
//        for (int i = 1; i <= 10; i++)
//        {
//            using (var context = CreateContext("GetEvents_ShouldReturnPagedEvents_WhenCalled"))
//            {
//                var eventItem = new Event
//                {
//                    Name = $"Event {i}",
//                    Date = DateTime.Now.AddDays(i),
//                    Location = $"Location {i}",
//                    Category = $"Category {i}",
//                    Description = $"Event {i}",
//                    MaxParticipants = 10
//                };
//                context.Events.Add(eventItem);
//                await context.SaveChangesAsync();
//            }
//        }

//        using (var context = CreateContext("GetEvents_ShouldReturnPagedEvents_WhenCalled"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            // Act
//            var (events, totalCount) = await repository.GetEvents(pageNumber: 2, pageSize: 5);

//            // Assert
//            Assert.Equal(5, events.Count());
//            Assert.Equal(10, totalCount);
//        }
//    }

//    [Fact]
//    public async Task GetEventsByUser_ShouldReturnRegisteredEvents_WhenUser_HasRegistrations()
//    {
//        // Arrange
//        var userId = "user123";
//        var participant = new Participant { Id = 1, UserId = userId, DateOfBirth = DateTime.Parse("2022-05-05"), Email = "email", FirstName = "firstname", LastName = "lastname" };
//        var event1 = new Event { Id = 1, Name = "Event 1", Description = "Description 1", Date = DateTime.Now.AddDays(1), Location = "Location 1", Category = "Category 1", MaxParticipants = 100 };
//        var event2 = new Event { Id = 2, Name = "Event 2", Description = "Description 2", Date = DateTime.Now.AddDays(2), Location = "Location 2", Category = "Category 2", MaxParticipants = 50 };

//        var enrollment1 = new Enrollment { Id = 1, EventId = event1.Id, ParticipantId = participant.Id, RegistrationDate = DateTime.Now };
//        var enrollment2 = new Enrollment { Id = 2, EventId = event2.Id, ParticipantId = participant.Id, RegistrationDate = DateTime.Now };

//        using (var context = CreateContext("GetEventsByUser _ShouldReturnRegisteredEvents_WhenUser HasRegistrations"))
//        {
//            context.Participants.Add(participant);
//            context.Events.AddRange(event1, event2);
//            context.Enrollments.AddRange(enrollment1, enrollment2);
//            await context.SaveChangesAsync();
//        }

//        // Act
//        using (var context = CreateContext("GetEventsByUser _ShouldReturnRegisteredEvents_WhenUser HasRegistrations"))
//        {
//            var repository = new EventRepository(context, _mapper, _validator, _cache);

//            var result = await repository.GetEventsByUser(userId);

//            // Assert
//            var eventList = result.ToList();
//            Assert.Equal(2, eventList.Count); // Ожидаем два события
//            Assert.Contains(eventList, e => e.Name == "Event 1");
//            Assert.Contains(eventList, e => e.Name == "Event 2");
//        }
//    }
//}
