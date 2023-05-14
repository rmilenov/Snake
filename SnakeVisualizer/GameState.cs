using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SnakeVisualizer
{
    public class GameState
    {
        [JsonProperty("Rows")]
        public int Rows { get; }//
        [JsonProperty("Columns")]
        public int Columns { get; }//
        [JsonProperty("Grid")]
        public GridValue[,] Grid { get; private set; }//
        [JsonProperty("Direction")]
        public Direction Direction { get; private set; }//
        [JsonProperty("Score")]
        public int Score { get; private set; }//
        private bool ScoreChanged { get; set; }
        [JsonProperty("GameOver")]
        public bool GameOver { get; private set; }//
        [JsonProperty("Player")]
        public string Player { get; set; }//
        [JsonProperty("Speed")]
        public int Speed { get; set; }//

        public readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();//
        public readonly LinkedList<Position> snakePositions = new LinkedList<Position>();//
        private readonly Random random = new Random();

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[Rows, Columns];
            Direction = Direction.Right;
            Player = "";
            Speed = 200;
            ScoreChanged = false;
            


            AddSnake();
            AddFood();

        }

        private void AddSnake()
        {
            int row = Rows / 2;
            for (int col = 1; col <= 3; col++)
            {
                Grid[row, col] = GridValue.Snake;
                snakePositions.AddFirst(new Position(row, col));
            }
        }
        private IEnumerable<Position> EmptyPositions()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Grid[row, col] == GridValue.Empty)
                    {
                        yield return new Position(row, col);
                    }
                }
            }
        }
        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count == 0) { return; }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Column] = GridValue.Food;
        }
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }
        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Direction;
            }
            return dirChanges.Last.Value;
        }
        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }
            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }
        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir)) dirChanges.AddLast(dir);
        }
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
        }
        private GridValue Collision(Position newHeadPosition)
        {
            if (OutsideGrid(newHeadPosition))
            {
                return GridValue.Outside;
            }
            if (newHeadPosition == TailPosition())
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPosition.Row, newHeadPosition.Column];
        }
        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Direction = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPosition = HeadPosition().Translate(Direction);
            GridValue hit = Collision(newHeadPosition);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                //Sound.collision.Play();
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPosition);

            }
            else if (hit == GridValue.Food)
            {

                //Sound.ding.Play();
                AddHead(newHeadPosition);
                Score++;
                ScoreChanged = true;
                ChangeSpeed();
                AddFood();

            }
        }
        public int ChangeSpeed()// Lower value means faster
        {


            if ((ScoreChanged && this.Score > 9 && this.Score % 10 == 0) && this.Speed >= 80)
            {
                this.Speed -= 20;
                this.ScoreChanged = false;
            }

            return this.Speed;
        }
    }
}
