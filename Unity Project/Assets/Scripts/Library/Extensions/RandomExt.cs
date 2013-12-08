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
    }
}
