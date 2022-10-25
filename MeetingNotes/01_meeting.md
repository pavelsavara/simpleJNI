#  01 meeting

* chceme - v 1 procesu provozovat 2 VM a provolavat mezi nimi navzajem metody
  * pro obe existuji low level apicka (cecko), co nam to umozni
    * P/invoke pro .NET https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke
    * JNI (Java native interface) pro javu
  * nad touto low level vrstvou je treba poskytnout API v C#/jave - odstinit uzivatele od low level detailu

##  Faze prace na projektu:

1. analyza business use-cases - kdy je  v prumyslu uzitecne volat Javu ze C#? Na jakych platformach (Windows, Android...)?
2. analyza existujicich reseni - kdo a jak problem resil prede mnou
   * existuje hromada komercnich a 2 open source reseni 
     * MS open source reseni v ramci Xamarin (code base neni moc pekna)
     * cca 10 let stare reseni (Pavel Savara) - dnes neaktualni, pry nedostatecne
3. research - jake jsou moznosti implementace - navrhnout vice moznosti reseni, diskuze - pro/proti, tradeoff - do vanoc
4. Vybrat 1 moznost implementace
5. urcit scope 
6. implementace, unit testy, CI/CD, git
7. dema pro alespon nektere usecases (z faze 1) - demostrovat, ze muj produkt je uzitecny

## Na zvazeni

* Java a .Net jsou velice podobne - Java object a C# object na sobe maji nadefinovane podobne metody, metody se mohou ale lisit napr. jmeny - je treba rozhodnout, jak moc integrovat C# vygenerovany z Javy do zbytku C# kodu (a naopak) - vytvorit C# objekt s klasickymi object metodami? Priznat explicitneji, ze jdeme z Javy? - Jak moc integrovat do ciloveho jazyku?

## Scope

* zacit "jednoduse" - pouze staticke metody s primitivnimi typy - netreba delat "objektovku" - osahavat java typy pomoci Java reflection (ci java compileru, ci jinak, co z toho je lepsi?), netreba emulovat tyto typy v .NET
  1. volat C# -> Java
  2. volat Java -> C#
  3. jen na Windows (.NET chce byt multiplatformni, dnes nedostatecne)
  4. na Linuxu, Androidu - bude treba vic resit marshaling a volaci konvence - nestaci `stdCall`, treba `cdecl` a buhvico jeste (je velice zadouci, aby reseni bylo multiplatformni)

## Co je slozite (jine v Jave a C#)

* objektovka
* enumy
* delegaty - java je nema, callbacky se delaji pres intefaces
* `ref` a `out` parametry
* generika - urcite mimo scope diplomky - velice slozite, future work

## Casovy plan

* idealni by bylo byt ready pro .NET 8 (release v listopadu), deadline na kod v pulce leta cca?
* do vanoc navrhy reseni (research, studium problematiky)
* leden - intenzivnejsi komunikace s mentorem, volba navrhu, ktery bude naimplementovan, definovani scopu, dekompozice navrhu na workitemy, casove esstimates workitemu,  casovy plan implementacni faze
* cca 10 hod tydne, meetingy cca 1 tydne

 

