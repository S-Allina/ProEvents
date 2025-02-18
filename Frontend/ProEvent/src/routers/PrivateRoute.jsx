import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useSelector } from 'react-redux';
import Loader from '../Components/Loader/Loader';
import { useEffect, useState } from 'react';

const PrivateRoute = () => {
  const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
  const userRole = useSelector((state) => state.auth.user?.role);
  const location = useLocation();
  const [isRoleChecked, setIsRoleChecked] = useState(false);

  useEffect(() => {
    if (userRole !== undefined && userRole !== null) {
      // Отслеживаем только userRole
      console.log(userRole);
      setIsRoleChecked(true);
    }
  }, [userRole]); // Зависимость только от userRole

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (!isRoleChecked) {
    console.log(isRoleChecked);
    return <Loader />;
  }

  return <Outlet />;
};

export default PrivateRoute;
