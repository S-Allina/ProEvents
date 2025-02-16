import { configureStore } from '@reduxjs/toolkit';
import eventReducer from './slices/eventSlice';
import authReducer from './slices/authSlice';

import { eventApi } from './services/eventApi';
import { authApi } from './services/authApi.ts';
import { enrollmentApi } from './services/enrollmentApi.ts';
import { participantApi } from './services/participantApi.ts';

export const store = configureStore({
  reducer: {
    [eventApi.reducerPath]: eventApi.reducer,
    [authApi.reducerPath]: authApi.reducer,
    [enrollmentApi.reducerPath]: enrollmentApi.reducer,
    [participantApi.reducerPath]: participantApi.reducer,
    event: eventReducer,
    auth: authReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .concat(eventApi.middleware)
      .concat(authApi.middleware)
      .concat(enrollmentApi.middleware)
      .concat(participantApi.middleware),
});
