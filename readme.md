# Zidium
Zidium - это open source мониторинг приложений и информационных систем.
Написан на .Net 5, работает на разных платформах.

## Требования
- Любая платформа, поддерживающая .Net 5. Подробнее можно узнать тут: https://docs.microsoft.com/en-us/dotnet/core/install
- .Net 5 runtime
- Вместо .net можно использовать Docker
- PostgreSql или MS Sql Server (в том числе Express), также поддерживается встроенная база Sqlite

## Установка через Docker

Самый простой способ попробовать систему - запустить в Docker и указать встроенную базу Sqlite.
Важно - не рекомендуем использовать Sqlite в production!

### Запуск с Sqlite

docker cli:

`docker run -d --name zidium-sqlite -p 8080:80 -p 10000:10000 -e "ZIDIUM_webSite=http://localhost:8080" -e "ZIDIUM_secretKey=XXX" -v zidium-sqlite:/zidium/sqlite zidiumteam/zidium-simple:latest`

или docker-compose:

```
services:
  zidium:
    container_name: zidium-sqlite
    image: zidiumteam/zidium-simple:latest
    restart: always
    ports:
      - 8080:80
      - 10000:10000
    environment:
      - ZIDIUM_webSite=http://localhost:8080
      - ZIDIUM_secretKey=XXX
    volumes:
      - zidium-sqlite:/zidium/sqlite
```

Команда сделает следующее:
- Запустит личный кабинет на порту 8080
- Запустит api на порту 10000
- Установит внутренний секретный ключ XXX (обязательно поменяйте его в команде)
- Данные Sqlite будут вынесены в том zidium-sqlite, усправляемый Docker
- Создаст пользователя с логином Admin и паролем 12345. Обязательно смените пароль в личном кабинете!

### Проверьте, как всё работает
Откройте в браузере http://localhost:10000/test , должен отображаться текст SUCCESS.
Откройте в браузере личный кабинет http://localhost:8080 и зайдите с вашим логином и паролем.
Перейдите в раздел "Компоненты", вы увидите дерево компонентов, все должны быть зелёными.

### Запуск с другой СУБД и с расширенными настройками

В production мы рекомендуем использовать СУБД Postgres или MsSql.
Также вам потребуется более тонкая настройка системы.

Создайте файл zidium.appsettings.json. Для примера будем считать, что его полный путь /path/zidium.appsettings.json.

```
{
  "database": {
    "providerName": "Npgsql",
    "connectionString": "Host=host.docker.internal;Port=5432;Database=Zidium.Work;Username=postgres;Password=12345;ApplicationName=Zidium"
  },
  "webSite": "http://localhost:8080",
  "secretKey": "XXX"
}
```

- Настройку под конкретную СУБД смотрите ниже в разделе "Выбор СУБД".
- Чтобы агент мог отправлять email'ы, в разделе smtp заполните параметры подключения к вашему smtp-серверу.
- Чтобы агент мог отправлять уведомления через Telegram, в настройке TelegramBotToken укажите токен вашего Telegram-бота.
- Чтобы агент мог отправлять уведомления через VKontakte, в настройке VKontakteAuthToken укажите токен вашего сообщества VKontakte (боты VKontakte отправляют сообщения только от имени сообщества).

Теперь запустите саму систему.

docker cli:

`docker run -d --name zidium -p 8080:80 -p 10000:10000 --mount type=bind,source=/path/zidium.appsettings.json,target=/zidium/zidium.appsettings.json zidiumteam/zidium-simple:latest`

или docker-compose:

```
services:
  zidium:
    container_name: zidium
    image: zidiumteam/zidium-simple:latest
    restart: always
    ports:
      - 8080:80
      - 10000:10000
    volumes:
      - type: bind
        source: /path/zidium.appsettings.json
        target: /zidium/zidium.appsettings.json
```

Команда сделает следующее:
- Запустит личный кабинет на порту 8080
- Запустит api на порту 10000
- Возьмёт настройки из файла /path/zidium.appsettings.json
- Установит внутренний секретный ключ XXX (обязательно поменяйте его в конфигурационном файле)
- Создаст пользователя с логином Admin и паролем 12345. Обязательно смените пароль в личном кабинете!

