using OSSimulator;

string[] arguments = Environment.GetCommandLineArgs();
int n = arguments.Length;
int i, j;

/// Initialize default values for all arguments
CommonParameters.TimeSlices[0] = 1;

for (i = 1; i < CommonParameters.TimeSlices.Length; i++)
{
    CommonParameters.TimeSlices[i] = 2 * CommonParameters.TimeSlices[i - 1];
}

CommonParameters.RandomSeed = 1;
CommonParameters.IORequestChance = 10;
CommonParameters.IOCompletionChance = 4;
CommonParameters.VerboseOutput = false;
CommonParameters.Policy = SchedulingPolicy.NonAggressivePreEmptive;

// parse the command line options and update the values as needed
for (i = 1; i < n; i++)
{
    string arg = arguments[i];

    for (j = 0; j < CommonParameters.TimeSlices.Length; j++)
    {
        if (arg.Equals("-" + j))
        {
            try
            {
                CommonParameters.TimeSlices[j] = int.Parse(arguments[i + 1]);
                ++i;
            }
            catch (Exception) { }
        }
    }

    if (arg.Equals("-s"))
    {
        try
        {
            CommonParameters.RandomSeed = int.Parse(arguments[i + 1]);
            CommonParameters.RandomNumberGenerator = new Random(CommonParameters.RandomSeed);
            ++i;
        }
        catch (Exception) { }
    }
    else if (arg.Equals("-r"))
    {
        try
        {
            CommonParameters.IORequestChance = int.Parse(arguments[i + 1]);
            ++i;
        }
        catch (Exception) { }
    }
    else if (arg.Equals("-c"))
    {
        try
        {
            CommonParameters.IOCompletionChance = int.Parse(arguments[i + 1]);
            ++i;
        }
        catch (Exception) { }
    }
    else if (arg.Equals("-v"))
    {
        CommonParameters.VerboseOutput = true;
    }
    else if (arg.Equals("-A"))
    {
        CommonParameters.Policy = SchedulingPolicy.AggressivePreEmptive;
        CommonParameters.InputFilePath = arguments[i + 1];
        ++i;
    }
    else if (arg.Equals("-N"))
    {
        CommonParameters.Policy = SchedulingPolicy.NonAggressivePreEmptive;
        CommonParameters.InputFilePath = arguments[i + 1];
        ++i;
    }
    else if (arg.Equals("-S"))
    {
        CommonParameters.Policy = SchedulingPolicy.PreEmptiveShortestJobFirst;
        CommonParameters.InputFilePath = arguments[i + 1];
        ++i;
    }
}

if(CommonParameters.InputFilePath == null)
{
    // take last parameter as file name
    CommonParameters.InputFilePath = arguments[arguments.Length - 1];
}

CommonParameters.Print();

Coordinator coordinator = new Coordinator();
coordinator.execute();
