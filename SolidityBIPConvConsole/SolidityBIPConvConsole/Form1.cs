using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;


namespace SolidityBIPConvConsole
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, EventArgs e)
        {
            ClearBTN_Click(sender,e); // call function Clean button to clear text.
            string text = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    text = File.ReadAllText(file);
                    InputBox.Text = (text);
                    Main NewMAin = new Main();
                    NewMAin.MainMethod(text, OutputBox);
                }
                catch (IOException)
                {
                    MessageBox.Show("ERROR UPLOADING FILE", "ERROR UPLOAD FILE",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }


        


        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            InputBox.ScrollBars = ScrollBars.Both;
        }
        private void OutputBox_TextChanged(object sender, EventArgs e)
        {
            OutputBox.ScrollBars = ScrollBars.Both;
        }

        private void ClearBTN_Click(object sender, EventArgs e)
        {
            InputBox.Clear();
            OutputBox.Clear();
        }

        private void SaveBTN_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "bip files (*.bip)|*.bip|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Close();
                }
            }
            string Output = OutputBox.Text;
            string path = @Path.GetFullPath(saveFileDialog1.FileName);
            File.WriteAllText(path, Output); // Gets text from output box and saves to a new file
        }

    }

    public class Main
    {
        public List<string> ContractNameList = new List<string>();
        public BIPtemplate bIPFinishedTemplate = new BIPtemplate();
        public TextBox OutPutBox;

        public void MainMethod(String FileString, TextBox ReceivedOutputBox)
        {
            OutPutBox = ReceivedOutputBox;
            string Text = FileString;
            //OutputBox.Text = Text;
            // ReadAllText automatically closes file after reading it
            Text = ClearComments(Text); // clear comments
            BeginBIP();
            SeparateContracts(Text);
            EndBIP();
        }


        public string ClearComments(string text)
        {
            string text1;
            string firstTag = "/*";
            string secondTag = "*/";
            string Tag1 = "//";
            string Tag2 = Environment.NewLine;
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(firstTag), Regex.Escape(secondTag)), RegexOptions.Singleline); // removes comments accross multiple lines
            Regex regex1 = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(Tag1), Regex.Escape(Tag2)), RegexOptions.RightToLeft); // removes comments accross one line 
            text1 = regex1.Replace(text, Environment.NewLine);
            return regex.Replace(text1, Environment.NewLine);
        }

        public void BeginBIP()
        {
            OutPutBox.AppendText("model Globalmodel" + Environment.NewLine + // WRITE TO FILE ////////////////////
                                   "port type ePort" + Environment.NewLine +
                                   "connector type SINGLE(syncPort p)" + Environment.NewLine +
                                               " define p" + Environment.NewLine +
                               "end" + Environment.NewLine);

        }
        public void SeparateContracts(string text)
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

        public void SeparateFunction(string text, string ContractName)
        {
            List<SimpliedFunctions> function = new List<SimpliedFunctions>(); // list of functions
            String[] separateFunctions = text.Replace("function", "#function").Replace("fallback", "#fallback").Replace("receive", "#receive").Split('#'); // Each string in the array of strings contains a whole function
            List<string> list = new List<string>(separateFunctions); // converting Array of strings to list 
            List<String> States;
            string initial_state = "initial to invoke";


            list.RemoveAt(0); // removing contract name and variables



            foreach (string s in list)
            {
                SimpliedFunctions func = new SimpliedFunctions(); // intializing a new class object 
                func.SetAllfunction(s); // adding separated functions to each class object
                function.Add(func); // Add functions to a list of functions 
            }

            bIPFinishedTemplate.SetContractName(ContractName + "1");

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
            OutPutBox.AppendText("atomic type " + ContractName + Environment.NewLine); // Write To File Function ///////////////////////////////////////////////
                                                                     // define ports

            foreach (string s in bIPFinishedTemplate.PortsList)
            {
                OutPutBox.AppendText(s + Environment.NewLine);

            }

            // define states
            States = bIPFinishedTemplate.States.Distinct().ToList<string>();
            string JoinedStates = string.Join(",", States);
            JoinedStates = " place " + JoinedStates ;
            OutPutBox.AppendText(JoinedStates + Environment.NewLine); // Write To File Function ///////////////////////////////////////////////
            OutPutBox.AppendText(initial_state + Environment.NewLine); // Write To File Function ///////////////////////////////////////////////
                                                     // set initial states


            // transition BIP

            foreach (string s in bIPFinishedTemplate.TransitionList)
            {
                OutPutBox.AppendText(s + Environment.NewLine);  // Write To File Function ///////////////////////////////////////////////
                
            }


            OutPutBox.AppendText("end" + Environment.NewLine);  // Write To File Function ///////////////////////////////////////////////
                                       //connectors
            bIPFinishedTemplate.ResetLists();
        }

        public void EndBIP()
        {
            OutPutBox.AppendText("compound type globModel" + Environment.NewLine); // Write To File Function ///////////////////////////////////////////////
            foreach (string s in ContractNameList)
            {
                OutPutBox.AppendText("component " + s + " " + s + "1" + Environment.NewLine); // Write To File Function ///////////////////////////////////////////////
            }



            foreach (string s in bIPFinishedTemplate.ConnectorList) // Write To File Function ///////////////////////////////////////////////
            {
                OutPutBox.AppendText(s + Environment.NewLine); // Write To File Function ///////////////////////////////////////////////
            }


            //endBIP 
            OutPutBox.AppendText("end " + Environment.NewLine +    // Write To File Function ///////////////////////////////////////////////
               "component globModel Root " + Environment.NewLine +
               "end " + Environment.NewLine);
        }
}


    public class BIPtemplate
    {
        public Dictionary<string, string> TheoremOfTransformation = new Dictionary<string, string>(){
    {"fallback", "function_"}, // Header theorems start here
    {"receive", "function_"},
    {"function", "function_"},
    {"public", "public_"},
    {"external", "external_"},
    {"internal", "internal_"},
    {"private", "private_"},
    {"non_payable", "nonpayable_"},
    {"payable", "payable_"},
    {"tx.origin", "tx.origin"}, // Body Theorems start here
    {"selfdestruct", "Selfdestruct_"},
    {"delegatecall", "Delegatecall_"},
    {"this.balance ==", "this.balance"},
    {"if(", "Ifcondition_"},
    {"elseif", "ElseIfcondition_"},
    {"else{", "Else_"},
    {"while(", "WhileLoop_"},
    {"for(", "ForLoop_"},
    {"require(", "Requirecondition_"},
    {"assert(", "Assertcondition_"},
    {".transfer(", "address_sending_ether_transfer"},
    {".call", "Sending_ether_call"},
    {".send(", "Sending_ether_send"},
    {"Invoke", "Invoke"},
       };

        public Dictionary<string, string> MathOperators = new Dictionary<string, string>(){
    {"+", "add"}, // Header theorems start here
    {"-", "sub"},
    {"/", "div"},
    {"*", "mult"},
    {"+=", "add"},
    {"-=", "sub"},
    {"/=", "div"},
    {"*=", "mult"},
       };

        public string[] MathOperatorsList = { "+", "-", "/", "*", "+=", "-=", "/=", "*=" };
        public string[] TheoremList = { "tx.origin", "selfdestruct", "delegatecall", "this.balance ==", "if(", "for(", "require(", "while(", "elseif", ".transfer(", ".call", ".send(", "else{" }; // Transfer, Call, send, and else do not have counters
        public int[] StatesCounterArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // counter for: "if(", "for(" , "require(", State counter (Ref num 9)
        public List<string> PortsList = new List<string>(); // states (place s1, s2, s3, ...)
        public List<string> TransitionList = new List<string>(); // states (place s1, s2, s3, ...)
        public List<string> ConnectorList = new List<string>(); // states (place s1, s2, s3, ...)
        public List<string> States = new List<string>(); // states (place s1, s2, s3, ...)

        public string ContractName;

        //Incrementer for all statements above.


        private string[] TransitionName;
        private int TransitionCounter = 0;

        public string ports; // ports String
        public string Transitions; // Transitions String
        public string Connector; // connectors String

        // start class BIP
        // variables
        // initial state ( initial to invoke )
        // end class BIP
        public BIPtemplate()
        {
            TransitionName = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", " Y", "Z" };
        }

        public void SetContractName(string CName)
        {
            ContractName = CName;
        }

        public void TransitionGenerator()
        {
            TransitionCounter++;
        }

        public string GetCurrentTransitionName()
        {
            return TransitionName[TransitionCounter];
        }

        public Dictionary<string, string> GetHeaderTheorom()
        {
            return TheoremOfTransformation;
        }

        public void AddToPorts(string P)
        {
            PortsList.Add(P);
            ports += P;
        }

        public void AddToTransitions(string T)
        {
            TransitionList.Add(T);
            Transitions += T;
            Console.WriteLine(T);
        }

        public void AddToConnector(string C)
        {
            ConnectorList.Add(C);
            Connector += C;
        }

        public void AddToStates(string S)
        {
            States.Add(S);
        }
        public void AddLineToTransition()
        {
            TransitionList.Add(Environment.NewLine);
        }

        public void AddLineToPorts()
        {
            PortsList.Add(Environment.NewLine);
        }
        public void AddLineToConnector()
        {
            ConnectorList.Add(Environment.NewLine);
        }

        public void ResetLists()
        {
            PortsList.Clear();
            TransitionList.Clear();
            States.Clear();
        }





    }