### Проверьте, как всё работает
Откройте в браузере http://localhost:10000/test , должен отображаться текст SUCCESS.
Откройте в браузере личный кабинет http://localhost:8080 и зайдите с вашим логином и паролем.
Перейдите в раздел "Компоненты", вы увидите дерево компонентов, все должны быть зелёными.

### Сборка для других архитектур

По техническим причинам мы делаем образы только для архитектуры linux/amd64.
Если вам нужна сборка для архитектур arm64, armv8 и т.п., то выполните её самостоятельно:
```
git clone https://github.com/Zidium/ZidiumServer.git .
git pull
docker build -t local/zidium-simple:latest .
```

## Установка в виде приложений
Zidium состоит из трёх приложений:
- Web-сервис (DispatcherService)
- Личный кабинет (UserAccount)
- Агент для выполнения периодических действий (Agent)

Для упрощения в примерах используется встроенная база Sqlite.
Важно - не рекомендуем использовать Sqlite в production!
Настройку под конкретную СУБД смотрите ниже в разделе "Выбор СУБД".

### Скачайте релиз и распакуйте его в папку

Последний релиз всегда тут: https://github.com/Zidium/ZidiumServer/releases

### Разверните приложение DispatcherService
Задайте нужные настройки:
- Рядом с файлом appsettings.json создайте пустой файл appsettings.prod.json. Не редактируйте исходный файл appsettings.json! Он содержит примеры настроек и будет перезаписан в очередном обновлении.
- В файле appsettings.prod.json укажите провайдера и строку соединения с базой в разделе "database".
- В настройке "secretKey" укажите внутренний секретный ключ, который будут использовать сервисы для связи друг с другом.
- В настройке "webSite" укажите внешнее имя, которое вы позже дадите web-приложению личного кабинета. Это нужно для правильного формирования ссылок в уведомлениях.

Должен получиться примерно такой файл appsettings.prod.json:
```
{
  "database": {
    "providerName": "Sqlite",
    "connectionString": "Data Source=%localappdata%\\Zidium\\Sqlite\\Zidium.Work.db"
  },
  "webSite": "http://localhost:60001",
  "secretKey": "*****"
}
```

Если вы используете IIS:
- Создайте в IIS новый сайт, назовите его, например, Zidium.Dispatcher и укажите физическую папку DispatcherService.
- Убедитесь, что будет создан новый пул приложений.
- В привязке укажите протокол http и свободный порт, например, 60000.
- Снимите галочку "запустить сайт сейчас" и нажмите ОК.
- Перейдите в список пулов приложений, для пула Zidium.Dispatcher поставьте в дополнительных настройках значение True для настройки "отключить перезапуск с перекрытием".
- Теперь запустите сайт в IIS.

Для других web-серверов действуйте согласно их руководству.

Для запуска в качестве отдельного приложения:
- Укажите нужный хост и порт в секции Kestrel файла appsettings.prod.json
- Запустите приложение командой `dotnet Zidium.DispatcherService.dll`

Откройте в браузере http://localhost:60000/test , должен отображаться текст SUCCESS.
При запуске будет обновлена модель базы данных.
Если нет ни одного пользователя, то будет создан пользователь с логином Admin и паролем 12345. Обязательно смените пароль в личном кабинете!

### Разверните личный кабинет
Задайте нужные настройки:
- Рядом с файлом appsettings.json создайте пустой файл appsettings.prod.json. Не редактируйте исходный файл appsettings.json! Он содержит примеры настроек и будет перезаписан в очередном обновлении.
- В файле appsettings.prod.json укажите провайдера и строку соединения с базой в разделе "database".
- В настройке "dispatcherUrl" укажите адрес сервиса диспетчера, который вы развернули ранее.
- В настройке "secretKey" укажите внутренний секретный ключ, который вы указали ранее для диспетчера.

Должен получиться примерно такой файл appsettings.prod.json:
```
{
  "database": {
    "providerName": "Sqlite",
    "connectionString": "Data Source=%localappdata%\\Zidium\\Sqlite\\Zidium.Work.db"
  },
  "dispatcherUrl": "http://localhost:60000",
  "secretKey": "*****"
}
```
Если вы используете IIS:
- Создайте в IIS новый сайт, назовите его, например, Zidium.UserAccount и укажите физическую папку UserAccount.
- Убедитесь, что будет создан новый пул приложений.
- В привязке укажите протокол http и свободный порт, например, 60001.
- Теперь запустите сайт в IIS.

