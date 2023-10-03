using System;
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
 * ------SEKCJA 1-----------
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
*/
