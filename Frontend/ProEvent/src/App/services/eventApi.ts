import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { API_BASE_URL } from '../../config/api';
import { EventDTO } from '../../Models/EventDTO';

interface ApiResponse<T> {
  isSuccess: boolean;
  result: T;
  displayMessage: string | null;
  errorMessages: string[] | null;
}

interface PaginationResponse<EventDTO> {
  events: EventDTO[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

interface EventFilterParams {
  pageNumber?: number;
  pageSize?: number;
  startDate?: string;
  endDate?: string;
  location?: string;
  category?: string;
  name?: string;
  isPassed: boolean;
}

const baseQuery = fetchBaseQuery({
  baseUrl: API_BASE_URL,
  prepareHeaders: (headers) => {
    headers.set('Content-Type', 'application/json');

    const token = localStorage.getItem('token');
    console.log(token);
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

export const eventApi = createApi({
  reducerPath: 'eventApi',
  baseQuery: baseQuery,
  endpoints: (builder) => ({
    getEventsWithPagination: builder.query({
      query: (params) => {
        return {
          url: '/events',
          params: params,
        };
      },
      transformResponse: (response: any): PaginationResponse<EventDTO> => {
        return {
          events: response.events,
          totalCount: response.totalCount,
          pageNumber: response.pageNumber,
          pageSize: response.pageSize,
        };
      },
    }),
    getEventById: builder.query<EventDTO, number>({
      query: (id) => `/events/${id}`,
      transformResponse: (response: any) => {
        return response;
      },
    }),
    getEventByUserId: builder.query<ApiResponse<EventDTO[]>, number>({
      query: (id) => `/events/getByUserId/${id}`,
    }),
    createEvent: builder.mutation<ApiResponse<EventDTO>, EventDTO>({
      query: (event) => ({
        url: '/events',
        method: 'POST',
        body: event,
        prepareHeaders: (headers) => {
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
        prepareHeaders: (headers) => {
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
  useGetEventsWithPaginationQuery,
  useGetEventByUserIdQuery,
  useGetEventByIdQuery,
  useCreateEventMutation,
  useUpdateEventMutation,
  useDeleteEventMutation,
} = eventApi;