Для других web-серверов действуйте согласно их руководству.

Для запуска в качестве отдельного приложения:
- Укажите нужный хост и порт в секции Kestrel файла appsettings.prod.json
- Запустите приложение командой `dotnet Zidium.UserAccount.dll`

Откройте в браузере http://localhost:60001, должена открыться страница для входа в систему.
Если это первый запуск, то введите логин Admin и пароль 12345. Обязательно смените пароль в личном кабинете!

### Разверните приложение-агент

Для Windows рекомендуем использовать агент из папки Agent.WindowsService.
Для других ОС - из папки Agent.Console.

Задайте нужные настройки:
- Рядом с файлом appsettings.json создайте пустой файл appsettings.prod.json. Не редактируйте исходный файл appsettings.json! Он содержит примеры настроек и будет перезаписан в очередном обновлении.
- В файле appsettings.prod.json укажите провайдера и строку соединения с базой в разделе "database".
- В настройке "dispatcherUrl" укажите адрес сервиса диспетчера, который вы развернули ранее.
- В настройке "secretKey" укажите внутренний секретный ключ, который вы указали ранее для диспетчера.
- Чтобы агент мог отправлять email'ы, в разделе smtp заполните параметры подключения к вашему smtp-серверу.
- Чтобы агент мог отправлять уведомления через Telegram, в настройке TelegramBotToken укажите токен вашего Telegram-бота.
- Чтобы агент мог отправлять уведомления через VKontakte, в настройке VKontakteAuthToken укажите токен вашего сообщества VKontakte (боты VKontakte отправляют сообщения только от имени сообщества).

Должен получиться примерно такой файл appsettings.prod.json:
```
{
  "database": {
    "providerName": "Sqlite",
    "connectionString": "Data Source=%localappdata%\\Zidium\\Sqlite\\Zidium.Work.db"
  },
  "dispatcherUrl": "http://localhost:60000",
  "secretKey": "*****"
}
```

Для запуска агента в режиме windows-службы:
- Используйте приложение из папки Agent.WindowsService
- Выполните `Zidium.Agent.exe -install`

Для запуска агента в режиме консольного приложения:
- Используйте приложение из папки Agent.Console
- Выполните `dotnet Zidium.Agent.dll`

### Проверьте, как всё работает
Откройте в браузере личный кабинет и зайдите с вашим логином и паролем.

Перейдите в раздел "Компоненты", вы увидите дерево компонентов, все должны быть зелёными.

## Выбор СУБД

Работа с разными СУБД отличается только указанием нужного провайдера и строки соединения в конфигурационных файлах.

Самый простой способ для начала работы - встроенная база Sqlite.
Пример настроек для Sqlite:
```
"providerName": "Sqlite",
"connectionString": "Data Source=%localappdata%\\Zidium\\Sqlite\\Zidium.Work.db"
```

Мы не рекомендуем использовать Sqlite для production, так как она не обеспечивает нужной производительности и надёжности.
Применяйте только для знакомства с системой!

Пример настроек для Postgres:
```
"providerName": "Npgsql",
"connectionString": "Host=localhost;Port=5432;Database=Zidium.Work;Username=postgres;Password=12345;ApplicationName=Zidium.Dispatcher"
```

Пример настроек для MsSql:
```
"providerName": "MsSql",
"connectionString": "Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Zidium.Work;Application Name=Zidium.Dispatcher"
```

## Логирование

По умолчанию логи пишутся в папку %localappdata%\Zidium\Logs, а также в сам Zidium.
Файлы настроены на автоудаление через 30 дней.

Вывод в консоль отключен из соображений производительности, но для отладки его можно включить в разделе NLog\rules.

## Что ещё полезного можно сделать
На боевом сервере мы рекомендуем использовать open source приложение Zidium Server Monitor (https://github.com/Zidium/ServerMonitor) для мониторинга состояния самого сервера.

Чтобы Zidium Server Monitor обращался к вашему сервису Api: 
- В файле настроек Zidium.config в разделе access в атрибуте url укажите укажите адрес приложения Dispatcher, например, http://localhost:60000.
- В личном кабинете в разделе "Ключи доступа к Api" добавьте новый ключ доступа.
- В файле настроек Zidium.config в разделе access в атрибуте secretKey укажите ключ доступа.
