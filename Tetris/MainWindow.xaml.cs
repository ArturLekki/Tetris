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
        #region POLA / WŁAŚCIWOŚCI

        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;

        private GameState gameState = new GameState();

        #endregion

        #region KONSTRUKTORY

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        #endregion


        #region METODY - CZYLI ZDARZENIA / EVENTY W MAIN WINDOW

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r,c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            for(int r = 0; r < grid.Rows; r++)
            {
                for(int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r,c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while(!gameState.GameOver)
            {
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(gameState.GameOver)
            {
                return;
            }

            switch(e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        #endregion
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


    
    

---SEKCJA CZWARTA KLASA GAMESTATE - KLASA ŁĄCZĄCA WSZYSTK0 CO STWORZONE DO TERAZ------

    Zawiera pola:
    1. Block currentBlock; - to pole zawiera tez zdefiniowaną właściwosc get i set, 
    w której gdy aktualizujemy wartość tego pola to wywołujemy metodę Reset()(można bo
    pole jest typu block i to metoda z tej klasy). Metoda ta ustawia poprawnie startową 
    pozycję i rotation obecnego bloku.
    2. public GameGrid GameGrid { get; }- propertis dla pobrania siatki gry.
    3. BlockQueue BlockQueue { get; }- propertis dla pobrania następnego bloku.
    4. bool GameOver { get; private set; }- propertis dla ustawienia czy gra sie skonczyła.

    Konstruktor:
    1. Inicjuje gameGrid z 22 wierszy i 10 kolumn.
    2. Inicjuje blockQueue aby uzyc randomowego bloku dla currentBlock właściwości.

    Metody:
    1. BlockFits()- sprawdza czy obecny blok jest w legalnej pozycji czy nie. Loopuje ona
    przez pozycje komórek obecnego bloku i jesli ktoraś komórka jest poza gridem lub 
    nakłada się na inną komórkę to zwraca false. Jesli jednak cała petle przejdzie OK
    to wraca true;
    2. RotateBlockCW()- obraca obecny blok wg wskazówek zegara ale tylko gdy jet to możliwe
    Jeśli nie jest to możliwe to obraca go spowrotem do poprzedniej pozycji dzieki metodzie
    CurrentBlock.RotateCCW();
    3. RotateBlockCCW()- obraca obecny blok przeciwnie do wskazówek zegara ale tylko 
    gdy jet to możliwe. DZiała analogicznie do metody powyżej: RotateBlockCW().
    4. MoveBlockLeft()- sterowanie blokiem w lewo. Stratgia taka sama jak w metodach wyżej. 
    jesli ruszy się do nielegalnej pozycji to wróci go spowrotem.
    5. MoveBlockRight()- sterowanie blokiem w prawo. Stratgia taka sama jak w metodach 
    wyżej. jesli ruszy się do nielegalnej pozycji to wróci go spowrotem.
    6. IsGameOver()- sprawdza czy gra jest skonczona. Gdy któryś z ukrytych wierszy na 
    samej górze NIE jest pusty to gra jest przegrana.
    7. PlaceBlock()- wywołana gdy currentBlock nie może być przesunięty na dół.
    Najpierw loopuje przez pozycje komórek obecnego bloku i ustawia jego pozycje w gameGrid
    na podstawie ID danego bloku. Potem czyści każdy potencjalny pełny wiersz. I sprawdza
    czy gra jest skończona. Jesli tak to ustawia własciwosc gameOver na true. Jeśli nie to
    uaktualnia obecny blok.
    8. MoveBlockDown() -przesuwa blok na dół. Działa tak samo jak inne metody move tylko 
    wywołujemy placeBlock() w miejscu gdy obecny blok nie moze byc przesuniety na dół.
    


---SEKCJA PIĄTA FOLDER ASSETS I SKOPIOWANIE DO NIEGO GRAFIK------

    Oprócz tego kazda grafika we właściwościach ma zaznaczoną opcję: Build Action: Resource



---SEKCJA SZÓSTA USER INTERFACE------

    MainWindow.xaml - główne okno aplikacji. W atrybucie <Window>:
    1. ustawiamy tytuł szerokosc/wys okna, kolory
    2. KeyDown="Window_KeyDown"- Dodajemy KeyDownEvent -a by wykryć kiedy user naciśnie 
    klawisz.

    W atrybucie <Grid>:
    1. Ustawiamy definicje wierszy i kolumn
    2. Ustawiamy obraz z folderu Assets jako tło
    3. Ustawiamy atrybut CANVAS. Siatka gry będzie tu rysowana  w 
    wierszu 1 i kolumnie 1. Szerokosc elementu canvas=250 i height=500 co daje 25pikseli
    na widoczną komórkę w siatce gry.
    4. Opakowanie elementu Canvas w element viewBox ponieważ przy zmianie rozmiaru okna
    nasz canvas nie zmienia rozmiaru. Teraz trzeba przenieść z canvas atrybuty:
         Grid.Row="1" oraz Grid.Column="1" do elementu ViewBox. Od teraz canvas skaluje się
    razem z oknem.
    5. Dodanie do elementu canvas atrubutu ClipToBounds - zapewnia ze kazde dziecko ktore
    rozszerza poza granicami canvas nie bedzie pokazany(potrzebne dla hidden rows)
    6. Dodanie do elementu canvas atrybutu Loaded-wyzwalane, gdy dany element Canvas oraz
    wszystkie jego elementy podrzędne zostają załadowane i są gotowe do wyświetlenia na 
    interfejsie użytkownika.
    7. Wyświetlanie wyniku ponad GameGridem. TextBlock w wierszu 0 i kolumnie 1. W wierszu
    1 i kolumnie 0 pokazujemy jaki blok jest obecnie
    8. Z lewej strony stack panel wyswietlajacy obecny blok
    9. Z prawej strony stack panel wyswietlajacy jaki będzie następny blok
    10. Game Over menu(nowy grid) - pokazuje wynik i button do restartu gry. Domyślnie musi być ukryte
    więc ustawiamy w nim atrybut visibility="hidden"


---SEKCJA SIÓDMA CODE BEHIND czyli plik MainWindow.xaml.cs (nie uzywamy tu MVVM pattern)--

    POLA WEWNĄTRZ KLASY MAINWINDOW:

    1. ImageSource[] tileImages = new ImageSource[]- Ustawiamy tablicę zawierającą obrazki 
    kafelek(komórek). Kolejność dodawania obrazków do tablic nie jest randomowa. W indeksie 
    0 jest obrazek pustego kafelka. A kolejność następnyych brazków jest pasująca z ID 
    bloków. Przykład: blok"i" ma id=1 i powinien miec kolor Cyan, wiec w tej tablicy mamy
    w indeksie 1 ten obrazek o kolorze cyan.
    2. ImageSource[] blockImages = new ImageSource[]- Ustawiamy tablicę zawierającą obrazki
    dla całych bloków. Bedzie to uzywane do pokazania obecnego bloku oraz jaki bedzie next.
    Tutaj tez kolejnosc dodawania do tablicy jest pasujaca z ID bloków.
    3. Image[,] imageControls;- Ustawiamy tablicę kontroli obrazków.
    Działanie to: jest jedna kontrola obrazu na każdą komórkę w siatce gry.
    4. GameState gameState = new GameState();-Ustawiamy obiekt ktory jest stanem gry.

    METODY:
    1. SetupGameCanvas(GameGrid grid) -ustawia kontrole obrazu w elemencie canvas.
    Tablica ImageContntrols bedzie miala 22wiersze i 10 kolumn tak samo jak siatka gry.
    Następnie zmienna dla width i height każdej komórki, która wynosi 25(bo canvas width
    jest 250 i canvas height=500, co daje 25 pikseli dla kazdej widocznej komórki).
    Nastepnie loopuje przez kazdy wiersz i kolumnę w siatce gry, gdzie dla kazdej pozycji
    tworzy kontrole obrazu z width=25 i height=25. Nastepnie ustawiamy pozycję kontroli
    obraz poprawnie(liczymy wiersze od góry do dołu, a kolumny od lewej do prawej), wiec
    ustawiamy dystans od góry elementu canvas do góry obrazu równe: (r-2)*cellSize.
    -2 jest po to aby wypchnąć najwyzsze ukryte wiersze do góry, dzieki temu nie są w 
    elemencie canvas. Podobnie dystans od lewej strony elementu canvas do lewej strony
    obrazu: c * cellSize. Nastepnie robimy obraz jako wykres elementu canvas i dodajemy go
    do tablicy, która bedzie zwrócona poza pętlą już. Od tego momentu mamy dwu-wymarową
    tablicę z jedym obrazem dla kazdej komórki dla siatki gry. Dwa top wiersze uzywane do
    spawnowania bloków są ustawione ponad elementem canvas, dzieki temu są ukryte.

    2. DrawGrid(GameGrid grid)- rysuje siatkę gry w elemencie canvas. Loopuje przez 
    wszystkie pozycje, i dla kazdej pozycji pobiera startowe ID i ustawia źródło obrazu
    na danej pozycji używając ID.

    3. DrawBlock(Block block)- rysuje obecny blok. Trzeba loopować po pozycje komórek
    i uaktualniać źródło obrazu takim samym sposobem jak w metodzie DrawGrid.

    4. Draw(); - metoda łączy rysowanie siatki gry i bloku razem. Wywołanie tej metody
    w momencie gdy element canvast jest w pełni załadowany(Loaded) - czyli w zdarzeniu
    GameCanvas_Loaded(object sender, RoutedEventArgs e).

    5. Window_KeyDown(object sender, KeyEventArgs e)- jest to event dodany do elementu 
    Window. Najpierw dodanie zabezpiecznia(if) gdy gra jest skonczona, wcisnięcie klawisza 
    jakiegokolwiek nie powinno nic robić. Uzyjemy tutaj klawiszy strzałek(switch) 
    do ruchu blokiem. Strzałka do góry obraca blok wg wskazówek zegara, a klawisz Z przeci-
    wnie do ruchu wskazowek zegara.Default w tym switchu sprawia, że reDraw robiony jest
    gdy user wciśnie klawisz który cos robi, a nie jakis przypadkowy.
    Poza switchem wywołujemy metodę Draw().

    6. async Task GameLoop()- metoda musi byc asynchroniczna aby moc poczekać bez blokowa-
    nia user interfajsu. Najpier rysuje stan gry. Potem petla leci az do mementu gameOvera.
    W ciele petli czekamy 500ms, przesuwamy blok na dół, i rysujemy na nowo. Po za pętlą
    ustawiamy widoczność Menu dla końca gry na visible. GameLoop jest
    startowane gdy Element canvas jest Loaded. Dlatego trzeba zmienić metodę GameCanvas na
    metode asynchroniczną z uzyciem słów async i await i tam wywołać metodę GameLoop();

    7. async void PlayAgain_Click(object sender, RoutedEventArgs e) - Tworzy nowy obiekt
    klasy GameState, chowa element GameOverMenu i restartuje GameLoop.

    KONSTRUKTOR:
    1. inicjuje tablicę kontroli obrazów za pomocą wywołania metody:SetupGameCanvas()




31.05: https://www.youtube.com/watch?v=jcUctrLC-7M
*/
