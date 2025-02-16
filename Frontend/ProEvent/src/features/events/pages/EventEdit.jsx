import { useParams } from 'react-router-dom';
import EventForm from './EventAdd';

const EventEdit = () => {
  const { id } = useParams();
  console.log(id);
  return (
    <div>
      <EventForm eventId={id} />
    </div>
  );
};

export default EventEdit;
