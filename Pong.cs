using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ponng
{
    public partial class Form1 : Form
    {
        //игровые объекты 


        int leftpaddleY = 200;//левая ракетка   
        int rightpaddleY = 200;// правая ракетка     
        int ballX = 400;//мяч х   
        int ballY = 300;//мяч y
        int ballSpeedX = 5;//скорость мяча x
        int ballSpeedY = 5;//скорость мяча y
        int leftscore = 0;//отображение счета слева
        int rightscore = 0;//отображение счета справа
        bool gameRunning = true;//игра активна  
        Timer timer;//таймер для анимации
        int aiSpeed = 3;
        public Form1()
        {
            InitializeComponent();
            this.Text = "Pong Game";//настройка формы 
            this.Size = new Size(800, 600);// устанавливаем размеры окна 
            this.StartPosition = FormStartPosition.CenterParent;//начальная позиция окна чтобы оно было по центру 
            this.DoubleBuffered = true; //убирает мерцания 
            this.KeyPreview = true;//свойства позволяет приложению первым получать события с клавы 
            this.Paint += Form1_Paint;//подключаем событие для отрисовки 
            //подключаем событие чтобы считывать нажатие клавиш
            this.KeyDown += Form1_KeyDown;//подключаем событие чтобы считывать нажатие клавиш

            //Настраиваем таймер
            timer = new Timer();//Создаем объект таймера
            timer.Interval = 16;//Устанавливаем значение 16 для 60 кадров в секунду

            timer.Tick += Timer_Tick;//Запускаем обновление игры для каждого кадра
            timer.Start();//Запускаем таймер

        }
        //Функция игры для обновления каждого кадра
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (gameRunning)
            {
                //двигаем мяч
                ballX += ballSpeedX;//Двигаем мяч по оси Х
                ballY += ballSpeedY;//Двигаем мяч по оси У
                //Отскок от вверха и низа
                if (ballY <= 0 || ballY >= this.ClientSize.Height)
                {
                    ballSpeedY = -ballSpeedY;
                }
                //отскок от левой ракетки
                if (ballX <= 50 && ballX >= 30 && ballY >= leftpaddleY && ballY <= leftpaddleY + 100)
                {
                    ballSpeedX = -ballSpeedX;
                    ballX = 50;//не даем мячу пройти сквозь
                }
                if (ballX >= this.ClientSize.Width - 70 && ballX <= this.ClientSize.Width - 50 && ballY >= rightpaddleY && ballY <= rightpaddleY + 100)
                {
                    ballSpeedX -= ballSpeedX;
                    ballX = this.ClientSize.Width - 70;
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
                if(rightpaddleY+50<ballY)
                    rightpaddleY += aiSpeed;
                else if (rightpaddleY + 50 >ballY)
                    rightpaddleY-= aiSpeed;
                //Ограничиваем движение ракеток
                leftpaddleY = Math.Max(0, Math.Min(this.ClientSize.Height - 100, leftpaddleY));//ограничиваем движение правой ракетки
                rightpaddleY = Math.Max(0, Math.Min(this.ClientSize.Height - 100, leftpaddleY));//ограничиваем движение левой ракетки
            }
            this.Invalidate();//перерисовываем
        }
        //Функция для сброса мяч после гола
        void ResetBall()
        {
            ballX = this.ClientSize.Width / 2;
            ballY = this.ClientSize.Height / 2;
            ballSpeedX = (new Random().Next(2) == 0) ? 5 : -5;
            ballSpeedY = (new Random().Next(2) == 0) ? 5 : -5; // Добавьте эту строку, если её нет
        }
        private void Form1_Paint(object sender, PaintEventArgs e)//Рисование  игры
        {
            Graphics g = e.Graphics;//создаем объект Graphics для доступа к элементам рисования 
            g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);//заливаем фон черным цветом
            Pen whitePen = new Pen(Color.White, 3);  //цвет белый
            g.DrawLine(whitePen, this.Width / 2, 0, this.Width / 2, this.Height);// рисуем центральную линию 
            g.FillRectangle(Brushes.White, 30, leftpaddleY, 15, 100); //Рисуем левую ракетку
            g.FillRectangle(Brushes.White, this.Width - 45, rightpaddleY, 15, 100);//Рисуем правую ракетку
            g.FillEllipse(Brushes.White, ballX - 8, ballY - 8, 16, 16);//рисуем мяч

            Font font = new Font("Arial", 30);   //Рисуем счет
            g.DrawString(leftscore.ToString(), font, Brushes.White, this.Width / 4, 20); //Левый счет
            g.DrawString(rightscore.ToString(), font, Brushes.White, 3 * this.Width / 4, 20); //Правый счет (ИСПРАВЛЕНО!)

            if (!gameRunning)
            {
                string winner = (leftscore >= 5) ? "Левый победитель" : "Правый победитель";
                g.DrawString(winner, font, Brushes.Yellow, this.Width / 2 - 100, this.Height / 2);
                g.DrawString("Нажмите R для перезапуска", new Font("Arial", 16), Brushes.White, this.Width / 2 - 120, this.Height / 2 + 50);

            }
        }
        // управление клавиатурой

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //левая ракетка w-вверх s-вниз
            if (e.KeyCode == Keys.W)
            {
                leftpaddleY -= 20;//елси нажата w вверх на 20 пикселей
            }
            if (e.KeyCode == Keys.S)
            {
                leftpaddleY += 20; //если нажата s вниз на 20 пикселей
            }
            //перезапуск игры
            if (e.KeyCode == Keys.R)
            {
                leftscore = 0;
                rightscore = 0;
                ResetBall();
                timer.Start();
            }
        }
    }
}