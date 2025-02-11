# ProEvents

## Описание

Этот проект представляет собой приложение для планирования мероприятий, написанное на asp.net Core + React/Redux toolkit, развернутого с использованием Docker Compose.

## Требования

*   [Docker](https://www.docker.com/) должен быть установлен на вашем компьютере.
*   [Docker Compose](https://docs.docker.com/compose/install/) должен быть установлен на вашем компьютере.
*   [Git](https://git-scm.com/)

## Инструкции по запуску

1.  Клонируйте репозиторий:

    ```bash
    git clone (https://github.com/S-Allina/ProEvents.git)
    ```

2.  Перейдите в директорию проекта:

    ```bash
    cd ProEvent
    ```

3.  Запустите приложение с помощью Docker Compose:

    ```bash
    docker-compose up -d
    ```

4.  (Необязательно) Проверьте логи:

    ```bash
    docker-compose logs webapi
    docker-compose logs mssql-server
    docker-compose logs frontend
    ```

5.  Приложение будет доступно по следующему адресу:

    * `http://localhost:3000`

## Конфигурация

Файл `docker-compose.yml` содержит конфигурацию для всех сервисов:

*   `webapi`: Web API на .NET.
*   `mssql-server`: SQL Server база данных.
*   `frontend`: Frontend приложение.

## Тестовые аккаунты
* Для аккаунта администратора:
  Логин: Admin
  Пароль: Admin_11
* Для аккаунта пользователя:
  Логин: User1
  Пароль: User_111
