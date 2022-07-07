using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Program;
public class mainprogram
{


    public static List<string> ContractNameList = new List<string>();
    public static BIPtemplate bIPFinishedTemplate = new BIPtemplate();
    public static string InputFilePath;
    public static string text;


    public static void Main()
    {
        GetFilePath();
        // ReadAllText automatically closes file after reading it
        text = ClearComments(text); // clear comments
        BeginBIP();
        SeparateContracts(text);
        EndBIP();

    }


    public static void GetFilePath()
    {
        Console.WriteLine("Please provide solidity file Path: ");
        InputFilePath = Console.ReadLine();


        try
        {
            text = System.IO.File.ReadAllText(InputFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine("There was an error reading the file: ");
            Console.WriteLine("PATH NOT VALID: " + e.Message);
            System.Environment.Exit(0);
        }

    }

    public static string ClearComments(string text)
    {
        string text1;
        string firstTag = "/*";
        string secondTag = "*/";
        string Tag1 = "//";
        string Tag2 = "\n";
        Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(firstTag), Regex.Escape(secondTag)), RegexOptions.Singleline); // removes comments accross multiple lines
        Regex regex1 = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(Tag1), Regex.Escape(Tag2)), RegexOptions.RightToLeft); // removes comments accross one line 
        text1 = regex1.Replace(text, "\n");
        return regex.Replace(text1, "\n");
    }

    public static void BeginBIP()
    {
        WriteTOFile("model Globalmodel" + "\n" + // WRITE TO FILE ////////////////////
                                   "port type ePort" + "\n" +
                                   "connector type SINGLE(syncPort p)" + "\n" +
                                               " define p" + "\n" +
                               "end" + "\n\n");
    }
    public static void SeparateContracts(string text)
    {
        string ContractName = "";
        text = text.Substring(text.IndexOf("contract") + "contract".Length); // Trims everything before word "contract" found.
        String[] Contracts = Regex.Split(text, "contract"); // Splits contracts by contract name

        foreach (string contract in Contracts)
        {
            foreach (char s in contract)
            {
                if (s == '{')
                {
                    break;
                }
                ContractName += s;

            }
            ContractName = ContractName.Replace(" ", ""); // removing space between
            ContractNameList.Add(ContractName);
            SeparateFunction(contract, ContractName);
            ContractName = ""; // resets contract name
        }

    }

    public static void SeparateFunction(string text, string ContractName)
    {
        List<SimpliedFunctions> function = new List<SimpliedFunctions>(); // list of functions
        String[] separateFunctions = text.Replace("function", "#function").Replace("fallback", "#fallback").Replace("receive", "#receive").Split('#'); // Each string in the array of strings contains a whole function
        List<string> list = new List<string>(separateFunctions); // converting Array of strings to list 
        List<String> States;
        string initial_state = "initial to Invoke";


        list.RemoveAt(0); // removing contract name and variables



        foreach (string s in list)
        {
            SimpliedFunctions func = new SimpliedFunctions(); // intializing a new class object 
            func.SetAllfunction(s); // adding separated functions to each class object
            function.Add(func); // Add functions to a list of functions 
        }

        bIPFinishedTemplate.SetContractName(ContractName);

        for (int i = 0; i < function.Count; i++)
        {
            function[i].SetCurrentTemplate(bIPFinishedTemplate);  // Using the same template for all classes, this template will be sent over to all functions.
            function[i].SeparatefunctionHeader();
            function[i].InterpertHeader();
            function[i].SeparateBody();
            function[i].InterpertBody();
            bIPFinishedTemplate.AddLineToTransition();
            bIPFinishedTemplate.AddLineToPorts();
            bIPFinishedTemplate.AddLineToConnector();
            bIPFinishedTemplate.TransitionGenerator();
        }
        WriteTOFile("atomic type " + ContractName + "\n"); // Write To File Function ///////////////////////////////////////////////
                                                           // define ports

        foreach (string s in bIPFinishedTemplate.PortsList)
        {
            WriteTOFile(s);// Write To File Function ///////////////////////////////////////////////
                           // define ports

        }

        // define states
        States = bIPFinishedTemplate.States.Distinct().ToList<string>();
        string JoinedStates = string.Join(",", States);
        JoinedStates = " place " + JoinedStates + "\n\n";
        WriteTOFile(JoinedStates); // Write To File Function ///////////////////////////////////////////////
        WriteTOFile(initial_state + "\n"); // Write To File Function ///////////////////////////////////////////////
                                           // set initial states


        // transition BIP

        foreach (string s in bIPFinishedTemplate.TransitionList)
        {
            WriteTOFile(s);  // Write To File Function ///////////////////////////////////////////////
        }


        WriteTOFile("end" + "\n\n");  // Write To File Function ///////////////////////////////////////////////
                                            //connectorsI
        bIPFinishedTemplate.ResetListes();
    }

    public static void EndBIP()
    {
        WriteTOFile("compound type globModel  \n"); // Write To File Function ///////////////////////////////////////////////
        foreach (string s in ContractNameList)
        {
            WriteTOFile("component " + s + " " + s + "1"); // Write To File Function ///////////////////////////////////////////////
        }



        foreach (string s in bIPFinishedTemplate.ConnectorList) // Write To File Function ///////////////////////////////////////////////
        {
            WriteTOFile(s); // Write To File Function ///////////////////////////////////////////////
        }


        //endBIP 
        WriteTOFile("end \n" +    // Write To File Function ///////////////////////////////////////////////
               "component globModel Root \n" +
               "end \n");
    }


    public static void WriteTOFile(string text)
    {
        string path = @"C:\Users\admindsi\Desktop\BIP.bip";
        // This text is always added, making the file longer over time
        // if it is not deleted.
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(text);
        }
    }
}
   

    

    




