import styled from 'styled-components';
import { useGetEventsWithPaginationQuery } from '../../../App/services/eventApi';
import EventCard from '../components/EventCard';
import EventFilter from '../components/EventFilters';
import { useEffect, useState } from 'react';
import Loader from '../../../Components/Loader/Loader';
import { Box, Pagination, TextField } from '@mui/material';
import { useDispatch, useSelector } from 'react-redux';
import { setPageNumber } from '../../../App/slices/eventSlice';
import { Search } from '@mui/icons-material';

const EventList = () => {
  const [filters, setFilters] = useState({});
  const [searchTerm, setSearchTerm] = useState('');
  const dispatch = useDispatch();
  const pageNumber = useSelector((state) => state.event.pageNumber);
  const pageSize = useSelector((state) => state.event.pageSize);

  const formatDateToISO = (date) => {
    return date ? new Date(date).toISOString() : undefined;
  };

  const {
    data: paginatedEventsData,
    isLoading: paginatedEventsIsLoading,
    isFetching: paginatedEventsIsFetching,
    refetch: refetchPaginatedEvents,
  } = useGetEventsWithPaginationQuery(
    {
      pageNumber,
      pageSize,
      startDate: formatDateToISO(filters.startDate),
      endDate: formatDateToISO(filters.endDate),
      location: filters.location,
      category: filters.category,
      name: searchTerm,
    },
    {
      skip: false,
    },
  );

  const handleFilterChange = (newFilters) => {
    setFilters(newFilters);
  };

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handleChangePage = (event, value) => {
    dispatch(setPageNumber(value));
  };

  useEffect(() => {
    refetchPaginatedEvents();
  }, [
    pageNumber,
    pageSize,
    filters.startDate,
    filters.endDate,
    filters.location,
    filters.category,
    searchTerm,
    refetchPaginatedEvents,
  ]);

  const isLoading = paginatedEventsIsLoading || paginatedEventsIsFetching;

  if (isLoading) {
    return <Loader />;
  }

  const eventsToDisplay = paginatedEventsData?.events;
  const totalCount = paginatedEventsData?.totalCount || 0;

  return (
    <StyledWrapper>
      <div className="FitersAndSearch">
        <EventFilter onFilterChange={handleFilterChange} />
        <Box sx={{ display: 'flex', alignItems: 'flex-end', marginBottom: 2 }}>
          <Search sx={{ color: 'action.active', mr: 1, my: 0.5 }} />
          <TextField
            id="search-input"
            label="Search by name"
            variant="standard"
            value={searchTerm}
            onChange={handleSearchChange}
          />
        </Box>
      </div>
      <div className="container">
        {eventsToDisplay && Array.isArray(eventsToDisplay) ? (
          eventsToDisplay.map((event) => <EventCard key={event.id} event={event} />)
        ) : (
          <h2>{'К сожалению ничего не найдено:('}</h2>
        )}
      </div>

      {totalCount > 0 && (
        <Pagination
          count={Math.ceil(totalCount / pageSize)}
          page={pageNumber}
          onChange={handleChangePage}
          color="secondary"
          variant="outlined"
        />
      )}
    </StyledWrapper>
  );
};

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  .container {
    display: flex;
    flex-wrap: wrap;
    gap: 50px;
    justify-content: left;
    margin-bottom: 20px;
  }
  .FitersAndSearch {
    display: flex;
    justify-content: space-between;
  }
`;

export default EventList;
