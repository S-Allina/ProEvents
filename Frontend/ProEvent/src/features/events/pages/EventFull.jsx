import styled from 'styled-components';
import Button from '../../../Components/Button/Button';
import PropTypes from 'prop-types';
import CloseIcon from '@mui/icons-material/Close';
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDeleteEventMutation, useGetEventByIdQuery } from '../../../App/services/eventApi';
import Loader from '../../../Components/Loader/Loader';
import { useCreateEnrollmentMutation } from '../../../App/services/enrollmentApi';
import { useSelector } from 'react-redux';
import { IconButton, Snackbar } from '@mui/material';

const EventFull = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [imageUrl, setImageUrl] = useState(null);
  const { data, isLoading, isError, error } = useGetEventByIdQuery(id);
  const [createEnrollment] = useCreateEnrollmentMutation();
  const userId = useSelector((state) => state.auth.user.userId);
  const [deleteEvent, { isLoading: isDeleting }] = useDeleteEventMutation();
  const [open, setOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [snackbarSeverity, setSnackbarSeverity] = useState('success');
  const userRole = useSelector((state) => state.auth.user?.role);

  console.log(data);
  useEffect(() => {
    if (data && data.result.image) {
      setImageUrl(`data:image/jpeg;base64,${data.result.image}`);
    } else {
      setImageUrl(null);
    }
  }, [data]);
  if (isLoading) {
    return <Loader />;
  }
  if (isError) {
    return <div>Error: {error.message}</div>;
  }
  const handleUsersClick = () => {
    navigate(`/UserList/${id}`);
  };

  const handleEnroll = async () => {
    try {
      const enrollmentDTO = {
        eventId: id,
        participantId: 0,
        userId: userId,
        registrationDate: new Date().toISOString(),
      };
      const result = await createEnrollment(enrollmentDTO).unwrap();
      if (result.isSuccess) {
        setSnackbarMessage('Вы успешно зарегистрированы!');
        setSnackbarSeverity('success');
      } else {
        setSnackbarMessage(result.displayMessage);
        setSnackbarSeverity('error');
      }
    } catch (error) {
      setSnackbarMessage(error.data.message);
      setSnackbarSeverity('error');      
    }finally {
      setOpen(true);
    }
  };
  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }
    setOpen(false);
  };
  const handleEditClick = () => {
    navigate(`/events/edit/${id}`);
  };
  const handleDeleteClick = () => {
    deleteEvent(id)
      .unwrap()
      .then((result) => {
        if (result.isSuccess) {
          alert('Удаление прошло успешно');
          navigate(`/`);
        } else {
          alert(`Не удалось удалить событие: ${result.displayMessage || 'Неизвестная ошибка'}`);
        }
      })
      .catch((error) => {
        console.error("Ошибка при удалении события:", error);
        alert(`Произошла ошибка при удалении события: ${error.message || 'Неизвестная ошибка'}`);
      });
  };
  const isAdmin = userRole === 'Admin';
  const action = (
    <>
      <IconButton size="small" aria-label="close" color="inherit" onClick={handleClose}>
        <CloseIcon fontSize="small" />
      </IconButton>
    </>
  );
  return (
    <StyledWrapper>
       <Snackbar
                open={open}
                autoHideDuration={6000}
                onClose={handleClose}
                message={snackbarMessage}
                action={action}
                severity={snackbarSeverity}
              />
      <div className="image">{imageUrl && <img src={imageUrl} alt={data.result.name} />}</div>
      <div className="content">
        <h2>{data.result.name}</h2>
        <p className="discription">{data.result.description}</p>
        <p>Дата: {data.result.date}</p>
        <p>Место: {data.result.location}</p>
        <p>Категория: {data.result.category}</p>
        <p>Максимальное число участников: {data.result.maxParticipants}</p>
        <div className="buttons">
          {data.status != 'NoPlaces' ? (
            <Button onClick={handleEnroll}>Пойду!</Button>
          ) : (
            <p>Мест нет</p>
          )}
          {isAdmin && (<> <Button onClick={handleUsersClick}>Участники</Button>
          <Button onClick={handleEditClick}>Изменить</Button>
                   
                      <Button onClick={handleDeleteClick} disabled={isDeleting}>
                       {isDeleting ? 'Удаление...' : 'Удалить'}
                      </Button></>
                    )}
        </div>
      </div>
    </StyledWrapper>
  );
};
const StyledWrapper = styled.div`
  display: flex;
  gap: 50px;
  margin: 0 -30px;
  .content {
    width: 50%;
    height: 100%;
    display: flex;
    justify-content: space-around;
    align-items: flex-start;
    flex-direction: column;
    padding: 50px;
    padding-top: 10px;
    padding-left: 0px;
    background: white;
    border-radius: 30px;
    box-shadow: 15px 15px 30pxrgba (190, 190, 190, 0.31), -15px -15px 30pxrgba (255, 255, 255, 0.42);
    transition: 0.2s ease-in-out;
  }
  .buttons {
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
    align-items: space-between;
    width: 100%;
  }
  .buttons Button {
    width: 215px;
  }
  .buttons p {
    display: block;
    width: 215px;
    font-size: 20px;
    font-weight: bold;
  }
  .image {
    max-width: 50%;
    max-height: 480px;
    border-radius: 30px;
    padding-left: 30px;
  }
  .image img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 30px;
  }

  h2 {
    font-family: 'Lucida Sans' sans-serif;
    font-size: 35px;
    font-weight: 600;
    margin: 0;
    color: #7c30e1;
  }
  .discription {
    color: #999999;
    font-size: 13px;
  }
  p {
    font-family: 'Lucida Sans' sans-serif;
    color: rgb(210, 110, 252);
    font-size: 17px;
    margin: 10px;
  }
`;
EventFull.propTypes = {
  event: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    description: PropTypes.string,
    date: PropTypes.string.isRequired,
    location: PropTypes.string,
    category: PropTypes.string,
    maxParticipants: PropTypes.number,
    image: PropTypes.string,
  }).isRequired,
};
export default EventFull;
