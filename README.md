# Overview
A telegram bot that let you communicate with the SMS-intranet site and fetch information about employees, such as email, phone number, and current location among the offices.

# Supported requests
Бот представляет следующие команды для взаимодействия с интранет сайтом:

* Команда 'подписки' показывает список текущих подписок
* Команда 'где' показывает местоположение пользователя из СКУД, например 'где иванов'
* Команда 'почта' показывает почтовый адрес сотрудника с сайта интранет, например 'почта иванов'
* Команда 'телефон' показывает номера телефонов сотрудника с сайта интранет, например 'телефон иванов'
* Команда 'отписаться' удаляет подписку по порядковому номеру, например 'отписаться 3'. Чтобы узнать номера подисок выполните команду 'подписки'.
* Команда 'подписаться' оформляет подписку на изменение указанной команды, например 'подписаться где иванов'

# Configuration
Настройки хранятся в файле settings.json
```
{
  "BotApiKey": "...",         // telegram bot api key
  "Secret": "...",            // secret key, is required to authorize a new user
  "IntranetUserName": "...",  // intranet user name
  "IntranetPassword": "...",  // intranet user password
  "IntranetDomain": "...",    // domain name
  "Proxy": "127.0.0.1:8080"   // proxy server address, optional 
}

```

# Design and extensions
* Для добавления новой команды создайте реализацию класса IBotCommand, команда будет автоматически подгружена
* Для периодических задач, например, 'подписаться где иванов', используется класс BotTaskProcessor, который отслеживает список задач и выполняет их по расписанию. Список задач хранится в файле tasks.json 
