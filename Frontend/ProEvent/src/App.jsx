import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Home } from './features/home/pages/Home';
import EventList from './features/events/pages/EventList';
import Layout from './Layout/Layout';
import { Provider } from 'react-redux';
import { store } from './App/store';
import EventAdd from './features/events/pages/EventAdd';
import EventEdit from './features/events/pages/EventEdit';
import EventByUser from './features/events/pages/EventByUser';
import { Login } from './features/auth/pages/Login';
import EventFull from './features/events/pages/EventFull';
import PrivateRoute from './routers/PrivateRoute';
import UserList from './features/users/UserList';
import { Register } from './features/auth/pages/Register';
import AdminRoute from './routers/AdminRoute';
import Profile from './features/profile/pages/Profile';
import { Error500 } from './features/Error/Error500';

function App() {
  return (
    <Provider store={store}>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="*" element={<Error500 />} />
          <Route element={<PrivateRoute />}>
            <Route
              path="/"
              element={
                <Layout>
                  <Home />
                </Layout>
              }
            />
            <Route
              path="/events"
              element={
                <Layout>
                  <EventList />
                </Layout>
              }
            />

            <Route
              path="/events/full/:id"
              element={
                <Layout>
                  <EventFull />
                </Layout>
              }
            />
            <Route
              path="/events/my-calendar"
              element={
                <Layout>
                  <EventByUser />
                </Layout>
              }
            />
            <Route
              path="/profile"
              element={
                <Layout>
                  <Profile />
                </Layout>
              }
            />
            <Route
              path="/profile/:userId"
              element={
                <Layout>
                  <Profile />
                </Layout>
              }
            />
            <Route element={<AdminRoute />}>
              <Route
                path="/events/add"
                element={
                  <Layout>
                    <EventAdd />
                  </Layout>
                }
              />
              <Route
                path="/events/edit/:id"
                element={
                  <Layout>
                    <EventEdit />
                  </Layout>
                }
              />
              <Route
                path="/UserList/:eventId"
                element={
                  <Layout>
                    <UserList />
                  </Layout>
                }
              />
              <Route
                path="/UserList"
                element={
                  <Layout>
                    <UserList />
                  </Layout>
                }
              />
            </Route>
          </Route>
        </Routes>
      </Router>
    </Provider>
  );
}

export default App;
