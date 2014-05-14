using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class RandomExt
    {
        // Optimized for sets where max is much larger than amount
        // Use a different method if max is smaller than around 2 x amount (untested)
        public static List<int> PickSeveral(this System.Random random, int amount, int max)
        {
            List<int> pickedList = new List<int>();
            for (int n = 0; n < amount; n++)
            {
                int pickedIndex = random.Next(max);
                int originalPickedIndex = pickedIndex;
                bool down = true;
                while (pickedList.Contains(pickedIndex))
                { // Step down until zero is hit and then up to max
                    if (down)
                    {
                        pickedIndex--;
                        if (pickedIndex < 0)
                        {
                            down = false;
                            pickedIndex = originalPickedIndex + 1;
                        }
                    }
                    else
                    {
                        pickedIndex++;
                        if (pickedIndex == max)
                        { // No options left.  Break picking
                            n = amount;
                            break;
                        }
                    }
                }
                pickedList.Add(pickedIndex);
            }
            return pickedList;
        }

        public static bool NextBool(this System.Random rand)
        {
            return rand.Next(2) == 1;
        }

        public static float NextAngle(this System.Random rand)
        {
            return (float)(rand.NextDouble() * 360);
        }

        public static int NextNegative(this System.Random rand)
        {
            return rand.NextBool() ? -1 : 1;
        }

        public static float NextFloat(this System.Random rand)
        {
            return (float)rand.NextDouble();
        }

        public static Rotation NextRotation(this System.Random rand)
        {
            switch (rand.Next(4))
            {
                case 0:
                    return Rotation.ClockWise;
                case 1:
                    return Rotation.CounterClockWise;
                case 2:
                    return Rotation.OneEighty;
                default:
                    return Rotation.None;
            }
        }

        public static Rotation NextClockwise(this System.Random rand)
        {
            switch (rand.Next(2))
            {
                case 0:
                    return Rotation.ClockWise;
                default:
                    return Rotation.CounterClockWise;
            }
        }

        public static Rotation NextFlip(this System.Random rand)
        {
            switch (rand.Next(2))
            {
                case 0:
                    return Rotation.None;
                default:
                    return Rotation.OneEighty;
            }
        }

        public static GridDirection NextDirection(this System.Random rand, bool diag = false)
        {
            switch (rand.Next(4))
            {
                case 0:
                    return GridDirection.HORIZ;
                case 1:
                    return GridDirection.VERT;
                case 2:
                    return diag ? GridDirection.DIAGBLTR : GridDirection.HORIZ;
                default:
                    return diag ? GridDirection.DIAGTLBR : GridDirection.VERT;
            }
        }

        private static double spareRoll;
        private static bool hasSpare;
        // Broken
        private static double NextMargsalia(this System.Random rand, bool useSpare)
        {
            double magn;
            if (useSpare && hasSpare)
            {
                magn = spareRoll;
                hasSpare = false;
            }
            else
            {
                double roll1 = rand.NextDouble() * 2 - 1;
                double roll2 = rand.NextDouble() * 2 - 1;
                double s = Math.Pow(roll1, 2) + Math.Pow(roll2, 2);
                if (s >= 1)
                { // Retry
                    return NextNormalDist(rand, useSpare);
                }
                double commonTerm = Math.Sqrt(-2 * Math.Log(s) / s);
                magn = roll1 * commonTerm;
                if (!hasSpare)
                {
                    spareRoll = roll2 * commonTerm;
                    hasSpare = true;
                }
            }
            return magn;
        }

        public static double NextNormalDist(this System.Random rand, bool useSpare = false)
        {
            return NextMargsalia(rand, useSpare);
        }

        public static int NextNormalDist(this System.Random rand, int min, int max, double wingCutoff = 2, bool useSpare = false)
        {
            if (max < min)
            {
                throw new ArgumentException("Max must be greater than or equal to min");
            }
            if (max == min)
            {
                return max;
            }
            double magn = NextNormalDist(rand, useSpare);
            while (Math.Abs(magn) > wingCutoff)
            {
                magn = NextNormalDist(rand, true);
            }
            magn = (magn + wingCutoff) / 2 / wingCutoff;
            if (magn.EqualsWithin(1d))
            {
                return max;
            }
            return (int)((magn * (max - min) + min));
        }
    }
}
