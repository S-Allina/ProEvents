import { useSelector } from 'react-redux';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';
import { ErrorNotAdmin } from '../features/Error/ErrorNotAdmin';

const AdminRoute = ({ children }) => {
  const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
  const userRole = useSelector((state) => state.auth.user?.role);
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (userRole !== 'Admin') {
    return <ErrorNotAdmin />;
  }

  return children ? children : <Outlet />;
};
AdminRoute.propTypes = {
  children: PropTypes.node,
};
export default AdminRoute;
