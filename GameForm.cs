using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FlappyBird;

public class GameForm : Form
{
    private readonly Timer gameTimer = new();
    private readonly Random random = new();
    private readonly Rectangle bird = new(120, 200, 34, 26);
    private readonly List<PipePair> pipes = new();

    private int velocity;
    private int gravity = 1;
    private int jumpStrength = -12;
    private int score;
    private bool gameOver;

    public GameForm()
    {
        Text = "Flappy Bird (C#)";
        ClientSize = new Size(480, 640);
        DoubleBuffered = true;
        BackColor = Color.SkyBlue;
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;

        gameTimer.Interval = 16;
        gameTimer.Tick += (_, _) => UpdateGame();

        KeyDown += HandleKeyDown;
        MouseDown += (_, _) => Jump();

        ResetGame();
    }

    private void HandleKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            if (gameOver)
            {
                ResetGame();
                return;
            }

            Jump();
        }
    }

    private void Jump()
    {
        if (!gameOver)
        {
            velocity = jumpStrength;
        }
    }

    private void ResetGame()
    {
        score = 0;
        velocity = 0;
        gameOver = false;
        bird.Y = 200;

        pipes.Clear();
        for (int i = 0; i < 4; i++)
        {
            pipes.Add(CreatePipePair(520 + i * 180));
        }

        gameTimer.Start();
        Invalidate();
    }

    private PipePair CreatePipePair(int x)
    {
        const int gap = 150;
        int topHeight = random.Next(80, 380);

        Rectangle topPipe = new(x, 0, 70, topHeight);
        Rectangle bottomPipe = new(x, topHeight + gap, 70, ClientSize.Height - (topHeight + gap));

        return new PipePair(topPipe, bottomPipe);
    }

    private void UpdateGame()
    {
        if (gameOver)
        {
            return;
        }

        velocity += gravity;
        bird.Y += velocity;

        for (int i = 0; i < pipes.Count; i++)
        {
            PipePair pair = pipes[i];
            pair.Top.X -= 4;
            pair.Bottom.X -= 4;

            if (!pair.Scored && pair.Top.Right < bird.Left)
            {
                pair.Scored = true;
                score++;
            }

            if (pair.Top.Right < 0)
            {
                int farthestX = pipes.Max(p => p.Top.X);
                pipes[i] = CreatePipePair(farthestX + 180);
                continue;
            }

            pipes[i] = pair;

            if (bird.IntersectsWith(pair.Top) || bird.IntersectsWith(pair.Bottom))
            {
                EndGame();
            }
        }

        if (bird.Top <= 0 || bird.Bottom >= ClientSize.Height)
        {
            EndGame();
        }

        Invalidate();
    }

    private void EndGame()
    {
        gameOver = true;
        gameTimer.Stop();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        using SolidBrush groundBrush = new(Color.ForestGreen);
        g.FillRectangle(groundBrush, 0, ClientSize.Height - 40, ClientSize.Width, 40);

        using SolidBrush pipeBrush = new(Color.SeaGreen);
        foreach (PipePair pipe in pipes)
        {
            g.FillRectangle(pipeBrush, pipe.Top);
            g.FillRectangle(pipeBrush, pipe.Bottom);
        }

        using SolidBrush birdBrush = new(Color.Gold);
        g.FillEllipse(birdBrush, bird);

        using Font scoreFont = new("Segoe UI", 24, FontStyle.Bold);
        using SolidBrush textBrush = new(Color.White);
        g.DrawString($"Skor: {score}", scoreFont, textBrush, 12, 12);

        if (gameOver)
        {
            using Font gameOverFont = new("Segoe UI", 20, FontStyle.Bold);
            string text = "Oyun Bitti!\nTekrar başlamak için SPACE";
            SizeF textSize = g.MeasureString(text, gameOverFont);

            float x = (ClientSize.Width - textSize.Width) / 2;
            float y = (ClientSize.Height - textSize.Height) / 2;

            using SolidBrush overlay = new(Color.FromArgb(150, 0, 0, 0));
            g.FillRectangle(overlay, 0, 0, ClientSize.Width, ClientSize.Height);
            g.DrawString(text, gameOverFont, textBrush, x, y);
        }
    }

    private struct PipePair
    {
        public Rectangle Top;
        public Rectangle Bottom;
        public bool Scored;

        public PipePair(Rectangle top, Rectangle bottom)
        {
            Top = top;
            Bottom = bottom;
            Scored = false;
        }
    }
}
