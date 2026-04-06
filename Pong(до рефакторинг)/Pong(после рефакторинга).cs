using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pong
{
    //игровые объекты
    public partial class Form1 : Form
    {
        bool goUp, goDown;//Логические переменные для планого управления
        int paddleSpeed = 8;//Скорость игрока
        Random rnd = new Random();//Общий объект класса для создания случайных чисел
        int leftPaddleY = 200;//левая ракетка
        int rightPaddleY = 200;// правая ракетка
        int ballX = 400;//мяч х
        int ballY = 300;//мяч y
        int ballSpeedX = 5;//скорость мяча x
        int ballSpeedY = 5;//скорость мяча y
        int leftscore = 0;//отображение счета слева
        int rightscore = 0;//отображение счета справа
        bool gameRunning = true;//игра активна  
        Timer timer;//таймер для анимации
        int aiSpeed = 6;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Pong Game";//настройка формы 
            this.Size = new Size(800, 600);// устанавливаем размеры окна
            this.StartPosition = FormStartPosition.CenterParent;//начальная позиция окна чтобы оно было по центру 
            this.DoubleBuffered = true;//убирает мерцания 
            this.KeyPreview = true;//свойства позволяет приложению первым получать события с клавы

            this.Paint += Form1_Paint; ;//подключаем событие для отрисовки
            //подключаем событие чтобы считывать нажатие клавиш
            this.KeyDown += Form1_KeyDown;//подключаем событие чтобы считывать нажатие клавиш
            //Настраиваем таймер
            timer = new Timer();//Создаем объект таймера
            timer.Interval = 16;//Устанавливаем значение 16 для 60 кадров в секунду

            timer.Tick += Timer_Tick;//Запускаем обновление игры для каждого кадра
            timer.Start();//Запускаем таймер
            this.KeyUp += Form1_KeyUp;//Подписка для события отпускания клавиши
        }
        //Функция игры для обновления каждого кадра
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (gameRunning)
            {
                //Добавление плавного перемещения на основе  нажатых клавиш
                if (goUp && leftPaddleY > 0) leftPaddleY -= paddleSpeed;//плавно вверх
                if(goDown&&leftPaddleY<this.Size.Height-100) leftPaddleY += paddleSpeed;//плавно вниз
                //двигаем мяч
                ballX += ballSpeedX;//Двигаем мяч по оси Х
                ballY += ballSpeedY;//Двигаем мяч по оси У
                //Отскок от верха и низа 
                if (ballY <= 0 || ballY >= this.ClientSize.Height)
                {
                    ballSpeedY = -ballSpeedY;
                }
                //отскок от левой ракетки
                if (ballX <= 45 && ballX >= 30 && ballY >=  leftPaddleY && ballY <= leftPaddleY + 100)
                {
                    ballSpeedX = Math.Abs(ballSpeedX)+1;//гарантируем движение в право
                    if(Math.Abs(ballSpeedX) < 15)
                    ballSpeedY += (ballSpeedY > 0) ? 1 : -1;
                    ballX = 46;//не даем мячу пройти сквозь, выталкиваем мяч чтоб он не застрял в текстуре
                }
                if (ballX >= this.ClientSize.Width - 60 && ballX <= this.ClientSize.Width - 45 && ballY >= rightPaddleY && ballY <= rightPaddleY + 100)
                {

                    ballSpeedX =- Math.Abs(ballSpeedX)+1;
                    if (Math.Abs(ballSpeedX) < 15)
                    {
                        ballSpeedY += (ballSpeedY > 0) ? 1 : -1;
                    }
                    ballX = this.ClientSize.Width - 61;
                }
                //гол с слева
                if (ballX <= 0)
                {
                    rightscore++;
                    ResetBall();
                }
                //гол с справа
                if (ballX >= this.ClientSize.Width)
                {
                    leftscore++;
                    ResetBall();
                }
                //Проверка победы
                if (leftscore >= 5 || rightscore >= 5)
                {
                    gameRunning = false;
                    timer.Stop();
                }
                //ai для правой ракетки
                if (rightPaddleY + 50 < ballY)
                    rightPaddleY += aiSpeed;
                else if (rightPaddleY + 50 > ballY)
                    rightPaddleY -= aiSpeed;

                leftPaddleY = Math.Max(0, Math.Min(this.ClientSize.Height - 100, leftPaddleY));
                rightPaddleY = Math.Max(0, Math.Min(this.ClientSize.Height - 100, rightPaddleY));
            }
            this.Invalidate();//перерисовываем
        }
        void ResetBall()
        {
            ballX = this.ClientSize.Width / 2;
            ballY = this.ClientSize.Height / 2;
            ballSpeedX = (new Random().Next(2) == 0) ? 5 : -5;
            ballSpeedY = (new Random().Next(2) == 0) ? 5 : -5;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            Pen whitePen = new Pen(Color.White, 3);
            g.DrawLine(whitePen, this.Width / 2, 0, this.Width / 2, this.Height);
            g.FillRectangle(Brushes.White, 30, leftPaddleY, 15, 100);
            g.FillRectangle(Brushes.White, this.Width - 45, rightPaddleY, 15, 100);
            g.FillEllipse(Brushes.White, ballX - 8, ballY - 8, 16, 16);

            Font font = new Font("Arial", 30);
            g.DrawString(leftscore.ToString(), font, Brushes.White, this.Width / 4, 20);
            g.DrawString(rightscore.ToString(), font, Brushes.White, 3 * this.Width / 4, 20);
            if (!gameRunning)
            {
                string winner = (leftscore >= 5) ? "Левый победитель" : "Правый победитель";
                g.DrawString(winner, font, Brushes.Yellow, this.Width / 2 - 100, this.Height / 2); //адресовываем имя победителя желтым посередине экрана
                g.DrawString("Нажмите R для перезапуска", new Font("Arial", 16), Brushes.White, this.Width / 2 - 120, this.Height / 2 + 50);//адресовываем текст для информации о перезапуске
            }
        }
        // управление клавиатурой
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Мы не двигаем ракетку здесь а просто включаем флаг движения
            if(e.KeyCode == Keys.W) goUp = true;//Пока нажата клавиша W ракетка двигается вверх
            if(e.KeyCode == Keys.S) goDown = true;//Пока нажата клавиша S ракетка двигается вниз
            //перезапуск строк
            if (e.KeyCode == Keys.R && !gameRunning)//Когда нажата клавиша R выполняется перезагрузка игры
            {
                leftscore = 0;rightscore = 0;
                gameRunning = true;
                ResetBall();
                timer.Start();
            }
        }
        //Функция для проверки отжата ли клавиша
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Когда клавиша отпущина ракетка перестает двигаться
            if (e.KeyCode == Keys.W) goUp = false;
            if (e.KeyCode == Keys.S) goDown = false;
        }
    }
}