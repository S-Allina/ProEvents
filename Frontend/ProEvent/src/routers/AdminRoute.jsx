import { useSelector } from 'react-redux';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types'; // Импортируем PropTypes
import { ErrorNotAdmin } from '../features/Error/ErrorNotAdmin';

const AdminRoute = ({ children }) => {


    
  const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
  const userRole = useSelector((state) => state.auth.user?.role); // Получаем роль из Redux
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (userRole !== 'Admin') {
    // Можно вернуть страницу с сообщением об отсутствии доступа,
    // или перенаправить на другую страницу.
    return <ErrorNotAdmin/>; // Пример: перенаправляем на главную страницу
  }

  return children ? children : <Outlet />; // Рендерим children или Outlet, если есть
};
AdminRoute.propTypes = {
    children: PropTypes.node, // Определяем prop children как React node
  };
export default AdminRoute;