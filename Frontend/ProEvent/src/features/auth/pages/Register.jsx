import IconButton from '@mui/material/IconButton';
import OutlinedInput from '@mui/material/OutlinedInput';
import InputLabel from '@mui/material/InputLabel';
import InputAdornment from '@mui/material/InputAdornment';
import FormControl from '@mui/material/FormControl';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import { useState } from 'react';
import Button from '../../../Components/Button/Button';
import { Alert, TextField } from '@mui/material';
import { useRegisterUserMutation } from '../../../App/services/authApi';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { login } from '../../../App/slices/authSlice';
import styled from 'styled-components';
import SignIn from '/img/SignIn.png';
import { DatePicker, LocalizationProvider } from '@mui/x-date-pickers';
import dayjs from 'dayjs';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import Loader from '../../../Components/Loader/Loader';

export function Register() {
  const [date, setDate] = useState(dayjs());
  const [showPassword, setShowPassword] = useState(false);
  const [loginValue, setloginValue] = useState('');
  const [password, setPassword] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [registerUser, { isLoading }] = useRegisterUserMutation();
  const [loginError, setLoginError] = useState(null);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (event) => {
    event.preventDefault();
  };

  const handleMouseUpPassword = (event) => {
    event.preventDefault();
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      const registerData = {
        userName: loginValue,
        password: password,
        firstName: firstName,
        lastName: lastName,
        email: email,
        dateOfBirth: date ? date.format('YYYY-MM-DD') : undefined,
      };

      const result = await registerUser(registerData).unwrap();
      if (result && result.token) {
        setLoginError('Всё успешно, поздравляю');

        dispatch(login(result));

        navigate('/');
      } else {
        console.error('Registration failed:', result);
      }
    } catch (err) {
      setLoginError('Ошибка: ' + err.data.message);
    }
  };
  if (isLoading) {
    return <Loader />;
  }
  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <StyledWrapper>
        <form onSubmit={handleSubmit}>
          <p>Создайте свой аккаунт!</p>
          {loginError && (
            <Alert severity="error" sx={{ marginBottom: 2 }}>
              {loginError}
            </Alert>
          )}
          <div className="fields">
            <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
              <TextField
                label="Логин"
                id="login"
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
                required
              />
            </FormControl>
            <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
              <TextField
                label="Фамилия"
                id="LastName"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                required
              />
            </FormControl>
            <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
              <TextField
                label="Имя"
                id="name"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                required
              />
            </FormControl>
            <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
              <TextField
                label="Email"
                id="email"
                variant="outlined"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </FormControl>
            <DatePicker
              label="Дата рождения"
              value={date}
              onChange={(newDate) => setDate(newDate)}
              renderInput={(params) => <TextField {...params} required />}
            />
          </div>
          <Button type="submit" disabled={isLoading}>
            {isLoading ? 'Регистрация...' : 'Зарегистрироваться'}
          </Button>
        </form>
        <div className="image">
          <img src={SignIn} alt="Изображение" />
        </div>
      </StyledWrapper>
    </LocalizationProvider>
  );
}
const StyledWrapper = styled.div`
  display: flex;
  gap: 50px;
  justify-content: space-between;
  form {
    width: 50%;

    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding: 20px 30px;
  }
  .fields {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
  }

  p {
    text-align: center;
    color: rgb(207, 154, 241);
    font-size: 20px;
    margin-bottom: 20px;
  }
  h2 {
    text-align: center;
    color: rgb(174, 43, 255);
    font-size: 44px;
    margin-bottom: 0;
  }
  .image {
    width: 50%;
    margin: auto 0;
    padding-right: 50px;
  }
`;
