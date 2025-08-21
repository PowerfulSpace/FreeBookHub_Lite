# FreeBookHub Lite

## О проекте
**FreeBookHub Lite** — это простой магазин книг, который будет постепенно расширяться и развиваться.

## Архитектура проекта
Проект состоит из нескольких сервисов, разделенных на четкие слои для удобства сопровождения и расширяемости.

### Backend
Каждый сервис в backend имеет 5 слоёв:

- **API** — Внешний слой, предоставляющий REST API.
- **Application** — Логика приложения: обработка команд, реализация бизнес-правил.
- **Domain** — Основные модели и доменная логика.
- **Infrastructure** — Работа с базой данных, внешними сервисами и техническими аспектами.
- **Common** —  Общие компоненты для сервиса.

#### Сервисы:
- **CartService** — Управление корзиной пользователя.
- **CatalogService** — Каталог книг, их свойства и поиск.
- **OrderService** — Оформление и управление заказами.
- **PaymentService** — Оплата заказов и управление транзакциями.
- **AuthService** — Аутентификация пользователей

### Frontend
- **Web** — Пользовательский интерфейс для взаимодействия с магазином.


## 🛠 Общий стек технологий

### 🌐 Backend
- **ASP.NET Core 9.0** — каркас всех сервисов
- **REST API** — взаимодействие между сервисами и с фронтендом
- **JWT Authentication** — авторизация и безопасность
- **CQRS (MediatR)** — разделение команд и запросов
- **Entity Framework Core (SQL Server)** — ORM и доступ к данным
- **FluentValidation** — валидация входных данных
- **Mapster** — преобразование моделей и DTO
- **Serilog** — централизованное логирование (консоль, файл, конфигурация)
- **Swagger (Swashbuckle)** — интерактивная документация API
- **DotNetEnv** — конфигурация через `.env` файлы
- **RabbitMQ** — асинхронное взаимодействие между сервисами (Order, Payment)
- **Redis (StackExchange.Redis)** — кэширование и быстрая работа с данными

### 🧪 Тестирование
- **xUnit** — модульные тесты
- **Moq** — создание mock-объектов
- **EFCore.InMemory** — тестовая база данных
- **Coverlet** — измерение покрытия кода

### 🖥 Frontend
- **ASP.NET Core Web (Vue 3 — планируется)** — пользовательский интерфейс для взаимодействия с магазином

### 📦 DevOps & Инфраструктура (планируется/частично используется)
- **Docker** — контейнеризация сервисов
- **Docker Compose** — оркестрация и локальный запуск
- **CI/CD (GitHub Actions)** — автоматизация сборки и тестов
- **Kubernetes** — планируется для оркестрации в будущем


## 🚀 Запуск проекта FreeBookHub Lite

### 1. Подготовка окружения

Клонируйте репозиторий:

`git clone https://github.com/your-username/FreeBookHub_Lite.git`  
`cd FreeBookHub_Lite`

Перейдите в папку `env` и создайте файлы окружения на основе примеров:

`cd env`  
`copy .env.mssql.example .env.mssql`  
`copy .env.authservice.example .env.authservice`  
`copy .env.cartservice.example .env.cartservice`  
`copy .env.catalogservice.example .env.catalogservice`  
`copy .env.orderservice.example .env.orderservice`  
`copy .env.paymentservice.example .env.paymentservice`

⚠️ Заполните реальные значения в файлах `.env.*` (например, пароли, JWT-секреты и строки подключения).

Убедитесь, что в корне проекта есть файлы:

- `docker-compose.yml`  
- `docker-compose.override.yml`

---

## 🛠 Подготовка сервисов для локальной разработки

Убедитесь, что в запускаемых проектах каждого сервиса (например, `PS.FreeBookHub_Lite.AuthService.API`) находятся файлы:

- `env.development`  
- `env.development.example`

### 📁 Создание файла окружения

В корне каждого сервиса создайте файл для локальной разработки:

`copy .env.development.example .env.development`

### ⚙️ Заполнение параметров

Откройте `.env.development` и укажите необходимые значения:

- строка подключения к локальной базе MSSQL  
- JWT-секреты  
- другие настройки, специфичные для сервиса  

### ❗ Важно

Эти файлы используются при запуске сервисов в режиме `ASPNETCORE_ENVIRONMENT=Development`  
и **не должны попадать в репозиторий**.

### 2. Запуск через Docker

Выполните команду в корне проекта:

`docker-compose up --build`

Это поднимет следующие сервисы:

- **MSSQL Server** (порт `1433`)  
- **RabbitMQ** (порт `5672`, панель управления: [http://localhost:15672](http://localhost:15672))  
- **Redis** (порт `6379`)  
- **AuthService** (порт `5000`)  
- **CartService** (порт `5001`)  
- **CatalogService** (порт `5002`)  
- **OrderService** (порт `5003`)  
- **PaymentService** (порт `5004`)  

---

### 3. Проверка работы

Swagger для сервисов будет доступен по адресам:

- [AuthService → http://localhost:5000/swagger](http://localhost:5000/swagger)  
- [CartService → http://localhost:5001/swagger](http://localhost:5001/swagger)  
- [CatalogService → http://localhost:5002/swagger](http://localhost:5002/swagger)  
- [OrderService → http://localhost:5003/swagger](http://localhost:5003/swagger)  
- [PaymentService → http://localhost:5004/swagger](http://localhost:5004/swagger)  

RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672)  
(логин/пароль по умолчанию: `guest/guest`)

---

### 4. Остановка сервисов

Чтобы остановить и удалить контейнеры:

`docker-compose down`

---
