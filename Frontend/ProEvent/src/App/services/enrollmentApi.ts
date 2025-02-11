import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { API_BASE_URL } from '../../config/api';
const baseQuery = fetchBaseQuery({
    baseUrl: API_BASE_URL,
    prepareHeaders: (headers, { getState }) => {
      // Можете добавлять заголовки здесь, если необходимо
      return headers;
    },
  });
  
export const enrollmentApi = createApi({
  reducerPath: 'enrollmentApi',
  baseQuery:baseQuery, // Замените на базовый URL вашего API
  endpoints: (builder) => ({
    createEnrollment: builder.mutation({ // Используем mutation для POST-запросов
      query: (enrollmentDTO) => ({
        url: '/enrollment', // Замените на фактический URL вашего API endpoint
        method: 'POST',
        body: enrollmentDTO,
      }),
    }),
    getParticipantsByEventId: builder.query({ // Используем mutation для POST-запросов
        query:(eventId) =>`/enrollment/GetByEventId/${eventId}`, 
        transformResponse: (response) => {
                  console.log("transformResponse", response);
                  return response; // Возвращаем объект EventDTO
                },// Замените на фактический URL вашего API endpoint
        }),
    deleteEnrollment: builder.mutation({
            query: (id) => ({
              url: `/enrollment/${id}`,
              method: 'DELETE',
            }),
          }),
      }),
  })


export const { useCreateEnrollmentMutation, useGetParticipantsByEventIdQuery, useDeleteEnrollmentMutation } = enrollmentApi;