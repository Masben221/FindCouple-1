using System;
using SFML.Learning;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using System.Threading;

class FindCouple : Game
{
    static string[] iconName;

    static int[,] cards;
    static int cardCount = 20;
    static int cardWidth = 100;
    static int cardHeight = 100;

    static int countPerLine = 5;
    static int space = 40;
    static int leftOffset = 70;
    static int topOffset = 40;

    static string kartaSound = LoadSound("razorvali-kartu.wav");
    static string clickSound = LoadSound("iz-kolodyi-vzyali-odnu-kartu.wav");
    static string errorSound = LoadSound("__raclure__wrong.wav");

    static int timeoff = 0;
    static bool timerStart = false;

    static int level = 0;

    static int OpenCardAmount = 0;
    static int firstOpenCardIndex = -1;
    static int secondOpenCardIndex = -1;
    static int remainingCard = cardCount;
    static bool Endgame = true;

    static void Stage_number(int level)
    {
        if (level == 0)
        {
            cardCount = 4;
            countPerLine = 2;
            space = 100;
            leftOffset = 240;
            topOffset = 140;
        }
        if (level == 1)
        {
            cardCount = 12;
            countPerLine = 4;
            space = 50;
            leftOffset = 120;
            topOffset = 90;
        }
        if (level == 2)
        {
            cardCount = 20;
            countPerLine = 5;
            space = 40;
            leftOffset = 70;
            topOffset = 40;
        }
        if (level == 3)
        {
            cardCount = 30;
            countPerLine = 6;
            space = 10;
            leftOffset = 70;
            topOffset = 30;
        }
    }
    static void difficulty_level()
    {
        OpenCardAmount = 0;
        firstOpenCardIndex = -1;
        secondOpenCardIndex = -1;
        remainingCard = cardCount;
        Endgame = true;
        
        LoadIcons();
           
        ClearWindow(55, 96, 25);

        DrawText(120, 200, "Нажми кнопку 1, 2 или 3 для выбора сложности", 24);
        DrawText(5, 2, "Уровень: " + level, 24);
        DisplayWindow();

        timeoff = 0;

        while (timeoff <= 0)
        {
            if (GetKey(Keyboard.Key.Num1) == true) timeoff = 49;

            if (GetKey(Keyboard.Key.Num2) == true) timeoff = 39;

            if (GetKey(Keyboard.Key.Num3) == true) timeoff = 29;
        }
        InitCard();

        SetStateToAllCards(1);

        ClearWindow(75, 36, 15);
        DrawCards();
        DrawText(5, 2, "Уровень: " + level, 24);
        DrawText(270, 2, "Запоминай карты!", 26);
        DisplayWindow();
        Delay(4000);

        SetStateToAllCards(0);
    }
    static void showTime(Object obj)
    {
        timeoff --;
    }
    static void LoadIcons()
    {
        iconName = new string[7];

        iconName[0] = LoadTexture("Icon_0.png");

        for (int i = 1; i < iconName.Length; i++)
        {
            iconName[i] = LoadTexture("Icon_" + (i).ToString() + ".png");
        }
    }

    static void Shuffle(int[] arr)
    {
        Random rand = new Random();

        for (int i = arr.Length - 1; i >= 1; i--)
        {
            int j = rand.Next(1, i + 1);

            int tmp = arr[j];
            arr[j] = arr[i];
            arr[i] = tmp;
        }
    }
    static void InitCard()
    {
        Random rnd = new Random();
        cards = new int[cardCount, 6];

        int[] iconId = new int[cards.GetLength(0)];
        int id = 0;

        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (i % 2 == 0)
            {
                id = rnd.Next(1, 7);
            }
            iconId[i] = id;
        }

        Shuffle(iconId);
        Shuffle(iconId);

