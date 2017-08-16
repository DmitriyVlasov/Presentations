- title : Writing Custom Functions in Power Query
- description : Разработка пользовательских функций в Power Query
- author : Dmitriy Vlasov
- theme : night
- transition : default

## Разработка пользовательских функций в Power Query

[Дмитрий Власов](http://DmitriyVlasov.ru),

Ведущий ERP разработчик в Awara IT Solutions

Первая встреча [группы пользователей Microsoft Power BI в Санкт-Петербурге](https://www.pbiusergroup.com/communities/community-home?CommunityKey=f6f83caa-78fb-4a11-92fb-522457064f77)

23 августа 2017

---
### План презентации
1. Основные компоненты языка Power Query
1. Библиотека Power Query
1. Средства разработки
1. Ссылки и материалы — ссылки на использованные в презентации материалы и полезные сайты для старта изучения Power Query

---

## Основные компоненты языка Power Query
* Обзор
* Синтаксис
* Базовые типы данных
* Базовые функции

---

### Обзор

**Power Query** чистый _(pure)_, поддерживающий функции высшего порядка _(higer-order)_, динамически тимизированный _(dynamically typed)_, частично ленивый _(partially lazy)_ функциональный язык программирования.

--- 

### Обзор

#### Основные элементы

* **Выражение** _(Expression)_ это формула вычисления.  Выражения имеют имя.
* **Значение** _(Value)_  это результат вычисления выражения. Значение имеет тип.
* **Запрос** Вычисляет выражения и возвращает значение.

---

### Синтаксис

#### Комментарии

```m
/*Многострочные комментарии*/

// Однострочный комментарий
```

---
 
### Синтаксис

#### Простейшие выражения
```m
42 // Значение
```

```m
6 * 7 // Произведение двух чисел
```

```m
#date // простейшая функция создания даты
```

```m
#date(2017,08,23) // Значение даты
```


---

### Синтаксис

#### Выражение let

```m
let           // Обязательное ключевое слово.
    A = 1,    // Далее с новой строки одно
    B = 2,    // или несколько выражений
    C = A + B // разделенных запятыми.
in            // Обязательное ключевое слово.
  C           // Возвращаемый результат, вычисляется перед возвратом.
```

---

### Синтаксис

#### Имена выражений

```m
let 
  // Имя выражения только на английском языке без пробелов:
  NumberValue = 42,

  // Имя выражения в форме человеко-читаемого предложения 
  // на любом языке любой длины:
  #"Ответ на главный вопрос жизни, вселенной и всего такого" = NumberValue,

in
  #"Ответ на главный вопрос жизни, вселенной и всего такого"

```

---

### Синтаксис

#### Условное выражение

```m
let 
  #"Ответ на главный вопрос жизни, вселенной и всего такого" = 42,
  currentAnswer = 10,

  // Если текущее значение равно 42, то вернуть бесконечность иначе вернуть пусто:
  nextAnswer = 
      if currentAnswer = 42
    then #infinity // Бесконечность
    else null      // Пусто
in
  nextAnswer
```

---

### Синтаксис

#### Пример функции

```m
let 
  // Имя выражения только на английском языке без пробелов
  NumberValue = 42,

  // Имя выражения в форме человеко-читаемого предложения.
  #"Ответ на главный вопрос жизни, вселенной и всего такого" = NumberValue,

  // Функция
  getNextAnswer = (currentAnswer) => 
    if currentAnswer = #"Ответ на главный вопрос жизни, вселенной и всего такого"
    then #infinity // Бесконечность
    else #nan // Не число
  
in
  getNextAnswer

```

---

### Синтаксис

#### Условное выражение

```m
let 
  // Имя выражения только на английском языке без пробелов
  NumberValue = 42,

  // Имя выражения в форме человеко-читаемого предложения.
  #"Ответ на главный вопрос жизни, вселенной и всего такого" = NumberValue,

  // Функция
  getNextAnswer = (currentAnswer) => 
    if currentAnswer = #"Ответ на главный вопрос жизни, вселенной и всего такого"
    then #infinity // Бесконечность
    else #nan // Не число
  
in
  getNextAnswer

```

---

### Базовые типы данных

#### Примитивные типы данных

| Тип | Пример | |
| --- | --- | --- |
| logical | `true` | `false` |
| text | `"Hello"` | `"Привет"` |
| number | `-42` | `3.14` |

--- 

### Базовые типы данных

#### Типы даты времени

| Тип | Пример |
| --- | --- |
| time | `#time(21,35,00)` |
| date | `#date(2017,08,23)` |
| datetime | `#datetime(2017,08,23 21,35,00)` |
| datetimezone | `#datetimetzone(2017,08,23 21,35,00 09,00)` |
| duration | `#duration(0,1,30,0)` |

---

### Базовые типы данных

#### Структуры данных

| Тип | Пример |
| --- | --- |
| list | `{1, 2, 3}` |
| record | `[FirstName = "Олег", LastName = "Попов"]` |
| table | `#table( { "Группа", "Резус" }, { {1, "+"}, {2, "-"} } )` |

---

### Базовые типы данных

#### Особые типы данных

| Тип | Пример | Примечание |
| --- | --- | --- |
| null | `null` | Отсутствие значения. |
| any | | Любой тип данных |
| binary | `#binary("AQID")` | Двоичные данные |
| function | `(x) => x + 1` | Функциональный тип данных |
| type | `type { number }` | Тип данных описывающий тип ;-). |

---

### Метаданные

* `#sections` - Возвращает запись содержащая ключ имя запроса и значение возвращаемое запросом значение в текущем документе.
* `#shared` - Возвращает все имена находящиеся в текущем окружении.