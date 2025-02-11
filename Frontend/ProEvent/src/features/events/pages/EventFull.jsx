import styled from 'styled-components';
import Button from '../../../Components/Button/Button';
// Предполагается, что у вас есть компонент Button
// import { EventDTO } from '../models/Event'; // Импортируем EventDTO
import PropTypes from 'prop-types';
// import { useNavigate, useParams } from 'react-router-dom';
// import { useDeleteEventMutation, useGetEventByIdQuery } from '../../../App/services/eventApi';

import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDeleteEventMutation, useGetEventByIdQuery } from '../../../App/services/eventApi';
import Loader from '../../../Components/Loader/Loader';
import { useCreateEnrollmentMutation } from '../../../App/services/enrollmentApi';
import { useSelector } from 'react-redux';

/**
 * @typedef {object} EventDTO
 * @property {number} id
 * @property {string} name
 * @property {string} description
 * @property {string} date - Сменить startTime и endTime на date
 * @property {string} location
 * @property {string} category
 * @property {number} maxParticipants
 * @property {string | null} image - Тип изменен на string или null
 */

/**
 * @param {object} props
 * @param {EventDTO} props.event
 * @returns {JSX.Element}
 */
const EventFull = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [imageUrl, setImageUrl] = useState(null);
  // const navigate = useNavigate(); // Получаем функцию navigate
  // const [deleteEvent, { isLoading: isDeleting }] = useDeleteEventMutation(); // Получаем функцию deleteEvent
  const { data, isLoading, isError, error } = useGetEventByIdQuery(id);
  const [createEnrollment] = useCreateEnrollmentMutation();
  const userId = useSelector((state) => state.auth.user.userId); // Получаем isSuccess
  const [deleteEvent, { isLoading: isDeleting }] = useDeleteEventMutation();

  console.log(data);
  useEffect(() => {
    if (data && data.image) {
      setImageUrl(`data:image/jpeg;base64,${data.image}`);
    } else {
      setImageUrl(null); // Or a default image URL
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

      console.log('Enrollment DTO:', enrollmentDTO); // Для отладки

      await createEnrollment(enrollmentDTO).unwrap();
      console.log('Enrollment created successfully!');
      // Дополнительная логика после успешной регистрации
    } catch (error) {
      console.error('Failed to create enrollment:', error);
      // Обработка ошибок (например, отображение сообщения об ошибке)
    }
  };

  const handleEditClick = () => {
    navigate(`/events/edit/${id}`);
  };
  const handleDeleteClick = async () => {
    try {
      await deleteEvent(id).unwrap();
      alert('Удаление прошло успешно'); // Замените на более удобный способ отображения ошибок
      navigate(`/`); // Внимание: может быть не лучшим решением для UI
    } catch (error) {
      alert('Не удалось удалить событие',error); // Замените на более удобный способ отображения ошибок
    }
  };
  return (
    <StyledWrapper>
      <div className="image">{imageUrl && <img src={imageUrl} alt={data.name} />}</div>
      <div className="content">
        <h2>{data.name}</h2>
        <p className="discription">{data.description}</p>
        <p>Дата: {data.date}</p>
        <p>Место: {data.location}</p>
        <p>Категория: {data.category}</p>
        <p>Максимальное число участников: {data.maxParticipants}</p>
        <div className="buttons">
          {data.status!='NoPlaces' ? (
            <Button onClick={handleEnroll}>
              Пойду! 
            </Button>
          ) : (
            <p>Мест нет</p>
          )}
          <Button onClick={handleUsersClick}>Участники</Button>
          <Button onClick={handleEditClick}>Изменить</Button>
          <Button onClick={handleDeleteClick} disabled={isDeleting}>
            {isDeleting ? 'Удаление...' : 'Удалить'}
          </Button>
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
    date: PropTypes.string.isRequired, // Проверяем, что date - строка
    location: PropTypes.string,
    category: PropTypes.string,
    maxParticipants: PropTypes.number,
    image: PropTypes.string, // Проверяем, что image - строка
  }).isRequired,
};
export default EventFull;