        // result init
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = 0; // state
            cards[i, 1] = (i % countPerLine) * (cardWidth + space) + leftOffset; // posX
            cards[i, 2] = (i / countPerLine) * (cardHeight + space) + topOffset; // posY
            cards[i, 3] = cardWidth; // width
            cards[i, 4] = cardHeight; // heigth
            cards[i, 5] = iconId[i]; // id
        }
    }
    static void SetStateToAllCards(int state)
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = state;
        }
    }
    static void DrawCards()
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (cards[i, 0] == 1) // open
            {
                DrawSprite(iconName[cards[i, 5]], cards[i, 1], cards[i, 2]);
            }

            if (cards[i, 0] == 0) // close
            {
                DrawSprite(iconName[0], cards[i, 1], cards[i, 2]);
            }
        }
    }
    static int GetIndexByMousePosition()
    {
        for(int i = 0; i < cards.GetLength(0); i++)
        {
            if (MouseX >= cards[i, 1] && MouseX <= cards[i, 1] + cards[i, 3] && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
            {
                return i;
            }
        }
        return -1;
    }
    static void Main(string[] args)
    {
      Stage_number(level);

      SetFont("comic.ttf");

      InitWindow(800, 600, "Find Couple");
             
      difficulty_level();

      if (timeoff > 0 && timerStart == false)
      {
          timerStart = true;
          Timer timer = new Timer(showTime, timeoff, 0, 1000);
      }
      while (true)
      {
            if (Endgame == true)
            { 
                DispatchEvents();
                Stage_number(level);

                if (OpenCardAmount == 2)
                {
                    if (cards[firstOpenCardIndex, 5] == cards[secondOpenCardIndex, 5])
                    {
                        cards[firstOpenCardIndex, 0] = -1;
                        cards[secondOpenCardIndex, 0] = -1;

                        remainingCard -= 2;

                        Delay(300);
                        PlaySound(kartaSound, 200);
                    }
                    else
                    {
                        cards[firstOpenCardIndex, 0] = 0;
                        cards[secondOpenCardIndex, 0] = 0;

                        Delay(500);
                        PlaySound(errorSound, 200);
                    }

                    firstOpenCardIndex = -1;
                    secondOpenCardIndex = -1;

                    OpenCardAmount = 0;
                }

                if (GetMouseButtonDown(0) == true)
                {
                    int index = GetIndexByMousePosition();

                    PlaySound(clickSound, 200);

                    if (index != -1 && index != firstOpenCardIndex && cards[index, 0] != -1)
                    {
                        cards[index, 0] = 1;

                        OpenCardAmount++;

                        if (OpenCardAmount == 1) firstOpenCardIndex = index;
                        if (OpenCardAmount == 2) secondOpenCardIndex = index;
                    }
                }

                ClearWindow(180, 150, 180);

                DrawCards();
                DrawText(5, 2, "Уровень: " + level, 24);
                DrawText(500, 2, "осталось времени: " + timeoff, 24);
                DrawText(10, 570, "ESC - выход из игры, ПРОБЕЛ - рестарт уровня", 20);
                DisplayWindow();

                Delay(1);

                if (remainingCard == 0 && level < 3)
                {
                    ClearWindow(5, 5, 75);
                    level++;
                    DrawText(170, 250, "Молодец! А теперь уровень " + level + ", удачи!", 24);
                    DisplayWindow();
                    Delay(3000);
                    Stage_number(level);
                    difficulty_level();
                }

                if (remainingCard == 0 && level == 3) Endgame = false;

                if (timeoff == 0)
                {
                    ClearWindow(5, 5, 75);
                    DrawText(220, 250, "Кончилось время! Ты продул!", 24);
                    DisplayWindow();
                    Endgame = false;
                    Delay(3000);
                }

                if (Endgame == false)
                {
                    ClearWindow(5, 5, 75);
                    DrawText(120, 220, "ESC - выход из игры, ПРОБЕЛ - рестарт уровня", 24);
                    DrawText(230, 270, "BackSpace - начать сначала", 24);
                    DisplayWindow();
                }
                //Console.WriteLine(timeoff);     
            }

            var isExit = GetKey(Keyboard.Key.Escape);
            var isRestartLevel = GetKey(Keyboard.Key.Space);
            var isRestartGame = GetKey(Keyboard.Key.BackSpace);
            var isChitCode = GetKey(Keyboard.Key.L);
        
            if (isExit) break;
            if (isRestartLevel) difficulty_level();
            if (isRestartGame)
            {
                level = 0;
                Stage_number(level);
                difficulty_level();
            }
            if (isChitCode)
            {
                level++;
                Stage_number(level);
                difficulty_level();
            }

            if (remainingCard == 0 && level == 3) break;
            if (level > 3) break;
      }
        // End Game
        ClearWindow(5, 75, 15);

        if (remainingCard == 0 && level >= 3) DrawText(220, 200, "Поздравляю! Это победа!", 24); // Console.WriteLine("Выиграл!");
        if (timeoff <= 0) DrawText(210, 250, "Ахах, ты сдался, неудачник!", 24); // Console.WriteLine("Проиграл!");
        DrawText(300, 300, "GAME OVER", 30);
        DisplayWindow();

        Delay(4000);

    }
}
