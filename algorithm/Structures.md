# Struktury danych w algorytmie

## Opis
Na potrzeby implementacji, ponieważ na dzień 1.11.2024 zaczęła się ona robić zbyt złożona i skomplikowana, więc postanowiłem zastanowić się nad całością raz jeszcze. Tym razem z myśla, że nie tworzę programu do robienia muzyki, a zwykły algorytm sprawdzający błędy.

Typy danych wzięte już z C#, włącznie z *?*, który oznacza dopuszczenie **null**. Typy w () to enumy. Default to też wiadomo.

Nazwy atrybutów dałem po polsku, żeby było wiadomo co to w ogóle jest, w kodzie przetłumaczę na angielski i - jak mi się będzie chciało - dodam komentarze albo dokumentację z nazwami polskimi.

Do każdej klasy też mogą dojść jakieś funkcje/atrybuty pomocnicze, których tutaj nie uwzglęgnię, ale nie będą miały na nic wpływu, co najwyżej usprawniały jakąś ilość operacji.

## Struktury muzyczne
Te wszystkie mogą posłużyć do stworzenia innych systemów muzycznych.

### Nuta (Note)
- Wysokość (Nazwa): string
- Oktawa: int

### Tonacja (Tonation)
Tutaj się przyda wnioskowanie, albo na podstawie nazwy, albo na podstawie ilości krzyżyków/bemoli (raczej to drugie, patrząc na to, w jaki sposób jest to robione po stronie edytora nut).
- Ilość krzyżyków: int
- Ilość bemoli: int
- Nazwa: string
- Tryb: (moll, dur)

### Metrum (Metre)
- Ilość: int
- Wartość: int

### Składnik (Component)
- Typ: (Root, Second, Third, Fourth, Fifth, Sixth, Seventh, Ninth)
- Czy obligatoryjny: bool | **default**: true
- Alteracja: (up, down, none) | **default**: none

### Funkcja (Function)
- Symbol: (T, Sii, Tiii, Diii, S, D, Tvi, Svi, Dvii, Svii)
- Czy moll: bool
- Składnik usunięty: Component? | **default**: null
- Składniki dodane: List\<Component\>  | **default**: []
- Składniki alterowane: List\<Component\>  | **default**: []
- Opóźnienia (Na razie WIP)
- Oparcie: Component? | **default**: null
- Pozycja: Component? | **default**: null
- Wtrącenie (Reverse, Front) | **default**: w przód
- Tonacja: Tonation <span style="color:green;">// to jest na potrzeby wtrącenia</span>
- Układ (rozległy, skupiony, dowolny) | **default**: dowolny
- Takt: int
- Beat w takcie: int
- Czas trwania: (WholeNote, HalfNoteDotted, HalfNote, QuarterNoteDotted, QuarterNote, EighthNoteDotted, EighthNote, SixteenthNote)

### Stos nut (Stack)
- Sopran: Note
- Alt: Note
- Tenor: Note
- Bas: Note
- Takt: int
- Beat w takcie: int
- Czas trwania: (WholeNote, HalfNoteDotted, HalfNote, QuarterNoteDotted, QuarterNote, EighthNoteDotted, EighthNote, SixteenthNote)

Tutaj potrzebna jest również możliwość tworzenia na podstawie czwórki samych komponentów funkcji oraz tej funkcji, a reszta zrobi się sama. Oczywiście zwykły konstruktor nutowy również będzie.







## Dodatkowe klasy/funkcjonalności muzyczne
Pomagające i potrzebne w rozwiązywaniu zadań, ale niekoniecznie niezbędne do trzymania jako osobne twory.

### Interwał
Klasa statyczna z operacjami na nutach, potrzebna do sprawdzania zasad oraz generowania zadań.

- Możliwość zbliżenia nut tak bardzo jak się da do siebie (w przypadku trytonu zawsze opcja kwinty zmniejszonej).
- Możliwość sprawdzenia ilości półtonów między nutami.
- Możliwość wygenerowania zbioru nut oddalonych o daną ilość półtonów, od podanej.

