# Zidium
Zidium - это open source мониторинг приложений и информационных систем.
Написан на .Net 5, работает на разных платформах.

## Требования
- Любая платформа, поддерживающая .Net 5. Подробнее можно узнать тут: https://docs.microsoft.com/en-us/dotnet/core/install
- .Net 5 runtime
- PostgreSql или MS Sql Server (в том числе Express) 

## Установка в виде приложений
Серверная чать Zidium состоит из трёх приложений:
- Web-сервис (DispatcherService)
- Личный кабинет (UserAccount)
- Агент для выполнения периодических действий (Agent)

Скачайте релиз и распакуйте его в папку

Создайте новую базу данных в вашей СУБД

### Разверните приложение DispatcherService
Задайте нужные настройки:
- Рядом с файлом appsettings.json создайте пустой файл appsettings.prod.json. Не редактируйте исходный файл appsettings.json! Он содержит примеры настроек и будет перезаписан в очередном обновлении.
- В файле appsettings.prod.json укажите провайдера и строку соединения с базой в разделе "database".
- В настройке "secretKey" укажите секретный ключ, который будут использовать приложения для доступа.
- В настройке "webSite" укажите внешнее имя, которое вы позже дадите web-приложению личного кабинета. Это нужно для правильного формирования ссылок в уведомлениях.

Должен получиться примерно такой файл appsettings.prod.json:
```
{
  "database": {
    "providerName": "Npgsql",
    "connectionString": "Host=localhost;Port=5432;Database=Zidium.Work;Username=postgres;Password=12345;"
  },
  "webSite": "https://localhost:60001",
  "secretKey": "*****"
}
```

Если вы используете IIS:
- Создайте в IIS новый сайт, назовите его, например, Zidium.Dispatcher и укажите физическую папку DispatcherService.
- Убедитесь, что будет создан новый пул приложений.
- В привязке укажите протокол https и свободный порт, например, 60000.
- Снимите галочку "запустить сайт сейчас" и нажмите ОК.
- Перейдите в список пулов приложений, для пула Zidium.Dispatcher поставьте в дополнительных настройках значение True для настройки "отключить перезапуск с перекрытием".
- Теперь запустите сайт в IIS.

Для других web-серверов действуйте согласно их руководству.

Для запуска в качестве отдельного приложения:
- Укажите нужный хост и порт в секции Kestrel файла appsettings.prod.json
- Запустите приложение командой `dotnet Zidium.DispatcherService.dll`

Откройте в браузере http://localhost:60000/test , должен отображаться текст SUCCESS.
При запуске будет обновлена модель базы данных.
Если нет ни одного пользователя, то будет создан пользователь с логином Admin и паролем 12345.

### Разверните личный кабинет
Задайте нужные настройки:
- Рядом с файлом appsettings.json создайте пустой файл appsettings.prod.json. Не редактируйте исходный файл appsettings.json! Он содержит примеры настроек и будет перезаписан в очередном обновлении.
- В файле appsettings.prod.json укажите провайдера и строку соединения с базой в разделе "database".
- В настройке "dispatcherUrl" укажите адрес сервиса диспетчера, который вы развернули ранее.
- В настройке "secretKey" укажите секретный ключ, который будут использовать приложения для доступа.

Должен получиться примерно такой файл appsettings.prod.json:
```
{
  "database": {
    "providerName": "Npgsql",
    "connectionString": "Host=localhost;Port=5432;Database=Zidium.Work;Username=postgres;Password=12345;"
  },
  "dispatcherUrl": "http://localhost:60000",
  "webSite": "https://localhost:60001",
  "secretKey": "*****"
}
```
Если вы используете IIS:
- Создайте в IIS новый сайт, назовите его, например, Zidium.UserAccount и укажите физическую папку UserAccount.
- Убедитесь, что будет создан новый пул приложений.
- В привязке укажите протокол https и свободный порт, например, 60001.
- Теперь запустите сайт в IIS.

Для других web-серверов действуйте согласно их руководству.

Для запуска в качестве отдельного приложения:
- Укажите нужный хост и порт в секции Kestrel файла appsettings.prod.json
- Запустите приложение командой `dotnet Zidium.DispatcherService.dll`

Откройте в браузере https://localhost:60001, должена открыться страница для входа в систему.
Если это первый запуск, то введите логин Admin и пароль 12345. Позже пароль можно поменять в управлении пользователями.

### Разверните приложение-агент
Задайте нужные настройки:
- Рядом с файлом appsettings.json создайте пустой файл appsettings.prod.json. Не редактируйте исходный файл appsettings.json! Он содержит примеры настроек и будет перезаписан в очередном обновлении.
- В файле appsettings.prod.json укажите провайдера и строку соединения с базой в разделе "database".
- В настройке "dispatcherUrl" укажите адрес сервиса диспетчера, который вы развернули ранее.
- В настройке "secretKey" укажите секретный ключ, который будут использовать приложения для доступа.
- Чтобы агент мог отправлять email'ы, в разделе smtp заполните параметры подключения к вашему smtp-серверу.
- Чтобы агент мог отправлять уведомления через Telegram, в настройке TelegramBotToken укажите токен вашего Telegram-бота.
- Чтобы агент мог отправлять уведомления через VKontakte, в настройке VKontakteAuthToken укажите токен вашего сообщества VKontakte (боты VKontakte отправляют сообщения только от имени сообщества).

Должен получиться примерно такой файл appsettings.prod.json:
```
{
  "database": {
    "providerName": "Npgsql",
    "connectionString": "Host=localhost;Port=5432;Database=Zidium.Work;Username=postgres;Password=12345;"
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

## Установка через Docker

Будет позже...

## Логирование

По умолчанию логи пишутся в папку %LocalApplicationData%\Zidium\Logs, а также в сам Zidium. Файлы настроены на автоудаление через 30 дней.
Вывод в консоль отключен из соображений производительности, но для отладки его можно включить в разделе NLog\rules.

## Что ещё полезного можно сделать
На боевом сервере мы рекомендуем использовать open source приложение Zidium Server Monitor (https://github.com/Zidium/ServerMonitor) для мониторинга состояния самого сервера.

Чтобы Zidium Server Monitor обращался к вашему сервису Api, а не к облачному, в файле настроек Zidium.config в разделе access в атрибуте url укажите укажите адрес приложения Dispatcher, например, https://localhost:60000.
