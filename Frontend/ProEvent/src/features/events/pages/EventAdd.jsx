import { useEffect, useState } from 'react';
import Button from '../../../Components/Button/Button';
import AddImg from '/img/AddImg.png';
import {
  useCreateEventMutation,
  useGetEventByIdQuery,
  useUpdateEventMutation,
} from '../../../App/services/eventApi';
import { useNavigate } from 'react-router-dom';
import PropTypes from 'prop-types';
import { categories } from '../../../config/categories';
import { DateTimePicker, LocalizationProvider } from '@mui/x-date-pickers';
import styled from 'styled-components';
import { Alert, FormControl, MenuItem, TextField } from '@mui/material';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import Loader from '../../../Components/Loader/Loader';
dayjs.locale('ru');

const EventForm = ({ eventId }) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [location, setLocation] = useState('');
  const [date, setDate] = useState(dayjs());
  const [category, setCategory] = useState('');
  const [maxParticipants, setMaxParticipants] = useState(0);
  const [imageBytes, setImageBytes] = useState(null);
  const [AddEditError, setAddEditError] = useState(null); // State for error message

  const [createEvent, { isLoading: isCreating,  refetch:refetchCreate  }] = useCreateEventMutation();
  const [updateEvent, { isLoading: isUpdating ,refetch:refetchUpdate}] = useUpdateEventMutation();

  const {
    data: eventData,
    isLoading,
    isError,
    error,
  } = useGetEventByIdQuery(eventId, {
    skip: !eventId, // Skip the query if eventId is not provided
  });

  const navigate = useNavigate();

  console.log(eventData);
  useEffect(() => {
    if (eventData) {
      setName(eventData.name);
      setDescription(eventData.description);
      setLocation(eventData.location);
      setDate(dayjs(eventData.date));
      setCategory(eventData.category);
      setMaxParticipants(eventData.maxParticipants);
    }
  }, [eventData]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const newEvent = {
      id: eventId ? parseInt(eventId) : 0, // Backend handles ID for create
      name,
      description,
      location,
      date: dayjs(date), // Ensure date is in ISO format
      category,
      maxParticipants: parseInt(maxParticipants),
      image: imageBytes,
    };

    try {
      if (eventId) {
        await updateEvent(newEvent).unwrap();
        console.log('Event updated successfully!');
        setAddEditError('Все обновлено успешно');
        refetchUpdate();
      } else {
        await createEvent(newEvent).unwrap();
        console.log('Event created successfully!');
        setAddEditError('Все создано успешно');
        refetchCreate();
      }
     
    } catch (err) {
      if (err && err.data && err.data.displayMessage) {
        setAddEditError(err.data.displayMessage);
      } else {
        setAddEditError('Неверный формат данных.'); // Default error message
      }
      // Handle error more visibly to the user
    }
  };

  const handleCancel = () => {
    navigate('/');
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const arrayBuffer = reader.result;
        const byteArray = new Uint8Array(arrayBuffer);

        // Разбиваем массив на части
        const CHUNK_SIZE = 8192; // Выберите размер части (например, 8192)
        let base64String = '';
        for (let i = 0; i < byteArray.length; i += CHUNK_SIZE) {
          const chunk = byteArray.subarray(i, i + CHUNK_SIZE);
          base64String += String.fromCharCode.apply(null, chunk);
        }

        const finalBase64String = btoa(base64String);
        setImageBytes(finalBase64String);
      };
      reader.readAsArrayBuffer(file);
    } else {
      setImageBytes(null);
    }
  };
  if (isLoading) {
    return <Loader />;
  }

  if (isError) {
    return <div>Error: {error.message}</div>;
  }

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <div className="content">
        <StyledFormControl className="text" onSubmit={handleSubmit}>
          {AddEditError && (
            <Alert severity="error" sx={{ marginBottom: 2 }}>
              {AddEditError}
            </Alert>
          )}
          <h2>{eventId ? 'Изменение события' : 'Создание события'}</h2>
          <TextField
            label="Название"
            variant="outlined"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
          <TextField
            label="Описание"
            id="outlined-multiline-static"
            variant="outlined"
            multiline
            rows={4}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <div className="content2">
            <TextField
              label="Место проведения"
              variant="outlined"
              value={location}
              onChange={(e) => setLocation(e.target.value)}
              required
            />
            <DateTimePicker
              value={date}
              label="Дата"
              onChange={(newDate) => setDate(newDate)}
              required
              renderInput={(params) => <TextField {...params} />}
            />
            <FormControl variant="outlined" fullWidth sx={{ m: 0, minWidth: 120 }}>
              <TextField
                select
                label="Категория"
                variant="outlined"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                placeholder="категории"
              >
                {categories.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))}
              </TextField>
            </FormControl>
            <TextField
              label="Количество участников"
              variant="outlined"
              type="number"
              value={maxParticipants}
              onChange={(e) => setMaxParticipants(e.target.value)}
              required
            />
          </div>
          <TextField variant="outlined" type="file" onChange={handleImageChange} />

          <div className="buttons">
            <Button type="submit" disabled={isCreating || isUpdating}>
              {eventId
                ? isUpdating
                  ? 'Сохранение...'
                  : 'Сохранить'
                : isCreating
                ? 'Создание...'
                : 'Создать'}
            </Button>
            <Button type="button" onClick={handleCancel}>
              Отмена
            </Button>
          </div>
        </StyledFormControl>
        <StyledImgContainer className="image">
          <img src={AddImg} alt="Изображение" />
        </StyledImgContainer>
      </div>
    </LocalizationProvider>
  );
};

const StyledFormControl = styled.form`
  width: 50%;
  display: flex;
  gap: 10px;
  max-height: 100%;
  flex-wrap: wrap;
  flex-direction: column;
  justify-content: space-around;
  margin: 0 40px;
  .content2 {
    display: grid;
    gap: 10px;
    max-height: 100%;
    grid-template-columns: 1fr 1fr;
  }
  h2 {
    margin: 0;
  }
`;
const StyledImgContainer = styled.div`
  width: 50%;
  max-height: 100%;
`;
EventForm.propTypes = {
  eventId: PropTypes.number,
};

export default EventForm;
