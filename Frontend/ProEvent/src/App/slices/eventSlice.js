import { createSlice } from "@reduxjs/toolkit";
import { eventApi } from "../services/eventApi";
// interface EventState {
//     events: EventDTO[];
//     loading: boolean;
//     error: string | null;
//     totalCount: number;
//     pageNumber: number;
//     pageSize: number;
//   }
  
  const initialState = {
    events: [],
    loading: false,
    error: null,
    totalCount: 0,
    pageNumber: 1,
    pageSize: 4,
  };
  
  const eventSlice = createSlice({
    name: 'event',
    initialState,
    reducers: {
      // Ваши собственные редьюсеры
      setPageNumber: (state, action) => {
        state.pageNumber = action.payload;
      },
      setPageSize: (state, action) => {
        state.pageSize = action.payload;
      },
    },
    extraReducers: (builder) => {
      builder.addMatcher(
        eventApi.endpoints.getEventsWithPagination.matchFulfilled, // Используем getEventsWithPagination
        (state, action) => {
          state.events = action.payload.events; // Обновляем состояние events
          state.totalCount = action.payload.totalCount; // Обновляем totalCount
          state.pageNumber = action.payload.pageNumber; // Обновляем pageNumber
          state.pageSize = action.payload.pageSize; // Обновляем pageSize
          state.loading = false;
          state.error = null;
        }
      );
      builder.addMatcher(
        eventApi.endpoints.getEventsWithPagination.matchPending, // Используем getEventsWithPagination
        (state) => {
          state.loading = true;
          state.error = null;
        }
      );
      builder.addMatcher(
        eventApi.endpoints.getEventsWithPagination.matchRejected, // Используем getEventsWithPagination
        (state, action) => {
          state.loading = false;
          state.error = action.error.message || 'An error occurred'; // Устанавливаем сообщение об ошибке
        }
      );
    },
  });
  
  export const { setPageNumber, setPageSize } = eventSlice.actions;
  export default eventSlice.reducer;