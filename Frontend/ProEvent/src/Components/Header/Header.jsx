
import { useLocation, useNavigate } from 'react-router-dom'; // Импортируем useLocation
import './Header.css';
import NavLink from '../NavLink/NavLink';
import { useLogoutUserMutation } from '../../App/services/authApi';
import { useDispatch } from 'react-redux';
import { logout } from '../../App/slices/authSlice';
import { Button } from '@mui/material';
import logo from '/img/logo.png';


function Header() {
  const location = useLocation();
  const [logoutUser, { isLoading }] = useLogoutUserMutation();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await logoutUser().unwrap(); // Выполняем запрос на сервер для выхода

      // Диспатчим action logout для обновления состояния Redux и удаления данных из localStorage
      dispatch(logout());

      // Перенаправляем на страницу логина (или любую другую, которую ты хочешь)
      navigate('/login');
    } catch (error) {
      console.error('Logout error:', error);
      // Здесь можно обработать ошибку, например, показать сообщение пользователю
    }
  };
  return (
    <header className="header">
              <img className='logo' src={logo} alt="Изображение" />
      
      <nav className="nav">
        <NavLink to="/" isActive={location.pathname === "/"}>Главная</NavLink>
        <NavLink to="/events" isActive={location.pathname === "/events"}>События</NavLink>
        <NavLink to="/events/my-calendar" isActive={location.pathname === "/events/my-calendar"}>Мой календарь</NavLink>
        <NavLink to="/profile" isActive={location.pathname === "/profile"}>Профиль</NavLink>
        <NavLink to="/UserList" isActive={location.pathname === "/UserList"}>Список пользователей</NavLink>
        <NavLink to="/events/add" isActive={location.pathname === "/events/add"}>Добавление события</NavLink>

        <Button onClick={handleLogout} disabled={isLoading}>
          {isLoading ? 'Выходим...' : 'Выйти'}
        </Button>
        <span className="nav-indicator"></span>
      </nav>
    </header>
  );
}

export default Header;