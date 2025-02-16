import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { API_BASE_URL } from '../../config/api';
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

export const enrollmentApi = createApi({
  reducerPath: 'enrollmentApi',
  baseQuery: baseQuery,
  endpoints: (builder) => ({
    createEnrollment: builder.mutation({
      query: (enrollmentDTO) => ({
        url: '/enrollments',
        method: 'POST',
        body: enrollmentDTO,
      }),
    }),
    getParticipantsByEventId: builder.query({
      query: (eventId) => `/enrollments?eventId=${eventId}`,
      transformResponse: (response) => {
        console.log('transformResponse', response);
        return response;
      },
    }),
    deleteEnrollment: builder.mutation({
      query: (id) => ({
        url: `/enrollments/${id}`,
        method: 'DELETE',
      }),
    }),
  }),
});

export const {
  useCreateEnrollmentMutation,
  useGetParticipantsByEventIdQuery,
  useDeleteEnrollmentMutation,
} = enrollmentApi;
