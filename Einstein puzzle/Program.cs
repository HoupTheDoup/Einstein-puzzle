namespace Einstein_Puzzle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using static System.Console;

    //enums of all the possible attributes
    public enum Colour { Red, Green, White, Yellow, Blue }
    public enum Nationality { Englishman, Swede, Dane, Norwegian, German }
    public enum Pet { Dog, Birds, Cats, Horse, Zebra }
    public enum Drink { Coffee, Tea, Milk, Beer, Water }
    public enum Smoke { PallMall, Dunhill, Blend, BlueMaster, Prince }

    public static class ZebraPuzzle
    {
        private static (Colour[] colours, Drink[] drinks, Smoke[] smokes, Pet[] pets, Nationality[] nations) _solved;

        static ZebraPuzzle()
        {
            var solve = //hint 1: There are five houses.
                        from colours in Permute<Colour>()

                        //hint 5: The green house is immediately to the left of the white house.
                        where (colours, Colour.White).IsRightOf(colours, Colour.Green)

                        //hint 10: The Norwegian lives in the first house.
                        from nations in Permute<Nationality>()
                        where nations[0] == Nationality.Norwegian

                        //hint 2: The English man lives in the red house.
                        where (nations, Nationality.Englishman).IsSameIndex(colours, Colour.Red)

                        //hint 15: The Norwegian lives next to the blue house.
                        where (nations, Nationality.Norwegian).IsNextTo(colours, Colour.Blue)

                        //hint 9: In the middle house they drink milk.
                        from drinks in Permute<Drink>()
                        where drinks[2] == Drink.Milk

                        //hint 6: They drink coffee in the green house.
                        where (drinks, Drink.Coffee).IsSameIndex(colours, Colour.Green)

                        //hint 4: The Dane drinks tea.
                        where (drinks, Drink.Tea).IsSameIndex(nations, Nationality.Dane)

                        //hint 3: The Swede has a dog.
                        from pets in Permute<Pet>()
                        where (pets, Pet.Dog).IsSameIndex(nations, Nationality.Swede)

                        //hint 7: The man who smokes Pall Mall has birds.
                        from smokes in Permute<Smoke>()
                        where (smokes, Smoke.PallMall).IsSameIndex(pets, Pet.Birds)

                        //hint 8:  In the yellow house they smoke Dunhill.
                        where (smokes, Smoke.Dunhill).IsSameIndex(colours, Colour.Yellow)

                        //hint 11: The man who smokes Blend lives in the house next to the house with cats.
                        where (smokes, Smoke.Blend).IsNextTo(pets, Pet.Cats)

                        //hint 12: In a house next to the house where they have a horse, they smoke Dunhill.
                        where (smokes, Smoke.Dunhill).IsNextTo(pets, Pet.Horse)

                        //hint 13: The man who smokes Blue Master drinks beer.
                        where (smokes, Smoke.BlueMaster).IsSameIndex(drinks, Drink.Beer)

                        //hint 14: The German smokes Prince.
                        where (smokes, Smoke.Prince).IsSameIndex(nations, Nationality.German)

                        //hint 16: They drink water in a house next to the house where they smoke Blend.
                        where (drinks, Drink.Water).IsNextTo(smokes, Smoke.Blend)

                        //selects the solution
                        select (colours, drinks, smokes, pets, nations);

            //executes the query and retrieves the first solution, assigning it to the _solved
            _solved = solve.First();
        }

        //returns the possiotions of the given object
        private static int IndexOf<T>(this T[] arr, T obj) => Array.IndexOf(arr, obj);

        //sets the possition of the first object to the right of the second object
        private static bool IsRightOf<T, U>(this (T[] a, T v) right, U[] a, U v) => right.a.IndexOf(right.v) == a.IndexOf(v) + 1;

        //checks if the specified elements in two arrays are located at the same index position in their respective arrays
        private static bool IsSameIndex<T, U>(this (T[] a, T v) x, U[] a, U v) => x.a.IndexOf(x.v) == a.IndexOf(v);

        //checks if the with neighboring indexes in their pespective arrays
        private static bool IsNextTo<T, U>(this (T[] a, T v) x, U[] a, U v) => (x.a, x.v).IsRightOf(a, v) || (a, v).IsRightOf(x.a, x.v);

        // made more generic from https://codereview.stackexchange.com/questions/91808/permutations-in-c

        //generates the permutation of a collection
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> values)
        {
            if (values.Count() == 1)
                return values.ToSingleton();

            return values.SelectMany(v => Permutations(values.Except(v.ToSingleton())), (v, p) => p.Prepend(v));
        }

        //returns all permutations as arrays
        public static IEnumerable<T[]> Permute<T>() => ToEnumerable<T>().Permutations().Select(p => p.ToArray());

        //creates IEnumerable with single element in it
        private static IEnumerable<T> ToSingleton<T>(this T item) { yield return item; }

        //return the enums as of type IEnumerable to be used in the query
        private static IEnumerable<T> ToEnumerable<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static new String ToString()
        {
            WriteLine(_solved);
            var sb = new StringBuilder();
            sb.AppendLine("House Colour Drink    Nationality Smokes     Pet");
            sb.AppendLine("───── ────── ──────── ─────────── ────────── ─────");
            var (colours, drinks, smokes, pets, nations) = _solved;
            for (var i = 0; i < 5; i++)
                //writes to console: index + 1 with max width of 5 characters 
                //                   colour[i] with max width of 6 charachets, alighned to the left
                //                   drinks[i] with max width of 8 charachets, alighned to the left
                //                   nations[i] with max width of 11 charachets, alighned to the left
                //                   smokes[i] with max width of 10 charachets, alighned to the left
                //                   pets[i] with max width of 10 charachets, alighned to the left
                sb.AppendLine($"{i + 1,5} {colours[i],-6} {drinks[i],-8} {nations[i],-11} {smokes[i],-10} {pets[i],-10}");
            return sb.ToString();
        }

        public static void Main(string[] arguments)
        {
            var owner = _solved.nations[_solved.pets.IndexOf(Pet.Zebra)];
            WriteLine($"The zebra owner is {owner}");
            Write(ToString());
            Read();
        }
    }
}