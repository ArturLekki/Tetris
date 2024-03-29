﻿using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new GameState();

        #endregion

        #region KONSTRUKTORY

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
            AddVersionNumber(); // APP ADD VERSION NUMBER TO WINDOW TITLE
            CheckForUpdates(); // APP UPDATE
        }

        #endregion


        #region METODY - CZYLI ZDARZENIA / EVENTY W MAIN WINDOW

        // APP UPDATE
        private async Task CheckForUpdates()
        {
            string serverAddr = @"https://awm-tec.pl/appupdates";
            string localAddr = @"F:\Temp\Releases";

            using (var manager = new UpdateManager(localAddr))
            {
                await manager.UpdateApp();
            }
        }

        // APP ADD VERSION NUMBER TO WINDOW TITLE
        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            this.Title += $" v.{versionInfo.FileVersion}";
        }


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

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
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
                    imageControls[r, c].Opacity = 1;
                    imageControls[r,c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.Nextblock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if(heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while(!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
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
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
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
 *   6. MoveRowDown(int r, int numRows)- przesuwa wiersz na dół o określona liczbę wierszy
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

    8. DrawNextBlock(BlockQueue blockQueue)-zapowiada jaki będzie następny blok oraz trzeba
    wywołać te metodę w metodzie Draw();

    KONSTRUKTOR:
    1. inicjuje tablicę kontroli obrazów za pomocą wywołania metody:SetupGameCanvas()



---SEKCJA 8, POPRAWKI ---

    1. Poprawa spawn pozycji bloku w klasie GameState w konstruktorze (Dodanie Fora z ifem)
    Obecnie blok spawnuje się w dwoch ukrytych wierszach, ale wstawimy odstep w wiersach
    2 i 3 do top widocznych wierszy.

    2. Druga poprawka to gdy Gra jest skonczona to nie widac ktory blok nas zabił. Mozna to
    naprawic pokazujac kilka pikseli wiersza 1 ktory jest obecnie ukryty. Czyli otwieramy
    MainWindow.xaml i trzeba zmienic wysokość=510 w elemencie Canvas. 
    A w CodeBehind w metodzie SetupGameCanvas(): gdy pozycjonujemy obrazki pionowo dodamy
    10 pikseli. OD teraz widać kawałek ukrytego wiersza o numerze 1 na samej górze.


---SEKCJA 9, WYNIK GRY, HOLD BLOCK - AKTUALIZACJA KLASY GAMESTATE I CODE BEHIND---

    W KLASIE GAMESTATE:
    1. Dodanie propertisa: int Score{get;set;}. Jest to całkowita liczba wyczyszczonych
    wierszy.
    2. w metodzie PlaceBlock() inkrementujemy tego propertisa za każdym razem gdy wiersz
    jest czyszczony
    3. W CodeBehind ustawiamy Score text w metodzie Draw();
    4. Final score trzeba tez ustawić w menu końca gry. W Code Behind w metodzie GameLoop
    Zaraz po linijce gdzie ustwaiamy menu na widoczne.
    5. w klasie GameState Dodanie propertisa Block HeldBlock{get;private set;}
    6. w klasie GameState Dodanie propertisa bool CanHold{get;private set;}
    7. w klasie GameState W konstruktorze ustawiamy CanHold=true;
    8. w klasie GameState tworzyy metode HoldBlock()- jesli nie mozna trzymac bloku to 
    po prostu wychodzimy z mtody. Jeśli nie ma bloku w trzymaniu to ustawiamy trzymany
    do aktualnego bloku, a obecny blok do nastepnego bloku. Jeśli jest blok(else) w 
    oczekiwaniu trzeba zamienić obecny blok i ten trzymany. Na koncu ustawiamy 
    canHold=false, zeby nie mozna było spamować trzymania.
    9. w klasie GameState W metodzie PlaceBlock trzeba ustawić CanHold=true pod 
    linijką gdzie update obecnego bloku jest.
    10. W CodeBehind wywołujemy metodę HoldBlock gdy user naciśnie C
    11. W CodeBehind Tworzymy metodę DrawHeldBlock(Block heldBlock) ktora pokazuje 
    trzymany blok i wywołamy ją z metody Draw();


---SEKCJA 10 HARD DROP FEATURE--------------------

    Pozwala userowi nacisnąc jeden klawisz, który przesunie blok w dół nie tylko o 1 wiersz
    ale o tyle ile jest to możliwe.
    1. W klasie GameState: Tworzymy metodę pomocniczą TileDropDistance(Position p) -pobiera
    pozycje i zwraca liczbę pustych komórek bezpośrednio pod tą pozycją. Ta metoda pozwala
    określić ile wierszy obecny blok może przesunać na dół. Wywoołujemy ją dla każdej
    komórki obecnego bloku i sprawdzamy mnimum w następnej metodzie: BlockDropDistance().
    2. W klasie GameState: Potem tworzymy DropBlock() metodę -przesuwa w dół obecny blok
    o tyle wierszy ile to mozliwe i układa go w siatce gry.
    3. W CodeBehind: wywołujemy metodę DropBlock() gdy SPACE jest wciśnięty.

    Rozpoczęcie pracy nad: GHOST BLOCK- czyli pokaże w ktorym miejscu DropBlock spadnie 
    po wcisnieciu tej spacji.
    1. w CodeBehind: DrawGhostBlock(Block block)- komórki gdzie blok wyląduje są lokalizowa
    ne przez Drop dystans do pozycji komórek aktualnego bloku. Potem ustawiamy Opacity
    kontroli danego obrazu. Potem aktualizacja źródła. Opacity to sztuczka, trzeba ją zre-
    setować gdy rysowany jest siatka gry i nowy obecny blok - czyli w metodzie DrawGrid()
    imageControls[r, c].Opacity = 1; Oraz w metodzie DrawBlock(Block block):
    imageControls[p.Row, p.Column].Opacity = 1;
    Wywołanie tej metody DrawGhostBlock będzie też w metodzie Draw() i musi zostać 
    wywołana przed rysowaniem bloku. Od teraz widoczne jest gdzie wyląduje dany blok.
    

---SEKCJA 11 ZWIEKSZANIE PREDKOSCI BLOKU WRAZ ZE WZROSTEM WYNIKU--------------------

    CODE BEHIND:
    1. dodanie stałych 
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;
    2. W game loop dodajemy zmienną opóźniającą. Gdy gra się zaczyna delay bedzie ustawiony 
    na maksymalna wartosc, na kazdy punkt user dostaje to delay jest zmniejszany. Ale nie
    będzie nigdy nizszy niz jego ustawiona wartosc minimalna.


---SEKCJA 12 DODANIE IKONY DO PROJEKTU----------

    1. PPM->NA PROJEKT->PROPERTIES->APPLICATION->ICON & MANIFEST->ICON: Assets\Icon.ico



---SEKCJA 13 DODANIE SQUIREL.WINDOWS INSTALLER + UPDATER----------

    ---ETAP INSTALACJA + KONFIGURACJA---

    Działanie: Po uruchomieniu aplikacji sprawdza czy jest nowa wersja i pobiera ją.
    Po ponownym uruchomieniu aplikacji będzie dopiero nowa werjsa.

    1. INSTALACJA PACZKI: PPM->Na PROJEKT->Manage Nuget packages->squirrel.windows->install
    2. Storzenie miejsca, gdzie update aplikacji startuje, czyli sprawdza czy jest nowa
    wersja aplikacji i jesli jest to ja pobierze i zainstaluje (np metoda startup). Lub
    mozna zrobić button ktory wywoluje metode updatowania aplikacji. W przyadku WPF robię to
    w MainWindow.xaml.cs w konstruktorze.
    Pierwszym parametrem metody upadte wywołanej w konstruktorze jest ścieżka czyli gdzie
    squirel ma sprawdzic czy wersja jest aktualna (tutaj moze to być serwer www lub lokal
    nie-). 
    3. Stworzenie lokalnie folderu dla aktualizacji F:\Temp\Releases ORAZ na moim serwerze
    www folderu appupdates.
    4. Edycja pliku Properties->AssemblyInfo.cs- zmiana wersji do:
        [assembly: AssemblyVersion("1.0.0")]   <-- usunięcie Revision number
        [assembly: AssemblyFileVersion("1.0.0")] <-- usunięcie Revision number


    5. Tworzenie własnego NuGet package pliku(bo tego wlasnie squirel uzywa do updatów)
    Trzeba pobrać z Microsoft Store->NuGet Package Explorer
    5.1. Create a new package->Edit Metadata POLA WYAMAGANE
        Id = to nazwa aplikacji(nie wolno spacji)
        Version
        Authors
        Description
    Save (Zapisać zmiany)
    5.2. Teraz prawe okno w nuget package manager to Package contents - tutaj:
    PPM->ADD NEW FOLDER-> Lib
    Teraz PPM na Lib-> add new folder-> net48
        Te dwa foldery to podstawowa struktura, a net48 oznacza wersje frameworka 4.8.*
    5.3. Teraz z lokalizacji F:\Projekty_testowe\Tetris\Tetris\bin\Release 
    KOPIUJEMY WSZYSTKO OPRÓCZ PLIKÓW Z ROZSZERZENIEM .pdb (bo te pliki sa uzywane do 
    debugowania, tego nie chcemy w instalerze). Kopiujemy te pliki do folderu Lib/net48/

    Teraz mamy wszystko Po lewej metadata po prawej kontent

    5.4. Zapisujemy w miejscu gdzie jest SOLUCJA czyli(F:\Projekty_testowe\Tetris): 
    File->Save->Tetris.1.0.0.nupkg
    5.5. Teraz w Package manager Console odpalamy polecenie: 
        Squirrel --releasify Tetris.1.0.0.nupkg

    Efektem jest utworzenie w miejscu SOLUCJI folderu Releases , w którym jest plik:
    setup.exe setup.msi, oraz Tetris-1.9.9-full.nupkg

    Teraz WSZYSTKIE PLIKI Z FOLDERU RELEASES kopiujemy do folderu gdzie aplikacja będzie
    sprawdzać czy sa nowe wersje do pobrania (u mnie lokalnie to: F:\Temp\Releases).

    5.6. Kazda zmiana paczki nuget(zmieniłem tytuł w metadata)-> usunąć zawartośc z folderu 
    solucji Releases/
    Oraz uruchomic polecenie takie samo bez zmiany wersji:
    Squirrel --releasify Tetris.1.0.0.nupkg
    I teraz nowo utworzone pliki kopiuję do F:\Temp\Releases, skad uruchamiam instalacje.


    5.7. Dodanie do tytułu okna palikacji numeru werji, nowa metoda oraz wywołanie jej 
    w konstruktorze w CodeBehind.


    ---ETAP JAK ZROBIĆ UPDATE---
    5.8. Po zrobieniu zmiany w kodzie, zmieniamy na RELEASE mode. W Properties w 
    AssemblyInfo.cs zmieniamy wersję na 1.0.1 (oba)
    Build->Build Solution
    Package Explorer->Open a local package->Tetris.1.0.0.nupkg->Edit metadata zmiana Wersji
    Otwarcie całej solucji w file explorer i w lokacji: 
    F:\Projekty_testowe\Tetris\Tetris\bin\Release szukamy wszystkie pliko ktore 
    zostały zmienione i przeciagamy znowu do package explorer do Lib/net48/
    Teraz FILE->SAVE AS/ Ta sama lokacja ale nowa wersja będzie w nazwie
    Teraz w PAckage manager console: Squirrel --releasify Tetris.1.0.1.nupkg
    Teraz w Solucji: F:\Projekty_testowe\Tetris\Releases kopiujemy wszystko i wklejamy
    w miejscu gdzie aplikacja sprawdza atualizacje  (F:\Temp\Releases)
    
    




38.11: https://www.youtube.com/watch?v=jcUctrLC-7M <-- tetris guide
27.06: https://www.youtube.com/watch?v=W8Qu4qMJyh4&t=1467s <-- Squirell.windows guide
*/
