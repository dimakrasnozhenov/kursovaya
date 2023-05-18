namespace Kursovaya
{
    internal class BinarySearch
    {
        private static int Finder(string keyword, string word)
        {
            if (keyword == word)
            {
                return 0;
            }

            int keywordc = 0;
            int wordc = 0;
            while (true)
            {
                if (keyword[keywordc] == word[wordc])
                {
                    keywordc++;
                    wordc++;
                    continue;
                }
                if (keyword[keywordc] < word[wordc])
                {
                    keywordc++;
                    wordc++;
                    return -1;
                }
                if (keyword[keywordc] > word[wordc])
                {
                    keywordc++;
                    wordc++;
                    return 1;
                }
            }
        }
        public int Find(string[] data, string keyword)
        {
            int FirstIndex = 0;
            int LastIndex = data.Length - 1;
            while (FirstIndex <= LastIndex)
            {
                int MiddelIndex = (FirstIndex + LastIndex) / 2;
                string word = data[MiddelIndex];
                int find = Finder(keyword, word);
                if (find == 1)
                {
                    FirstIndex = MiddelIndex + 1;
                }
                else if (find == -1)
                {
                    LastIndex = MiddelIndex - 1;
                }
                else
                {
                    return MiddelIndex;
                }
            }

            return -1;
        }
    }
}