public class SimpliedFunctions
{
    private string AllFunction;
    private String[] ReceivedVariables; // Received variables in header
    private string header = "";
        private string FunctionBody;
    BIPtemplate BIPTemplateC = new BIPtemplate();
    string StateSource = "Invoke"; // Source transition 
    string StateDestination = "Invoke"; // Destination transition 
    string StatementCondition;


    int TransitionCounter = 0;
    private string[] words; // Function header separate / Function name
    private string LastIfCondition;
    private int LastIfConditionNum;
    public int ElseCounter = 0;

    List<string> LoopCondA = new List<string>(); // incremeneter Condition
    List<string> LoopCondB = new List<string>(); // For loop Not Satisfied Condition
                                                 // 2 lists instead of one 2D array for functionality reasons

    List<int> IfCondition = new List<int>();     // If Statements Not satisfied Conditions
    List<int> ElseIfCondition = new List<int>();     // If Statements Not satisfied Conditions
    List<int> WhileCondition = new List<int>();  // While Loop Not satisfied Conditions



    string[] SplitConditions; // for ForLoop



    public void SetAllfunction(string value) { AllFunction = value; }
    public string GetAllFunction() { return AllFunction; }

    public void SeparatefunctionHeader()
    {
        string ReceivedVariablesString = "";


        foreach (char c in AllFunction) // takes all chars until char '{' is current char in loop, indicating the ending of the header 
        {
            header += c;
            if (c == '{')
            {
                break;
            }
        }


        for (int i = 0; i < header.Length; i++)
        {

            if (header[i] == '(')
            {
                i++;
                while (header[i] != ')')
                {
                    ReceivedVariablesString += header[i];
                    i++;
                }
            }
        }

        ReceivedVariables = ReceivedVariablesString.Split(','); // Saved All Received (input) variables separated by comma in an array of strings 
        header = RemoveBetween(header, "(", ")"); // Remove all variables inside header
        header = Regex.Replace(header, "[()]", ""); //remove paranthesis inside header
        header = Regex.Replace(header, "[{}]", ""); //remove paranthesis inside header

    }
    public void InterpertHeader() // Interperts header to second language 
    {
        words = header.Split(' ');

        foreach (string c in words)
        {
            if (BIPTemplateC.TheoremOfTransformation.ContainsKey(c))
            {

                if (c == "fallback")    // TO DO LIST ( make a function to dynamically print/manipulate varouis if statements below. Possibly overriding function works.
                {
                    WriteToTemplate(c, "fallback", 1);
                }
                else if (c == "receive")    // revision, si les function receive et fallback peuvent etre les deux dans la meme contrat
                {
                    WriteToTemplate(c, "receive", 1);
                }
                else if ((c == "external") && (StateSource == "function_receive"))
                {
                    WriteToTemplate(c, "receive", 1);
                }
                else if ((c == "payable") && (StateSource == "external_receive"))
                {
                    WriteToTemplate(c, "receive", 1);
                }
                else if ((c == "external") && (StateSource == "function_fallback"))
                {
                    WriteToTemplate(c, "fallback", 1);
                }
                else if ((c == "payable") && (StateSource == "external_fallback"))
                {
                    WriteToTemplate(c, "fallback", 1);
                }
                else
                {
                    WriteToTemplate(c, words[1], 1);
                }

                //Console.WriteLine(StateSource + "ENTERED");
                //Console.WriteLine(c);
                //Console.WriteLine(StateSource+ "  " + StateDestination + " " + c);
            }
        }
    }


