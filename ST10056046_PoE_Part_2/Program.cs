using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ST10056046_PoE_Part_2
{
    internal class Program
    {
        public delegate void alert(int amt);

        public const string FOODGROUPS = "To which food group does your ingredient belong?\n" +
            "1. Fruits and vegetables(Apples, Pineapples, Potatoes, Beans, Pumpkins, etc.)?\n" +
            "2. Grains/Carbs (Stuff such as bread, dough, muffins, pasta)\n" +
            "3. Proteins (Meat, eggs, fish, nuts and nut products, etc.)\n" +
            "4. Dairy (Milk, and milk products such as cheese and yoghurt)\n" +
            "5. Fats (Such as margarine, butter, and vegetable oils)\n" +
            "6. Spices (Any and all spices are contained in this food group)\n" +
            "7. Liquids (Water)";

        public static string choice = default;//stores input given for choices
        public static SortedDictionary<string, Recipe> recipeHolder = new SortedDictionary<string, Recipe>();
        public static List<Ingredients> inputIngred = new List<Ingredients>();
        public static List<string> inputSteps = new List<string>();
        static void Main(string[] args)
        {
            //Add prefabbed recipes
            inputIngred.Add(new Ingredients("Macaroni", 1, "Pack", "Grains/Carbs", 250));
            inputIngred.Add(new Ingredients("Water", 2, "Cup(s)", "Liquids", 10));
            inputIngred.Add(new Ingredients("Pepper", 2, "Teaspoon(s)", "Spices", 25));
            inputIngred.Add(new Ingredients("Salt", 1, "Tablespoon(s)", "Spices", 25));

            inputSteps.Add("Throw the ingredients together");
            inputSteps.Add("Stir for 20 minutes");
            inputSteps.Add("Serve hot and ready");
            inputSteps.Add("Enjoy!!");

            recipeHolder.Add("Macaroni", new Recipe(inputIngred, inputSteps));

            inputIngred.Clear();
            inputSteps.Clear();

            inputIngred.Add(new Ingredients("Bread", 2, "Slices", "Grains/Carbs", 28));
            inputIngred.Add(new Ingredients("Butter", 2, "Teaspoon(s)", "Fats", 5));
            inputIngred.Add(new Ingredients("Cheese", 2, "Slices", "Dairy", 20));

            inputSteps.Add("Cut the bread.");
            inputSteps.Add("Cut the cheese.");
            inputSteps.Add("Spread the butter and combine!");
            inputSteps.Add("Enjoy!!");

            recipeHolder.Add("Sandwich", new Recipe(inputIngred, inputSteps));

            inputIngred.Clear();
            inputSteps.Clear();

            inputIngred.Add(new Ingredients("Bread", 2, "Slices", "Grains/Carbs", 28));
            inputIngred.Add(new Ingredients("Butter", 1, "Cup(s)", "Fats", 5));
            inputIngred.Add(new Ingredients("Cheese", 2, "Slices", "Fats", 20));

            inputSteps.Add("Cut the bread.");
            inputSteps.Add("Cut the cheese.");
            inputSteps.Add("Enjoy!!");

            recipeHolder.Add("Burger", new Recipe(inputIngred, inputSteps));

            inputIngred.Clear();
            inputSteps.Clear();
            //End of prefabbed recipes

            Run();
        }

        static void Run()
        {
            Console.WriteLine("Welcome to our recipe app!");

            while (true)
            {
                Console.WriteLine("Choose an option:\n1. Display\n2. View\n3. Add\n4. Delete\n5. Exit");
                string run = Console.ReadLine();
                switch (run)
                {
                    case "1":
                        DisplayRecipes();
                        break;
                    case "2":
                        Console.WriteLine("Which recipe would you like to view?");
                        string recipeToView = Console.ReadLine();

                        if (recipeHolder.ContainsKey(recipeToView))
                        {
                            ViewRecipe(recipeToView);
                        }
                        else
                        {
                            Console.WriteLine("Recipe does not exist!");
                            Console.ReadKey();
                            Console.Clear();
                        }


                        break;
                    case "3":
                        AddRecipe();
                        break;
                    case "4":
                        Console.WriteLine("What to delete?");
                        string clear = Console.ReadLine();
                        ClearRecipe(clear);
                        break;
                    case "5":
                        Console.WriteLine("Goodbye!!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Please select an existing option!!");
                        break;
                }

            }
        }

        public static void ViewRecipe(string recipeToView)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Here is your recipe!\n\n" + recipeHolder[recipeToView].ToString());

            alert calAlert = CalAlert;
            calAlert(recipeHolder[recipeToView].GetTotalCal());

            Console.ResetColor();

        }

        public static void CalAlert(int totalcal)
        {
            if (totalcal > 300) Console.WriteLine("Calories exceed 300! Watch out!");
        }

        public static void DisplayRecipes()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;

            SortedDictionary<string, Recipe>.KeyCollection keys = recipeHolder.Keys;

            Console.WriteLine($"We have {keys.Count} recipes. Here they are:\n");
            foreach (string key in keys)
            {
                Console.WriteLine(key);
            }

            Console.ResetColor();
        }

        public static void ClearRecipe(string nameToClear)
        {
            if (!recipeHolder.ContainsKey(nameToClear))
            {
                Console.WriteLine("No recipe to remove!!");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Cleared recipe!!");
                recipeHolder.Remove(nameToClear);
                Console.ReadKey();
                Console.Clear();
            }
        }

        public static void AddRecipe()
        {
            int totalIngredients = default;
            int totalSteps = default;
            string recipeName = default;
            string name = default;
            double amount = default;
            string unitTypeChoice = default;
            string unit = default;
            string foodGroup = default;
            int calories = default;
            bool success = false;
            string step = default;
            //clears prewritten input
            inputIngred.Clear();
            inputSteps.Clear();

            while (!success)
            {
                success = true;

                Console.WriteLine("Name of your recipe?");
                recipeName = Console.ReadLine();

                if (!recipeHolder.ContainsKey(recipeName))
                {
                    do
                    {
                        try
                        {
                            Console.WriteLine("How many ingredients will your recipe use?");
                            totalIngredients = int.Parse(Console.ReadLine());
                            if (totalIngredients <= 0)
                            {
                                throw new FormatException();
                            }
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Please enter a number that is more than 0!!");
                        }
                    }
                    while (totalIngredients < 1);



                    for (int i = 0; i < totalIngredients; i++)
                    {
                        Console.WriteLine("Name of your ingredient?");
                        name = Console.ReadLine();
                        do
                        {
                            try
                            {
                                Console.WriteLine("How many of this particular ingredient is needed? ");
                                amount = double.Parse(Console.ReadLine());
                                if (amount <= 0)
                                {
                                    throw new FormatException();
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Please enter a number that is more than 0!!");
                            }
                        }
                        while (amount <= 0);

                        //

                        //resets unitTypeChoice
                        unitTypeChoice = "";
                        while ((unitTypeChoice != "1") && (unitTypeChoice != "2") && (unitTypeChoice != "3") && (unitTypeChoice != "4"))
                        {

                            Console.WriteLine("Please choose the unit size of the ingredient from the following: 1 for cup, 2 for tablespoon, 3 for teaspoon and 4 for your own unit.");
                            unitTypeChoice = Console.ReadLine().Trim();
                            //switch statement which will add the chosen unit to the indexed place of the array
                            switch (unitTypeChoice)
                            {
                                case "1":
                                    unit = "Cup(s)";
                                    break;
                                case "2":
                                    unit = "Tablespoon(s)";
                                    break;
                                case "3":
                                    unit = "Teaspoon(s)";
                                    break;
                                case "4":
                                    Console.WriteLine("Please add the name of your unit.");
                                    unit = Console.ReadLine();
                                    break;
                                default:
                                    Console.WriteLine("Please enter the correct number for the appropriate units!");
                                    break;
                            }
                        }
                        //
                        do
                        {
                            success = true;
                            Console.WriteLine(FOODGROUPS);
                            foodGroup = Console.ReadLine();

                            switch (foodGroup)
                            {
                                case "1":
                                    foodGroup = "Fruits and Veg";
                                    break;
                                case "2":
                                    foodGroup = "Grains/Carbs";
                                    break;
                                case "3":
                                    foodGroup = "Proteins";
                                    break;
                                case "4":
                                    foodGroup = "Dairy";
                                    break;
                                case "5":
                                    foodGroup = "Fats";
                                    break;
                                case "6":
                                    foodGroup = "Spices";
                                    break;
                                case "7":
                                    foodGroup = "Liquid";
                                    break;
                                default:
                                    Console.WriteLine("Please select an appropriate option for the food group!!");
                                    success = false;
                                    break;
                            }

                        }
                        while (!success);



                        do
                        {
                            try
                            {
                                Console.WriteLine("How many calories will this ingredient contain?");
                                calories = int.Parse(Console.ReadLine());
                                if (calories <= 0)
                                {
                                    throw new FormatException();
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Please enter a number that is more than 0!!");
                            }
                        }
                        while (calories <= 0);

                        inputIngred.Add(new Ingredients(name, amount, unit, foodGroup, calories));
                    }

                    do
                    {
                        try
                        {
                            Console.WriteLine("How many Steps will your recipe use?");
                            totalSteps = int.Parse(Console.ReadLine());
                            if (totalSteps <= 0)
                            {
                                throw new FormatException();
                            }
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Please enter a number that is more than 0!!");
                        }
                    }
                    while (totalSteps <= 0);

                    for (int i = 0; i < totalSteps; i++)
                    {
                        Console.WriteLine($"What to do for step {(i + 1)}:");
                        step = Console.ReadLine();
                        inputSteps.Add(step);
                    }

                    recipeHolder.Add(recipeName, new Recipe(inputIngred, inputSteps));
                }
                else
                {
                    Console.WriteLine("A recipe with this name already exists!!");
                    success = false;
                }
            }

            Console.WriteLine("Recipe successfully added!!");
            Console.ReadKey();
            Console.Clear();
        }

    }

    //No methods beyond this point Widske!!

    public class Recipe
    {
        string output = default;
        int TotalCalories = default;
        public List<Ingredients> Items;
        public List<string> Steps;

        public Recipe(List<Ingredients> ingredients, List<string> steps)
        {
            Steps = steps;
            Items = ingredients;

            output = "Ingredients: ";

            foreach (var item in Items)
            {
                output += item.ToString();
                TotalCalories += item.Calories;
            }

            output += "\nTotal calories: " + TotalCalories + "\n\nSteps: \n";


            foreach (var item in Steps)
            {
                output += item.ToString() + "\n";
            }
        }



        public int GetTotalCal()
        {
            return TotalCalories;
        }

        public override string ToString()
        {
            return output;
        }
    }

    public class Ingredients
    {
        public string toOut = default;
        public string Name = default;
        public double Amount = default;
        public string Unit = default;
        public string FoodGroup = default;
        public int Calories = default;

        public Ingredients(string name, double amount, string unit, string foodGroup, int calories)
        {
            Name = name;
            Amount = amount;
            Unit = unit;
            FoodGroup = foodGroup;
            Calories = calories;

            toOut += "\nName: " + Name + "\tAmount: " + Amount + "\tUnit: " + Unit + "\tFood Group: " + FoodGroup + "\tCalories: " + Calories;
        }

        public override string ToString()
        {
            return toOut;
        }

    }
}