
import styled from 'styled-components';
import Button from '../../../Components/Button/Button';
// Предполагается, что у вас есть компонент Button
// import { EventDTO } from '../models/Event'; // Импортируем EventDTO
import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';
import { useDeleteEventMutation } from '../../../App/services/eventApi';
import { useCreateEnrollmentMutation } from '../../../App/services/enrollmentApi';
import { useSelector } from 'react-redux';
import Snackbar from '@mui/material/Snackbar';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import { useState } from 'react';
import dayjs from 'dayjs';


const EventCard = ({ event }) => {
  const { id, name, date, location, image, status } = event;
  const imageUrl = image ? `data:image/jpeg;base64,${image}` : null;
  const [open, setOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState(''); // Состояние для хранения текста сообщения
  const [snackbarSeverity, setSnackbarSeverity] = useState('success'); // Состояние для типа сообщения (success, error, warning, info)

  const navigate = useNavigate();
  const [createEnrollment] = useCreateEnrollmentMutation();
  const userId = useSelector((state) => state.auth.user.userId);

  const formatDate = (date) => {
    if (!date) {
      return '';
    }
  
    return dayjs(date).format('DD.MM.YYYY');
  };
  const formattedDate = formatDate(date);

  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }
    setOpen(false);
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
      if(result.isSuccess) {
      // Обработка успешного ответа
      setSnackbarMessage("Вы успешно зарегистрированы!");
      setSnackbarSeverity('success');
      }
      else{
        setSnackbarMessage(result.displayMessage);
        setSnackbarSeverity('error');
      }
      setOpen(true);
      
console.log(result);
      
    } catch (error) {
      // Обработка ошибки
      console.error('Failed to create enrollment:', error);

     
    }
  };


  const handleEditClick = () => {
    navigate(`/events/edit/${id}`);
  };

  const handleFullClick = () => {
    navigate(`/events/Full/${id}`);
  };
  const handleUsersClick = () => {
    navigate(`/UserList/${id}`);
  };
  
  const action = (
    <>
  
      <IconButton
        size="small"
        aria-label="close"
        color="inherit"
        onClick={handleClose}
      >
        <CloseIcon fontSize="small" />
      </IconButton>
    </>
  );
  return (
    <StyledWrapper>
      <div className="card">
      <Snackbar
          open={open}
          autoHideDuration={6000}
          onClose={handleClose}
          message={snackbarMessage} // Используем динамический текст сообщения
          action={action}
          severity={snackbarSeverity} // Используем тип сообщения
        />
        <div className="img">
          {imageUrl && <img src={imageUrl} alt={name} />}
        </div>
        <div className="text">
          <p className="h3">{name}</p>
          <p className="p">
            {formattedDate}, {location}
          </p>
          <div className="buttons">
            { status!='NoPlaces' ? 
            <Button onClick={handleEnroll} > {/* Отключаем кнопку, пока данные не загружены */}
              Пойду! {/* Изменяем текст кнопки */}
            </Button>
            : <p>Мест нет</p>
}
            <Button onClick={handleFullClick}>Подробнее</Button>
            <Button onClick={handleEditClick}>Изменить</Button>
    <Button onClick={handleUsersClick}>Участники</Button>
          </div>
        </div>
      </div>
    </StyledWrapper>
  );
};

const StyledWrapper = styled.div`
  .card {
    width: 292px;
    height: 395px;
    background: white;
    border-radius: 30px;
    box-shadow: 15px 15px 30px #bebebe,
               -15px -15px 30px #ffffff;
    transition: 0.2s ease-in-out;
  }
.buttons{
    display: flex;
    gap: 10px;
    flex-wrap:wrap;
    align-items: space-between;
    width: 100%;}
    .buttons Button{
    width: 115px;}
  .img {
    width: 100%;
    height: 50%;
    border-top-left-radius: 30px;
    border-top-right-radius: 30px;
    background: linear-gradient(#e66465, #9198e5);
    display: flex;
    align-items: top;
    justify-content: right;
  }
.img img{
width: 100%;
border-top-left-radius: 30px;
    border-top-right-radius: 30px;
}

  .text {
    margin: 0px;
    padding: 10px 20px 20px 20px;
    display: flex;
    flex-direction: column;
    align-items: space-around;
  }

.buttons p{
margin:auto
}
  .text .h3 {
    font-family: 'Lucida Sans' sans-serif;
    font-size: 15px;
    font-weight: 600;
        margin: 0;
        
    color: black;
  }

  .text .p {
    font-family: 'Lucida Sans' sans-serif;
    color: #999999;
    font-size: 13px;
    margin:10px;
  }


   
  .save:hover {
    transform: scale(1.1) rotate(10deg);
  }

  .save:hover .svg {
    fill: #ced8de;
  }`;
  EventCard.propTypes = {
    event: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
      description: PropTypes.string,
      date: PropTypes.string.isRequired, // Проверяем, что date - строка
      location: PropTypes.string,
      category: PropTypes.string,
      maxParticipants: PropTypes.number,
      image: PropTypes.string, 
      status:PropTypes.string
    }).isRequired,
  };
export default EventCard;
