# Структура файла #

## Заголовок файла ##

Заголовок содержит информацию о файле истории. Оттуда можно узнать размер истории (непонятно, зачем), количество сообщений (непонятно, почему оно идёт два раза), UIN и никнейм человека. Все числовые значения хранятся в `Big Endian` формате. Кодировка - `UTF8`.
 
 Позиция | HEX | Размер | Описание 
------------- | ------------- | ------------- | -------------
0 | 0x00 | Char(3) | Подпись. Всегда "QHF".
4 | 0x04 | Int32 | Размер истории (в байтах).
34 | 0x22 | Int32 | Количество сообщений.
38 | 0x26 | Int32 | ???
44 | 0x2C | Int16 | Длина UIN.
46 | 0x2E | Char(N) | UIN. N = длина.
46 + N | 0x2E + N | Int16 | Длина ника.
48 + N | 0x30 + N | Char(N) | Никнейм. N = длина.

## Сообщения ##
После заголовка начинаются сообщения. Они идут подряд. Позиция отсчитывается после заголовка.

 Позиция | HEX | Размер | Описание
------------- | ------------- | ------------- | -------------
00 | 0x00 | Int16 | Подпись. Всегда равна 1.
02 | 0x02 | Int32 | Размер блока с сообщением.
06 | 0x06 | Int16 | Тип поля (?) ID сообщения.
08 | 0x08 | Int16 | Размер блока с ID сообщения.
10 | 0x0A | Int32 | ID сообщения. Просто порядковый номер.
14 | 0x0E | Int16 | Тип поля (?) даты отправки сообщения.
16 | 0x10 | Int16 | Размер поля с датой отправки сообщения.
18 | 0x12 | Int32 | Дата в UNIX-формате.
22 | 0x16 | Int16 | Тип поля ???
24 | 0x18 | Int16 | ???
26 | 0x1A | Byte | Является ли сообщение отправленным. Boolean.
27 | 0x1B | Int16 | Тип поля текста сообщения.
29 | 0x1D | Int16 | Размер блока, в котором хранится длина сообщения.
31 | 0x1F | Int32 | Длинна сообщения в байтах.
35 | 0x23 | Byte(N) | Текст сообщения, закодированный.

## Кодировка сообщения ##
Текст сообщений хранится в кодировке UTF8, как было сказано ранее. Однако байты закодировано, поэтому прежде чем перевести из буквы, требуется выполнить декодирование по следующей формуле:
```csharp
for (int i = 0; i < array.Length; i++)
    array[i] = (byte)(255 - array[i] - i - 1);
```

## Подробная справка по полям ##
Тип поля текста сообщений означает, что это за сообщение. Большинство из них - обычные текстовые сообщения, либо сообщения, отправленные, пока аккаунт был вне сети. Остальное - это служебные сообщения, такие как запрос авторизации, ответ на запрос авторизации и т.д.

Тип | Значение
------------- | -------------
01 | Сообщение, полученное пока аккаунт был в сети.
02 | Дата отправки сообщения.
03 | Отправитель сообщения.
05 | Запрос авторизации.
06 | Запрос на добавление в друзья.
13 | Сообщение, полученное пока аккаунт был вне сети.
14 | Запрос на авторизацию принят.