    public void InterpertBody()
    {
        //FunctionBody = FunctionBody.Replace(" ", string.Empty).Replace("\t", string.Empty);
        int Incrementer = -1;
        int SemiColenCounter = 0;// 3 -1 =2
        string CurrentString = ""; //resets
        string FullString = ""; // Does not reset 
        string IfStatement = "if("; // will be sent instead of "else and else if". ( Else if and Else will be treated as an "IfCondition_".


        int TempCounter;
        FunctionBody = FunctionBody.Replace(" ", ""); // removing space between 

        while (!(BIPTemplateC.TheoremList.Contains(CurrentString)))
        {
            Incrementer++;
            CurrentString += FunctionBody[Incrementer];
            FullString += FunctionBody[Incrementer];


            SemiColenCounter = SemiColCounter(Incrementer, FunctionBody, SemiColenCounter);
            //Console.WriteLine(CurrentString);

            foreach (string x in BIPTemplateC.TheoremList)
            {
                if (CurrentString.Contains(x))
                {
                    StatementCondition = ""; // resets reusable variables for conditions 
                                             //Console.WriteLine(StateSource + StateDestination); //////////////////////////////////////////////////////////////////////////////////////

                    if (x == "if(") // If Statements
                    {
                        TempCounter = BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, IfStatement)];
                        WriteToTemplate(IfStatement, "" + TempCounter, 1); // Send Empty Function name, alongside the counter
                        CurrentString = ""; // Reset string if found
                        while (!((FunctionBody[Incrementer] == '\r') || (FunctionBody[Incrementer] == '\n'))) // Take everything until line ends. 
                        {
                            SemiColenCounter = SemiColCounter(Incrementer, FunctionBody, SemiColenCounter);
                            StatementCondition += FunctionBody[Incrementer];
                            Incrementer++;
                        }
                        Incrementer++;
                        // make one specific for If statement, because it does not accept split conditions


                        StatementCondition = ReturnCharactersInsideParentheses(StatementCondition);

                        WriteToTemplate(IfStatement, "If_Condition_Not_Satisfied_" + TempCounter, 7); // To be last state (goes from NotSatisfied to following statemenet)
                        WriteToTemplate(IfStatement, "If_Condition_Satisfied_" + TempCounter, 7);

                        IfCondition.Add(TempCounter);
                        BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, IfStatement)]++;
                        break;
                    }
                    if (x == "elseif") // If Statements
                    {
                        BIPTemplateC.PortsList.RemoveAt(BIPTemplateC.PortsList.Count - 1);
                        BIPTemplateC.TransitionList.RemoveAt(BIPTemplateC.TransitionList.Count - 1);
                        BIPTemplateC.ConnectorList.RemoveAt(BIPTemplateC.ConnectorList.Count - 1);
                        TransitionCounter--;

                        TempCounter = BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)];
                        StateSource = LastIfCondition;
                        WriteToTemplate(x, "" + TempCounter, 1); // Send Empty Function name, alongside the counter
                        CurrentString = ""; // Reset string if found
                        while (!((FunctionBody[Incrementer] == '\r') || (FunctionBody[Incrementer] == '\n'))) // Take everything until line ends. 
                        {
                            SemiColenCounter = SemiColCounter(Incrementer, FunctionBody, SemiColenCounter);
                            StatementCondition += FunctionBody[Incrementer];
                            Incrementer++;
                        }
                        Incrementer++;
                        // make one specific for If statement, because it does not accept split conditions


                        StatementCondition = ReturnCharactersInsideParentheses(StatementCondition);

                        WriteToTemplate(x, "ElseIf_Condition_Not_Satisfied_" + TempCounter, 7); // To be last state (goes from NotSatisfied to following statemenet)
                        WriteToTemplate(x, "ElseIf_Condition_Satisfied_" + TempCounter, 7);

                        ElseIfCondition.Add(TempCounter);
                        BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)]++;

                        break;

                    }
                    if (x == "else{") // If Statements
                    {
                        BIPTemplateC.PortsList.RemoveAt(BIPTemplateC.PortsList.Count - 1);
                        BIPTemplateC.TransitionList.RemoveAt(BIPTemplateC.TransitionList.Count - 1);
                        BIPTemplateC.ConnectorList.RemoveAt(BIPTemplateC.ConnectorList.Count - 1);
                        TransitionCounter--;

                        StateSource = LastIfCondition;

                        WriteToTemplate(x, "" + LastIfConditionNum, 1); // Send Empty Function name, alongside the counter
                        ElseCounter++;
                        CurrentString = ""; // Reset string if found
                        break;
                    }

                    if (x == "for(") //ForLOOP
                    {
                        CurrentString = ""; // Reset string if found
                        while (!((FunctionBody[Incrementer] == '\r') || (FunctionBody[Incrementer] == '\n'))) // Take everything until line ends. 
                        {
                            SemiColenCounter = SemiColCounter(Incrementer, FunctionBody, SemiColenCounter);
                            StatementCondition += FunctionBody[Incrementer];
                            Incrementer++;
                        }

                        StatementCondition = ReturnCharactersInsideParentheses(StatementCondition);
                        SplitConditions = StatementCondition.Split(';');
                        TempCounter = BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)];
                        WriteToTemplate(x, "" + TempCounter, 1); // Send Empty Function name, alongside the counter
                        WriteToTemplate(x, "", 3);

                        WriteToTemplate(x, "ForLoopCondition_NotSatisfied_" + TempCounter, 2); // To be last state (goes from NotSatisfied to following statemenet)
                        WriteToTemplate(x, "ForLoopCondition_Satisfied_" + TempCounter, 2);
                        LoopCondA.Add("Incrementer_" + TempCounter);
                        LoopCondB.Add("ForLoopCondition_NotSatisfied_" + TempCounter);

                        BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)]++; // For loop counter
                        break;
                    }
                    if (x == "while(") // If Statements
                    {
                        CurrentString = ""; // Reset string if found
                        while (!((FunctionBody[Incrementer] == '\r') || (FunctionBody[Incrementer] == '\n'))) // Take everything until line ends. 
                        {
                            SemiColenCounter = SemiColCounter(Incrementer, FunctionBody, SemiColenCounter);
                            StatementCondition += FunctionBody[Incrementer];
                            Incrementer++;
                        }
                        StatementCondition = ReturnCharactersInsideParentheses(StatementCondition);
                        TempCounter = BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)];

                        WriteToTemplate(x, "" + TempCounter, 1);
                        WriteToTemplate(StateSource, "WhileLoopCondition_" + TempCounter, 6);
                        WriteToTemplate("WhileLoopCondition_" + TempCounter, "WhileLoopCondition_Not_Satisfied_" + TempCounter, 8); // To be last state (goes from NotSatisfied to following statemenet)
                        WriteToTemplate("WhileLoopCondition_" + TempCounter, "WhileLoopCondition_Satisfied_" + TempCounter, 8);

                        WhileCondition.Add(TempCounter);
                        BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)]++; // For loop counter
                        break;
                    }

                    if (x == "require(" || x == "assert(") //ForLOOP
                    {
                        TempCounter = BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)];
                        CurrentString = "";
                        WriteToTemplate(x, "" + TempCounter, 1); // Send Empty Function name, alongside the counter
                        while (!((FunctionBody[Incrementer] == '\r') || (FunctionBody[Incrementer] == '\n')))
                        {
                            StatementCondition += FunctionBody[Incrementer];
                            Incrementer++;
                        }
                        StatementCondition = ReturnCharactersInsideParentheses(StatementCondition);
                        StatementCondition = StatementCondition.Replace(" ", "");

                        if (StatementCondition.Contains("tx.origin"))
                        {
                            StatementCondition = "tx.origin_" + TempCounter;
                        }
                        else if (StatementCondition.Contains("delegatecall"))
                        {
                            StatementCondition = "delegatecall_" + TempCounter;
                        }
                        else
                        {
                            StatementCondition = "condition_" + TempCounter;
                        }

                        WriteToTemplate(x, "NotSatisfied_" + StatementCondition, 7); // Not satisfied
                        WriteToTemplate("Invoke", "", 1); // Satisfied
                        WriteToTemplate(x, StatementCondition, 7); // To be last state (goes from NotSatisfied to following statemenet)


                        StatementCondition = ReturnCharactersInsideParentheses(StatementCondition);
                        BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)]++;
                        break;
                    }

                    if (x == ".send(" || x == ".call" || x == ".transfer(")
                    {
                        CurrentString = ""; // Reset string if found
                        WriteToTemplate(x, "_" + words[1], 1);
                        Incrementer = IncrementUntilSemiColon(FunctionBody, Incrementer); // Increment until semicolen/new line is found
                        Incrementer++;

                        break;
                    }
                    else // Only 4 cases are left ( Ref: SelfDestruct, Delegatecall,Tx.Origin, this.balance
                    {
                        WriteToTemplate(x, "" + BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)], 1);
                        BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, x)]++;
                        Incrementer = IncrementUntilSemiColon(FunctionBody, Incrementer); // Increment until semicolen/new line is found


                        Incrementer++;
                        CurrentString = ""; // Reset string if found
                        break;
                    }

                }
            }

            if (FunctionBody[Incrementer] == ';')
            {
                bool MathOperator = false;

                (CurrentString, MathOperator) = MathOperation(CurrentString);
                if (MathOperator == true)
                {
                    WriteToTemplate(CurrentString, "", 9);
                }
                else
                {
                    WriteToTemplate("", "", 0);
                    BIPTemplateC.StatesCounterArray[9]++;
                }

                CurrentString = "";
            }

            if (SemiColenCounter == 0) // ENd of string, while loop exits from here 
            {
                WriteToTemplate("Invoke", "", 1);
                //Console.WriteLine(CurrentString);
                //Console.WriteLine(SemiColenCounter);
                break;
            }

            if (FunctionBody[Incrementer] == '}')
            {
                string tempS = "";
                int tempCounter = 0;
                int i = Incrementer;
                string IncrementString = "";
                if (LoopCondA.Count != 0 || IfCondition.Count != 0 || WhileCondition.Count != 0 || ElseIfCondition.Count != 0 || ElseCounter != 0)
                {
                    for (; i >= 0; i--)
                    {
                        tempCounter = SemiColCounter(i, FunctionBody, tempCounter);
                        if (tempCounter == 0 && FunctionBody[i] == '{')
                        {
                            break;
                        }
                    }

                    while (!((FunctionBody[i] == '\r') || (FunctionBody[i] == '\n')))
                    {
                        tempS += FunctionBody[i];
                        i--;
                    }
                    tempS = string.Join(" ", tempS.Split(' ').Select(x => new String(x.Reverse().ToArray()))); // reverse to read from beginning of line



                    for (int j = 0; j < tempS.Length; j++)
                    {
                        IncrementString += tempS[j];

                        if (IncrementString.Contains("for("))
                        {
                            WriteToTemplate(LoopCondB.Last(), LoopCondA.Last(), 4);
                            LoopCondA.RemoveAt(LoopCondA.Count - 1);
                            LoopCondB.RemoveAt(LoopCondB.Count - 1);
                            break;
                        }

                        if (IncrementString.Contains("if("))
                        {
                            WriteToTemplate("", "If_condition_Close_" + IfCondition.Last(), 5);
                            WriteToTemplate("If_Condition_Not_Satisfied_" + IfCondition.Last(), "If_condition_Close_" + IfCondition.Last(), 6);
                            LastIfCondition = "If_Condition_Not_Satisfied_" + IfCondition.Last();
                            LastIfConditionNum = IfCondition.Last();
                            // WORK HERE TOMORROW, add If ConditionCLose
                            IfCondition.RemoveAt(IfCondition.Count - 1);
                            break;
                        }

                        if (IncrementString.Contains("elseif"))
                        {
                            WriteToTemplate("", "If_condition_Close_" + LastIfConditionNum, 5);
                            WriteToTemplate("ElseIf_Condition_Not_Satisfied_" + ElseIfCondition.Last(), "If_condition_Close_" + LastIfConditionNum, 6);
                            LastIfCondition = "ElseIf_Condition_Not_Satisfied_" + ElseIfCondition.Last();
                            ElseIfCondition.RemoveAt(ElseIfCondition.Count - 1);
                            break;
                        }

                        if (IncrementString.Contains("else{"))
                        {
                            WriteToTemplate("", "If_condition_Close_" + LastIfConditionNum, 5);
                            StateSource = "If_condition_Close_" + LastIfConditionNum;
                            ElseCounter--;
                            break;
                        }

                        if (IncrementString.Contains("while("))
                        {
                            WriteToTemplate("Whileloop_Condition_Not_Satisfied_" + WhileCondition.Last(), "WhileLoopCondition_" + WhileCondition.Last(), 4);

                            WhileCondition.RemoveAt(WhileCondition.Count - 1);
                            break;
                        }
                    }
                }



            }

        }
    }

    public (string, bool) MathOperation(string myString)
    {
        string ReturnString = "";
        bool exist = false;
        bool insideCond = false;
        string[] substring = new string[10]; // maximum of 10 possible 
        myString = Regex.Replace(myString, "[({});]", ""); //remove paranthesis inside header
        myString = myString.Replace(" ", "").Replace(Environment.NewLine, "").Replace("\r", "").Replace("\t", ""); // removing space between 
        for (int i = 4; i < BIPTemplateC.MathOperatorsList.Length; i++)
        {
            if (myString.Contains(BIPTemplateC.MathOperatorsList[i]))
            {
                insideCond = true;
                exist = true;
                ReturnString = BIPTemplateC.MathOperatorsList[i];
                substring = Regex.Split(myString, Regex.Escape(BIPTemplateC.MathOperatorsList[i]));
                break;
            }
        }

        if (!insideCond)
        {
            for (int i = 0; i < 4; i++)
            {
                if (myString.Contains(BIPTemplateC.MathOperatorsList[i]))
                {
                    exist = true;
                    myString = myString.Replace("=", "_");
                    ReturnString = BIPTemplateC.MathOperatorsList[i];
                    substring = Regex.Split(myString, Regex.Escape(BIPTemplateC.MathOperatorsList[i]));
                    break;
                }
            }
        }

        if (BIPTemplateC.MathOperators.ContainsKey(ReturnString))
        {
            ReturnString = BIPTemplateC.MathOperators[ReturnString];
            foreach (string s in substring)
            {
                ReturnString += "_" + s;
            }
        }

        return (ReturnString, exist);
    }
    public int CountWords(string word, string myString)
    {
        string ifPattern = Regex.Escape(word); // For clearlity reasons, kept as separate string search
        int number = Regex.Matches(myString, ifPattern).Count;
        return number;
    }

    public int IncrementUntilSemiColon(string String, int number) // Increment until semicolen/new line is found
    {
        while (!((String[number] == '\r') || (String[number] == '\n') || (String[number] == ';'))) // If found in theorimlist, skip until new line then later break.
        {
            number++;
        }
        return number;
    }

    private int SemiColCounter(int Incrementer, string Body, int Counter)
    {
        if (Body[Incrementer] == '{')
        {
            Counter++;
        }

        if (Body[Incrementer] == '}')
        {
            Counter--;
        }
        return Counter;
    }

    public void SeparateBody() // Separates Body from header
    {
        FunctionBody = ReturnStringBtwSemiColons(AllFunction);

    }

    private void WriteToTemplate(string c, string FunctionName, int StatementIndicator) // Write to template / Used for everything else.
    {
        string ToAddToTransition; // 
        string ToAddToPorts;  //
        string toAddToConnector;  // Variables to store each transition in for clarity reasons. 
        switch (StatementIndicator)
        {
            case 0:

                StateDestination = "state_" + BIPTemplateC.StatesCounterArray[9] + "_" + words[1] + "Function"; // "Words[1]" stands for function name
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination);

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;
            case 1:

                StateDestination = BIPTemplateC.TheoremOfTransformation[c] + FunctionName;
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination);

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;
            case 2: // Specific to for loop satisfied/ Not satisfied

                StateSource = BIPTemplateC.TheoremOfTransformation[c] + BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, c)];
                StateDestination = FunctionName;
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination + " provided( " + SplitConditions[1] + ") //Please modify condition");

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;
            case 3: // Specific 

                StateSource = "Incrementer_" + BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, c)];
                StateDestination = BIPTemplateC.TheoremOfTransformation[c] + BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, c)];
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination + " do { " + SplitConditions[2] + ";}");

                WriteToTemplateExtention(StateSource, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;
            case 4: // C For LoopCondA (incremeneter Condition)
                    // FunctionName for LoopCondB (Not Satisfied Condition)

                StateDestination = FunctionName;
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination);

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector);
                StateSource = c;
                break;
            case 5:
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + FunctionName);

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector);

                break;
            case 6:
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + c + " to " + FunctionName);


                WriteToTemplateExtention(FunctionName, ToAddToTransition, ToAddToPorts, toAddToConnector);
                break;
            case 7: // If Statement Satisfied 

                StateSource = BIPTemplateC.TheoremOfTransformation[c] + BIPTemplateC.StatesCounterArray[Array.IndexOf(BIPTemplateC.TheoremList, c)];
                StateDestination = FunctionName;
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination + " provided( " + StatementCondition + " ) //Please modify condition");

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;
            case 8: // If Statement Satisfied 
                StateSource = c;
                StateDestination = FunctionName;
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination + " provided( " + StatementCondition + " ) //Please modify condition");

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;
            case 9:
                StateDestination = c; // "Words[1]" stands for function name
                ToAddToPorts = ("export port ePort " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter); // To add contract name later
                toAddToConnector = ("connector SINGLE " + BIPTemplateC.ContractName + "_" + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + "(" + BIPTemplateC.ContractName + "." + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter + ")");// To add contract name later
                ToAddToTransition = ("on " + BIPTemplateC.GetCurrentTransitionName() + TransitionCounter++ + " from " + StateSource + " to " + StateDestination);

                WriteToTemplateExtention(StateDestination, ToAddToTransition, ToAddToPorts, toAddToConnector); // Call extended function to add to template
                break;


        }
    }


    private void WriteToTemplateExtention(string StateDestination, string ToAddToTransition, string ToAddToPorts, string toAddToConnector)
    {
        BIPTemplateC.AddToStates(StateDestination);
        BIPTemplateC.AddToTransitions(ToAddToTransition);
        BIPTemplateC.AddToPorts(ToAddToPorts);
        BIPTemplateC.AddToConnector(toAddToConnector);
        StateSource = StateDestination;
    }
    //Console.WriteLine(BIPTemplateC.GetCurrentTransitionName());    

    public string RemoveBetween(string sourceString, string startTag, string endTag) // Function to remove string within two given strings
    {
        Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
        return regex.Replace(sourceString, startTag + endTag);
    }

    private string ReturnStringBtwSemiColons(string originalString)
    {
        string FinalString = "";

        for (int i = 0; i < originalString.Length; i++)
        {
            if (originalString[i] == '{')  // when current char is '{', start from current length 'i' and get all char storing it in one string
            {
                while (i < originalString.Length)
                {
                    FinalString += originalString[i];
                    i++;
                }
            }
        }
        return FinalString;
    }

    public string ReturnCharactersInsideParentheses(string bodyfunction)
    {
        String InsideString = " ";
        for (int i = 0; i < bodyfunction.Length; i++)
        {
            if (bodyfunction[i] == '(')
            {
                i++;
                while (bodyfunction[i] != ')')
                {
                    InsideString += bodyfunction[i];
                    i++;
                }
            }
        }
        return InsideString;
    }
    public void SetCurrentTemplate(BIPtemplate temp)
    {
        BIPTemplateC = temp; // sets current template 
    }

    }
}