### Skala
Klasa statyczna z operacjami generowania nut w danej tonacji

- Możliwość wygenerowania podstawowych nazw nut z podanej tonacji
- Możliwość wygenerowania dodatkowych nut w tonacji (II>, VI>, VII>, VI<, VI<)
- Możliwość sprawdzenia, czy nuta należy do tonacji czy nie

### Możliwe nuty (PossibleNotes)
Tutaj odbywać się będzie generowanie wszystkich możliwych wersji **komponentowych** danej funkcji

- Możliwość wygenerowania na podstawie funkcji, w wyniku będzie List\<List\<List\<Component\>\>\>



## Struktury algorytmu
Algorytm bazuje na strukturach muzycznych, ale nie wnosi do nich żadnych dodatkowych funkcjonalności stricte muzycznych

### Problem (Problem)
Tutaj indeksowanie funkcji może się odbywać przy pomocy pary (Takt, Beat w takcie).

- Funkcje: List\<Function\>
- Metrum: Metre
- Tonacja: Tonation

### Rozwiązanie (Solution)
Tutaj indeksowanie rozwiązań może się odbywać przy pomocy pary (Takt, Beat w takcie).

- Problem: Problem
- Rozwiązanie: List\<Component\>

### Błąd (Mistake)
Bazowa klasa abstrakcyjna błedu. Bład można podzielić na 2 typy, stąd ten podział, abstrakcja natomiast potem ułatwi serializację i operowanie po stronie frontendu. Typy błedów to:
- Nutowe - kiedy nuty nie zgadzają się z funkcją
- Stosowy - kiedy stosy użytkownika nie spełniają zasad
Takie 2 klasy będą rozszerzały klasę błedu i wzbogacały je o swoje funkcjonalności.

- Opis: string

Potrzebna jest też funkcja generowania opisu, nawet jeżeli na potrzeby testów i wersji podstawowej programu, bez przedstawiania graficznego.

### Bład nutowy (NoteMistake)
- Nuty: List\<Note\>
- Stos: Stack

### Błąd stosu (StackMistake)
- Stosy: List\<Stack\>
- Zasada: Rule

### Zasada (Rule)
Bazowa klasa abstrakcyjna zasad.
- Nazwa: string
- Opis: string
- Jednofunkcyjna: bool | **default**: false

Do tego będą klasy pochodne, implementujące funkcję (Check).

### Sprawdzanie rozwiązania (SolutionChecker)
Być może klasa statyczna, z funkcją sprawdzania zadania. Zwracać będzie listę błędów.

<ol>
<li>Weź zadanie</li>
<li>
Dla każdego stosu:
<ol>
<li>
Sprawdź, czy nuty się zgadzają (generowanie wszystkich wersji funkcji - sprawdzenie, czy propozycja jest równa jednej z nich). Jeżeli się nie zgadzają, to dodaj nuty oraz stos do listy błędów.
</li>
</ol>
</li>
<li>Weź wszystkie zasady</li>
<li>Dla każdej zasady:
<ol>
<li>
Iterowanie po wszystkich funkcjach. Jeżeli potrzebna 1 funkcja do sprawdzenia zasady, to jedna, jak 2 to dwie. Jeżeli nie spełnia zasady, to dodaj funkcję/parę funkcji do błędów funkcyjnych.
</li>
</ol>
</li>
</ol>

Po całym przebiegu powinniśmy mieć listę błędów, składającą się z błędów funkcyjnych i błędów nutowych.

### Generowanie rozwiązania (SolutionGenerator)
Na razie nie jest potrzebne

### Sprawdzanie problemu (ProblemChecker)
Potrzebne, żeby walidować kreator funkcji - czy ktoś nie stworzył zadania, które od razu będzie zawierać błędy.

W wersji podstawowej wystarczy sprawdzać, czy po D nie ma S, to wszystko. Potem się doda jakieś sprawdzanie oparć itp.

### Generowanie problemu (ProblemGenerator)
Na razie nie jest potrzebne