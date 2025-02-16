export interface EventDTO {
  id: number;
  name: string;
  description: string;
  date: string;
  location: string;
  category: string;
  maxParticipants: number;
  image: [] | null;
  status: string;
}
