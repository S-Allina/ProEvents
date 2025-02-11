import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { API_BASE_URL } from '../../config/api';
import { EventDTO } from '../../Models/EventDTO';

interface ApiResponse<T> {
  isSuccess: boolean;
  result: T;
  displayMessage: string | null;
  errorMessages: string[] | null;
}
interface PaginationResponse<T> {
  events: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}


const baseQuery = fetchBaseQuery({
  baseUrl: API_BASE_URL,
  prepareHeaders: (headers, { getState }) => {
    // Можете добавлять заголовки здесь, если необходимо
    return headers;
  },
});

export const eventApi = createApi({
  reducerPath: 'eventApi',
  baseQuery: baseQuery,
  endpoints: (builder) => ({
    getEventsWithPagination: builder.query<PaginationResponse<EventDTO>, { pageNumber: number; pageSize: number }>({
      query: ({ pageNumber, pageSize }) => `/events?pageNumber=${pageNumber}&pageSize=${pageSize}`, // Replace with your actual API endpoint
    }),

    getFilteredEvents: builder.query<ApiResponse<EventDTO[]>, any>({
      query: (filters) => ({
        url: '/events/filtered',
        params: filters, // Передаем объект фильтров
      }),
    }),
    getEventById: builder.query<EventDTO, number>({
        query: (id) => `/events/${id}`,
        transformResponse: (response: ApiResponse<EventDTO>) => {
          console.log("transformResponse", response);
          return response.result; // Возвращаем объект EventDTO
        },
      }),
      getEventByUserId: builder.query<ApiResponse<EventDTO[]>, number>({
        query: (id) => `/events/GetEventsByUser/${id}`,
    
      }),
      getEventByName: builder.query<ApiResponse<EventDTO[]>, string>({
        query: (name) => ({ // Принимаем имя в качестве аргумента
          url: 'events/search', // Базовый URL для поиска
          params: { name: name }, // Передаем имя как параметр запроса
        }),
      }),
      createEvent: builder.mutation<ApiResponse<EventDTO>, EventDTO>({
        query: (event) => ({
            url: '/events',
            method: 'POST',
            body: event,
            prepareHeaders: (headers) => { // Add this
                headers.set('Content-Type', 'application/json');
                return headers;
            },
        }),
    }),
    updateEvent: builder.mutation<ApiResponse<EventDTO>, EventDTO>({
        query: (event) => ({
            url: `/events`,
            method: 'PUT',
            body: event,
            prepareHeaders: (headers) => { // Add this
                headers.set('Content-Type', 'application/json');
                return headers;
            },
        }),
    }),
    deleteEvent: builder.mutation<ApiResponse<boolean>, number>({
      query: (id) => ({
        url: `/events/${id}`,
        method: 'DELETE',
      }),
    }),
  }),
});

export const {
  useGetEventByNameQuery,
  useGetEventsWithPaginationQuery,
  useGetFilteredEventsQuery,
  useGetEventByUserIdQuery,
  useGetEventByIdQuery,
  useCreateEventMutation,
  useUpdateEventMutation,
  useDeleteEventMutation,
} = eventApi;