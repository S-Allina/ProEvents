FROM node:18 AS build-env
WORKDIR /app

# Копируем package.json и package-lock.json (или yarn.lock)
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем весь код приложения
COPY . ./

# Собираем приложение для production
RUN npm run build

# Создаем Nginx образ для хостинга статики
FROM nginx:alpine
COPY --from=build-env /app/dist /usr/share/nginx/html   
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]