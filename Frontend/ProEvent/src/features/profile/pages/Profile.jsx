import { useEffect, useState } from 'react';
import Button from '../../../Components/Button/Button';
import ProfileImg from '/img/ProfileImg.png';
import { useParams } from 'react-router-dom';
import PropTypes from 'prop-types';
import { DateTimePicker, LocalizationProvider } from '@mui/x-date-pickers';
import styled from 'styled-components';
import { TextField } from '@mui/material';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import Loader from '../../../Components/Loader/Loader';
import {
  useGetParticipantByUserIdQuery,
  useUpdateParticipantMutation,
} from '../../../App/services/participantApi';
import { useSelector } from 'react-redux';
import { Error500 } from '../../Error/Error500';
dayjs.locale('ru');

const Profile = () => {
  const { userId: routeUserId } = useParams(); // Получаем userId из параметров маршрута (например, /profile/:userId)
  const loggedInUserId = useSelector((state) => state.auth.user.userId); // Получаем userId залогиненного пользователя из Redux store
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState(dayjs());
  const [isEditing, setIsEditing] = useState(false); // Отслеживаем, находится ли профиль в режиме редактирования

  // Определяем userId для получения данных: используем routeUserId, если он есть (случай администратора), иначе используем loggedInUserId
  const userIdToFetch = routeUserId || loggedInUserId;
  console.log(userIdToFetch);
  const {
    data: participantData,
    isLoading,
    isError,
  } = useGetParticipantByUserIdQuery(userIdToFetch);
  const [updateParticipant, { isLoading: isUpdating }] = useUpdateParticipantMutation();

  // const navigate = useNavigate();

  useEffect(() => {
    if (participantData && participantData.result) {
      const { firstName, lastName, email, dateOfBirth } = participantData.result;
      setFirstName(firstName || '');
      setLastName(lastName || '');
      setEmail(email || '');
      setDateOfBirth(dateOfBirth ? dayjs(dateOfBirth) : dayjs());
    }
  }, [participantData]);

  const handleSave = async () => {
    const updatedParticipant = {
      id: participantData.result.id, // Убедитесь, что у вас есть ID
      firstName,
      lastName,
      email,
      dateOfBirth: dayjs(dateOfBirth).toISOString(), // Отправляем дату в формате ISO
      userId: userIdToFetch, // Сохраняем userId一致
    };

    try {
      await updateParticipant(updatedParticipant).unwrap();
      setIsEditing(false); // Выключаем режим редактирования после успешного сохранения
      console.log('Профиль успешно обновлен!');
    } catch (err) {
      console.error('Не удалось обновить профиль:', err);
      // Обрабатываем ошибку (например, показываем сообщение об ошибке пользователю)
    }
  };

  const handleCancel = () => {
    setIsEditing(false); // Возвращаемся в режим просмотра
    // Сбрасываем значения формы к исходным данным (опционально)
    if (participantData && participantData.result) {
      const { firstName, lastName, email, dateOfBirth } = participantData.result;
      setFirstName(firstName || '');
      setLastName(lastName || '');
      setEmail(email || '');
      setDateOfBirth(dateOfBirth ? dayjs(dateOfBirth) : dayjs());
    }
  };

  if (isLoading) {
    return <Loader />;
  }

  if (isError) {
    return <Error500 />;
  }

  if (!participantData || !participantData.result) {
    return <div>Профиль не найден.</div>;
  }

  const { firstName: originalFirstName } = participantData.result; // Используем исходные данные

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <div className="content">
        <StyledFormControl className="text" onSubmit={(e) => e.preventDefault()}>
          {' '}
          {/* Предотвращаем отправку формы по умолчанию */}
          <h2>Профиль {originalFirstName}</h2>
          <TextField
            label="Имя"
            variant="outlined"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
            disabled={!isEditing} // Отключаем поле, если не в режиме редактирования
          />
          <TextField
            label="Фамилия"
            variant="outlined"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            required
            disabled={!isEditing}
          />
          <TextField
            label="Email"
            variant="outlined"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            disabled={!isEditing}
          />
          <DateTimePicker
            label="Дата рождения"
            value={dateOfBirth}
            onChange={(newDate) => setDateOfBirth(newDate)}
            required
            renderInput={(params) => <TextField {...params} disabled={!isEditing} />}
            disabled={!isEditing}
          />
          {/* Условно рендерим кнопки в зависимости от состояния редактирования */}
          {isEditing ? (
            <div className="buttons">
              <Button
                variant="contained"
                color="primary"
                onClick={handleSave}
                disabled={isUpdating}
              >
                {isUpdating ? 'Сохранение...' : 'Сохранить'}
              </Button>
              <Button variant="outlined" onClick={handleCancel}>
                Отмена
              </Button>
            </div>
          ) : (
            <div className="buttons">
              <Button variant="contained" color="primary" onClick={() => setIsEditing(true)}>
                Редактировать
              </Button>
            </div>
          )}
        </StyledFormControl>
        <StyledImgContainer className="image">
          <img src={ProfileImg} alt="Изображение" />
        </StyledImgContainer>
      </div>
    </LocalizationProvider>
  );
};
const StyledFormControl = styled.form`
  width: 40%;
  display: grid;
  gap: 30px;
  grid-template-columns: 1fr;
  max-height: 100%;

  justify-content: space-around;
  margin: 0 40px;
`;
const StyledImgContainer = styled.div`
  width: 50%;
  max-height: 100%;
`;
Profile.propTypes = {
  paticipantId: PropTypes.number,
};

export default Profile;
