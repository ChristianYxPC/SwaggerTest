namespace SwaggerTest.Helpers
{

    public static class RandomHelper
    {
        public static bool GetRandomBoolean()
        {
            return GetRandomInt(0, 100) % 2 == 1;
        }

        public static int GetRandomInt(int minValue = 0, int maxValue = 100)
        {
            return Random.Shared.Next(minValue, maxValue);
        }

        public static string GetRandomName()
        {
            return RandomName.GenerateName();
        }

        public static char GetRandomSmallLetter()
        {
            return RandomName.RandomLetter();
        }

        public static char GetRandomCapitalLetter()
        {
            return RandomName.RandomLetter(true);
        }

        #region Random Names
        private class RandomName
        {
            static int a = (int)'a'; static int z = (int)'z';
            static int A = (int)'A'; static int Z = (int)'Z';
            static int[] Vowel = new int[] { (int)'a', (int)'e', (int)'i', (int)'o', (int)'u' };
            static int[] CapitalVowel = new int[] { (int)'A', (int)'E', (int)'I', (int)'O', (int)'U' };

            public static string[] GenerateNames(int maxNameLength = 10, int numberOfName = 1)
            {
                string[] names = new string[numberOfName];
                for (int count = 0; count < numberOfName; count++)
                {

                    names[count] = GenerateName(maxNameLength);
                }
                return names;
            }

            public static string GenerateName(int maxNameLength = 10)
            {
                string name = "";
                int length = Random.Shared.Next(5, 10);
                char firstChar = RandomLetter(true);
                name += firstChar;
                bool isVowel = IsVowel(firstChar);
                for (int index = 1; index < length; index++)
                {
                    name += isVowel ? RandomLetter(false, true) : RandomVowel();
                    isVowel = !isVowel;
                }
                Console.WriteLine(name);
                return name;
            }

            public static bool IsVowel(char character)
            {
                return CapitalVowel.Contains((int)character) || Vowel.Contains((int)character);
            }

            public static char RandomVowel(bool isCapital = false)
            {
                return (char)(isCapital ? CapitalVowel : Vowel)[Random.Shared.Next(0, 4)];
            }

            public static char RandomLetter(bool isCapital = false, bool excludeVowel = false)
            {
                int character = 0;
            GENERATE_CHAR:
                character = isCapital ? Random.Shared.Next(A, Z) : Random.Shared.Next(a, z);
                if (excludeVowel && IsVowel((char)character))
                {
                    goto GENERATE_CHAR;
                }
                return (char)character;
            }
        }
        #endregion
    }
}
