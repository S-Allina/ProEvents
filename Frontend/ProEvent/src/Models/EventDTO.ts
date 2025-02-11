export interface EventDTO {
    id: number;
    name: string;
    description: string;
    date: string;  // или Date, если API возвращает ISO строку
    location: string;
    category: string;
    maxParticipants: number;
    image: [] | null; // Или string | undefined, если image может отсутствовать
    status: string;
  }