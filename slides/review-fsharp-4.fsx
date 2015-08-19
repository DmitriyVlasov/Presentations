(**
- title : Review F# 4.0 
- description : Новые возможности Fsharp 4.0
- author : Dmitriy Vlasov
- theme : Sky
- transition : default
*)

(**
***
## Новые возможности Fsharp 4.0

***
### Возможности языка и среды выполнения

---
### Конструкторы в качестве функций первого класса

#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Имена классов теперь можно использовать как значения функции первого класса, представляющие конструкторы для этого класса

---
### Унификация `mutable` и `ref`
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Синтаксис "mutable" теперь можно использовать везде, а записанные значения при необходимости автоматически преобразуются компилятором в значения на основе кучи "ref". 
' Добавлено новое необязательное предупреждение, которое позволит разработчику получать уведомления о таком преобразовании.

---
### Статические параметры для методов поставщиков типов
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Отдельные методы, предоставленные поставщиками типов, теперь могут указывать статические параметры.

---
### Поставщики типов, не допускающие значения NULL
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Поставщики типов теперь могут быть указаны как не допускающие значения NULL с помощью стандартного атрибута [<AllowNullLiteral(false)>].

---
### Неявное цитирование аргументов метода
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Аргументы метода типа Expr<'t> теперь могут быть прозрачно автоматически цитированы, передавая значение аргумента и выражения абстрактного синтаксического дерева, которое его создало.

---
### Расширенная грамматика препроцессора

#### F# 3.1 

```
#if TRACE
#else
#if DEBUG
#if PRODUCTION
#else
printfn "x is %d" x
#endif
#endif
#endif
```

#### F# 4.0

```
#if TRACE || (DEBUG && !PRODUCTION)
printfn "x is %d" x
#endif
```

' Логические операторы ||, && и ! можно использовать с группировкой круглыми скобками в директиве препроцессора #if.

---
### Рациональная дробь в единицах измерения
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Единицы измерения теперь поддерживают рациональные дроби, которые иногда используются в естественных науках, например, электрике.

---
### Упрощенное использование единиц измерения в функциях в стиле `printf`
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Числовые значения в единицах измерения теперь без проблем работают с числовыми спецификаторами формата printf, не требуя приведения едениц измерения к числу.

---
### Поддержка массивов .NET высокой размерности
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Массивы .NET ранга 5 или выше теперь могут быть использованы кодом F #.

---
### Свойства расширения в инициализаторах объектов
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Настраиваемые свойства расширения теперь можно задать в выражениях инициализатора объекта.

---
### Наследование от нескольких экземпляров универсального интерфейса
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```

' Классы, создаваемые на F#, теперь могут наследовать от классов, которые реализуют несколько экземпляров универсального интерфейса.

---
### Несколько свойств в атрибуте `[<StructuredFormatDisplay>]`
#### F# 3.1
```fsharp
// ...
```

#### F# 4.0
```fsharp
// ...
```
' Спецификатор форматирование %A, заданное через [<StructuredFormatDisplay>], теперь может включать несколько свойств.

---
### Не обязательное корневое пространство имен "Microsoft"
#### F# 3.1
```fsharp
open Microsoft.FSharp.Quotations
```

#### F# 4.0
```fsharp
open FSharp.Quotations
```
' При использовании или открытии модулей и пространств имен из FSharp.Core корневое пространство имен "Microsoft" теперь является необязательным.
*)


(**
***
### What is FsReveal?

- Generates [reveal.js](http://lab.hakim.se/reveal-js/#/) presentation from [markdown](http://daringfireball.net/projects/markdown/)
- Utilizes [FSharp.Formatting](https://github.com/tpetricek/FSharp.Formatting) for markdown parsing

***

### Reveal.js

- A framework for easily creating beautiful presentations using HTML.  
  
> **Atwood's Law**: any application that can be written in JavaScript, will eventually be written in JavaScript.

***

### FSharp.Formatting

- F# tools for generating documentation (Markdown processor and F# code formatter).
- It parses markdown and F# script file and generates HTML or PDF.
- Code syntax highlighting support.
- It also evaluates your F# code and produce tooltips.

***

### Syntax Highlighting

#### F# (with tooltips)

*)
let a = 5
let factorial x = [1..x] |> List.reduce (*)
let c = factorial a
(** 
`c` is evaluated for you
*)
(*** include-value: c ***)
(**

--- 

#### More F#

*)
[<Measure>] type sqft
[<Measure>] type dollar
let sizes = [|1700<sqft>;2100<sqft>;1900<sqft>;1300<sqft>|]
let prices = [|53000<dollar>;44000<dollar>;59000<dollar>;82000<dollar>|] 
(**

  ###`prices.[0]/sizes.[0]`

*)
(*** include-value: prices.[0]/sizes.[0] ***)
(**

---

#### C#

    [lang=cs]
    using System;


    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, world!");
        }
    }


---

#### JavaScript

    [lang=js]
    function copyWithEvaluation(iElem, elem) {
      return function (obj) {
          var newObj = {};
          for (var p in obj) {
              var v = obj[p];
              if (typeof v === "function") {
                  v = v(iElem, elem);
              }
              newObj[p] = v;
          }
          if (!newObj.exactTiming) {
              newObj.delay += exports._libraryDelay;
          }
          return newObj;
      };
    }

---

#### Haskell
 
    [lang=haskell]
    recur_count k = 1 : 1 : zipWith recurAdd (recur_count k) (tail (recur_count k))
            where recurAdd x y = k * x + y

    main = do
      argv <- getArgs
      inputFile <- openFile (head argv) ReadMode
      line <- hGetLine inputFile
      let [n,k] = map read (words line)
      printf "%d\n" ((recur_count k) !! (n-1))


*code from [NashFP/rosalind](https://github.com/NashFP/rosalind/blob/master/mark_wutka%2Bhaskell/FIB/fib_ziplist.hs)*

---

### SQL
 
    [lang=sql]
    select * 
    from 
      (select 1 as Id union all select 2 union all select 3) as X 
    where Id in (@Ids1, @Ids2, @Ids3)

*sql from [Dapper](https://code.google.com/p/dapper-dot-net/)* 

***

**Bayes' Rule in LaTeX**

$ \Pr(A|B)=\frac{\Pr(B|A)\Pr(A)}{\Pr(B|A)\Pr(A)+\Pr(B|\neg A)\Pr(\neg A)} $

***

### The Reality of a Developer's Life 

**When I show my boss that I've fixed a bug:**
  
![When I show my boss that I've fixed a bug](http://www.topito.com/wp-content/uploads/2013/01/code-07.gif)
  
**When your regular expression returns what you expect:**
  
![When your regular expression returns what you expect](http://www.topito.com/wp-content/uploads/2013/01/code-03.gif)
  
*from [The Reality of a Developer's Life - in GIFs, Of Course](http://server.dzone.com/articles/reality-developers-life-gifs)*

*)