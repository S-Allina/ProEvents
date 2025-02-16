import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { API_BASE_URL } from '../../config/api';

interface Participant {
  id: number;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  email: string;
  userId: string;
}

interface ApiResponse<T> {
  isSuccess: boolean;
  result: T;
  displayMessage: string | null;
  errorMessages: string[] | null;
}

export const participantApi = createApi({
  reducerPath: 'participantApi',
  baseQuery: fetchBaseQuery({
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
  }),
  endpoints: (builder) => ({
    getParticipants: builder.query<ApiResponse<Participant[]>, void>({
      query: () => `/participants`,
    }),
    getParticipantByUserId: builder.query<ApiResponse<Participant>, string>({
      query: (userId) => `/participants/GetByUserId/${userId}`,
    }),
    updateParticipant: builder.mutation<ApiResponse<Participant>, Participant>({
      query: (participant) => ({
        url: `/participants`,
        method: 'PUT',
        body: participant,
      }),
    }),
    deleteParticipant: builder.mutation<ApiResponse<boolean>, number>({
      query: (id) => ({
        url: `/participants/${id}`,
        method: 'DELETE',
      }),
    }),
  }),
});

export const {
  useGetParticipantsQuery,
  useGetParticipantByUserIdQuery,
  useUpdateParticipantMutation,
  useDeleteParticipantMutation,
} = participantApi;
