using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Program
{
    public class SimpliedFunctions
    {
        private string AllFunction;
        private String[] ReceivedVariables; // Received variables in header
        private string header = "";
        private string FunctionBody = null;
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
            bool Doubleops = false;
            string[] substring = new string[10]; // maximum of 10 possible 
            myString = Regex.Replace(myString, "[({});]", ""); //remove paranthesis inside header
            myString = myString.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", ""); // removing space between 
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

            for (int i = 8; i <= 9; i++)
            {
                if (myString.Contains(BIPTemplateC.MathOperatorsList[i]))
                {
                    Doubleops = true;
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

                if (Doubleops == true)
                {
                    for (int i = 8; i <= 9; i++)
                    {
                        if (myString.Contains(BIPTemplateC.MathOperatorsList[i]))
                        {
                            exist = true;
                            myString = myString.Replace("++", "_");
                            ReturnString = BIPTemplateC.MathOperatorsList[i] +1 ;
                            
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

        public static string RemoveBetween(string sourceString, string startTag, string endTag) // Function to remove string within two given strings
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
