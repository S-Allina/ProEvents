
import { TextField, MenuItem} from '@mui/material';
import Button from '../../../Components/Button/Button'
import FormControl from '@mui/material/FormControl';

import PropTypes from 'prop-types';
import { useState } from 'react';
import { categories } from '../../../config/categories';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import styled from 'styled-components';
import dayjs from 'dayjs';
const StyledFormControl = styled.form`
display: flex
;
    width: 60%;
    gap: 10px;
    justify-content: space-around;
    margin: 0 0px 10px 20px;

`;
  const EventFilter = ({ onFilterChange }) => {
    const [location, setLocation] = useState('');
    const [category, setCategory] = useState('');
    const [startDate, setStartDate] = useState(dayjs());
    const [endDate, setEndDate] = useState(dayjs());
  
 
    const handleLocationChange = (event) => {
      setLocation(event.target.value);
  };

  const handleCategoryChange = (event) => {
      setCategory(event.target.value);
  };

  const handleStartDateChange = (date) => {
      setStartDate(date);
  };

  const handleEndDateChange = (date) => {
      setEndDate(date);
  };

  const createQueryString = () => {
      const params = new URLSearchParams();
      if (location) params.append('location', location);
      if (category) params.append('category', category);
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      return params.toString();
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const queryString = createQueryString();
    console.log("Query string:", queryString); // Добавьте эту строку
    onFilterChange(queryString);
  };
  const handleResetFilters = () => {
      setLocation('');
      setCategory('');
      setStartDate(null);
      setEndDate(null);
      onFilterChange(''); // Clear the filter in the parent component
  };

  return (
      <LocalizationProvider dateAdapter={AdapterDayjs}>
          <StyledFormControl onSubmit={handleSubmit}>
              <TextField fullWidth sx={{ m: 0, maxWidth: 180, minWidth: 170 }}
                  label="Место проведения"
                  variant="outlined"
                  value={location}
                  onChange={handleLocationChange}
              />
              <FormControl variant="outlined" fullWidth sx={{ m: 0, maxWidth: 120, minWidth:120 }}>
                  <TextField
                      select
                      label="Категория"
                      variant="outlined"
                      value={category}
                      onChange={handleCategoryChange}
                      placeholder='категории'
                  >
                      <MenuItem value=""><em>None</em></MenuItem>
                      {categories.map((option) => (
                          <MenuItem key={option.value} value={option.value}>
                              {option.label}
                          </MenuItem>
                      ))}
                  </TextField>
              </FormControl>
              <DateTimePicker fullWidth sx={{ m: 0, maxWidth: 200, minWidth:150 }}
                  label="С"
                  value={startDate}
                  onChange={handleStartDateChange}
                  renderInput={(params) => <TextField {...params} />}
              />
              <DateTimePicker fullWidth sx={{ m: 0, maxWidth: 200, minWidth:150 }}
                  label="По"
                  value={endDate}
                  onChange={handleEndDateChange}
                  minDate={startDate}
                  renderInput={(params) => <TextField {...params} />}
              />
              <Button variant="contained" color="primary" type="submit">
                  Фильтр
              </Button>
              <Button variant="contained" color="secondary" onClick={handleResetFilters}>
                  Отмена
              </Button>
          </StyledFormControl>
      </LocalizationProvider>
  );
};

  
  EventFilter.propTypes = {
    onFilterChange: PropTypes.func.isRequired,
  };
  
  export default EventFilter;