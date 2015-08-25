(**
- title : Review F# 4.0 
- description : Новые возможности Fsharp 4.0
- author : Dmitriy Vlasov
- theme : night
- transition : default
*)

(*****
# Обзор F# 4.0
![FSharp](images/fsharp256.png)

[Дмитрий Власов](https://github.com/DmitriyVlasov)

Ведущий бизнес-аналитик АО "БТК групп"

[Saint-Peterburg .NET Community](https://plus.google.com/u/0/communities/115643868183582812348), 25 августа 2015
*)

(**---
### План презентации
1. **История разработки** четвертой версии F# в лицах
1. **Язык** — улучшения языка F# и среды исполнения
1. **Библиотека FSharp.Core** — улучшения основой библиотеки языка F#
1. **Интегрированная среда разработки** — новые функции Visual Studio 2015 и плагина "Visual F# Power Tools"
1. **Ссылки и материалы** — ссылки на использованые в презентации материалы и полезные сайты для старта изучения F#
*)

(*****
## История разработки
![FSharp Community photo](images/review-fsharp-4/contributor-team.png)
' Первая крупная версия выпущенная совместно с сообществом F # 4.0.
' Произошли многочисленные изменения в культуре разработке языка от определения спецификации до разработки библиотек и инструментов.

' Язык дизайна разрабатывается в настоящее время в открытую, совместночерез fslang.uservoice.com.
' Реализация языка смещается на полностью открытые технологии.
' Во главу угла ставиться кросплатформенность и поддержка различных редакторов.
' Все это делается с помощью ряда авторов, в том числе Microsoft, Microsoft Research, членов F # Software Foundation и многие другие.
*)

(*****
## Язык
* Конструкторы в качестве функций первого класса
* Унификация `mutable` и `ref`
* Статические параметры для методов поставщиков типов
* Неявное цитирование аргументов метода
* Расширенная грамматика препроцессора
* Рациональная дробь в единицах измерения
* Упрощенное использование единиц измерения в функциях в стиле `printf`
* Свойства расширения в инициализаторах объектов
* Наследование от нескольких экземпляров универсального интерфейса
* Несколько свойств в атрибуте `StructuredFormatDisplay`
* Поставщики типов, не допускающие значения `null`
* Поддержка массивов .NET высокой размерности.
* Не обязательное корневое пространство имен `Microsoft`
 *)

(**---
### Конструкторы в качестве функций первого класса
* Имена классов теперь можно использовать как значения функции первого класса, представляющие конструкторы для этого класса.*)

(*** hide ***)
open System

(**---
### Конструкторы в качестве функций первого класса
#### F# 3.1*)
let favoriteSites = 
  [ "tryfsharp.org"; "fsharpforfunandprofit.com"; "fsharp.org" ] 
  |> List.map (sprintf "http://%s") 
  |> List.map (fun s -> Uri(s)) 

[ ('a', 1); ('b', 2); ('c', 3) ] 
|> List.map ( fun (ch,n) -> String(ch,n) )

(**
#### F# 4.0*)
let favoriteSites' = 
  [ "tryfsharp.org"; "fsharpforfunandprofit.com"; "fsharp.org" ] 
  |> List.map (sprintf "http://%s") 
  |> List.map Uri 
  
[ ('a', 1); ('b', 2); ('c', 3) ] 
|> List.map String 

(**---
### Унификация `mutable` и `ref`
* Синтаксис `mutable` теперь можно использовать везде, а записанные значения при необходимости автоматически преобразуются компилятором в значения на основе кучи `ref`. 
* Добавлено новое необязательное предупреждение, которое позволит разработчику получать уведомления о таком преобразовании.*)

(**---
### Унификация `mutable` и `ref`

#### F# 3.1*)
let sumSquares n = 
  let total = ref 0 
  {1..n} 
  |> Seq.iter (fun i -> total := !total + i*i) 
  !total

(**
#### F# 4.0*)
let sumSquares' n = 
  let mutable total = 0 
  {1..n} 
  |> Seq.iter (fun i -> total <- total + i*i) 
  total 

(**---
### Статические параметры для методов поставщиков типов
* Отдельные методы, предоставленные поставщиками типов, теперь могут указывать статические параметры.*)

(*** hide ***)
#I @"..\packages\FSharp.Text.RegexProvider\lib\net40"

(**---
### Статические параметры для методов поставщиков типов*)
#r "FSharp.Text.RegexProvider.dll"
open FSharp.Text.RegexProvider
type PhoneRegex = Regex< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{4}$)">
PhoneRegex().Match("904-601-9540").PhoneNumber.Value

(**
Получив доступ к группам регулярного выражения, мы можем посмотреть его с помощью Intellisense:

![Example static provider](images/review-fsharp-4/example-static-provider.gif)*)

(**---
### Неявное цитирование аргументов метода
* Аргументы метода типа Expr<'t> теперь могут быть прозрачно автоматически цитированы, передавая значение аргумента и выражения абстрактного синтаксического дерева, которое его создало.*)

(**---
### Неявное цитирование аргументов метода*)
open FSharp.Quotations
open FSharp.Quotations.Patterns

type TestQuotation =  
 // Метод принимает в качестве аргумента цитату F# кода  
  static member EchoExpression([<ReflectedDefinition(true)>] x : Expr<_>) =  
    // Упроценный конвертор цитированного кода 
    let rec toCode = function
      | PropertyGet(_, v, _) -> v.Name 
      | Call(_, mthd, args) ->  
        let argStr = args |> List.map toCode |> String.concat "," 
        sprintf "%s.%s(%s)" mthd.DeclaringType.Name mthd.Name argStr  
    
    let (WithValue(value, _, expr)) = x 
    printfn "Выражение '%s' вычисляет '%O'" (toCode expr) value

(**---
### Неявное цитирование аргументов метода
#### F# 4.0
* Тестируем результат в интерактивной консоли: *)

(*** define-output:test-quotation ***)
let x, y = 42, 93
TestQuotation.EchoExpression(x)
TestQuotation.EchoExpression(y)
TestQuotation.EchoExpression(Math.Max(x,y))

(**
* В результате получим: *)
(*** include-output:test-quotation ***)

(**---
### Расширенная грамматика препроцессора
* Логические операторы ||, && и ! можно использовать с группировкой круглыми скобками в директиве препроцессора #if.*)

(**
#### F# 3.1*)
#if TRACE
#else
#if DEBUG
#if PRODUCTION
#else
printfn "x is %d" x
#endif
#endif
#endif

(**
#### F# 4.0*)
#if TRACE || (DEBUG && !PRODUCTION)
printfn "x is %d" x
#endif

(**---
### Рациональная дробь в единицах измерения
* Единицы измерения теперь поддерживают рациональные дроби, которые иногда используются в естественных науках, например, электрике.*)

open FSharp.Data.UnitSystems.SI.UnitSymbols 
 
[<Measure>] type cm 
[<Measure>] type Jones = cm Hz^(1/2) / W 

(**
* Тестируем результат в интерактивной консоли: *)

let testRational = 
  3.0<Jones> + sqrt (9.0<cm^2 Hz / W^2>)

(**
* В результате получим: 

```fsi
val testRational : float<Jones> = 6.0
```
*)

(**---
### Упрощенное использование единиц измерения в функциях в стиле `printf`
* Числовые значения в единицах измерения теперь без проблем работают с числовыми спецификаторами формата printf, не требуя приведения едениц измерения к числу.*)

(*** hide ***)
[<Measure>] type ft

(**
#### F# 3.1*)
let howManyFoot (m : float<m>) = 
  printfn "%f футов в %f метрах" (float (m * 3.28<ft/m>)) (float m) 
 
(**
#### F# 4.0*)
let howManyFoot' (m : float<m>) = 
  printfn "%f футов в %f метрах" (m * 3.28<ft/m>) m 

(**---
### Свойства расширения в инициализаторах объектов
* Настраиваемые свойства расширения теперь можно задать в выражениях инициализатора объекта.*)

open System.Collections.Generic

type Dictionary<'TKey, 'TValue> with
  member this.Items 
    with set (items) = 
      this.Clear() 
      for key, value in items do this.Add(key, value) 
 
(**
* Инициализируем значение за один шаг.*)
let testDictionary = 
  Dictionary( capacity = 100,
    Items = [(1,"One"); (2,"Two"); (3,"Three")] )

(**---
### Наследование от нескольких экземпляров универсального интерфейса
* Классы, создаваемые на F#, теперь могут наследовать от классов, которые реализуют несколько экземпляров универсального интерфейса.*)

type A( i : int) =  
  interface IComparable<int> with 
    member this.CompareTo(other) = i.CompareTo(other) 
 
/// Тип реализует оба интерфейса IComparable<int> и IComparable<string> 
type B( i : int) = 
  inherit A(i)
  interface IComparable<string> with 
    member this.CompareTo(other) = (string i).CompareTo(other) 

(**---
### Несколько свойств в атрибуте `StructuredFormatDisplay`
* Спецификатор форматирования `%A`, настроенный с помощью артибута `StructuredFormatDisplay`, теперь может включать несколько свойств.*)

[<StructuredFormatDisplay("{Name}, появился в FSharp версии {Version}")>] 
type Feature = { 
  Name    : string
  Version : float } 

(**
* Тестируем результат в интерактивной консоли: *)

(*** define-output:test-structured-format-display ***)
let feature = {Name = "Расширенный атрибут StructuredFormatDisplay"; Version = 4.0} 
printfn "%A" feature

(**
* В результате получим: *)
(*** include-output:test-structured-format-display ***)

(**---
### Прочие изменения в ядре языка F#
* **Поставщики типов, не допускающие значения `null`**
  * Поставщики типов теперь могут быть указаны как не допускающие значения `null` с помощью стандартного атрибута `AllowNullLiteral(false)`.
* **Поддержка массивов .NET высокой размерности.**
  * Массивы .NET ранга 5 или выше теперь могут быть использованы кодом F #.
* **Не обязательное корневое пространство имен `Microsoft`**
  * При использовании или открытии модулей и пространств имен из FSharp.Core корневое пространство имен "Microsoft" теперь является необязательным.  
  *)

(*****
## Библиотка FSharp.Core
* Нормализация API модулей Array, List и Seq
* Расширен перечень функций в модуле Option
* Синтаксис срезов для списков F#
* Оптимизированное структурное хэширование
* Оптимизированные неструктурные операторы сравнения
* Async расширения для `System.Net.WebClient`
* Улучшенная трассировка стека Async
*)

(**---
### Нормализация API модулей Array, List и Seq
* Набор функций обработки коллекций теперь будет согласованным в модулях Array, List и Seq (за исключением случаев, когда API-интерфейсы неприменимы к определенным типам).
* Реализации функций оптимизированы с учетом особенностей каждого типа: Array, List, Seq.
* См. подробнее: ["Regularizing and extending the List, Array and Seq modules"](https://github.com/fsharp/FSharpLangDesign/blob/master/FSharp-4.0/StaticMethodArgumentsDesignAndSpec.md)
*)

(**---
### Нормализация API модулей Array, List и Seq
#### F# 3.1*)
["a";"b";"b";"c";"c";]
|> List.map (fun s -> s.ToUpper())
|> Seq.distinct
|> Seq.toList
|> List.iter (printfn "Буква: %s")

(**
#### F# 4.0*)
["a";"b";"b";"c";"c";]
|> List.map (fun s -> s.ToUpper())
|> List.distinct
|> List.iter (printfn "Буква: %s")

(**---
### Расширен перечень функций в модуле Option
* В модуль `Option` были добавлены новые функции для преобразования объектов в `null` и из `null`, а также значений `System.Nullable`.
 
```
Option.toNullable: option:'T option -> Nullable<'T> 
Option.ofNullable: value:Nullable<'T> -> 'T option

Option.ofObj: value:'T -> 'T option  when 'T : null 
Option.toObj: value:'T option -> 'T when 'T : null 

Operators.tryUnbox: value:obj -> 'T option 

Operators.isNull: value:'T -> bool when 'T : null
```
* До F# 4.0 необходимо было подключать отдельную библиотеку, например [ExtCore](https://github.com/jack-pappas/ExtCore).
*)

(**---
### Расширен перечень функций в модуле Option

* Пример обработки не существующей переменой окружения:*)
let envKey = "GHOST_OF_VARIABLE"
let envValue = Environment.GetEnvironmentVariable(envKey)

(** Можем обрабатать `null`:*)
if envValue = null then
  printfn "Переменная '%s' окружения не существует" envKey
else 
  printfn "Переменная окружения '%s' имеет значение '%s'" envKey envValue

(** но, лучше сказать, что значение может быть, а может и нет:*)
match Option.ofObj envValue with
| None ->   printfn "Переменная '%s' окружения не существует" envKey
| Some v -> printfn "Переменная окружения '%s' имеет значение '%s'" envKey v

(**---
### Синтаксис срезов для списков F#
* Список F# теперь поддерживает синтаксис срезов для получения части списка.*)

let alphabet = ['а'..'я']
(*** define-output:test-list-slice ***)
printfn "Первые 3 буквы алфавита %A, а последние %A"
  (alphabet.[..2]) 
  (alphabet.[29..])

(** В результате получим *)
(*** include-output:test-list-slice ***)

(**---
### Оптимизированное структурное хэширование
* Была проведена значительная работа по улучшению производительности универсального хэша сравнения для типов-примитивов.
* Это позволило повысить производительность функций модулей Arrray, List, и Seq, таких как "distinct" и "groupBy".*)

(**---
### Оптимизированные неструктурные операторы сравнения
* Модуль `NonStructuralComparison` из пространства имен `FSharp.Core.Operators` теперь может открываться, заменяя операторы структурного сравнения F# по умолчанию на более эффективные неструктурные операторы. 
* Такая замена может обеспечить значительное повышение производительности при обработке типов с помощью пользовательских реализаций операторов, в частности типов значений.*)

(**---
### Оптимизированные неструктурные операторы сравнения
#### F# 3.1*)
module Tortoise = 
  let run () = 
    let today = DateTime.Now 
    let tomorrow = today.AddDays(1.0) 
    let mutable result = true 
    for i in 1 .. 10000000 do 
      result <- today = tomorrow

(**
#### F# 4.0*)
module Hare = 
  open NonStructuralComparison // <- F# 4.0
  let run () = 
    let today = DateTime.Now 
    let tomorrow = today.AddDays(1.0) 
    let mutable result = true 
    for i in 1 .. 10000000 do 
      result <- today = tomorrow

(**---
### Оптимизированные неструктурные операторы сравнения
#### F# 3.1 vs 4.0
*)

(*** hide ***)
#time
Tortoise.run()
#time
#time
Hare.run()
#time

(**
```
> Tortoise.run()
Real: 00:00:00.749, ЦП: 00:00:00.671, GC gen0: 305, gen1: 0, gen2: 0
val it : unit = ()

> Hare.run()
Real: 00:00:00.031, ЦП: 00:00:00.031, GC gen0: 0, gen1: 0, gen2: 0
val it : unit = ()
```
*)

(**---
### Прочие изменения в ядре языка F#

* **Async расширения для `System.Net.WebClient`**
  * Теперь доступны расширения `WebClient`: `AsyncDownloadFile` и `AsyncDownloadData`.
* **Улучшенная трассировка стека Async**
  * Исключения, возникающие в асинхронных вычислениях F#, теперь содержат более понятные трассировки стека, упрощая диагностику проблем.
* **Активный шаблон цитирования для значений System.Decimal** 
 * Добавлен активный шаблон для сопоставления значений константного литерала System.Decimal при цитировании.*)

(*****
## Интегрированная среда разработки
В инструментах Visual F# представлены новые возможности и функции и реализован ряд исправлений.

* **Интегрированная проверка наличия обновлений**
  * Проекты F# теперь правильно сообщают свой статус "обновления" при сборке в Visual Studio.
* **Автодополнение в инициализаторах объектов и именованных параметрах**
  * Теперь поддерживается автодополнение при нажатии сочетания клавиш **Ctrl+Space**:
  * Дополнение свойств, задаваемых в выражениях инициализатора объекта;
  * Дополнение именованных параметров в вызовах метода и конструктора.*)

(**---
## Интегрированная среда разработки
* **Отладка скриптов**
  * Теперь возможна отладка скриптов F# непосредственно в отладчике Visual Studio.
* **Метаданные сборки в шаблонах проектов**
  * Все шаблоны проектов F# теперь имеют файл AssemblyInfo.fs, содержащий общие атрибуты метаданных уровня сборки.
* **Исправления ошибок, связанных с поддержкой работы с каталогами** 
  * Средства Visual F# не поддерживают непосредственно каталоги в проектах.
  * Был исправлен ряд системных ошибок проекта для улучшения поддержки каталогов, добавленной расширением Visual F# Power Tools.*)

(*****
## Ссылки и материалы

* Использованные материалы
  * Основные
  * Обзоры F# 4.0 на английском языке
* Использованые инструменты
* Материалы для изучения F#

---
### Использованные материалы
#### Основные 

* [Спецификация Visual Fsharp 4.0 на Github](https://github.com/Microsoft/visualfsharp/wiki/F%23-4.0-Status) 
* [Замечания к выпуску Visual Studio 2015 глава Visual F# 4.0](https://www.visualstudio.com/ru-ru/news/vs2015-vs.aspx#fsharp) 
* [Release Notes Visual Studio 2015, chapter Visual F# 4.0](https://www.visualstudio.com/en-us/news/vs2015-vs#fsharp) 
* [Visual F# team "Announcing a preview of F# 4.0 and the Visual F# Tools in VS 2015"](http://blogs.msdn.com/b/fsharpteam/archive/2014/11/12/announcing-a-preview-of-f-4-0-and-the-visual-f-tools-in-vs-2015.aspx) 
* [Visual F# team "Rounding out Visual F# 4.0 in VS 2015 RC"](http://blogs.msdn.com/b/dotnet/archive/2015/04/29/rounding-out-visual-f-4-0-in-vs-2015-rc.aspx)  
* [Dot Net blog "Announcing the RTM of Visual F# 4.0"](http://blogs.msdn.com/b/dotnet/archive/2015/07/20/announcing-the-rtm-of-visual-f-4-0.aspx)
* [Библиотека FSharp.Text.RegexProvider](http://fsprojects.github.io/FSharp.Text.RegexProvider/)

#### Обзоры F# 4.0 на английском языке

* [Lincoln Atkinson "Six Quick Picks from Visual F# 4.0"](https://channel9.msdn.com/Events/Visual-Studio/Visual-Studio-2015-Final-Release-Event/Six-Quick-Picks-from-Visual-F-40)  
* [Kay Ewbank "F# 4.0 Signals A Culture Change"](http://www.i-programmer.info/news/89-net/8843-f-40-signals-a-culture-change.html) 
* [Jonathan Wood "Exciting New Things In F# 4.0"](http://www.wintellect.com/devcenter/jwood/exciting-new-things-in-f-4-0)

---
### Использованые инструменты

* Материалы собраны с помощью [Onenote](http://www.onenote.com/) 
* Веб-презентация оформлена маркдаун с помощью фреймворка [FsReveal](http://fsprojects.github.io/FsReveal/) 
* Библиотека литературного программирования и документирования [FSharp.Formatting](http://tpetricek.github.io/FSharp.Formatting/) 
* Скринкасты оформлены с помощью программы [Screen to Gif](https://screentogif.codeplex.com/)

---
### Материалы для изучения F#

1. Быстрый старт. Прекрасный ресурс для начального обзора F# http://www.tryfsharp.org). (Работает только на рабочей станции, требует установленный Microsoft Silverlight)
1. Лучшие матералы о практическом программировании на F# от основ до архитектуры приложений в функциональной парадигме от Скота Волшина(Scott Wlaschin) http://fsharpforfunandprofit.com
1. Плагин для Visual Studio [Visual FSharp Power Tools](http://fsprojects.github.io/VisualFSharpPowerTools)
1. Готовый каркас разработки приложений на базе F# [Project Scaffold](http://fsprojects.github.io/ProjectScaffold/)
1. Домашная страница проекта F# http://fsharp.org
1. Еженедельник сообщества F# от Сергея Тихона [F# Weekly](https://sergeytihon.wordpress.com/category/f-weekly/)
*)
