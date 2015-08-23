(**
- title : Review F# 4.0 
- description : Новые возможности Fsharp 4.0
- author : Dmitriy Vlasov
- theme : night
- transition : default
*)

(*****
# F# 4.0
![FSharp](images/fsharp256.png)
*)

(*****
## Язык*)

(*****
### Не обязательное корневое пространство имен `Microsoft`
* При использовании или открытии модулей и пространств имен из `FSharp.Core` корневое пространство имен `Microsoft` теперь является необязательным.*)

(** 
#### F# 3.1 *)
open Microsoft.FSharp.Quotations

(**
#### F# 4.0 *)
open FSharp.Quotations

(*****
### Конструкторы в качестве функций первого класса
* Имена классов теперь можно использовать как значения функции первого класса, представляющие конструкторы для этого класса.*)

(*** hide ***)
open System

(**---
#### F# 3.1*)
let favoriteSites = 
  [ "tryfsharp.org"; "fsharpforfunandprofit.com"; "fsharp.org" ] 
  |> List.map (sprintf "http://%s") 
  |> List.map (fun s -> Uri(s)) 

[ ('a', 1); ('b', 2); ('c', 3) ] 
|> List.map ( fun (ch,n) -> String(ch,n) )

(**---
#### F# 4.0*)
let favoriteSites' = 
  [ "tryfsharp.org"; "fsharpforfunandprofit.com"; "fsharp.org" ] 
  |> List.map (sprintf "http://%s") 
  |> List.map Uri 
  
[ ('a', 1); ('b', 2); ('c', 3) ] 
|> List.map String 

(*****
### Унификация `mutable` и `ref`
* Синтаксис `mutable` теперь можно использовать везде, а записанные значения при необходимости автоматически преобразуются компилятором в значения на основе кучи `ref`. 
* Добавлено новое необязательное предупреждение, которое позволит разработчику получать уведомления о таком преобразовании.*)

(**---
#### F# 3.1*)
let sumSquares n = 
  let total = ref 0 
  {1..n} 
  |> Seq.iter (fun i -> total := !total + i*i) 
  !total

(**---
#### F# 4.0*)
let sumSquares' n = 
  let mutable total = 0 
  {1..n} 
  |> Seq.iter (fun i -> total <- total + i*i) 
  total 

(*****
### Статические параметры для методов поставщиков типов
* Отдельные методы, предоставленные поставщиками типов, теперь могут указывать статические параметры.*)

(*** hide ***)
#I @"..\packages\FSharp.Text.RegexProvider\lib\net40"

(**---
#### F# 4.0*)
#r "FSharp.Text.RegexProvider.dll"
open FSharp.Text.RegexProvider
type PhoneRegex = Regex< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{4}$)">
PhoneRegex().Match("904-601-9540").PhoneNumber.Value

(**
Тепень мы получили доступ к группам регулярного выражения. Вы можете посмотреть его с помощью Intellisense

![Example static provider](images/review-fsharp-4/example-static-provider.gif)*)

(*****
### Поставщики типов, не допускающие значения `null`
* Поставщики типов теперь могут быть указаны как не допускающие значения `null` с помощью стандартного атрибута `AllowNullLiteral(false)`.*)

(**---
#### F# 3.1*)

(**---
#### F# 4.0*)

(*****
### Неявное цитирование аргументов метода
* Аргументы метода типа Expr<'t> теперь могут быть прозрачно автоматически цитированы, передавая значение аргумента и выражения абстрактного синтаксического дерева, которое его создало.*)

(**---
#### F# 4.0*)
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

(*****
### Расширенная грамматика препроцессора
* Логические операторы ||, && и ! можно использовать с группировкой круглыми скобками в директиве препроцессора #if.*)

(**---
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

(**---
#### F# 4.0*)
#if TRACE || (DEBUG && !PRODUCTION)
printfn "x is %d" x
#endif

(*****
### Рациональная дробь в единицах измерения
* Единицы измерения теперь поддерживают рациональные дроби, которые иногда используются в естественных науках, например, электрике.*)

(**---
#### F# 4.0*)
open FSharp.Data.UnitSystems.SI.UnitSymbols 
 
[<Measure>] type cm 
[<Measure>] type Jones = cm Hz^(1/2) / W 

(**---
#### F# 4.0
* Тестируем результат в интерактивной консоли: *)

let testRational = 
  3.0<Jones> + sqrt (9.0<cm^2 Hz / W^2>)

(**
* В результате получим: 

```fsi
val testRational : float<Jones> = 6.0
```
*)

(*****
### Упрощенное использование единиц измерения в функциях в стиле `printf`
* Числовые значения в единицах измерения теперь без проблем работают с числовыми спецификаторами формата printf, не требуя приведения едениц измерения к числу.*)

(*** hide ***)
[<Measure>] type ft

(**---
#### F# 3.1*)
let howManyFoot (m : float<m>) = 
  printfn "%f футов в %f метрах" ( float (m * 3.28<ft/m>) ) (float m) 
 
(**---
#### F# 4.0*)
let howManyFoot' (m : float<m>) = 
  printfn "%f футов в %f метрах" (m * 3.28<ft/m>) m 

(*****
### Поддержка массивов .NET высокой размерности
* Массивы .NET ранга 5 или выше теперь могут быть использованы кодом F #.*)

(**---
#### F# 3.1*)

(**---
#### F# 4.0*)

(*****
### Свойства расширения в инициализаторах объектов
* Настраиваемые свойства расширения теперь можно задать в выражениях инициализатора объекта.*)

(**---
#### F# 3.1*)

(**---
#### F# 4.0*)
open System.Collections.Generic

type Dictionary<'TKey, 'TValue> with
  member this.Items 
    with set (items) = 
      this.Clear() 
      for key, value in items do this.Add(key, value) 
 
(**---
#### F# 4.0
* Инициализируем значение за один шаг.*)
let testDictionary = 
  Dictionary( capacity = 100,
    Items = [(1,"One"); (2,"Two"); (3,"Three")] )

(*****
### Наследование от нескольких экземпляров универсального интерфейса
* Классы, создаваемые на F#, теперь могут наследовать от классов, которые реализуют несколько экземпляров универсального интерфейса.*)

(**---
#### F# 4.0*)
type A( i : int) =  
  interface IComparable<int> with 
    member this.CompareTo(other) = i.CompareTo(other) 
 
/// Тип реализует оба интерфейса IComparable<int> и IComparable<string> 
type B( i : int) = 
  inherit A(i)
  interface IComparable<string> with 
    member this.CompareTo(other) = (string i).CompareTo(other) 

(*****
### Несколько свойств в атрибуте `StructuredFormatDisplay`
* Спецификатор форматирования `%A`, настроенный с помощью артибута `StructuredFormatDisplay`, теперь может включать несколько свойств.*)

(**---
#### F# 3.1*)

(**---
#### F# 4.0*)
[<StructuredFormatDisplay("{Name}, появился в FSharp версии {Version}")>] 
type Feature = { 
  Name    : string
  Version : float } 

(**---
#### F# 4.0
* Тестируем результат в интерактивной консоли: *)

(*** define-output:test-structured-format-display ***)
let feature = {Name = "Расширенный атрибут StructuredFormatDisplay"; Version = 4.0} 
printfn "%A" feature

(**
* В результате получим: *)
(*** include-output:test-structured-format-display ***)

(*****
## Библиотка языка FSharp.Core
*)

(*****
### Нормализация API модулей Array, List и Seq
* Набор функций обработки коллекций теперь будет согласованным в модулях Array, List и Seq (за исключением случаев, когда API-интерфейсы неприменимы к определенным типам).
* Реализации функций оптимизированы с учетом особенностей каждого типа: Array, List, Seq.
* См. подробнее: ["Regularizing and extending the List, Array and Seq modules"](https://github.com/fsharp/FSharpLangDesign/blob/master/FSharp-4.0/StaticMethodArgumentsDesignAndSpec.md)
*)

(**---
#### F# 3.1*)
["a";"b";"b";"c";"c";]
|> List.map (fun s -> s.ToUpper())
|> Seq.distinct
|> Seq.toList
|> List.iter (printfn "Буква: %s")

(**---
#### F# 4.0*)
["a";"b";"b";"c";"c";]
|> List.map (fun s -> s.ToUpper())
|> List.distinct
|> List.iter (printfn "Буква: %s")
 
(*****
### Оптимизированное структурное хэширование
* Была проведена значительная работа по улучшению производительности универсального хэша сравнения для типов-примитивов.
* Это позволило повысить производительность функций модулей Arrray, List, и Seq, таких как "distinct" и "groupBy".*)

(*****
### Оптимизированные неструктурные операторы сравнения
* Модуль `NonStructuralComparison` из пространства имен `FSharp.Core.Operators` теперь может открываться, заменяя операторы структурного сравнения F# по умолчанию на более эффективные неструктурные операторы. 
* Такая замена может обеспечить значительное повышение производительности при обработке типов с помощью пользовательских реализаций операторов, в частности типов значений.*)

(**---
#### F# 3.1*)
module Tortoise = 
  let run () = 
    let today = DateTime.Now 
    let tomorrow = today.AddDays(1.0) 
    let mutable result = true 
    for i in 1 .. 10000000 do 
      result <- today = tomorrow

(**---
#### F# 4.0*)
module Hare = 
  open NonStructuralComparison 
  let run () = 
    let today = DateTime.Now 
    let tomorrow = today.AddDays(1.0) 
    let mutable result = true 
    for i in 1 .. 10000000 do 
      result <- today = tomorrow

(**---
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

(*****
### Синтаксис срезов для списков F#
* Список F# теперь поддерживает синтаксис срезов для получения части списка.*)

let alphabet = ['а'..'я']
(*** define-output:test-list-slice ***)
printfn "Первые 3 буквы алфавита %A, а последние %A"
  (alphabet.[..2]) 
  (alphabet.[29..])

(** В результате получим *)
(*** include-output:test-list-slice ***)

(*****
### Async расширения для `System.Net.WebClient`
* Теперь доступны расширения `WebClient`: `AsyncDownloadFile` и `AsyncDownloadData`.
*)

(**---
#### F# 4.0*)

(*****
### Улучшенная трассировка стека Async
* Исключения, возникающие в асинхронных вычислениях F#, теперь содержат более понятные трассировки стека, упрощая диагностику проблем.*)

(**---
#### F# 3.1*)

(**---
#### F# 4.0*)


(*****
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
#### F# 4.0

* В первом случае мы получим `null`, а во втором `None`.*)
let nullEnvValue = Environment.GetEnvironmentVariable("GHOST_OF_VARIABLE")
let optionEnvValue = Option.ofObj nullEnvValue

(*****
### Активный шаблон цитирования для значений System.Decimal 
* Добавлен активный шаблон для сопоставления значений константного литерала System.Decimal при цитировании.*)

(**---
#### F# 4.0*)

(*****
## Интегрированная среда разработки
В инструментах Visual F# представлены новые возможности и функции и реализован ряд исправлений.*)

(*****
### Интегрированная проверка наличия обновлений 
* Проекты F# теперь правильно сообщают свой статус "обновления" при сборке в Visual Studio.*)

(*****
### Автодополнение в инициализаторах объектов и именованных параметрах

Теперь поддерживается автодополнение при нажатии сочетания клавиш **Ctrl+Space**:

* Дополнение свойств, задаваемых в выражениях инициализатора объекта;
* Дополнение именованных параметров в вызовах метода и конструктора.*)

(*****
### Отладка скриптов 
* Теперь возможна отладка скриптов F# непосредственно в отладчике Visual Studio.*)

(*****
### Метаданные сборки в шаблонах проектов 
* Все шаблоны проектов F# теперь имеют файл AssemblyInfo.fs, содержащий общие атрибуты метаданных уровня сборки.*)

(*****
### Исправления ошибок, связанных с поддержкой работы с каталогами 
* Средства Visual F# не поддерживают непосредственно каталоги в проектах.
* Был исправлен ряд системных ошибок проекта для улучшения поддержки каталогов, добавленной расширением Visual F# Power Tools. *)