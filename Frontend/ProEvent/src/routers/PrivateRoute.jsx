import {  Navigate, Outlet, useLocation } from 'react-router-dom';
import {  useSelector } from 'react-redux';

const PrivateRoute = () => {
  const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
  const location = useLocation();
  console.log('PrivateRoute - isAuthenticated:', isAuthenticated); // Добавлено

  return isAuthenticated ? (
    <Outlet />
  ) : (
    <Navigate to="/login" replace state={{ from: location }} />
  );
};
export default PrivateRoute;