import { ScheduleXCalendar } from '@schedule-x/react';
import { createViewWeek, createViewMonthGrid, createViewDay, createCalendar } from '@schedule-x/calendar';
import styled from 'styled-components';
import '@schedule-x/theme-default/dist/calendar.css';
import { useSelector } from 'react-redux';
import { useGetEventByUserIdQuery } from '../../../App/services/eventApi';
import { useEffect, useMemo, useState } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import Paper from '@mui/material/Paper';
import Button from '../../../Components/Button/Button';
import { useDeleteEnrollmentMutation } from '../../../App/services/enrollmentApi';
import Loader from '../../../Components/Loader/Loader';
import { IconButton, Snackbar } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { Error500 } from '../../Error/Error500';

const EventByUser = () => {
  const userId = useSelector((state) => state.auth.user.userId);
  const { data, isLoading, isError,  refetch } = useGetEventByUserIdQuery(userId, {
      refetchOnMountOrArgChange: true,
  });
  const [open, setOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState(''); // Состояние для хранения текста сообщения
  const [snackbarSeverity, setSnackbarSeverity] = useState('success'); // Состояние для типа сообщения (success, error, warning, info)
  const [deleteEvent] = useDeleteEnrollmentMutation();
  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }
    setOpen(false);
  };
  const calendarEvents = useMemo(() => {
      if (data?.result && Array.isArray(data.result)) {
          return data.result.map(event => {
              const eventDate = new Date(event.date);
              const formattedDate = `${eventDate.getFullYear()}-${String(eventDate.getMonth() + 1).padStart(2, '0')}-${String(eventDate.getDate()).padStart(2, '0')} ${String(eventDate.getHours()).padStart(2, '0')}:${String(eventDate.getMinutes()).padStart(2, '0')}`;

              return {
                  id: event.enrollmentId,
                  title: event.name,
                  start: formattedDate,
                  end: formattedDate,
              };
          });
      } else {
          return [];
      }
  }, [data]);

  const [calendar, setCalendar] = useState(null);

  useEffect(() => {
      if (calendarEvents) {
          const newCalendar = createCalendar({
              views: [
                createViewMonthGrid(),
                  createViewDay(),
                  createViewWeek(),
             
              ],
              events: calendarEvents,
          });
          setCalendar(newCalendar);
      }
  }, [calendarEvents]);

  if (isLoading) {
      return <Loader />;
  }

  if (isError) {
      return <Error500/>;
  }

  const handleDelete = async (enrollmentId) => {
      try {
          await deleteEvent(enrollmentId);
          setSnackbarMessage("Ваша запись на событие была отменена!");
          setSnackbarSeverity('success');
          refetch();
          
      } catch (error) {
        setSnackbarMessage('При отмене произошла ошибка' + error.message);
        setSnackbarSeverity('error');
      }
  };

  const columns = [
      { field: 'name', headerName: 'Название', width: 100 },
      { field: 'date', headerName: 'Дата', width: 100 },
      {
          field: 'location',
          headerName: 'Место',
          width: 90,
      },
      {
          field: 'category',
          headerName: 'Категория',
          width: 90,
      }, {
          field: 'actions',
          headerName: 'Действия',
          width: 200,
          renderCell: (params) => (
              <Button
                  variant="contained"
                  color="error"
                  size="small"
                  onClick={() => handleDelete(params.row.id)}
              >
                  Удалить
              </Button>
          ),
      }
  ];

  const rows = data.result.map(event => {
      const eventDate = new Date(event.date);
      const formattedDate = `${eventDate.getFullYear()}-${String(eventDate.getMonth() + 1).padStart(2, '0')}-${String(eventDate.getDate()).padStart(2, '0')} ${String(eventDate.getHours()).padStart(2, '0')}:${String(eventDate.getMinutes()).padStart(2, '0')}`;

      return {
          id: event.enrollmentId,
          name: event.name,
          location: event.location,
          date: formattedDate,
          category: event.category,
          status: event.status
      };
  });

  const paginationModel = { page: 0, pageSize: 5 };
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
         <Snackbar
                  open={open}
                  autoHideDuration={6000}
                  onClose={handleClose}
                  message={snackbarMessage} // Используем динамический текст сообщения
                  action={action}
                  severity={snackbarSeverity} // Используем тип сообщения
                />
          <div className="cl">
              <ScheduleXCalendar calendarApp={calendar} />
          </div>
          <div className='tb'>
              <Paper sx={{ height: '500px', width: '100%' }}>
                  <DataGrid
                      rows={rows}
                      columns={columns}
                      initialState={{ pagination: { paginationModel } }}
                      pageSizeOptions={[5, 10]}
                      sx={{ border: 0 }}
                  />
              </Paper>
          </div>
      </StyledWrapper>
  );
};

const StyledWrapper = styled('div')({
  display:'flex',
  gap:'30px',
  '.cl' :{
    width:'60%',
  },
    '.tb' : {
    width:'40%'}
    ,
    '.passed-event': {
        color: 'gray !important', // !important для переопределения стилей DataGrid
    },
});
export default EventByUser;
