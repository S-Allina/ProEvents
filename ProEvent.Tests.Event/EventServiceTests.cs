using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using ProEvent.DAL.Interfaces.IRepository;
using ProEvent.Domain.Models;
using ProEvent.BLL.Services;
using ProEvent.BLL.DTOs;
using ProEvent.Domain.Enums;
using ProEvent.BLL.Interfaces.IService;

namespace ProEvent.Tests.Events
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<Event>> _eventValidatorMock;
        private readonly EventService _eventService;
        private readonly IEnrollmentService _enrollmentServiceMock;
        public EventServiceTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _cacheMock = new Mock<IMemoryCache>();
            _mapperMock = new Mock<IMapper>();
            _eventValidatorMock = new Mock<IValidator<Event>>();
            _eventService = new EventService(_eventRepositoryMock.Object,  _cacheMock.Object, _mapperMock.Object, _eventValidatorMock.Object, new Mock<IEnrollmentService>().Object);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnEventDTO_WhenEventExists()
        {
            int eventId = 1;
            var eventItem = new Event
            {
                Id = eventId,
                Name = "Test Event",
                Description = "This is a test event.",
                Image = null,
                Date = DateTime.Now,
                Location = "Test Location",
                Category = "Test Category",
                MaxParticipants = 100
            };

            var eventDto = new EventDTO(
            eventItem.Name,
            eventItem.Description,
            eventItem.Image,
            eventItem.Date,
            eventItem.Location,
            eventItem.Category,
            eventItem.MaxParticipants
            );

            var eventStatus = EventStatus.Relevant;

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventItem);
            _mapperMock.Setup(m => m.Map<EventDTO>(eventItem))
            .Returns(eventDto);

            var result = await _eventService.GetEventById(eventId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(eventDto.Name, result.Name);
            _eventRepositoryMock.Verify(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<EventDTO>(eventItem), Times.Once);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnNull_WhenEventDoesNotExist()
        {
            int eventId = 1;

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null);

            var result = await _eventService.GetEventById(eventId, CancellationToken.None);

            Assert.Null(result);
            _eventRepositoryMock.Verify(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<EventDTO>(It.IsAny<Event>()), Times.Never);
        }

      

       
        [Fact]
        public async void GetEvents_ShouldThrowOperationCanceledException_WhenCanceled()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => _eventService.GetEvents(1, 4, cancellationToken: cancellationTokenSource.Token));
            Assert.NotNull(exception);
        }


        private bool AreByteArraysEqual(byte[]? array1, byte[]? array2)
        {
            if (array1 == null && array2 == null)
            {
                return true;
            }
            if (array1 == null || array2 == null)
            {
                return false;
            }
            return array1.SequenceEqual(array2);
        }

        [Fact]
        public async Task GetEventById_EventExists_ReturnsEventDto()
        {
            int eventId = 1;
            var eventFromRepository = new Event
            {
                Id = eventId,
                Name = "Test Event",
                Description = "Test Description",
                Date = DateTime.Now.AddDays(1),
                Location = "Test Location",
                Category = "Test Category",
                MaxParticipants = 100,
                Image = null
            };
            var eventStatus = EventStatus.Relevant;

            var expectedEventDto = new EventDTO("Test Event", "Test Description", null, DateTime.Now.AddDays(1), "Test Location", "Test Category", 100)
            {
                Id = eventId,
                Status = eventStatus
            };

            _eventRepositoryMock.Setup(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventFromRepository);


            _mapperMock.Setup(mapper => mapper.Map<EventDTO>(eventFromRepository))
                       .Returns(expectedEventDto);

            var result = await _eventService.GetEventById(eventId);

            Assert.NotNull(result);
            Assert.Equal(expectedEventDto.Id, result.Id);
            Assert.Equal(expectedEventDto.Name, result.Name);
            Assert.Equal(expectedEventDto.Description, result.Description);
            Assert.Equal(expectedEventDto.Date, result.Date);
            Assert.Equal(expectedEventDto.Location, result.Location);
            Assert.Equal(expectedEventDto.Category, result.Category);
            Assert.Equal(expectedEventDto.MaxParticipants, result.MaxParticipants);
            Assert.True(AreByteArraysEqual(expectedEventDto.Image, result.Image), "Images are not equal.");
            Assert.Equal(expectedEventDto.Status, result.Status);

            _eventRepositoryMock.Verify(repo => repo.GetEventById(eventId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<EventDTO>(eventFromRepository), Times.Once);

        }
        [Fact]
        public async Task DeleteEvent_ShouldReturnTrue_WhenEventDeletedSuccessfully()
        {
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.DeleteEvent(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

            var result = await _eventService.DeleteEvent(eventId, CancellationToken.None);

            Assert.True(result);
            _eventRepositoryMock.Verify(repo => repo.DeleteEvent(eventId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_ShouldReturnFalse_WhenEventNotFound()
        {
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.DeleteEvent(eventId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

            var result = await _eventService.DeleteEvent(eventId, CancellationToken.None);

            Assert.False(result);
            _eventRepositoryMock.Verify(repo => repo.DeleteEvent(eventId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_ShouldThrowOperationCanceledException_WhenCanceled()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => _eventService.DeleteEvent(1, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CreateUpdateEvent_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            var eventDTO = new EventDTO("", "Description", null, DateTime.Now.AddDays(1), "Location", "Category", 100);
            var eventEntity = new Event("", "Description", DateTime.Now.AddDays(1), "Location", "Category", 100);

            _mapperMock.Setup(m => m.Map<Event>(eventDTO)).Returns(eventEntity);
            var validationErrors = new List<ValidationFailure>
                {
                    new ValidationFailure("Name", "Название события обязательно.")
                };
            _eventValidatorMock.Setup(v => v.ValidateAsync(eventEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

            var exception = await Assert.ThrowsAsync<ProEvent.Domain.Exceptions.ValidationException>(() =>
            _eventService.CreateUpdateEvent(eventDTO, CancellationToken.None));

            Assert.NotNull(exception);
            Assert.Single(exception.Errors);
            Assert.Equal("Название события обязательно.", exception.Errors.First());
        }

        [Fact]
        public async Task CreateUpdateEvent_ShouldThrowValidationException_WhenDateIsInThePast()
        {
            var eventDTO = new EventDTO("New Event", "Description", null, DateTime.Now.AddDays(-1), "Location", "Category", 100);
            var eventEntity = new Event("New Event", "Description", DateTime.Now.AddDays(-1), "Location", "Category", 100);

            _mapperMock.Setup(m => m.Map<Event>(eventDTO)).Returns(eventEntity);
            var validationErrors = new List<ValidationFailure>
                {
                    new ValidationFailure("Date", "Дата события не может быть в прошлом.")
                };
            _eventValidatorMock.Setup(v => v.ValidateAsync(eventEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

            var exception = await Assert.ThrowsAsync<ProEvent.Domain.Exceptions.ValidationException>(() =>
            _eventService.CreateUpdateEvent(eventDTO, CancellationToken.None));

            Assert.NotNull(exception);
            Assert.Single(exception.Errors);
            Assert.Equal("Дата события не может быть в прошлом.", exception.Errors.First());
        }
    }
}