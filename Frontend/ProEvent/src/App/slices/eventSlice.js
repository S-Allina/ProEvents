import { createSlice } from '@reduxjs/toolkit';
import { eventApi } from '../services/eventApi';

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
    setPageNumber: (state, action) => {
      state.pageNumber = action.payload;
    },
    setPageSize: (state, action) => {
      state.pageSize = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder.addMatcher(
      eventApi.endpoints.getEventsWithPagination.matchFulfilled,
      (state, action) => {
        state.events = action.payload.events;
        state.totalCount = action.payload.totalCount;
        state.pageNumber = action.payload.pageNumber;
        state.pageSize = action.payload.pageSize;
        state.loading = false;
        state.error = null;
      },
    );
    builder.addMatcher(eventApi.endpoints.getEventsWithPagination.matchPending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addMatcher(
      eventApi.endpoints.getEventsWithPagination.matchRejected,
      (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'An error occurred';
      },
    );
  },
});

export const { setPageNumber, setPageSize } = eventSlice.actions;
export default eventSlice.reducer;
