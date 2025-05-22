# SuperFarmer

## Jak uruchomić aplikację?

1. Upewnij się, że masz zainstalowany **.NET SDK (8.0 lub nowszy)**.
2. Otwórz folder projektu.
3. Przejdź do projektu `SuperFarmer`.
4. Uruchom aplikację:
    ```bash
    dotnet run
    ```
5. Otwórz przeglądarkę i przejdź do adresu `http://localhost:5240`

## Architektura i podejście

Aplikacja została napisana jako **monolityczna aplikacja webowa** w **ASP.NET Core MVC**.   
Nie korzysta z bazy danych – wszystko jest przechowywane w pamięci.

Zdecydowałem się na takie podejście, ponieważ miałem bardzo mało czasu, a gra farmer to w dużej mierze operacje na danych i ich wizualizacja. Dlatego webówka okazała się szybkim i wystarczającym rozwiązaniem.

Gdybym miał więcej czasu:
- zrobiłbym oddzielny backend (API) i frontend (np. React, Blazor),
- umożliwiłbym grę online między graczami,
- dodałbym zapisywanie stanu rozgrywki do bazy danych,
- zdecydowałbym się pewnie na aplikację **.NET MAUI**.

