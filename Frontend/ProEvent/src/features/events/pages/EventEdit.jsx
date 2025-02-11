import { useParams } from 'react-router-dom';
import EventForm from './EventAdd';

const EventEdit = () => {
  const { id } = useParams(); // Получаем id из URL
console.log(id);
  return (
    <div>
      <EventForm eventId={id} /> {/* Передаем id в EventForm */}
    </div>
  );
};

export default EventEdit;