using System;

namespace Lab2
{
    public class Program
    {
        private const string MaterialsFilePath = "Materials.json";
        private const string LocalBasePath = "LocalBase.txt";
        private const string GlobalBasePath = "GlobalBase.txt";

        static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper(MaterialsFilePath, GlobalBasePath, LocalBasePath);
            bootstrapper.Bootstrap();

            bootstrapper.UserInterface.PrintExitSuggestion();
            bootstrapper.UserInterface.OnExitPrint += EndProgram;

            new ProductionSystem(bootstrapper.Materials, Bootstrapper.Conditions,
                Bootstrapper.OnConditionValueRecieved, bootstrapper.UserInterface)
                .StartLooping();
        }

        private static void EndProgram()
        {
            Environment.Exit(0);
        }
    }
}
