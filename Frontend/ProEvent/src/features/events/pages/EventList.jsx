import styled from 'styled-components';
import { useGetEventByNameQuery, useGetEventsWithPaginationQuery,useGetFilteredEventsQuery } from '../../../App/services/eventApi';
import EventCard from '../components/EventCard';
import EventFilter from '../components/EventFilters';
import { useEffect, useState } from 'react';
import Loader from '../../../Components/Loader/Loader';
import { Box, Pagination, TextField } from '@mui/material';
import { useDispatch, useSelector } from 'react-redux';
import { setPageNumber } from '../../../App/slices/eventSlice';
import { Search } from '@mui/icons-material';
// import { setError, setLoading } from '../../../App/slices/eventSlice';

const EventList = () => {
  const [filters, setFilters] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const dispatch = useDispatch();
  const pageNumber = useSelector((state) => state.event.pageNumber);
  const pageSize = useSelector((state) => state.event.pageSize);
  const {
    data:paginatedEventsData,
    isLoading: paginatedEventsIsLoading,
  } = useGetEventsWithPaginationQuery({ pageNumber, pageSize });
  const usePagination = Object.values(filters).every(v => !v);

  // Удаляем skip: usePagination из хука, потому что фильтры теперь объект
  const {
    data:filteredEventsData,
    isLoading: filteredEventsIsLoading,
    refetch:refetchFilters,
  } = useGetFilteredEventsQuery(filters, { skip: !!searchTerm },);
  const { data:searchResults, refetch:refetchSearch } = useGetEventByNameQuery(searchTerm, { skip: !searchTerm }); // Пропускаем, если нет поискового запроса

  const handleFilterChange = (newFilters) => {
    setFilters(newFilters);
  };
  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  useEffect(() => {
    // Refetch for filters
    if (!searchTerm) {
        refetchFilters();
    }
  }, [filters, refetchFilters, searchTerm]);
  const eventsToDisplay = searchTerm
  ? searchResults?.result
  : usePagination
    ? paginatedEventsData?.events
    : filteredEventsData?.result;
  const handleChangePage = (event, value) => {
    dispatch(setPageNumber(value));
  };

  useEffect(() => {
  }, [pageNumber, pageSize]);







  useEffect(() => {
    // Refetch for search
    if (searchTerm) {
        refetchSearch();
    }
  }, [searchTerm, refetchSearch]);

if(paginatedEventsIsLoading || filteredEventsIsLoading) {
  return <Loader />;
}
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
      {eventsToDisplay && Array.isArray(eventsToDisplay) ?
        eventsToDisplay.map((event) => (
          <EventCard key={event.id} event={event} />
        )) : <h2>{'К сожалению ничего не найдено:('}</h2>}
    </div>

    {/* Pagination component */}
    {usePagination && paginatedEventsData?.totalCount && (
        <Pagination
        count={Math.ceil(paginatedEventsData?.totalCount / pageSize)}
        page={pageNumber}
        onChange={handleChangePage}
        color="secondary"
         variant="outlined"
      />
    )}
  </StyledWrapper>
);
}

    
const StyledWrapper = styled.div`
display:flex;
flex-direction:column;
align-items:center;
  .container {
    display: flex;
    flex-wrap: wrap;
    gap: 50px;
    justify-content: left;
    margin-bottom:20px;
  }
    .FitersAndSearch{
    display: flex;
    justify-content: space-between;
    }
`;

export default EventList;