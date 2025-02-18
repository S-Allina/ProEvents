import styled from 'styled-components';
import '@schedule-x/theme-default/dist/calendar.css';
import PropTypes from 'prop-types';
import { DataGrid } from '@mui/x-data-grid';
import Paper from '@mui/material/Paper';
import {
  useDeleteEnrollmentMutation,
  useGetParticipantsByEventIdQuery,
} from '../../App/services/enrollmentApi';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '../../Components/Button/Button';
import Loader from '../../Components/Loader/Loader';
import {
  useDeleteParticipantMutation,
  useGetParticipantsQuery,
} from '../../App/services/participantApi';
import { useState } from 'react';
import { IconButton, Snackbar } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

const UserList = () => {
  const { eventId } = useParams();
  const navigate = useNavigate();

  const isEventSpecific = !!eventId;
  const [open, setOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [snackbarSeverity, setSnackbarSeverity] = useState('success');
  const {
    data: eventParticipantsData,
    isLoading: isEventParticipantsLoading,
    isError: isEventParticipantsError,
    error: eventParticipantsError,
    refetch: refetchEventParticipants,
  } = useGetParticipantsByEventIdQuery(eventId, {
    skip: !isEventSpecific,
    refetchOnMountOrArgChange: true,
  });

  const {
    data: allParticipantsData,
    isLoading: isAllParticipantsLoading,
    isError: isAllParticipantsError,
    error: allParticipantsError,
    refetch: refetchAllParticipants,
  } = useGetParticipantsQuery(undefined, {
    skip: isEventSpecific,
    refetchOnMountOrArgChange: true,
  });
  console.log(allParticipantsData);
  console.log(eventParticipantsData);
  const [deleteEnrollment, { isLoading: isDeletingEnrollment }] = useDeleteEnrollmentMutation();
  const [deleteParticipant, { isLoading: isDeletingParticipant }] = useDeleteParticipantMutation();

  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }
    setOpen(false);
  };
  const handleDelete = async (id) => {
    try {
      if (isEventSpecific) {
        await deleteEnrollment(id);

        refetchEventParticipants();
      } else {
        await deleteParticipant(id);

        refetchAllParticipants();
      }

      setSnackbarMessage('Пользователь удален!');
      setSnackbarSeverity('success');
      setOpen(true);
    } catch (error) {
      setSnackbarMessage('При отмене произошла ошибка' + error.message);
      setSnackbarSeverity('error');
      setOpen(true);
    }
  };
  const handleToProfile = async (id) => {
    navigate(`/Profile/${id}`);
  };
  let data, isLoading, isError, error, rows;

  if (isEventSpecific) {
    data = eventParticipantsData;
    isLoading = isEventParticipantsLoading;
    isError = isEventParticipantsError;
    error = eventParticipantsError;

    rows =
      data?.result?.map((participant) => {
        const birthDate = new Date(participant.dateOfBirth);
        const formattedDate = `${birthDate.getFullYear()}-${String(
          birthDate.getMonth() + 1,
        ).padStart(2, '0')}-${String(birthDate.getDate()).padStart(2, '0')} ${String(
          birthDate.getHours(),
        ).padStart(2, '0')}:${String(birthDate.getMinutes()).padStart(2, '0')}`;

        return {
          id: participant.enrollmentId,
          firstName: participant.firstName,
          lastName: participant.lastName,
          dateOfBirthday: formattedDate,
          email: participant.email,
          dateRegister: participant.registrationDate,
          userId: participant.userId,
        };
      }) || [];
  } else {
    data = allParticipantsData;
    isLoading = isAllParticipantsLoading;
    isError = isAllParticipantsError;
    error = allParticipantsError;

    rows =
      data?.participants?.map((participant) => {
        const birthDate = new Date(participant.dateOfBirth);
        const formattedDate = `${birthDate.getFullYear()}-${String(
          birthDate.getMonth() + 1,
        ).padStart(2, '0')}-${String(birthDate.getDate()).padStart(2, '0')} ${String(
          birthDate.getHours(),
        ).padStart(2, '0')}:${String(birthDate.getMinutes()).padStart(2, '0')}`;

        return {
          id: participant.id,
          firstName: participant.firstName,
          lastName: participant.lastName,
          dateOfBirthday: formattedDate,
          email: participant.email,
          dateRegister: 'N/A',
          userId: participant.userId,
        };
      }) || [];
  }

  const columns = [
    { field: 'firstName', headerName: 'Имя', width: 200 },
    { field: 'lastName', headerName: 'Фамилия', width: 200 },
    {
      field: 'dateOfBirthday',
      headerName: 'Дата рождения',
      width: 200,
    },
    {
      field: 'email',
      headerName: 'Email',
      width: 200,
    },
    {
      field: 'dateRegister',
      headerName: 'Дата регистрации',
      width: 200,
    },
    {
      field: 'actions',
      headerName: 'Действия',
      width: 300,
      renderCell: (params) => (
        <StyledWrapper>
          <Button
            variant="contained"
            color="error"
            size="small"
            onClick={() => handleDelete(params.row.id)}
            disabled={isDeletingEnrollment || isDeletingParticipant}
          >
            {allParticipantsData != undefined ? 'Удалить' : 'Отменить'}
          </Button>

          <Button
            variant="contained"
            color="error"
            size="small"
            onClick={() => handleToProfile(params.row.userId)}
          >
            Профиль
          </Button>
        </StyledWrapper>
      ),
    },
    {
      field: 'userId',
      headerName: 'Id',
      width: 0,
    },
  ];

  const paginationModel = { page: 0, pageSize: 5 };

  if (isLoading) {
    return <Loader />;
  }

  if (isError) {
    return <div>Error: {error?.message || 'Произошла ошибка при загрузке данных.'}</div>;
  }
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
      <Paper sx={{ height: '500px', width: '100%' }}>
        <DataGrid
          rows={rows}
          columns={columns}
          initialState={{ pagination: { paginationModel } }}
          pageSizeOptions={[5, 10]}
          sx={{ border: 0 }}
        />
      </Paper>
    </StyledWrapper>
  );
};
UserList.propTypes = {
  eventId: PropTypes.number,
};
const StyledWrapper = styled.div`
  display: flex;
  gap: 30px;

  .tb {
    width: 100%;
  }
`;
export default UserList;
