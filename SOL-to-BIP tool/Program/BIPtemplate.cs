using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Program
{
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
     {"++", "add"},
      {"--", "sub"},
       };

        public string[] MathOperatorsList = { "+", "-", "/", "*", "+=", "-=", "/=", "*=" , "++" , "--"};
        public string[] TheoremList = { "tx.origin", "selfdestruct", "delegatecall", "this.balance ==", "if(", "for(", "require(", "while(", "elseif", ".transfer(", ".call", ".send(", "else{" }; // Transfer, Call, send, and else do not have counters
        public int[] StatesCounterArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // counter for: "if(", "for(" , "require(", State counter (Ref num 9)
        public List<string> PortsList = new List<string>(); // states (place s1, s2, s3, ...)
        public List<string> TransitionList = new List<string>(); // states (place s1, s2, s3, ...)
        public List<string> ConnectorList = new List<string>(); // states (place s1, s2, s3, ...)
        public string ContractName;

        //Incrementer for all statements above.


        private string[] TransitionName;
        private int TransitionCounter = 0;

        public string ports; // ports String
        public string Transitions; // Transitions String
        public string Connector; // connectors String

        public List<string> States = new List<string>(); // states (place s1, s2, s3, ...)

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
            TransitionList.Add("\n");
        }

        public void AddLineToPorts()
        {
            PortsList.Add("\n");
        }
        public void AddLineToConnector()
        {
            ConnectorList.Add("\n");
        }

        public void ResetListes()
        {
            PortsList.Clear();
            TransitionList.Clear();
            States.Clear();
        }



    }
}
