
import IconButton from '@mui/material/IconButton';
import OutlinedInput from '@mui/material/OutlinedInput';
import InputLabel from '@mui/material/InputLabel';
import InputAdornment from '@mui/material/InputAdornment';
import FormControl from '@mui/material/FormControl';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import { useEffect, useState } from "react";
import Button from '../../../Components/Button/Button'
import { Alert, TextField } from '@mui/material';
import { useLoginUserMutation } from '../../../App/services/authApi';
import { useDispatch, useSelector } from 'react-redux';
import { NavLink, useLocation, useNavigate } from 'react-router-dom'; // Импортируем useNavigate
import { login } from '../../../App/slices/authSlice';
import styled from 'styled-components';
import image from '/img/image.png';


export function Login() {
  const location = useLocation();
  const [showPassword, setShowPassword] = useState(false);
  const [loginValue, setloginValue] = useState('');
  const [password, setPassword] = useState('');
  const [loginUser] = useLoginUserMutation();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [loginError, setLoginError] = useState(null); // State for error message

  const handleClickShowPassword = () => setShowPassword((show) => !show);
  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };
  const handleMouseUpPassword = (event) => {
    event.preventDefault();
  };
  const isLoggedIn = useSelector((state) => state.auth.isAuthenticated);
  useEffect(() => {
    if (isLoggedIn) {
      const redirectPath = location.state?.from || '/';
      navigate(redirectPath, { replace: true });
    }
  }, [isLoggedIn, navigate, location]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setLoginError(null); // Clear any previous errors
    try {
      const result = await loginUser({ userName: loginValue, password }).unwrap();
      console.log(result);
      if (result && result.token) {
        dispatch(login(result));
        navigate('/');
      }
    } catch (err) {
      // Set the error message based on the error from the API

      if (err && err.data && err.data.displayMessage) {
        setLoginError(err.data.displayMessage);
      } else {
        setLoginError('Неверный логин или пароль.'); // Default error message
      }
    }
  };

  return (
    <StyledWrapper>
      <form onSubmit={handleSubmit}> {/* Добавляем обработчик onSubmit */}
      <h2>С возвращением!</h2>
      <p>Мы рады видеть вас снова! Войдите в свой аккаунт, чтобы получить доступ ко всем вашим запланированным событиям и возможностям планирования:</p>
      {loginError && (
          <Alert severity="error" sx={{ marginBottom: 2 }}>
            {loginError}
          </Alert>
        )}
          <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
              <TextField
                  label="Логин"
                  id="name"
                  variant="outlined"
                  value={loginValue}
                  onChange={(e) => setloginValue(e.target.value)}
                  required
              />
          </FormControl>

          <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
              <InputLabel htmlFor="outlined-adornment-password">Пароль</InputLabel>
              <OutlinedInput
                  id="outlined-adornment-password"
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  endAdornment={
                      <InputAdornment position="end">
                          <IconButton
                              aria-label={showPassword ? 'hide password' : 'show password'}
                              onClick={handleClickShowPassword}
                              onMouseDown={handleMouseDownPassword}
                              onMouseUp={handleMouseUpPassword}
                              edge="end"
                          >
                              {showPassword ? <VisibilityOff /> : <Visibility />}
                          </IconButton>
                      </InputAdornment>
                  }
                  label="Пароль"
              />
          </FormControl>
          <Button type="submit" > {/* Устанавливаем type="submit" и отключаем во время загрузки */}
              Войти
          </Button>
          <p>Ещё нет аккаунта?  <NavLink to="/register" isActive={location.pathname === "/register"}>Зарегистрируйся!</NavLink></p>
          {/* {isError && <div style={{ color: "red" }}>Ошибка авторизации: {error.message}</div>} Отображение ошибки */}
          </form>
          <div className="image">
            <img src={image} alt="Изображение" />  {/* Добавляем изображение */}
          </div>
      </StyledWrapper>
  );
}
const StyledWrapper = styled.div`
  display:flex;
  gap:50px;
  justify-content:space-between;
  form{
  width:40%;
  display:flex;
  flex-direction:column;
  justify-content:center;
  align-items:center;
  padding:20px 40px;
  }
  p NavLink{
  color:rgb(207, 154, 241);
      font-size: 24px;
display:inline;
  }
  p{
   text-align: center;
    color:rgb(207, 154, 241);
    font-size: 20px;
    margin-bottom:10px;
  }
h2{
    text-align: center;
    color:rgb(174, 43, 255);
    font-size: 44px;
    margin-bottom:0;}
    .image{
    width:36%;
padding-right:50px;
    }
  `;