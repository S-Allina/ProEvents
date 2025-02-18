import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

const initialState = {
  isAuthenticated: localStorage.getItem('token') ? true : false,
  user: {
    userId: localStorage.getItem('userId') || null,
    userName: localStorage.getItem('userName') || null,
    email: localStorage.getItem('userEmail') || null,
    role: localStorage.getItem('userRole') || null,
  },
  token: localStorage.getItem('token') || null,
  isLoading: false,
  error: null,
};

export const checkAuth = createAsyncThunk('auth/checkAuth', async () => {
  const token = localStorage.getItem('token');
  if (token) {
    return { isAuthenticated: true };
  } else {
    return { isAuthenticated: false };
  }
});

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    login: (state, action) => {
      state.isAuthenticated = true;
      state.user = {
        userId: action.payload.userId,
        userName: action.payload.userName,
        email: action.payload.email,
        role: action.payload.role,
      };
      console.log('action ' + action);
      state.token = action.payload.token;
      localStorage.setItem('token', action.payload.token);
      localStorage.setItem('userId', action.payload.userId);
      localStorage.setItem('userName', action.payload.userName);
      localStorage.setItem('userEmail', action.payload.email);
      localStorage.setItem('userRole', action.payload.role);
    },
    logout: (state) => {
      state.isAuthenticated = false;
      state.user = null;
      state.token = null;
      localStorage.removeItem('token');
      localStorage.removeItem('userId');
      localStorage.removeItem('userName');
      localStorage.removeItem('userEmail');
      localStorage.removeItem('userRole');
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(checkAuth.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(checkAuth.fulfilled, (state, action) => {
        state.isLoading = false;
        state.isAuthenticated = action.payload.isAuthenticated;
      })
      .addCase(checkAuth.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message;
      });
  },
});

export const { login, logout } = authSlice.actions;
export default authSlice.reducer;
