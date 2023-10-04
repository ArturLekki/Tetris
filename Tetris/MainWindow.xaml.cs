﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}

/*
----------SEKCJA 1 OGÓLNE INFORMACJE I KLASY BAZOWE-----------
 * 
 *   Tetris opeira sie na siatce (grid) 20 wierszy i 10 kolumn. Ale na samej górze
 *   dodajemy 2 dodatkowe wiersze, aby spawnować bloki(te 2 wiersze beda ukryte).
 *   
 *   Wiesze liczone bedą od góry do dołu. Kolumy od lewej do prawej.
 *   Top left corner ma współrzędne [0,0]
 *   
 *   Siatka gry(grid) jest reprezentowana przez 2-wymiarową tablicę INT, gdzie:
 *   puste komórki mają wartość = 0
 *   jasno-niebieskie = 1
 *   niebieskie = 2
 *   pomarańczowe = 3
 *   żółte = 4
 *   zielone = 5
 *   fioletowe = 6
 *   czerwone = 7
 *   
 *   Blok ktory jest aktualnie kontrolowany przez usera NIE JEST W TEJ TABLICY.
 *   Jest on przechowywany oddzielnie.
 *   
 *   ------PIERWSZA KLASA----------
 *   Klasa reprezentująca GAME GRID: (Project->PPM->Add->Class)
 *   Ta klasa trzyma 2-wymiarową prostokątną tablicę, gdzie:
 *   Pierwszy wymiar to wiersze, drugi wymiar to kolumny.
 *   Klasa posiada propy reprezentujące liczbę wierszy i liczbe kolumn.
 *   Klasa posiada INDEXER - ktory zapewnia łatwy dostęp do tablicy. Dzięki niemu mozna:
 *   - użyc indeksowania bezpośrednio na obiekcie klasy GameGrid
 *   Konstuktor weżmie liczbe wierszy i kolumn jako parametry, co umożliwi zmianę wielkości
 *   siatki gry gdy chcemy aby była niestandardowa. W ciele konstruktora zapisujemy wiersze
 *   i kolumny i incicjujemy tablicę 2-wymairową.
 *   
 *   Klasa zawiera metody:
 *   1. IsInside(int r, int c)- sprawdza czy podane wiersz i kolumna znajdują się wewnątrz 
 *   siatki grid czy nie.
 *   2. IsEmpty(int r, int c)- sprawdza czy podana komórka jest pusta lub nie. 
 *   Sprawdza czy jest wewnatrz grida i czy ma wartość = 0;
 *   3. IsRowFull(int r)- sprawdza czy cały wiersz jest pełny
 *   4. IsRowEmpty(int r)-sprawdza czy wiersz jest pusty
 *   5. ClearRow(int r)- czyści wiersz czyli po prostu ustawia wartość komórek w całym 
 *   wierszu na wartość = 0
 *   6. MoveDown(int r, int numRows)- przesuwa wiersz na dół o określona liczbę wierszy
 *   7. ClearFullRows()- czyści pełne wiersze.
 *   
 *   Czyszczenie pełnego wiersza: użyjemy zmiennej zwanej Cleared, ktora zawiera ilosc
 *   wyczyszczonych wierszy. Zaczynając od najniższego wiersza sprawdzajac czy jest pełny,
 *   Jeśli nie jest to idziemy do następnego czyli jeden wyżej, jeśli jest pełny to
 *   jest czyszczony i wartość zmiennej Cleared zwiększamy o 1. Po wyczyszczeniu wiersza,
 *   ten powyżej jest przenoszony niżej o 1. Ten proces jest zatrzymany gdy pusty wiersz 
 *   jest napotkany.
 *   
 *   ----Działanie i reprezentacja bloków oraz rotacja ich----------
 *   W tetrisie jest 7 różnych bloków. Każdy z nich to połączenie 4 komórek, tylko w różnych
 *   kształtach (i,j,l,o,s,t,z).
 *   Każdy blok obraca sie wg specyficznego punktu, tworząc obszar, który jest duzo większy
 *   (BOUNDING BOX - czyli pudełko ograniczające). To pudełko musi być na tyle duże aby
 *   pomieścić blok we wszystkich czterech pozycjach gdy user nim obraca. TOP LEFT komórka
 *   bounding boxa ma swoje polozenie czyli wiersz 0 i kolumna 0, tak samo jak w game grid.
 *   
 *   W fazie 0 stopni czyli domyślne położenie poziome bloku(i) - jego współrzędne w 
 *   bounding boxie to: [wiersz, kolumna] = (1,0) (1,1) (1,2) (1,3)
 *   
 *   W innej fazie np 90 stopni czyli pionowo blok(i) ma współrzędne w bounding boxie:
 *   [wiersz, kolumna] = (0,2) (1,2) (2,2) (3,2)
 *   
 *   Pozostałe fazy to 180 stopni i 270 stopni.
 *   ODpowiedzialnym za określenie jaka faza jest aktualnie bloku(czyli ile stopni) bedzie
 *   zmienna INT -  tym sposobem tylko zmiana jednej zmiennej sprawi ze cały blok obróci się
 *   do danej fazy.
 *   
 *   ----Poruszanie bloków lewo/prawo i w dół----------
 *   Jest to reprezentowane poprzez przechowywanie przesunięcia wiersza i przesuniecia 
 *   kolumny. Czyli TOP LEFT corner BOUNDING BOXA jego współrzędne nas interesują.
 *   Przesunięcie wiersza i kolumny względem tej współrzędnej bounding boxa na siatce grid.
 *   Do reprezentacji pozycji(komórki w gridzie) sluzy klasa Position
 *   
 *   
 *   ------DRUGA KLASA----------
 *  Do reprezentacji pozycji(komórki w gridzie) sluzy klasa Position
    Przechowuje wiersz i kolumnę, oraz konstruktor zapisujący te dane.


    ------TRZECIA KLASA----------
    Block - klasa abstrakcyjna bedąca wzorem dla klas dziedziczących, które bedą już
    konkretnym kształtem bloku. Ta klasa zawiera dane dla kazdego bloku czyli:
    1. Position[][] Tiles { get; }- pozycja  bloku - tablica 2-wymairowa zawierająca
    bounding box bloku czyli jego pozycje w kazdej z 4 faz.
    2. Position StartOffset { get; }- Początkowe przesunięcie tego bloku 
    (wraz z bounding boxem) - decyduje gdzie blok sie zrespi na głównej siatce (game grid).
    3. int Id { get; } - ktore okresla konkretny rodzaj bloku
    4.  int rotationState;- Obecna faza bloku (czyli jaki kąt ma atkualnie)
    5. Position offset; - Obecne przesunięcie bloku (czyli jaka pozycja jego bounding boxa 
    jest - wg. top left corner)

    W konstruktorze ustawiane jest przesunięcie atkualne na wartości poczatkowe czyli
    poczatkowe przesuniecie

    Metoda TilePositions(), która zwraca pozycję w siatce GridGame okupowaną przez dany 
    blok i z daną jego fazą (czyli jego kątem obrotu) i jego aktualnym przesunięciem.
    Metoda ta loopuje przez pozycję komórek w obecnym stanie rotacji i dodaje przesunięcie
    wiersza i kolumny.

    Metoda RotateCW(), która obraca blok o 90 stopni wg wskazówek zegara (w prawo). Robi się
    to przez inkrementowanie aktualnego stanu rotacji (określany prez wartości od 0).

    Metoda RotateCCW(), któa obraca blok o 90 stopni przeciwnie do wskazówek zegara 
    (w lewo).

    Metoda Move(int rows, int columns),która przesuwa blok o podana ilosc wierszy i kolumn.
    
    Metoda Reset(), która resetuje rotacje bloku i pozycję do wartości początkowych.



---SEKCJA DRUGA KLASY POCHODNE-----------------------------------------------------------


    ------CZWARTA KLASA----------
    Block_i, która dziedziczy po klasie abstrakcyjnej Block. Posiada nastepujące pola:
    1. Position[][] tiles- pozycję komórek(tiles) dla wszystkich 4 rotacji bloku. Tablica 
    dwuwymiarowa , gdzie pierwszy wymiar stanowi obiekt zawierający dany zestaw rotacji,
    a drugi wymiar to juz wartości tego zestawu.
    2. Wypełnione właściwości: TZN:  int Id => 1; bo ta wartosc reprezentuje blok "i".
    3. Position StartOffset => new Position(-1, 3); Początkowe przesunięcie (-1,3) Aby blok 
    spawn był na środku topowego wiersza.
    4. Position[][] Tiles => tiles; - dla tej wlasciwosci z klasy bazowej zwracamy tablice
    która jest stworzona powyżej czyli: Position[][] tiles.

    Klasa ta dziedziczy z klasy bazowej(która jest abstract), do pól i wlaściwosci ktore 
    odziedziczyła tylko wypełnia je wartościami(te abstrakcyjne pola i metody itd musza 
    byc tylko implementowane). Nie ma tutaj metod i konstruktora ponieważ funkcjonalności
    dziedziczy z klasy bazowej. Tutaj jest tylko wypełnienie wartościami pól i propów.
    


    ------PIĄTA KLASA----------
    Block_o, jest unikatowy ponieważ okupuje te same pozycje w każdym stanie rotacji,
    Można by skopiowawc i wkleić te same pozycje 4 razy, ale bedzie działać jeśli wprowadzi
    się tylko 1 stan rotacji. Ta klasa też dziedziczy z klasy abstrakcyjnej Block i 
    wypełnia pola i właściwości abstrakcyjne(bo te śa wymuszane z klasy bazowej) 
    wartościami.

    1. Position[][] tiles - okresla pozycję komórek(tiles) dla wszystkich 4 rotacji bloku.
    Tablica  dwuwymiarowa , gdzie pierwszy wymiar stanowi obiekt zawierający dany
    zestaw rotacji, a drugi wymiar to juz wartości tego zestawu.
    2. int Id => 4; bo ta wartosc reprezentuje blok "o".
    3. Position StartOffset => new Position(0, 4); - Początkowe przesunięcie
    4. Position[][] Tiles => tiles; dla tej wlasciwosci z klasy bazowej zwracamy tablice
    która jest stworzona powyżej czyli: Position[][] tiles.


    ------KLASY POCHODNE POZOSTAŁE TAK SAMO, TYLKO ZMIANA WARTOŚCI----------



---SEKCJA TREZCIA KLASA KOLEJKOWANIA BLOKÓW---------------------------------------


    BlockQueue - klasa odpowiedzialna za wybieranie następego w grze. Zawiera pola: 
    1. Block[] blocks - tablicę bloków z instancją wszystkich 7 rodzajów bloków, które 
    będą używane.
    2. Random random = new Random(); - obiekt do losowania random
    3. Block Nextblock { get; private set; }- właściwosc dla następnego bloku w kolejce

    Metody:
    1.  RandomBlock()- metoda zwracająca randomowy block z tablicy bloków
    2. GetAndUpdate()- zwraca następny blok i aktualizuje właściwość. Metoda wybiera tak
    długo az wybierze inny, aby nie powtarzał się jeden po drugim

    Konstruktor: 
    1. Inicjuje następny blok jako random blok uzywając metody RandomBlock()


    12.50: https://www.youtube.com/watch?v=jcUctrLC-7M
    
*/
