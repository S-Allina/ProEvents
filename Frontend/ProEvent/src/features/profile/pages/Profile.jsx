import { useEffect, useState } from 'react';
import Button from '../../../Components/Button/Button';
import ProfileImg from '/img/ProfileImg.png';
import { useParams } from 'react-router-dom';
import PropTypes from 'prop-types';
import { DatePicker, LocalizationProvider } from '@mui/x-date-pickers';
import styled from 'styled-components';
import { Alert, TextField } from '@mui/material';
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
  const { userId: routeUserId } = useParams();
  const loggedInUserId = useSelector((state) => state.auth.user.userId);
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState(dayjs());
  const [isEditing, setIsEditing] = useState(false);
  const [AddEditError, setAddEditError] = useState(null);
  const userIdToFetch = routeUserId || loggedInUserId;
  console.log(userIdToFetch);
  const {
    data: participantData,
    isLoading,
    isError,
  } = useGetParticipantByUserIdQuery(userIdToFetch);
  const [updateParticipant, { isLoading: isUpdating }] = useUpdateParticipantMutation();
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
      id: participantData.result.id,
      firstName,
      lastName,
      email,
      dateOfBirth: dayjs(dateOfBirth).toISOString(),
      userId: userIdToFetch,
    };

    try {
      const response = await updateParticipant(updatedParticipant).unwrap();
      if (!response.isSuccess) {
        const { displayMessage, errorMessages } = response;
        const combinedErrors = errorMessages.join(', ');
        setAddEditError(`${displayMessage || 'Ошибка обновления профиля.'}: ${combinedErrors}`);
        return;
      }
      setIsEditing(false);
      setAddEditError('Профиль успешно обновлен!');
    } catch (err) {
      if (err && err.data) {
        const { message, errors } = err.data;
        setAddEditError(`${message}: ${errors.join(', ')}`);
      } else {
        setAddEditError('Неверный формат данных.');
      }
    }
  };

  const handleCancel = () => {
    setIsEditing(false);
    if (participantData && participantData.result) {
      const { firstName, lastName, email, dateOfBirth } = participantData.result;
      setFirstName(firstName || '');
      setLastName(lastName || '');
      setEmail(email || '');
      setDateOfBirth(dateOfBirth ? dayjs(dateOfBirth) : dayjs());
      setAddEditError('');
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

  const { firstName: originalFirstName } = participantData.result;

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <div className="content">
        <StyledFormControl className="text" onSubmit={(e) => e.preventDefault()}>
          <h2>Профиль {originalFirstName}</h2>
          {AddEditError && (
            <Alert severity="error" sx={{ marginBottom: 2 }}>
              {AddEditError}
            </Alert>
          )}
          <TextField
            label="Имя"
            variant="outlined"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
            disabled={!isEditing}
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
          <DatePicker
            label="Дата рождения"
            value={dateOfBirth}
            onChange={(newDate) => setDateOfBirth(newDate)}
            required
            renderInput={(params) => <TextField {...params} disabled={!isEditing} />}
            disabled={!isEditing}
          />
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
